using Microsoft.AspNetCore.Mvc;

namespace YTTheater.Server.Features.Auth;

[Controller]
public class AuthController : ControllerBase
{
    [HttpGet, Route("/auth/login")]
    public async Task<string> Login()
    {
        return "deez nuts";
    }
}
