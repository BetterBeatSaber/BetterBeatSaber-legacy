using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

using BetterBeatSaber.Server.Discord;
using BetterBeatSaber.Server.Discord.Interfaces;
using BetterBeatSaber.Server.Integrations;
using BetterBeatSaber.Server.Interfaces;
using BetterBeatSaber.Server.Jobs;
using BetterBeatSaber.Server.Leaderboards;
using BetterBeatSaber.Server.Leaderboards.BeatLeader;
using BetterBeatSaber.Server.Leaderboards.ScoreSaber;
using BetterBeatSaber.Server.Network;
using BetterBeatSaber.Server.Network.Interfaces;
using BetterBeatSaber.Server.Services;
using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Server.Twitch;
using BetterBeatSaber.Server.Twitch.Interfaces;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json.Serialization;

using Octokit;

using Sentry;

using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

using AppContext = BetterBeatSaber.Server.AppContext;
using IPlayerService = BetterBeatSaber.Server.Services.Interfaces.IPlayerService;
using PlayerService = BetterBeatSaber.Server.Services.PlayerService;
using ProductHeaderValue = Octokit.ProductHeaderValue;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
       .ClearProviders()
       .AddConsole();

if(builder.Environment.IsProduction())
    builder.WebHost.UseSentry();

#region Controllers

builder.Services.AddControllers(options => {
    options.ModelMetadataDetailsProviders.Add(new NewtonsoftJsonValidationMetadataProvider(new SnakeCaseNamingStrategy()));
}).AddNewtonsoftJson(options => {
    options.SerializerSettings.ContractResolver = new DefaultContractResolver {
        NamingStrategy = new SnakeCaseNamingStrategy()
    };
});

#endregion

#region Authentication

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters {
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
    ValidateIssuer = true,
    ValidateAudience = false,
    ValidateLifetime = false,
    ValidateIssuerSigningKey = true
});

#endregion

#region Services

var steamWebInterfaceFactory = new SteamWebInterfaceFactory(builder.Configuration.GetValue<string>("SteamApiKey"));

builder.Services
       .AddSwaggerGen()
       .AddHttpClient()
       .AddDbContext<AppContext>(contextOptions => contextOptions.UseMySQL(builder.Configuration.GetConnectionString("Default")!, options => {
           options.EnableRetryOnFailure(2);
       }))
       .AddServerTiming()
       .AddRateLimiter(options => {

           options.OnRejected += (context, _) => {

               if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                   context.HttpContext.Response.Headers.RetryAfter = ((int) retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);

               context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

               return new ValueTask();

           };

           options.AddPolicy("authenticate", _ => RateLimitPartition.GetFixedWindowLimiter("authenticate", _ => new FixedWindowRateLimiterOptions {
               Window = TimeSpan.FromSeconds(5),
               PermitLimit = 1
           }));

           options.AddPolicy("twitch", _ => RateLimitPartition.GetFixedWindowLimiter("twitch", _ => new FixedWindowRateLimiterOptions {
               Window = TimeSpan.FromSeconds(1),
               PermitLimit = 1
           }));

       })

       .AddSingleton<ISteamUserAuth, SteamUserAuth>(_ => steamWebInterfaceFactory.CreateSteamWebInterface<SteamUserAuth>())
       .AddSingleton<ISteamUser, SteamUser>(_ => steamWebInterfaceFactory.CreateSteamWebInterface<SteamUser>())

       .AddSingleton<IGitHubClient, GitHubClient>(serviceProvider => new GitHubClient(new ProductHeaderValue("BetterBeatSaber")) {
           Credentials = new Credentials(serviceProvider.GetService<IConfiguration>()?.GetValue<string>("GithubToken"))
       })
       .AddSingleton<IGithubService, GithubService>()

       .AddScoped<IJwtService, JwtService>()
       .AddScoped<ITokenService, TokenService>()
       .AddScoped<IAzureService, AzureService>()
       .AddScoped<IConfigService, ConfigService>()

       .Configure<Server.ServerOptions>(builder.Configuration.GetSection("Server"))
       .AddSingleton<IServer, Server>()

       .AddSingleton<ITwitchService, TwitchService>()
       .AddSingleton<IDiscordService, DiscordService>()

       .AddSingleton<DiscordIntegration>()
       .AddSingleton<PatreonIntegration>()
       .AddSingleton<TwitchIntegration>()

       .AddSingleton<IBaseLeaderboardClient, ScoreSaberClient>()
       .AddSingleton<IBaseLeaderboardClient, BeatLeaderClient>()

       .AddHostedService<UpdateLeaderboardJob>()
       .AddHostedService<RefreshAccessTokensJob>()

       .AddScoped<IIntegrationService, IntegrationService>()
       .AddScoped<IPlayerService, PlayerService>();

if (builder.Environment.IsProduction())
    builder.Services.AddSingleton<IModuleService, ModuleService>();
else
    builder.Services.AddSingleton<IModuleService, ModuleDevelopmentService>();

#endregion

var app = builder.Build();

using var scope = app.Services.CreateScope();

#region Initialize Services

foreach (var serviceDescriptor in builder.Services.Where(s => s.ServiceType.GetInterfaces().Any(i => i.FullName == typeof(IInitializable).FullName) || (s.ImplementationType != null && s.ImplementationType.IsSubclassOf(typeof(LifetimeService<>)))))
    await ((IInitializable) scope.ServiceProvider.GetRequiredService(serviceDescriptor.ServiceType)).Init();

#endregion

if (app.Environment.IsDevelopment())
    app.UseFileServer(new FileServerOptions {
        FileProvider = new PhysicalFileProvider(Path.Combine(Environment.CurrentDirectory, "Data")),
        RequestPath = "/data",
        EnableDirectoryBrowsing = true,
        StaticFileOptions = {
            ServeUnknownFileTypes = true
        }
    });

app.UseRouting();

//app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(_ => true).AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

if(!app.Environment.IsDevelopment())
    app.Use(async (httpContext, next) => {
        var playerId = httpContext.User.FindFirstValue(TokenService.JwtIdClaimName);
        if (playerId != null) {
            SentrySdk.ConfigureScope(scope => scope.User = new Sentry.User {
                Id = playerId
            });
        }
        await next.Invoke();
    });

if(app.Environment.IsProduction())
    app.UseRateLimiter();

app.MapControllers();

#region Database Migrations

app.Logger.LogInformation("Running Migrations");

scope.ServiceProvider
     .GetService<AppContext>()!
     .Database
     .Migrate();

#endregion

app.Run();