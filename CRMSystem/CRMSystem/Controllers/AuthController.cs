using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                email = user.Email,
                roles = roles
            });
        }
        return Unauthorized("Invalid credentials");
    }
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // If using SignInManager:
        _signInManager.SignOutAsync();
        return Ok(new { roles = new string[] { } }); // Return empty roles
    }
}



public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}