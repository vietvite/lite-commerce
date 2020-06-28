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
    [Authorize(Roles = WebUserRoles.SALEMAN)]
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ILogger<OrdersController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// View page: List of order
        /// </summary>
        /// <param name="page"></param>
        /// <param name="country"></param>
        /// <param name="category"></param>
        /// <param name="employee"></param>
        /// <param name="shipper"></param>
        /// <returns></returns>
        public IActionResult Index(
            int page = 1,
            string country = "",
            string category = "",
            string employee = "",
            string shipper = ""
        )
        {
            int rowCount = 0;
            int pageSize = 10;
            List<Order> listOfOrder = CatalogBLL.ListOfOrder(page, pageSize, out rowCount,
                country ?? "", category ?? "", employee ?? "", shipper ?? "");

            var model = new Models.OrderPaginationResult()
            {
                Page = page,
                Data = listOfOrder,
                PageSize = pageSize,
                RowCount = rowCount,
                selectedCountry = country,
                selectedCategory = category,
                selectedEmployee = employee,
                selectedShipper = shipper,
            };

            return View(model);
        }

        /// <summary>
        /// View page: Order detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Review(string id = "")
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    List<OrderDetails> listOrderDetails = CatalogBLL.GetOrder(Convert.ToInt32(id));
                    if (listOrderDetails == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(listOrderDetails);
                }
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message + ": " + ex.StackTrace);
            }
        }

        /// <summary>
        /// View Page: Add new order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult New(string id = "")
        {
            try
            {
                ViewData["HeaderTitle"] = "Create new order";
                Order newOrder = new Order()
                {
                    OrderID = 0
                };
                return View(newOrder);
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message + ": " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Add view order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult New(OrderPostRequest model)
        {
            try
            {
                CheckNotNull(model);

                int rowCount = 0;
                List<Product> listOfProduct = CatalogBLL.ListOfProduct(1, -1, "", out rowCount, "", "");

                int unitPrice = 0;
                foreach (var product in listOfProduct)
                {
                    if (product.ProductID == model.OrderProduct)
                        unitPrice = product.UnitPrice;
                }

                OrderDetails order = new OrderDetails()
                {
                    OrderID = model.OrderID,
                    OrderDate = model.OrderDate,
                    RequiredDate = model.RequiredDate,
                    ShippedDate = model.ShippedDate,
                    Freight = model.Freight,
                    ShipAddress = model.ShipAddress,
                    ShipCity = model.ShipCity,
                    ShipCountry = model.ShipCountry,
                    Shipper = new Shipper()
                    {
                        ShipperID = Convert.ToInt32(model.ShipperID),
                    },
                    Customer = new Customer()
                    {
                        CustomerID = model.CustomerID,
                    },
                    Employee = new Employee()
                    {
                        EmployeeID = Convert.ToInt32(model.EmployeeID),
                    },
                    Product = new Product()
                    {
                        ProductID = Convert.ToInt32(model.OrderProduct),
                    },
                    UnitPrice = unitPrice,
                    Quantity = model.Quantity,
                    Discount = model.Discount,
                };
                // Save data into DB
                if (model.OrderID == 0)
                {
                    int newProductID = CatalogBLL.AddOrder(order);
                    return RedirectToAction("AddProduct", new { id = newProductID });
                }
                else
                {
                    CatalogBLL.UpdateOrder(order);
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

        [HttpGet]
        /// <summary>
        /// View page: add product to order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult AddProduct(string id = "")
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return RedirectToAction("Index");

                List<OrderDetails> listOrderDetails = CatalogBLL.GetOrder(Convert.ToInt32(id));
                if (listOrderDetails == null)
                    return RedirectToAction("Index");

                return View(listOrderDetails);
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message + ": " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Add product to order detail
        /// </summary>
        /// <param name="orderDetail"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddProduct(OrderDetailPostRequest orderDetail, string id = "")
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return RedirectToAction("Index");

                int rowCount = 0;
                List<Product> listOfProduct = CatalogBLL.ListOfProduct(1, -1, "", out rowCount, "", "");

                int unitPrice = 0;
                foreach (var product in listOfProduct)
                {
                    if (product.ProductID == Convert.ToInt32(orderDetail.ProductID))
                        unitPrice = product.UnitPrice;
                }

                OrderDetails order = new OrderDetails()
                {
                    OrderID = Convert.ToInt32(id),
                    Product = new Product()
                    {
                        ProductID = Convert.ToInt32(orderDetail.ProductID),
                    },
                    UnitPrice = unitPrice,
                    Quantity = orderDetail.Quantity,
                    Discount = orderDetail.Discount,
                };
                bool ok = CatalogBLL.UpdateOrder(order);
                if (ok)
                    return RedirectToAction("AddProduct", new { id = id });

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message + ": " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Order detail post request
        /// </summary>
        public class OrderDetailPostRequest
        {
            // Order details
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
            public decimal Discount { get; set; }
            public string ProductID { get; set; }
        }

        private void CheckNotNull(OrderPostRequest order)
        {
            if (order.Freight == 0)
                ModelState.AddModelError("Freight", "Freight expected");
            if (string.IsNullOrEmpty(order.ShipAddress))
                ModelState.AddModelError("ShipAddress", "Ship address expected");
            if (string.IsNullOrEmpty(order.ShipCity))
                ModelState.AddModelError("ShipCity", "Ship city expected");
            if (string.IsNullOrEmpty(order.ShipCountry))
                ModelState.AddModelError("ShipCountry", "Ship country expected");

            if (order.ShipperID == "0")
                ModelState.AddModelError("ShipperID", "Shipper expected");
            if (order.CustomerID == "0")
                ModelState.AddModelError("CustomerID", "Customer expected");
            if (order.EmployeeID == "0")
                ModelState.AddModelError("EmployeeID", "Employee expected");

            if (order.OrderProduct == 0)
                ModelState.AddModelError("OrderProduct", "OrderProduct expected");
            if (order.Quantity == 0)
                ModelState.AddModelError("Quantity", "Quantity expected");

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
