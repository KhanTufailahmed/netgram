using netgram.Models;

using netgram.ViewModel;

using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;

namespace netgram.Controllers
{

    public class AccountController : Controller

    {

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(

            UserManager<ApplicationUser> userManager,

            SignInManager<ApplicationUser> signInManager)

        {

            _userManager = userManager;

            _signInManager = signInManager;

        }


        [HttpGet]

        public IActionResult Register()

        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Posts");
            }

            return View();

        }

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Register(RegisterViewModel model)

        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Posts");
            }

            if (!ModelState.IsValid)

            {

                return View(model);

            }

            var user = new ApplicationUser

            {

                DisplayName = model.DisplayName,

                UserName = model.Email,

                Email = model.Email

            };

            var result = await _userManager.CreateAsync(

                user,

                model.Password

            );

            if (result.Succeeded)

            {

                await _signInManager.SignInAsync(

                    user,

                    isPersistent: false

                );

                return RedirectToAction("Index", "Home");

            }

            foreach (var error in result.Errors)

            {

                ModelState.AddModelError(

                    string.Empty,

                    error.Description

                );

            }

            return View(model);

        }



        [HttpGet]

        public IActionResult Login(string? returnUrl = null)

        {

            ViewData["ReturnUrl"] = returnUrl;

            return View();

        }

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(

            LoginViewModel model,

            string? returnUrl = null)

        {

            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)

            {

                return View(model);

            }

            var result = await _signInManager.PasswordSignInAsync(

                model.Email,

                model.Password,

                model.RememberMe,

                lockoutOnFailure: true

            );

            if (result.Succeeded)

            {

                if (!string.IsNullOrEmpty(returnUrl) &&

                    Url.IsLocalUrl(returnUrl))

                {

                    return LocalRedirect(returnUrl);

                }

                return RedirectToAction("Index", "Home");

            }

            if (result.IsLockedOut)

            {

                ModelState.AddModelError(

                    string.Empty,

                    "Your account is temporarily locked. Please try again later."

                );

                return View(model);

            }

            ModelState.AddModelError(

                string.Empty,

                "Invalid email or password."

            );

            return View(model);

        }



        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Logout()

        {

            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");

        }

    }
}