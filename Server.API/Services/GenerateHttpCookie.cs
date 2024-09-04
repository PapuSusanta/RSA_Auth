namespace Server.API.Services;

public class GenerateHttpCookie
{
    private static GenerateHttpCookie instance = null!;
    public static GenerateHttpCookie Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            instance = new GenerateHttpCookie();
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    private GenerateHttpCookie() { }

    public void SetCookie(string Token, HttpContext _context)
    {
        _context.Response.Cookies.Append("JWT_KEY", Token, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddSeconds(30),
            HttpOnly = true,
            IsEssential = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });
    }
}