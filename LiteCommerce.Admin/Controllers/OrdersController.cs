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
    // [Authorize(Roles = WebUserRoles.DATA_MANAGER)]
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ILogger<OrdersController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(
            int page = 1,
            string searchValue = "",
            string country = "",
            string category = "",
            string employee = "",
            string shipper = ""
        )
        {
            int rowCount = 0;
            int pageSize = 10;
            List<Order> listOfOrder = CatalogBLL.ListOfOrder(page, pageSize, out rowCount, country ?? "", category ?? "", employee ?? "", shipper ?? "");

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
                // if (string.IsNullOrEmpty(id))
                // {
                ViewData["HeaderTitle"] = "Create new order";
                Order newOrder = new Order()
                {
                    OrderID = 0
                };
                return View(newOrder);
                // }
                // else
                // {
                //     ViewData["HeaderTitle"] = "Edit order";
                //     OrderDetails editOrder = CatalogBLL.GetOrder(Convert.ToInt32(id));
                //     if (editOrder == null)
                //     {
                //         return RedirectToAction("Index");
                //     }
                //     return View(editOrder);
                // }
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message + ": " + ex.StackTrace);
            }
        }

        [HttpPost]
        public IActionResult New(Order model)
        {
            try
            {
                // CheckNotNull(model);
                // SetEmptyNullableField(model);

                // Save data into DB
                if (model.OrderID == 0)
                {
                    CatalogBLL.AddOrder(model);
                }
                else
                {
                    CatalogBLL.UpdateOrder(model);
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
        public IActionResult Delete(int[] orderIDs)
        {
            CatalogBLL.DeleteCategories(orderIDs);
            return RedirectToAction("Index");
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
