using Microsoft.AspNetCore.Mvc;

namespace BetterBeatSaber.Server.Extensions; 

public static class ControllerExtensions {

    public static ActionResult InternalError(this Controller controller, string? text = null) {
        return controller.StatusCode(500, text ?? "Internal Server Error");        
    }

    public static void Pagination() {
        
    }

}