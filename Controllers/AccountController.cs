using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace apipruebasb.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    [HttpGet("login")]
    public async Task Login()
    {
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
        {
            RedirectUri = Url.Action("GoogleResponse")
        });
    }

    [HttpGet("GoogleResponse")]
    public async Task<IActionResult> GoogleResponse()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var claims = authenticateResult?.Principal?.Identities?
            .FirstOrDefault()?.Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

        if (authenticateResult == null || !authenticateResult.Succeeded)
            return BadRequest(); // TODO: Handle this better.
                                 // await HttpContext.SignInAsync(GoogleDefaults.AuthorizationEndpoint, authenticateResult.Principal);


        var principal = authenticateResult.Principal;

        foreach (var claim in principal.Claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
        }

        // Alternatively, you can convert the properties to a JSON string for easier viewing
        // var propertiesJson = JsonConvert.SerializeObject(authenticateResult.Properties.Items, Formatting.Indented);
        // Console.WriteLine($"All Properties:\n{propertiesJson}");
        var token = authenticateResult.Properties.GetTokenValue("access_token");

        return Ok(claims);
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
