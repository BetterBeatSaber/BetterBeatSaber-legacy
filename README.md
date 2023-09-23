# Better Beat Saber

## Developing

First of all, you should use [JetBrains Rider](https://www.jetbrains.com/rider).

### Compiling the Source

_You shouldn't try it ..._

What you have to have installed:
- .NET Core SDK v7.0.0
- .NET Framework v4.7.2 Developer Pack
- An environment variable called "BeatSaberDirectory" directing to your Beat Saber Directory

And also these Libs/Plugins installed:
- BeatSaberMarkupLanguage
- SongCore
- SongDetailsCache

And other requirements:
- An MySQL Server
- Changed the `ServerDataDirectory` Property in the `Directory.Build.Props` file to it's corresponding path

Any other requirement you can read of the `appsettings.json`.

### Example `appsettings.json` file

```json
{
  "Sentry": {
    "Dsn": ""
  },
  "GithubToken": "",
  "ConnectionStrings": {
    "Default": ""
  },
  "Server": {
    "Name": "",
    "IP": "",
    "Port": 12345,
    "MaxConnections": 1
  },
  "SteamApiKey": "",
  "Patreon": {
    "ClientId": "",
    "ClientSecret": "",
    "RedirectUri": ""
  },
  "Discord": {
    "ClientId": "",
    "ClientSecret": "",
    "RedirectUri": "",
    "BotToken": ""
  },
  "Twitch": {
    "ClientId": "",
    "ClientSecret": "",
    "RedirectUri": "",
    "Login": "",
    "AuthToken": ""
  },
  "Storage": {
    "URL": "",
    "AccessKey": "",
    "SecretKey": ""
  },
  "Azure": {
    "Cognitive": {
      "SubscriptionKey": "",
      "Region": ""
    },
    "Translation": {
      "SubscriptionKey": "",
      "Region": ""
    }
  },
  "Jwt": {
    "Issuer": "",
    "Audience": "",
    "Key": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "System.Net.Http.HttpClient": "Error",
      "Microsoft.EntityFrameworkCore.Database.Command": "Error"
    }
  }
}
```