using Newtonsoft.Json;

namespace BetterBeatSaber.Server.Steam.Models; 

// ReSharper disable StringLiteralTypo

public record AuthResponse(AuthResponseData Response);

public record AuthResponseData(AuthResponseParams? Params, AuthResponseError? Error);

public record AuthResponseParams(string Result, [JsonProperty("steamid")] string SteamId, [JsonProperty("ownersteamid")] string OwnerSteamId, [JsonProperty("vacbanned")] bool VacBanned, [JsonProperty("publisherbanned")] bool PublisherBanned);

public record AuthResponseError(short ErrorCode, [JsonProperty("errordesc")] string ErrorDescription);