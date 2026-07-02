using MemeApp.Api.Dtos;
using MemeApp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MemeApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(JwtTokenService jwtTokenService) : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new ErrorResponse("Email и пароль обязательны."));
        }

        if (!jwtTokenService.ValidateAdminCredentials(request.Email, request.Password))
        {
            return Unauthorized(new ErrorResponse("Неверный email или пароль."));
        }

        var token = jwtTokenService.GenerateToken(request.Email);
        if (token is null)
        {
            return StatusCode(500, new ErrorResponse("JWT_SECRET не настроен на сервере."));
        }

        return Ok(new LoginResponse(token, DateTime.UtcNow.AddHours(24)));
    }
}
