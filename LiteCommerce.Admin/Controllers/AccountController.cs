using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using LiteCommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using LiteCommerce.DomainModels;
using LiteCommerce.BusinessLayers;
using LiteCommerce.Services;

namespace LiteCommerce.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IPasswordHasher _passwordHasher;

        public AccountController(ILogger<AccountController> logger, IHttpContextAccessor contextAccessor, IPasswordHasher passwordHasher)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _passwordHasher = passwordHasher;
        }

        public IActionResult Index()
        {
            return View();
        }

        // [Authorize(Roles = "Employee")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult LogIn()
        {
            //TODO: Check whether user is logged in yet, if true redirect to dashboard
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> LogIn(string email = "", string password = "")
        {
            string hashedPassword = _passwordHasher.Hash(password);
            _logger.LogWarning("Hashed password: " + hashedPassword);
            UserAccount user = UserAccountBLL.Authenticate(email, hashedPassword, UserAccountTypes.Employee);
            if (user != null)
            {
                // WebUserData userData = new WebUserData() {
                //     UserID = user.UserID,
                //     FullName = user.Fullname,
                //     GroupName = "Employee",
                //     LoginTime = Datetime.Now,
                //     SessionID = Session.SessionID,
                //     ClientIP = Request.UserHostAddress,
                //     Photo = user.Photo,
                // }
                // FormsAuthentication.SetAuthCookie(userData.ToCookieString(), false);

                // string SessionId = _contextAccessor.HttpContext.Session.Id;
                var claims = new List<Claim>
                {
                    new Claim("UserID", user.UserID),
                    new Claim("FullName", user.Fullname),
                    new Claim("GroupName", WebUserRoles.STAFF),
                    new Claim("LoginTime", Convert.ToString(DateTime.Now)),
                    // new Claim("SessionID", _contextAccessor.HttpContext.Session.Id),
                    new Claim("ClientIP", _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()),
                    new Claim("Photo", user.Photo),
                    new Claim("Title", user.Title),
                };

                await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "UserInfo")));

                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ModelState.AddModelError("LoginError", "Login Fail");
                ViewBag.email = email;
                ViewBag.password = password;
                return View();
            }
        }

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("LogIn");
        }

        private bool ValidateLogin(string userName, string password)
        {
            // For this sample, all logins are successful.
            return true;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
