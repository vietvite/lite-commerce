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
    [Authorize(Roles = WebUserRoles.DATA_MANAGER)]
    public class ShipperController : Controller
    {
        private readonly ILogger<ShipperController> _logger;

        public ShipperController(ILogger<ShipperController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            int pageSize = 10;
            List<Shipper> listOfShipper = CatalogBLL.ListOfShipper(page, pageSize, searchValue ?? "", out rowCount);

            var model = new Models.ShipperPaginationResult()
            {
                Page = page,
                Data = listOfShipper,
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
                    ViewData["HeaderTitle"] = "Create new shipper";
                    Shipper newShipper = new Shipper()
                    {
                        ShipperID = 0
                    };
                    return View(newShipper);

                }
                else
                {
                    ViewData["HeaderTitle"] = "Edit shipper";
                    Shipper editShipper = CatalogBLL.GetShipper(Convert.ToInt32(id));
                    if (editShipper == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(editShipper);
                }
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message + ": " + ex.StackTrace);
            }
        }

        [HttpPost]
        public IActionResult Input(Shipper model)
        {
            try
            {
                CheckNotNull(model);
                SetEmptyNullableField(model);

                // Save data into DB
                if (model.ShipperID == 0)
                {
                    CatalogBLL.AddShipper(model);
                }
                else
                {
                    CatalogBLL.UpdateShipper(model);
                }
                return RedirectToAction("Index");
            }
            catch (MissingFieldException)
            {
                return View(model);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message + ": " + ex.StackTrace);
                return View(model);
            }
        }

        private void CheckNotNull(Shipper shipper)
        {
            if (string.IsNullOrEmpty(shipper.CompanyName))
                ModelState.AddModelError("CompanyName", "Company name expected");

            if (ModelState.ErrorCount > 0)
            {
                throw new MissingFieldException();
            }
        }

        private void SetEmptyNullableField(Shipper shipper)
        {
            if (string.IsNullOrEmpty(shipper.Phone))
                shipper.Phone = "";
        }



        [HttpPost]
        public IActionResult Delete(int[] shipperIDs)
        {
            CatalogBLL.DeleteShippers(shipperIDs);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
