using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Server.API.Services;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/token", (HttpContext context) =>
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
        issuer: "RSA",
        audience: "RSA",
        claims: claims,
        expires: DateTime.UtcNow.AddSeconds(30),
        signingCredentials: credential
    );
    var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
    var http = GenerateHttpCookie.Instance;
    http.SetCookie(token, context);
    return Results.Ok();
});

app.Run();