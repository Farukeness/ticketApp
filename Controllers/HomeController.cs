using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ticketApp.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using ticketApp.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ticketApp.ViewModel;
namespace ticketApp.Controllers;

public class HomeController : Controller
    {
    
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private RoleManager<AppRole> _roleManager;
        public HomeController(UserManager<AppUser> userManager,
         SignInManager<AppUser> signInManager,
         RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        
                        var roles = await _userManager.GetRolesAsync(user);
                        var rol = roles.FirstOrDefault();
                        if (rol == "Admin"){return RedirectToAction("Index", "Admin");}
                        if (rol == "Developer"){return RedirectToAction("Index", "Developer");}
                        if (rol == "User"){return RedirectToAction("Index", "User");}
                        
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hatalı  Parola");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Hatalı Email");
                }
            }
            return View(model);
        }
        
    }
