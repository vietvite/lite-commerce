using System.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LiteCommerce.Models;
using Microsoft.AspNetCore.Authorization;
using LiteCommerce.DomainModels;
using LiteCommerce.BusinessLayers;

namespace LiteCommerce.Controllers
{
    [Authorize]
    public class CountriesController : Controller
    {
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(ILogger<CountriesController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            int pageSize = 10;
            List<Country> listOfCountry = CatalogBLL.ListOfCountry(page, pageSize, searchValue ?? "", out rowCount);

            var model = new Models.CountryPaginationResult()
            {
                Page = page,
                Data = listOfCountry,
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
                    ViewData["HeaderTitle"] = "Create new country";
                    Country newCountry = new Country();
                    return View(newCountry);
                }
                else
                {
                    ViewData["HeaderTitle"] = "Edit country";
                    Country editCountry = CatalogBLL.GetCountry(id);
                    if (editCountry == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(editCountry);
                }
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message + ": " + ex.StackTrace);
            }
        }

        [HttpPost]
        public IActionResult Input(Country model, string id = "")
        {
            try
            {
                CheckNotNull(model);

                if (string.IsNullOrEmpty(id))
                    CatalogBLL.AddCountry(model);
                else
                    CatalogBLL.UpdateCountry(model);

                return RedirectToAction("Index");
            }
            catch (MissingFieldException)
            {
                ViewData["CountryID"] = model.CountryID;
                ViewData["CountryName"] = model.CountryName;
                return View(new Country());
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message + ": " + ex.StackTrace);
                if (ex.Message.Contains("The duplicate key value is"))
                    ModelState.AddModelError("CountryID", model.CountryID + " is already existed.");

                ViewData["CountryID"] = model.CountryID;
                ViewData["CountryName"] = model.CountryName;
                return View(new Country());
            }
        }

        [HttpPost]
        public IActionResult Delete(string[] countryIDs)
        {
            CatalogBLL.DeleteCountries(countryIDs);
            return RedirectToAction("Index");
        }

        private void CheckNotNull(Country country)
        {
            if (string.IsNullOrEmpty(country.CountryID))
                ModelState.AddModelError("CountryID", "Country ID expected");
            if (string.IsNullOrEmpty(country.CountryName))
                ModelState.AddModelError("CountryName", "Country name expected");

            if (ModelState.ErrorCount > 0)
                throw new MissingFieldException();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
