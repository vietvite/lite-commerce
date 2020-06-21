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

        [HttpPost]
        public IActionResult New(OrderPostRequest model)
        {
            try
            {
                // CheckNotNull(model);
                // SetEmptyNullableField(model);

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
                    UnitPrice = model.UnitPrice,
                    Quantity = model.Quantity,
                    Discount = model.Discount,
                };
                // Save data into DB
                if (model.OrderID == 0)
                {
                    CatalogBLL.AddOrder(order);
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

        [HttpPost]
        public IActionResult AddProduct(OrderDetailPostRequest orderDetail, string id = "")
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return RedirectToAction("Index");

                OrderDetails order = new OrderDetails()
                {
                    OrderID = Convert.ToInt32(id),
                    Product = new Product()
                    {
                        ProductID = Convert.ToInt32(orderDetail.ProductID),
                    },
                    UnitPrice = orderDetail.UnitPrice,
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

        [HttpPost]
        public IActionResult Delete(int[] orderIDs)
        {
            // TODO: impl delete orders

            // CatalogBLL.DeleteCategories(orderIDs);
            return RedirectToAction("Index");
        }

        public class OrderDetailPostRequest
        {
            // Order details
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
            public decimal Discount { get; set; }
            public string ProductID { get; set; }
        }

        // private void CheckNotNull(Order order)
        // {
        //     if (string.IsNullOrEmpty(order.OrderName))
        //         ModelState.AddModelError("OrderName", "Order name expected");

        //     if (ModelState.ErrorCount > 0)
        //         throw new MissingFieldException();
        // }

        // private void SetEmptyNullableField(Order order)
        // {
        //     if (string.IsNullOrEmpty(order.Description))
        //         order.Description = "";
        // }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
