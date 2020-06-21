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
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace LiteCommerce.Controllers
{
    [Authorize(Roles = WebUserRoles.DATA_MANAGER)]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ProductController(ILogger<ProductController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index(int page = 1, string searchValue = "", string category = "", string supplier = "")
        {
            int rowCount = 0;
            int pageSize = 10;

            List<Product> listOfProduct = CatalogBLL.ListOfProduct(page, pageSize, searchValue ?? "", out rowCount, category, supplier);

            var model = new Models.ProductPaginationResult()
            {
                Page = page,
                Data = listOfProduct,
                PageSize = pageSize,
                RowCount = rowCount,
                SearchValue = searchValue,
                SelectedCategory = category,
                SelectedSupplier = supplier,
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Input(string id = "")
        {
            try
            {
                // Create Product
                if (string.IsNullOrEmpty(id))
                {
                    ViewData["HeaderTitle"] = "Create new product";
                    Product newProduct = new Product()
                    {
                        ProductID = 0
                    };
                    return View(newProduct);
                }
                else
                {
                    // Edit Product
                    ViewData["HeaderTitle"] = "Edit product";
                    Product editProduct = CatalogBLL.GetProduct(Convert.ToInt32(id));

                    if (editProduct == null)
                    {
                        return RedirectToAction("Index");
                    }

                    // Get list attributes of product for attribute tab
                    List<ProductAttribute> listAttribute = CatalogBLL.ListOfAttribute(Convert.ToInt32(id));
                    ViewData["listAttribute"] = listAttribute;

                    return View(editProduct);
                }
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message + ": " + ex.StackTrace);
            }
        }

        [HttpPost]
        public IActionResult Input(ProductPostRequest model, ProductAttribute attribute, List<ProductAttribute> lstAttribute, string id = "", string query = "")
        {
            if (query != null && query == "AddAttribute")
            {
                // Attribute POST
                CatalogBLL.AddProductAttribute(attribute);
                return RedirectToAction("Input", new { id = id });
            }
            else
            {
                // Product POST
                try
                {
                    CheckNotNull(model);
                    SetEmptyNullableField(model);

                    string photoPath = UploadedFile(model);
                    photoPath = string.IsNullOrEmpty(id)
                        ? ""
                        : string.IsNullOrEmpty(photoPath)
                            ? CatalogBLL.GetProduct(Convert.ToInt32(id)).PhotoPath
                            : photoPath;

                    Product newProduct = new Product()
                    {
                        ProductID = model.ProductID,
                        ProductName = model.ProductName,
                        SupplierID = model.SupplierID,
                        CategoryID = model.CategoryID,
                        QuantityPerUnit = model.QuantityPerUnit,
                        UnitPrice = model.UnitPrice,
                        Descriptions = model.Descriptions,
                        PhotoPath = photoPath
                    };

                    // Save data into DB
                    if (newProduct.ProductID == 0)
                    {
                        CatalogBLL.AddProduct(newProduct);
                    }
                    else
                    {
                        CatalogBLL.UpdateProduct(newProduct);
                    }
                    return RedirectToAction("Index");
                }
                catch (MissingFieldException)
                {
                    return View(model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ": " + ex.StackTrace);
                    return View(model);
                }
            }
        }

        [HttpPost]
        public IActionResult Attribute(List<ProductAttribute> listAttribute, string id = "")
        {
            try
            {
                CatalogBLL.UpdateProductAttribute(listAttribute);
                return RedirectToAction("Input", new { id = id });
            }
            catch (MissingFieldException)
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ": " + ex.StackTrace);
                return View();
            }
        }

        [HttpGet]
        public IActionResult Attribute(string id = "", string attributeID = "")
        {
            try
            {
                CatalogBLL.DeleteProductAttributes(id, attributeID);
                return RedirectToAction("Input", new { id = id });
            }
            catch (MissingFieldException)
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ": " + ex.StackTrace);
                return View();
            }
        }

        [HttpPost]
        public IActionResult Delete(int[] productIDs)
        {
            CatalogBLL.DeleteProducts(productIDs);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Review(string id = "")
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("Product ID not provided");
                }
                ViewData["HeaderTitle"] = "Review product";
                Product product = CatalogBLL.GetProduct(Convert.ToInt32(id));
                if (product == null)
                    return RedirectToAction("Index");

                // Get list attributes of product for attribute tab
                List<ProductAttribute> listAttribute = CatalogBLL.ListOfAttribute(Convert.ToInt32(id));
                ViewData["listAttribute"] = listAttribute;

                return View(product);
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message + ": " + ex.StackTrace);
            }
        }

        private void CheckNotNull(ProductPostRequest product)
        {
            if (string.IsNullOrEmpty(product.ProductName))
            {
                ModelState.AddModelError("ProductName", "Company name expected");
            }
            if (product.CategoryID <= 0)
            {
                ModelState.AddModelError("CategoryID", "Category expected");
            }
            if (product.SupplierID <= 0)
            {
                ModelState.AddModelError("SupplierID", "Supplier expected");
            }

            if (ModelState.ErrorCount > 0)
            {
                throw new MissingFieldException();
            }
        }

        private void SetEmptyNullableField(ProductPostRequest product)
        {
            if (string.IsNullOrEmpty(product.QuantityPerUnit))
                product.QuantityPerUnit = "";

            if (product.UnitPrice <= 0)
                product.UnitPrice = -1;

            if (string.IsNullOrEmpty(product.Descriptions))
                product.Descriptions = "";
        }

        private string UploadedFile(ProductPostRequest model)
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
