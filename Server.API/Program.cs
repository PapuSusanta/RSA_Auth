using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Server.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(op =>
{
    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(op =>
{
    var rsaKey = RSA.Create();
    string xmlKey = File.ReadAllText("RSAKeys/PublicKey.xml");
    rsaKey.FromXmlString(xmlKey);

    op.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = builder.Configuration["JWT_Settings:Issuer"],
        ValidAudience = builder.Configuration["JWT_Settings:Audience"],
        ValidateIssuer = true,
        ValidateAudience = true,
        RequireExpirationTime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new RsaSecurityKey(rsaKey),
    };

    op.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            ctx.Request.Cookies.TryGetValue("JWT_KEY", out var token);
            Console.WriteLine(token);
            if (!string.IsNullOrEmpty(token))
            {
                ctx.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("api/Login", (HttpContext context, IConfiguration config) =>
{
    // var key = "6f47cdb3d8b17362924e92f6a8d2761f72076d8c46a7242f346d9924581c2e69";

    // var credential = new SigningCredentials(
    //                 new SymmetricSecurityKey(
    //                     Encoding.UTF8.GetBytes(key)),
    //                 SecurityAlgorithms.HmacSha256);

    var rsaKey = RSA.Create();
    string xmlKey = File.ReadAllText("RSAKeys/PrivateKey.xml");
    rsaKey.FromXmlString(xmlKey);
    var rsaSecurityKey = new RsaSecurityKey(rsaKey);

    var credential = new SigningCredentials(
                    rsaSecurityKey,
                    SecurityAlgorithms.RsaSha256);

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, "John Doe"),
        new Claim(ClaimTypes.Email, "test@test.com"),
    };

    var securityToken = new JwtSecurityToken(
        issuer: config["JWT_Settings:Issuer"],
        audience: config["JWT_Settings:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddSeconds(30),
        signingCredentials: credential
    );
    var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
    var http = GenerateHttpCookie.Instance;
    http.SetCookie(token, context);
    return Results.Ok();
});

app.MapPost("api/Logout", (HttpContext context) =>
{
    var http = GenerateHttpCookie.Instance;
    http.RemoveCookie(context);
    return Results.Ok();
});

app.MapGet("api/Protected", () =>
{
    return Results.Ok("Very secure data");
}).RequireAuthorization();



app.Run();