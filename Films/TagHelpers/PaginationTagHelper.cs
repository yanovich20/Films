using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using Films.Models;

namespace Films.TagHelpers
{
    public class PaginationTagHelper : TagHelper
    {
        private readonly int MaxPagesToView = 5;
        private IUrlHelperFactory urlHelperFactory;
        public PaginationTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PageViewModel PageModel { get; set; }
        public string PageAction { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";

            // набор ссылок будет представлять список ul
            TagBuilder tag = new TagBuilder("ul");
            tag.AddCssClass("pagination");

            int diff = 0;
            int diffToEnd = 0; 
            TagBuilder item = null;
            
            var startIndex = 0;

            startIndex = PageModel.PageNumber - (int)Math.Ceiling((double)MaxPagesToView / 2.0);
            
            if (startIndex <= 1)
            {
                 startIndex = 1;
                 diffToEnd = (int)Math.Ceiling((double)MaxPagesToView / 2.0) - PageModel.PageNumber + 1;
            }
            
            var endIndex = 0;
            
            endIndex = PageModel.PageNumber + MaxPagesToView / 2 + diffToEnd - 1;
           
            if (endIndex > PageModel.TotalPages)
            {
                endIndex = PageModel.TotalPages; 
                var diffValue = PageModel.TotalPages - PageModel.PageNumber + 1;
                diffValue = MaxPagesToView/2 - diffValue;
                startIndex = startIndex - diffValue;
            }
            if (PageModel.TotalPages > MaxPagesToView && PageModel.PageNumber > (int)Math.Ceiling((double)MaxPagesToView / 2.0)+1) 
            {
                item  = AddLinkToStart(urlHelper);
                tag.InnerHtml.AppendHtml(item);
            }
            if (PageModel.PageNumber > (int)Math.Ceiling((double)MaxPagesToView / 2.0) + 1)
            {
                item = AddMultiPoint(urlHelper);//добавить многоточие слева
                tag.InnerHtml.AppendHtml(item);
            }
            if(PageModel.TotalPages<=MaxPagesToView)
            {
                startIndex = 1;
                endIndex = PageModel.TotalPages;
            }
            for (int i=startIndex;i<=endIndex; i++)
            {
                item = CreateTag(i, urlHelper);
                tag.InnerHtml.AppendHtml(item);
            }
            var countRight = PageModel.TotalPages - PageModel.PageNumber - MaxPagesToView/2 + 1; //Для подсчета того, нужно ли добавлять многоточие справа
            bool multiPointIsNeeded = countRight > 0 ? true : false;
            if (multiPointIsNeeded)
            {
                item= AddMultiPoint(urlHelper);//добавить многоточие справа
                tag.InnerHtml.AppendHtml(item);
            }
            if (PageModel.TotalPages > MaxPagesToView && PageModel.PageNumber <= PageModel.TotalPages-1) 
            {
                item = AddLinkToEnd(urlHelper);
                tag.InnerHtml.AppendHtml(item);
            }
            output.Content.AppendHtml(tag);
        }

        TagBuilder AddLinkToStart(IUrlHelper helper) {
            return CreateTag("В начало", 1, helper);
        }

        TagBuilder AddLinkToEnd(IUrlHelper helper)
        {
            return CreateTag("В конец",PageModel.TotalPages, helper);
        }

        TagBuilder AddMultiPoint(IUrlHelper helper) {
            return CreateTag("...", -1, helper);
        }

        TagBuilder CreateTag(string message,  int pageNumber, IUrlHelper urlHelper )
        {
            TagBuilder item = new TagBuilder("li");
            TagBuilder link = new TagBuilder("a");
            if (pageNumber == this.PageModel.PageNumber)
            {
                item.AddCssClass("active");
            }
            else
            {
                if (pageNumber > 0)
                    link.Attributes["href"] = urlHelper.Action(PageAction, new { page = pageNumber });
                else
                    link.Attributes["href"] = "";
            }
            item.AddCssClass("page-item");
            link.AddCssClass("page-link");
            link.InnerHtml.Append(message);
            item.InnerHtml.AppendHtml(link);
            return item;
        }
        TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper)
        {
            TagBuilder item = new TagBuilder("li");
            TagBuilder link = new TagBuilder("a");
            if (pageNumber == this.PageModel.PageNumber)
            {
                item.AddCssClass("active");
            }
            else
            {
                link.Attributes["href"] = urlHelper.Action(PageAction, new { page = pageNumber });
            }
            item.AddCssClass("page-item");
            link.AddCssClass("page-link");
            link.InnerHtml.Append(pageNumber.ToString());
            item.InnerHtml.AppendHtml(link);
            return item;
        }
    }
}
