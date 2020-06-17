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

namespace LiteCommerce.Controllers
{
    [Authorize(Roles = WebUserRoles.DATA_MANAGER)]
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int page = 1, string searchValue = "", string country = "")
        {
            int rowCount = 0;
            int pageSize = 10;
            List<Customer> listOfCustomer = CatalogBLL.ListOfCustomer(page, pageSize, searchValue ?? "", out rowCount, country ?? "");

            var model = new Models.CustomerPaginationResult()
            {
                Page = page,
                Data = listOfCustomer,
                PageSize = pageSize,
                RowCount = rowCount,
                SearchValue = searchValue,
                Country = country,
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Input(String id = "")
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    ViewData["HeaderTitle"] = "Create new customer";
                    Customer newCustomer = new Customer()
                    {
                        // CustomerID = "0"
                    };
                    return View(newCustomer);
                }
                else
                {
                    ViewData["HeaderTitle"] = "Edit customer";
                    _logger.LogWarning("->" + id + "<-");
                    Customer editCustomer = CatalogBLL.GetCustomer(id);
                    if (editCustomer == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(editCustomer);
                }
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message + ": " + ex.StackTrace);
            }
        }

        [HttpPost]
        public IActionResult Input(Customer model)
        {
            try
            {
                CheckNotNull(model);
                SetEmptyNullableField(model);

                // Save data into DB
                if (model.CustomerID == null)
                {
                    model.CustomerID = Guid.NewGuid().ToString().Substring(0, 5); ;
                    CatalogBLL.AddCustomer(model);
                }
                else
                {
                    CatalogBLL.UpdateCustomer(model);
                }
                return RedirectToAction("Index");
            }
            catch (MissingFieldException)
            {
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ":\n" + ex.StackTrace);
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Delete(string[] customerIDs)
        {
            CatalogBLL.DeleteCustomers(customerIDs);
            return RedirectToAction("Index");
        }

        private void CheckNotNull(Customer supplier)
        {
            if (string.IsNullOrEmpty(supplier.CompanyName))
                ModelState.AddModelError("CompanyName", "Company name expected");

            if (ModelState.ErrorCount > 0)
            {
                throw new MissingFieldException();
            }
        }

        private void SetEmptyNullableField(Customer supplier)
        {
            if (string.IsNullOrEmpty(supplier.ContactName))
                supplier.ContactName = "";

            if (string.IsNullOrEmpty(supplier.ContactTitle))
                supplier.ContactTitle = "";

            if (string.IsNullOrEmpty(supplier.Address))
                supplier.Address = "";

            if (string.IsNullOrEmpty(supplier.City))
                supplier.City = "";

            if (string.IsNullOrEmpty(supplier.Country))
                supplier.Country = "";

            if (string.IsNullOrEmpty(supplier.Phone))
                supplier.Phone = "";

            if (string.IsNullOrEmpty(supplier.Fax))
                supplier.Fax = "";
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
