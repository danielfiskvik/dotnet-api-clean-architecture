using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using ModularApp.Modules.Workspace.Application;
using ModularApp.WebApi;
using ModularApp.WebApi.Configurations;
using ModularApp.Modules.Workspace.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddPersistenceModule(builder.Configuration)
    .AddWebApiModule()
    .AddApplicationModule();

builder.Services
    .AddControllers()
    // Adding NewtonsoftJson because reference loop must be set to ignore since we expose EF Core Domain with multiple reference through the API.
    .AddNewtonsoftJson(options => 
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ** Add Authentication & OIDC configuration **
builder.Services.AddAuthentication(options => 
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()  // Cookie authentication for storing tokens
    .AddOpenIdConnect(options =>
    {
        options.Authority = builder.Configuration["OIDC:Authority"];  // OIDC Provider URL (e.g., Auth0, Azure AD, Keycloak)
        options.ClientId = builder.Configuration["OIDC:ClientId"];
        options.ClientSecret = builder.Configuration["OIDC:ClientSecret"];
        options.ResponseType = "code";  // Authorization Code Flow (without PKCE)
        options.SaveTokens = true;  // Store tokens in authentication cookies
        options.GetClaimsFromUserInfoEndpoint = true;  // Get additional claims from the user info endpoint
        options.Scope.Add("openid");  // OIDC standard scopes
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.CallbackPath = "/callback";  // This must match the redirect URI in your identity provider
        options.SignedOutRedirectUri = "/";

        options.Events.OnTokenValidated = async context =>
        {
            // Add custom claims handling if needed
            var claims = context.Principal?.Identities?.FirstOrDefault()?.Claims;
            await Task.CompletedTask;
        };
    });

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    /*
    options.AddPolicy("CorsPolicy", b =>
    {
        b
            .WithOrigins(allowedOrigins) // Allow frontend URL(s)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();  // Allow cookies and credentials
    });
    */
    options.AddPolicy("AllowAll", b =>
    {
        b
            .AllowAnyOrigin()   // Allow requests from any domain
            .AllowAnyHeader()   // Allow any HTTP headers
            .AllowAnyMethod();  // Allow any HTTP methods (GET, POST, PUT, DELETE, etc.)
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.InitializePersistenceModuleAsync(builder.Configuration);

app.UseHttpsRedirection();

app
    .UseCors("AllowAll");
    //.UseCors("CorsPolicy");

// ** Add authentication and authorization middleware **
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Make the Program class public using a partial class declaration. If not then the Integration Tests will not work.
namespace ModularApp.WebApi
{
    public partial class Program { }
}