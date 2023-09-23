using System.Net;

using Amazon.S3;
using Amazon.S3.Model;

using BetterBeatSaber.Server.Models;
using BetterBeatSaber.Server.Services.Interfaces;

namespace BetterBeatSaber.Server.Services; 

public sealed class ConfigService : IConfigService {

    private readonly AmazonS3Client _s3Client;

    public ConfigService(IConfiguration configuration) {
        _s3Client = new AmazonS3Client(configuration.GetValue<string>("Storage:AccessKey"), configuration.GetValue<string>("Storage:SecretKey"), new AmazonS3Config {
            ServiceURL = configuration.GetValue<string>("Storage:URL"),
            Timeout = TimeSpan.FromSeconds(10),
            ForcePathStyle = true
        });
    }

    public async Task<string?> DownloadConfig(Player player, string id) {
        try {
            
            var response = await _s3Client.GetObjectAsync(new GetObjectRequest {
                BucketName = "userdata",
                Key = string.Join("/", player.Id, id)
            });

            if (response.HttpStatusCode != HttpStatusCode.OK)
                return null;
            
            using var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream);

            return player.DecryptToString(memoryStream.ToArray());

        } catch (Exception) {
            return null;
        }
    }

    public async Task<bool> UploadConfig(Player player, string id, string config) {
        try {
            
            var response = await _s3Client.PutObjectAsync(new PutObjectRequest {
                BucketName = "userdata",
                Key = string.Join("/", player.Id, id),
                InputStream = new MemoryStream(player.Encrypt(config)),
                ContentType = "application/octet-stream"
            });

            return response.HttpStatusCode == HttpStatusCode.OK;
            
        } catch (Exception) {
            return false;
        }
    }

    public Task<bool> DeleteAllConfigs(Player player) {
        throw new NotImplementedException();
    }

}