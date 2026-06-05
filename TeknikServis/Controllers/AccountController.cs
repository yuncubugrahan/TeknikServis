using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeknikServis.Models;

namespace TeknikServis.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> u,
                                  SignInManager<IdentityUser> s)
        { _userManager = u; _signInManager = s; }

        [HttpGet] public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel m)
        {
            if (!ModelState.IsValid) return View(m);
            var r = await _signInManager.PasswordSignInAsync(
                m.Email, m.Password, m.RememberMe, false);
            if (r.Succeeded)
                return RedirectToAction("Index", "ServiceRequests");
            ModelState.AddModelError("", "E-posta veya şifre hatalı.");
            return View(m);
        }

        [HttpGet] public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel m)
        {
            if (!ModelState.IsValid) return View(m);
            var user = new IdentityUser
            { UserName = m.Email, Email = m.Email };
            var r = await _userManager.CreateAsync(user, m.Password);
            if (r.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "ServiceRequests");
            }
            foreach (var e in r.Errors)
                ModelState.AddModelError("", e.Description);
            return View(m);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "ServiceRequests");
        }

        public IActionResult AccessDenied() => View();
    }
}