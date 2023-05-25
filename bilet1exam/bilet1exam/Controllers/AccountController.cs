using bilet1exam.ViewModels.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bilet1exam.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid) return View(register);
            IdentityUser identityUser = new IdentityUser
            {
                UserName = register.UserName,
                Email = register.Email,
            };
            IdentityResult registerResult = await _userManager.CreateAsync(identityUser, register.Password);
            if (!registerResult.Succeeded)
            {
                foreach (var error in registerResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(register);
            }
            return RedirectToAction(nameof(Login));
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM, string? ReturnUrl)
        {
            if (!ModelState.IsValid) return View(loginVM);
            IdentityUser user = await _userManager.FindByNameAsync(loginVM.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "email or password is wrong");
                return View(loginVM);
            }
            var signingResult = await _signInManager.CheckPasswordSignInAsync(user, loginVM.Password, true);
            if (!signingResult.Succeeded)
            {
                ModelState.AddModelError("", "email or password is wrong");
                return View(loginVM);
            }

            await _signInManager.SignInAsync(user, loginVM.RememberMe);
            if (Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
