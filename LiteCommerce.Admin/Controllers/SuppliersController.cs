using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LiteCommerce.Models;
using Microsoft.AspNetCore.Authorization;
using LiteCommerce.BusinessLayers;
using LiteCommerce.DomainModels;

namespace LiteCommerce.Controllers
{
    [Authorize]
    public class SuppliersController : Controller
    {
        private readonly ILogger<SuppliersController> _logger;

        public SuppliersController(ILogger<SuppliersController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            int pageSize = 10;
            List<Supplier> listOfSupplier = CatalogBLL.ListOfSupplier(page, pageSize, searchValue ?? "", out rowCount);

            var model = new Models.SupplierPaginationResult()
            {
                Page = page,
                Data = listOfSupplier,
                PageSize = pageSize,
                RowCount = rowCount,
                SearchValue = searchValue,
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Input(string id = "")
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    ViewData["HeaderTitle"] = "Create new supplier";
                    Supplier newSupplier = new Supplier()
                    {
                        SupplierId = 0
                    };
                    return View(newSupplier);

                }
                else
                {
                    ViewData["HeaderTitle"] = "Edit supplier";
                    Supplier editSupplier = CatalogBLL.GetSupplier(Convert.ToInt32(id));
                    if (editSupplier == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(editSupplier);
                }
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message + ": " + ex.StackTrace);
            }
        }

        [HttpPost]
        public IActionResult Input(Supplier supplier)
        {
            try
            {
                CheckNotNull(supplier);
                SetEmptyNullableField(supplier);

                // Save data into DB
                if (supplier.SupplierId == 0)
                {
                    CatalogBLL.AddSupplier(supplier);
                }
                else
                {
                    CatalogBLL.UpdateSupplier(supplier);
                }
                return RedirectToAction("Index");
            }
            catch (MissingFieldException)
            {
                return View(supplier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ": " + ex.StackTrace);
                return View(supplier);
            }
        }

        [HttpPost]
        public IActionResult Delete(int[] supplierIDs)
        {
            CatalogBLL.DeleteSuppliers(supplierIDs);
            return RedirectToAction("Index");
        }

        private void CheckNotNull(Supplier supplier)
        {
            if (string.IsNullOrEmpty(supplier.CompanyName))
            {
                ModelState.AddModelError("CompanyName", "Company name expected");
            }
            if (string.IsNullOrEmpty(supplier.ContactName))
            {
                ModelState.AddModelError("ContactName", "Contact name expected");
            }
            if (ModelState.ErrorCount > 0)
            {
                throw new MissingFieldException();
            }
        }

        private void SetEmptyNullableField(Supplier supplier)
        {
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

            if (string.IsNullOrEmpty(supplier.HomePage))
                supplier.HomePage = "";
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
