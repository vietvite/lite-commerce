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
        public static List<SelectListItem> Countries()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "USA", Text = "United State" });
            list.Add(new SelectListItem() { Value = "UK", Text = "England" });
            list.Add(new SelectListItem() { Value = "VN", Text = "Vietnam" });
            return list;
        }

        public static List<SelectListItem> Categories(bool allowSelectAll = true)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if(allowSelectAll) {
                list.Add(new SelectListItem() { Value = "0", Text = "-- All categories --" });
            }
            int rowCount = 0;
            List<Category> listCategories = CatalogBLL.ListOfCategory(1,-1,"", out rowCount);
            if(listCategories != null) {
                foreach (var cate in listCategories)
                {
                    list.Add(new SelectListItem() { 
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
            if(allowSelectAll) {
                list.Add(new SelectListItem() { Value = "0", Text = "-- All suppliers --" });
            }
            int rowCount = 0;
            List<Supplier> listSuppliers = CatalogBLL.ListOfSupplier(1,-1,"", out rowCount);
            if(listSuppliers != null) {
                foreach (var supplier in listSuppliers)
                {
                    list.Add(new SelectListItem() { 
                        Value = string.Format("{0}", supplier.SupplierId), 
                        Text = supplier.CompanyName
                    });
                }
            }

            return list;
        }
    }
}