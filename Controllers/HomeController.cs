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
    
       
        public IActionResult Index()
        {
            
            return View();
        }
        
    }
