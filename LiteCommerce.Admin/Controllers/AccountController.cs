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

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(PasswordPostRequest password)
        {
            try
            {
                Employee employee = null;
                ValidatePassword(password, out employee);

                // Update new password
                employee.Password = _passwordHasher.Hash(password.NewPassword);
                CatalogBLL.UpdateEmployee(employee);

                return RedirectToAction("Index", "Dashboard");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message + ":\n" + ex.StackTrace);
                ViewData["OldPassword"] = password.OldPassword;
                ViewData["NewPassword"] = password.NewPassword;
                ViewData["ReNewPassword"] = password.ReNewPassword;
                return View();
            }
        }

        public class PasswordPostRequest
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
            public string ReNewPassword { get; set; }
        }

        public void ValidatePassword(PasswordPostRequest password, out Employee outEmployee)
        {
            if (string.IsNullOrEmpty(password.OldPassword))
                ModelState.AddModelError("OldPassword", "Please type old password.");
            if (string.IsNullOrEmpty(password.NewPassword))
                ModelState.AddModelError("NewPassword", "Please type new password.");
            if (string.IsNullOrEmpty(password.ReNewPassword))
                ModelState.AddModelError("ReNewPassword", "Please retype new password.");

            if (password.NewPassword != password.ReNewPassword)
                ModelState.AddModelError("Error", "Retype new password not match.");

            // Get old password & compare two hashed
            string oldPasswordHashed = _passwordHasher.Hash(password.OldPassword);
            outEmployee = CatalogBLL.GetEmployee(Convert.ToInt32(User.FindFirst("UserID").Value));

            if (outEmployee.Password != oldPasswordHashed)
                ModelState.AddModelError("Error", "Old password not correct.");

            if (ModelState.ErrorCount > 0)
                throw new Exception();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult LogIn()
        {
            if (!User.Identity.IsAuthenticated)
                return View();

            return RedirectToAction("Index", "Dashboard");
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> LogIn(string email = "", string password = "")
        {
            try
            {
                CheckNotNull(email, password);
                string hashedPassword = _passwordHasher.Hash(password);
                UserAccount user = UserAccountBLL.Authenticate(email, hashedPassword, UserAccountTypes.Employee);

                if (user == null)
                    throw new Exception("Email or password is not valid.");

                var claims = new List<Claim>
                {
                    new Claim("UserID", user.UserID),
                    new Claim("FullName", user.Fullname),
                    new Claim(ClaimTypes.Role, user.Groupname),
                    new Claim("LoginTime", Convert.ToString(DateTime.Now)),
                    new Claim("ClientIP", _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()),
                    new Claim("Photo", user.Photo),
                    new Claim("Title", user.Title),
                };
                await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "UserInfo")));

                return RedirectToAction("Index", "Dashboard");
            }
            catch (MissingFieldException)
            {
                ViewData["email"] = email ?? "";
                ViewData["password"] = password ?? "";

                return View();
            }
            catch (System.Exception ex)
            {
                ViewData["email"] = email ?? "";
                ViewData["password"] = password ?? "";

                ModelState.AddModelError("LoginError", ex.Message);
                return View();
            }
        }

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("LogIn");
        }

        private void CheckNotNull(string email = "", string password = "")
        {
            if (string.IsNullOrEmpty(email))
                ModelState.AddModelError("LoginError", "Email expected");

            if (string.IsNullOrEmpty(password))
                ModelState.AddModelError("LoginError", "Password expected");

            if (ModelState.ErrorCount > 0)
            {
                throw new MissingFieldException();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
