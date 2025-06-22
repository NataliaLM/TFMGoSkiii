using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using TFMGoSki.Models;
using TFMGoSki.ViewModels;

namespace TFMGoSki.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;

        public AccountController(UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: /Account/AllUsers
        [HttpGet]
        public async Task<IActionResult> AllUsers()
        {
            var users = _userManager.Users.ToList();
            return View(users); // O puedes devolver Json si es para API: return Json(users);
        }

        /* ---------- REGISTER ---------- */

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            //ViewBag.Roles = _roleManager.Roles
            //                .Select(r => new SelectListItem(r.Name, r.Name));

            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem("Client", "Client"),
                new SelectListItem("Worker", "Worker"),
                new SelectListItem("Admin", "Admin")
            };

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new List<SelectListItem>
                {
                    new SelectListItem("Client", "Client"),
                    new SelectListItem("Worker", "Worker"),
                    new SelectListItem("Admin", "Admin")
                };

                return View(model);
            }

            var user = new User
            {
                UserName = model.Email,               
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Add the Admin role to the database
                IdentityResult roleResult;
                bool adminRoleExists = await _roleManager.RoleExistsAsync(model.RoleName);
                if (!adminRoleExists)
                {
                    roleResult = await _roleManager.CreateAsync(new Role(model.RoleName));
                }

                // Select the user, and then add the admin role to the user
                //User? userA = await _userManager.FindByNameAsync(model.Name);
                if (!await _userManager.IsInRoleAsync(user, model.RoleName))
                {
                    var userResult = await _userManager.AddToRoleAsync(user, model.RoleName);
                }

                // Asignar rol elegido
                //await _userManager.AddToRoleAsync(user, model.RoleName);

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        /* ---------- LOGIN ---------- */

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);

            ModelState.AddModelError("", "Credenciales incorrectas.");
            return View(model);
        }

        /* ---------- LOGOUT ---------- */

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        /* ---------- PROFILE ---------- */

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        /* ---------- DELETE ACCOUNT ---------- */

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }

            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View("Profile", user);
        }
    }
}
