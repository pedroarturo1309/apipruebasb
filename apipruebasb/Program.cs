using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using apipruebasb_repository.Usuario;
using apipruebasb_repository;
using Microsoft.EntityFrameworkCore;
using apipruebasb_repository.IMDB;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using apipruebasb_repository.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var MyAllowSpecificOrigins = "_sceAllowSpecificOrigins";

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PruebasbDBContext>(
        options => options.UseSqlServer("name=ConnectionStrings:LocalConnection"));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                  policy =>
                  {
                      policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                  });
});

#region Declaracion de repositorios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IIMDBRepository, IMDBRepository>();
#endregion

#region  Configuracion de authenticacion
builder.Services
    .AddAuthentication(auth =>
    {
        auth.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer()
    .AddCookie()
    .AddGoogle(google =>
    {
        google.SaveTokens = true;
        // En el código de configuración del proveedor externo (por ejemplo, Google)
        google.ClientId = builder.Configuration["Authentication:Google:clientId"];
        google.ClientSecret = builder.Configuration["Authentication:Google:clientSecret"];
        google.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");

        google.Events.OnCreatingTicket = ctx =>
            {
                List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

                tokens.Add(new AuthenticationToken()
                {
                    Name = "TicketCreated",
                    Value = DateTime.UtcNow.ToString()
                });

                ctx.Properties.StoreTokens(tokens);

                return Task.CompletedTask;
            };
    })
    .AddMicrosoftAccount(micro =>
    {
        micro.SaveTokens = true;
        micro.ClientId = builder.Configuration["Authentication:Microsoft:clientId"];
        micro.ClientSecret = builder.Configuration["Authentication:Microsoft:clientSecret"];

        micro.Events.OnCreatingTicket = ctx =>
          {
              List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

              tokens.Add(new AuthenticationToken()
              {
                  Name = "TicketCreated",
                  Value = DateTime.UtcNow.ToString()
              });

              ctx.Properties.StoreTokens(tokens);

              return Task.CompletedTask;
          };
    });

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use((context, next) =>
{
    CurrentUser.Configure(context.RequestServices.GetRequiredService<IHttpContextAccessor>());
    return next(context);
});


app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.UseMiddleware<JwtMiddleware>();
app.MapControllers();

app.Run();
