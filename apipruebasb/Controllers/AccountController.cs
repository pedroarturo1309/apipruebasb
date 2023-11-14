using System.Security.Claims;
using apipruebasb_repository.Usuario;
using apipruebasb_repository.Usuario.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace apipruebasb.Controllers;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<AccountController> _logger;
    private readonly IUsuarioRepository _usuarioRepository;

    public AccountController(ILogger<AccountController> logger, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _usuarioRepository = usuarioRepository;
    }

    [HttpGet("Prueba")]
    public IActionResult Prueba()
    {
        return Ok(Summaries);
    }

    [HttpGet("login-google")]
    public async Task LoginGoogle()
    {
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
        {
            RedirectUri = Url.Action("GoogleResponse")
        });
    }

    [HttpGet("login-microsoft")]
    public async Task LoginMicrosoft()
    {
        await HttpContext.ChallengeAsync(MicrosoftAccountDefaults.AuthenticationScheme, new AuthenticationProperties()
        {
            RedirectUri = Url.Action("MicrosoftResponse")
        });
    }

    [HttpGet("MicrosoftResponse")]
    public async Task<IActionResult> MicrosoftResponse()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return ResponseAuthentication(authenticateResult);

    }

    private IActionResult ResponseAuthentication(AuthenticateResult authenticateResult)
    {
        var claims = authenticateResult?.Principal?.Identities?
                    .FirstOrDefault()?.Claims.Select(claim => new
                    {
                        claim.Issuer,
                        claim.OriginalIssuer,
                        claim.Type,
                        claim.Value
                    }).ToList();


        var userInfo = new UsuarioOauthDTO
        {
            AutenticadoDesde = authenticateResult?.Principal?.Identities?
                    .FirstOrDefault()?.Claims.FirstOrDefault()?.Issuer,
            Nombres = claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.GivenName)?.Value,
            Apellidos = claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.Surname)?.Value,
            CorreoElectronico = claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value,
        };


        if (authenticateResult == null || !authenticateResult.Succeeded)
            return BadRequest(); // TODO: Handle this better.
                                 // await HttpContext.SignInAsync(GoogleDefaults.AuthorizationEndpoint, authenticateResult.Principal);

        var token = authenticateResult.Properties.GetTokenValue("access_token");

        _usuarioRepository.VerificarInicioSesionOauth(userInfo);

        return Ok(new { token, userInfo, claims });
    }

    [HttpGet("GoogleResponse")]
    public async Task<IActionResult> GoogleResponse()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return ResponseAuthentication(authenticateResult);
    }
    [HttpGet("LogOut")]
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }

    [HttpPost("Registrar")]
    public IActionResult Registrar([FromBody] UsuarioOauthDTO model)
    {
        var respuesta = _usuarioRepository.RegistrarUsuarioLocal(model);
        return Ok(respuesta);
    }

    [HttpPost("login-local")]
    public IActionResult IniciarSesion([FromBody] InicioSesionDTO model)
    {
        var respuesta = _usuarioRepository.IniciarSesionLocal(model.CorreoElectronico, model.Contrasena);
        return Ok(respuesta);
    }
}
