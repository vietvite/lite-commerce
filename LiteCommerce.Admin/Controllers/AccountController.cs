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
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace LiteCommerce.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AccountController(
            ILogger<AccountController> logger,
            IHttpContextAccessor contextAccessor,
            IPasswordHasher passwordHasher,
            IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _passwordHasher = passwordHasher;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// View page: Profile infomation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            Employee employee = CatalogBLL.GetEmployee(Convert.ToInt32(User.FindFirst("UserID").Value));
            return View(employee);
        }

        /// <summary>
        /// Update profile infomation
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Index(EmployeePostRequest model, string id = "")
        {
            try
            {
                CheckNotNull(model);

                string photoPath = UploadedFile(model);
                photoPath = string.IsNullOrEmpty(photoPath)
                    ? CatalogBLL.GetEmployee(model.EmployeeID).PhotoPath
                    : photoPath;

                Employee employee = new Employee
                {
                    EmployeeID = model.EmployeeID,
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    BirthDate = model.BirthDate,
                    Email = model.Email,
                    Address = model.Address,
                    City = model.City,
                    Country = model.Country,
                    HomePhone = model.HomePhone,
                    PhotoPath = photoPath,
                };

                CatalogBLL.UpdateEmployeeProfile(employee);
                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message + ": " + ex.StackTrace);
                Employee employee = new Employee
                {
                    EmployeeID = model.EmployeeID,
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    BirthDate = model.BirthDate,
                    Email = model.Email,
                    Address = model.Address,
                    City = model.City,
                    Country = model.Country,
                    HomePhone = model.HomePhone,
                };
                return View(employee);
            }
            // return View(employee);
        }

        /// <summary>
        /// View Page: Change password
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Update user password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ChangePassword(PasswordPostRequest password)
        {
            try
            {
                Employee employee = null;
                ValidatePassword(password, out employee);

                // Update new password
                employee.Password = _passwordHasher.Hash(password.NewPassword);
                CatalogBLL.ChangePasswordEmployee(employee);

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

        /// <summary>
        /// Validate input of change password post request
        /// </summary>
        /// <param name="password"></param>
        /// <param name="outEmployee"></param>
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

        /// <summary>
        /// View page: login page
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Validate whether email and password is null
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        private void CheckNotNull(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
                ModelState.AddModelError("LoginError", "Email expected");

            if (string.IsNullOrEmpty(password))
                ModelState.AddModelError("LoginError", "Password expected");

            if (ModelState.ErrorCount > 0)
                throw new MissingFieldException();
        }

        /// <summary>
        /// Validate update user profile post request is null
        /// </summary>
        /// <param name="model"></param>
        private void CheckNotNull(EmployeePostRequest model)
        {
            if (string.IsNullOrEmpty(model.LastName))
                ModelState.AddModelError("LastName", "Last name expected");

            if (string.IsNullOrEmpty(model.FirstName))
                ModelState.AddModelError("FirstName", "First name expected");

            if (model.BirthDate.Year < 1753 || model.BirthDate.Year > 9999)
                ModelState.AddModelError("BirthDate", "BirthDate's year must be between 1753 and 9999");

            if (string.IsNullOrEmpty(model.Email))
                ModelState.AddModelError("Email", "Email expected");

            if (string.IsNullOrEmpty(model.Address))
                model.Address = "";

            if (string.IsNullOrEmpty(model.City))
                model.City = "";

            if (string.IsNullOrEmpty(model.Country))
                model.Country = "";

            if (string.IsNullOrEmpty(model.HomePhone))
                model.HomePhone = "";

            if (ModelState.ErrorCount > 0)
                throw new MissingFieldException();
        }

        /// <summary>
        /// Upload image handler
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string UploadedFile(EmployeePostRequest model)
        {
            string uniqueFileName = null;

            if (model.PhotoPath != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PhotoPath.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.PhotoPath.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
