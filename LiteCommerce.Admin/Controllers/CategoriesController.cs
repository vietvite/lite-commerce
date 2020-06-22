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
    public class CategoriesController : Controller
    {
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ILogger<CategoriesController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            int pageSize = 10;
            List<Category> listOfCategory = CatalogBLL.ListOfCategory(page, pageSize, searchValue ?? "", out rowCount);

            var model = new Models.CategoryPaginationResult()
            {
                Page = page,
                Data = listOfCategory,
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
                    ViewData["HeaderTitle"] = "Create new category";
                    Category newCategory = new Category()
                    {
                        CategoryID = 0
                    };
                    return View(newCategory);
                }
                else
                {
                    ViewData["HeaderTitle"] = "Edit category";
                    Category editCategory = CatalogBLL.GetCategory(Convert.ToInt32(id));
                    if (editCategory == null)
                        return RedirectToAction("Index");

                    return View(editCategory);
                }
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message + ": " + ex.StackTrace);
            }
        }

        [HttpPost]
        public IActionResult Input(Category model)
        {
            try
            {
                CheckNotNull(model);
                SetEmptyNullableField(model);

                // Save data into DB
                if (model.CategoryID == 0)
                {
                    CatalogBLL.AddCategory(model);
                }
                else
                {
                    CatalogBLL.UpdateCategory(model);
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
        public IActionResult Delete(int[] categoryIDs)
        {
            CatalogBLL.DeleteCategories(categoryIDs);
            return RedirectToAction("Index");
        }

        private void CheckNotNull(Category category)
        {
            if (string.IsNullOrEmpty(category.CategoryName))
                ModelState.AddModelError("CategoryName", "Category name expected");

            if (ModelState.ErrorCount > 0)
                throw new MissingFieldException();
        }

        private void SetEmptyNullableField(Category category)
        {
            if (string.IsNullOrEmpty(category.Description))
                category.Description = "";
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
