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
    [Authorize]
    public class AttributesController : Controller
    {
        private readonly ILogger<AttributesController> _logger;

        public AttributesController(ILogger<AttributesController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string categoryID = "0")
        {
            int rowCount = 0;
            List<LiteCommerce.DomainModels.Attribute> listOfAttribute = CatalogBLL.ListOfAttribute(categoryID ?? "", out rowCount);

            var model = new Models.AttributePaginationResult()
            {
                Data = listOfAttribute,
                RowCount = rowCount,
                CategoryID = categoryID,
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
                    ViewData["HeaderTitle"] = "Create new attribute";
                    LiteCommerce.DomainModels.Attribute newAttribute = new LiteCommerce.DomainModels.Attribute()
                    {
                        AttributeID = 0
                    };
                    return View(newAttribute);
                }
                else
                {
                    ViewData["HeaderTitle"] = "Edit attribute";
                    LiteCommerce.DomainModels.Attribute editAttribute = CatalogBLL.GetAttribute(Convert.ToInt32(id));
                    if (editAttribute == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(editAttribute);
                }
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message + ": " + ex.StackTrace);
            }
        }

        [HttpPost]
        public IActionResult Input(LiteCommerce.DomainModels.Attribute model)
        {
            try
            {
                CheckNotNull(model);

                // Save data into DB
                if (model.AttributeID == 0)
                {
                    CatalogBLL.AddAttribute(model);
                }
                else
                {
                    CatalogBLL.UpdateAttribute(model);
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

        [HttpPost]
        public IActionResult Delete(int[] attributeIDs)
        {
            CatalogBLL.DeleteAttributes(attributeIDs);
            return RedirectToAction("Index");
        }

        private void CheckNotNull(LiteCommerce.DomainModels.Attribute attribute)
        {
            if (string.IsNullOrEmpty(attribute.AttributeName))
                ModelState.AddModelError("AttributeName", "Attribute name expected");
            if (attribute.CategoryID <= 0)
                ModelState.AddModelError("CategoryID", "Category ID expected");

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
