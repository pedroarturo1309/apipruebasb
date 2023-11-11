using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.Configure<CookiePolicyOptions>(options => {
//     options.MinimumSameSitePolicy = SameSiteMode.None;
// });




builder.Services
    .AddAuthentication(auth =>
    {
        auth.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        auth.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(google =>
    {
        // En el código de configuración del proveedor externo (por ejemplo, Google)
        google.ClientId = builder.Configuration["Authentication:Google:clientId"];
        google.ClientSecret = builder.Configuration["Authentication:Google:clientSecret"];
        google.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
        google.ClaimActions.MapJsonKey("urn:google:access_token", "access_token", "token");
    })
    .AddMicrosoftAccount(micro => {
         micro.ClientId = builder.Configuration["Authentication:Microsoft:clientId"];
        micro.ClientSecret = builder.Configuration["Authentication:Microsoft:clientSecret"];
       
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
