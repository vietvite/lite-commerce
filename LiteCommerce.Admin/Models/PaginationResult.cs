using System;
namespace LiteCommerce.Models
{
    public abstract class PaginationResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public string SearchValue { get; set; }
        public int PageCount
        {
            get
            {
                int pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble((double)RowCount/PageSize)));
                return pageCount != 0 ? pageCount : 1;
            }
        }
    }
}