using System.Security.Claims;
using apipruebasb_repository.Usuario;
using apipruebasb_repository.Usuario.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("login")]
    public async Task Login()
    {
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
        {
            RedirectUri = Url.Action("GoogleResponse")
        });
    }

    [HttpGet("LoginMicrosoft")]
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

    [HttpGet("LoginCallback")]
    public async Task<IActionResult> LoginCallback(string returnUrl = "PEDRO")
    {
        var authenticateResult = await HttpContext.AuthenticateAsync("External");

        if (!authenticateResult.Succeeded)
            return BadRequest(); // TODO: Handle this better.

        var claimsIdentity = new ClaimsIdentity("Application");

        claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier));
        claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.Email));

        await HttpContext.SignInAsync(GoogleDefaults.AuthorizationEndpoint,
            new ClaimsPrincipal(claimsIdentity));

        return Ok();
        // return LocalRedirect(returnUrl);
    }
}
