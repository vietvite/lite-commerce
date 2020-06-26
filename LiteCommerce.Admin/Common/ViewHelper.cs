using System.Collections.Generic;
using System;
using System.Linq;
using LiteCommerce.DomainModels;
using Microsoft.AspNetCore.Html;

namespace LiteCommerce.Common
{
    public static class ViewHelper
    {
        #region Utility function
        public static Func<string, string> MaybeSearchQuery =
            (SearchValue) => string.IsNullOrEmpty(SearchValue) ? "" : $"&searchvalue={SearchValue}";

        public static Func<int, string> JustPageQuery = (page) => $"page={page}";

        public static Func<int, string, string, string> EitherSingularOrPluralWord =
            (count, singularWord, pluralWord) => count > 1 ? pluralWord : singularWord;

        public static Func<decimal, decimal, decimal> CalcDiscountPrice =
            (UnitPrice, Discount) => UnitPrice - (UnitPrice * Discount);

        public static Func<List<OrderDetails>, decimal> CalcTotalPrice =
            list => list.Aggregate((decimal)0, (acc, product)
                => acc += product.UnitPrice - (product.UnitPrice * product.Discount));

        public static Func<string, string, string> EitherPhotoPathOrDefault =
            (PhotoPath, DefaultPath) => string.IsNullOrEmpty(PhotoPath)
                ? DefaultPath
                : $"/uploads/{PhotoPath}";

        public static Func<int, string> MaybeID = id => id != 0 ? $"{id}" : "";

        #endregion


        #region Paginate button
        // public static Func<string, int, string, string, IHtmlContent> RenderButton =
        //     (ControllerName, Page, SearchValue, DisplayContent) =>
        //         new HtmlContentBuilder().AppendHtml($"<li><a href = '{ControllerName}?{JustPageQuery(Page)}{MaybeSearchQuery(SearchValue)}'>{DisplayContent}</a></li>");
        public static Func<string, int, string, string, string, IHtmlContent> RenderButton =
            (ControllerName, Page, DisplayContent, SearchValue, OptionalQuery) =>
                new HtmlContentBuilder().AppendHtml($"<li><a href = '{ControllerName}?{JustPageQuery(Page)}{MaybeSearchQuery(SearchValue)}{OptionalQuery}'>{DisplayContent}</a></li>");

        public static IHtmlContent RenderPageButtonToRight(string ControllerName, int CurrentPage, int Width, string SearchValue, string OptionalQuery)
        {
            var html = new HtmlContentBuilder();
            for (int i = CurrentPage; i <= Width; i++)
                html.AppendHtml(RenderButton(ControllerName, i, Convert.ToString(i), SearchValue, OptionalQuery));

            return html;
        }
        public static IHtmlContent RenderPageButtonToLeft(string ControllerName, int CurrentPage, int Width, string SearchValue, string OptionalQuery)
        {
            var html = new HtmlContentBuilder();
            for (int i = Width; i <= CurrentPage; i++)
                html.AppendHtml(RenderButton(ControllerName, i, Convert.ToString(i), SearchValue, OptionalQuery));

            return html;
        }

        // public static Func<string, string, IEnumerable<int>, IHtmlContent> RenderListButton =
        //     (ControllerName, SearchValue, ListPageNumber) =>
        //         ListPageNumber.Aggregate(new HtmlContentBuilder(), (acc, iter) =>
        //             (HtmlContentBuilder)acc.AppendHtml(RenderButton(ControllerName, iter, SearchValue, Convert.ToString(iter))));

        public static IHtmlContent RenderPageButton(string ControllerName, int CurrentPage, int MaxPage, int Width, string SearchValue, string OptionalQuery = "")
        {
            var html = new HtmlContentBuilder();

            // For case range from `CurrentPage` to `Page1` < `Width`
            int MaxWidthLeft = CurrentPage - Width > 0
                ? CurrentPage - Width : 1;

            // For case range from `CurrentPage` to `PageMax` > `Width`
            int MaxWidthRight = (CurrentPage + Width) < MaxPage
                ? CurrentPage + Width : MaxPage;

            OptionalQuery = OptionalQuery ?? "";

            // | « |...|
            if (MaxWidthLeft > 1)
            {
                html.AppendHtml(RenderButton(ControllerName, 1, "«", SearchValue, OptionalQuery));
                html.AppendHtml("<li><a>...</a></li>");
            }

            // Page is first page => | 1 | 2 | 3 | 4 |...| » |
            if (CurrentPage == 1)
                html.AppendHtml(RenderPageButtonToRight(ControllerName, CurrentPage, MaxWidthRight, SearchValue, OptionalQuery));

            // Page is last page => |n-4|n-3|n-2|n-1| n |
            else if (CurrentPage == MaxPage)
                html.AppendHtml(RenderPageButtonToLeft(ControllerName, CurrentPage, MaxWidthLeft, SearchValue, OptionalQuery));

            // Page is between first and last page => |n-3|n-2|n-1| n |n+1|n+2|n+3|
            else
            {
                html.AppendHtml(RenderPageButtonToLeft(ControllerName, CurrentPage - 1, MaxWidthLeft, SearchValue, OptionalQuery));
                html.AppendHtml($"<li class='active'><a>{CurrentPage}</a></li>");
                html.AppendHtml(RenderPageButtonToRight(ControllerName, CurrentPage + 1, MaxWidthRight, SearchValue, OptionalQuery));
            }

            // |...| » |
            if (MaxWidthRight < MaxPage)
            {
                html.AppendHtml("<li><a>...</a></li>");
                html.AppendHtml(RenderButton(ControllerName, MaxPage, "»", SearchValue, OptionalQuery));
            }

            return html;
        }
        #endregion
    }
}