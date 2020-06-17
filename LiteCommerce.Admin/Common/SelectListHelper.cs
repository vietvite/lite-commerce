using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using LiteCommerce.BusinessLayers;
using LiteCommerce.DomainModels;

namespace LiteCommerce
{
    public static class SelectListHelper
    {
        /// <summary>
        /// Select list các quốc gia
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> Countries(bool allowSelectAll = true)
        {
            int rowCount;
            List<Country> lstCountries = CatalogBLL.ListOfCountry(1, -1, "", out rowCount);
            List<SelectListItem> list = new List<SelectListItem>();
            if (allowSelectAll)
            {
                list.Add(new SelectListItem() { Value = "", Text = "-- All countries --" });
            }
            foreach (var country in lstCountries)
            {
                list.Add(new SelectListItem() { Value = country.CountryID, Text = country.CountryName });
            }
            return list;
        }

        public static List<SelectListItem> Categories(bool allowSelectAll = true)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (allowSelectAll)
            {
                list.Add(new SelectListItem() { Value = "", Text = "-- All categories --" });
            }
            int rowCount = 0;
            List<Category> listCategories = CatalogBLL.ListOfCategory(1, -1, "", out rowCount);
            if (listCategories != null)
            {
                foreach (var cate in listCategories)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = string.Format("{0}", cate.CategoryID),
                        Text = cate.CategoryName
                    });
                }
            }

            return list;
        }

        public static List<SelectListItem> Suppliers(bool allowSelectAll = true)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (allowSelectAll)
            {
                list.Add(new SelectListItem() { Value = "", Text = "-- All suppliers --" });
            }
            int rowCount = 0;
            List<Supplier> listSuppliers = CatalogBLL.ListOfSupplier(1, -1, "", out rowCount);
            if (listSuppliers != null)
            {
                foreach (var supplier in listSuppliers)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = string.Format("{0}", supplier.SupplierId),
                        Text = supplier.CompanyName
                    });
                }
            }

            return list;
        }

        public static List<SelectListItem> Employees(bool allowSelectAll = true)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (allowSelectAll)
            {
                list.Add(new SelectListItem() { Value = "", Text = "-- All employees --" });
            }
            int rowCount = 0;
            List<Employee> listEmployees = CatalogBLL.ListOfEmployee(1, -1, "", out rowCount);
            if (listEmployees != null)
            {
                foreach (var employee in listEmployees)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = string.Format("{0}", employee.EmployeeID),
                        Text = employee.FirstName + " " + employee.LastName,
                    });
                }
            }

            return list;
        }

        public static List<SelectListItem> Shippers(bool allowSelectAll = true)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (allowSelectAll)
            {
                list.Add(new SelectListItem() { Value = "", Text = "-- All shippers --" });
            }
            int rowCount = 0;
            List<Shipper> listShippers = CatalogBLL.ListOfShipper(1, -1, "", out rowCount);
            if (listShippers != null)
            {
                foreach (var shipper in listShippers)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = string.Format("{0}", shipper.ShipperID),
                        Text = shipper.CompanyName,
                    });
                }
            }

            return list;
        }
    }
}