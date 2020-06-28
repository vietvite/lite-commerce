using Microsoft.VisualBasic;
using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LiteCommerce.Models;
using Microsoft.AspNetCore.Authorization;
using LiteCommerce.DomainModels;
using LiteCommerce.BusinessLayers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using LiteCommerce.Services;
using LiteCommerce.Common;

namespace LiteCommerce.Controllers
{
    [Authorize(Roles = WebUserRoles.HR_MANAGER)]
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IPasswordHasher _passwordHasher;

        public EmployeeController(ILogger<EmployeeController> logger, IWebHostEnvironment hostingEnvironment, IPasswordHasher passwordHasher)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// View page: List of employees
        /// </summary>
        /// <param name="page"></param>
        /// <param name="searchValue"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        public IActionResult Index(int page = 1, string searchValue = "", string country = "")
        {
            int rowCount = 0;
            int pageSize = 10;
            List<Employee> listOfEmployee = CatalogBLL.ListOfEmployee(page, pageSize, searchValue ?? "", out rowCount, country);

            var model = new Models.EmployeePaginationResult()
            {
                Page = page,
                Data = listOfEmployee,
                PageSize = pageSize,
                RowCount = rowCount,
                SearchValue = searchValue,
                SelectedCountry = country,
            };

            return View(model);
        }

        /// <summary>
        /// View page: Add new or update employee
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Input(string id = "")
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["HeaderTitle"] = "Create new employee";
                    Employee newEmployee = new Employee()
                    {
                        EmployeeID = 0
                    };
                    return View(newEmployee);

                }
                else
                {
                    TempData["HeaderTitle"] = "Edit employee";
                    Employee editEmployee = CatalogBLL.GetEmployee(Convert.ToInt32(id));
                    if (editEmployee == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(editEmployee);
                }
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message + ": " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Add new or update employee
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Input(EmployeePostRequest model, string id = "")
        {
            try
            {
                if (string.IsNullOrEmpty(model.LastName))
                    ModelState.AddModelError("LastName", "Last name expected");

                if (string.IsNullOrEmpty(model.FirstName))
                    ModelState.AddModelError("FirstName", "First name expected");

                if (string.IsNullOrEmpty(model.Title))
                    ModelState.AddModelError("Title", "Title expected");

                if (model.BirthDate.Year < 1753 || model.BirthDate.Year > 9999)
                    ModelState.AddModelError("BirthDate", "BirthDate's year must be between 1753 and 9999");

                if (model.HireDate.Year < 1753 || model.HireDate.Year > 9999)
                    ModelState.AddModelError("HireDate", "HireDate's year must be between 1753 and 9999");

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

                if (string.IsNullOrEmpty(model.Notes))
                    model.Notes = "";

                string photoPath = UploadedFile(model);
                photoPath = string.IsNullOrEmpty(id)
                    ? ""
                    : string.IsNullOrEmpty(photoPath)
                        ? CatalogBLL.GetEmployee(model.EmployeeID).PhotoPath
                        : photoPath;

                Employee employee = new Employee
                {
                    EmployeeID = model.EmployeeID,
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Title = model.Title,
                    BirthDate = model.BirthDate,
                    HireDate = model.HireDate,
                    Email = model.Email,
                    Address = model.Address,
                    City = model.City,
                    Country = model.Country,
                    HomePhone = model.HomePhone,
                    Notes = model.Notes,
                    PhotoPath = photoPath,
                };

                // TODO: Save input into DB
                if (model.EmployeeID == 0)
                {
                    // Set default password & role for new employee
                    employee.Password = _passwordHasher.Hash(LiteCommerce.Common.Constants.DefaultPassword);
                    employee.Roles = WebUserRoles.SALEMAN;

                    CatalogBLL.AddEmployee(employee);
                }
                else
                {
                    CatalogBLL.UpdateEmployee(employee);
                }
                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message + ": " + ex.StackTrace);
                ViewData["HeaderTitle"] = string.IsNullOrEmpty(id)
                    ? "Create new employee"
                    : "Edit employee";
                return View(model);
            }
        }

        /// <summary>
        /// Delete employees
        /// </summary>
        /// <param name="employeeIDs"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(int[] employeeIDs)
        {
            CatalogBLL.DeleteEmployees(employeeIDs);
            return RedirectToAction("Index");
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
