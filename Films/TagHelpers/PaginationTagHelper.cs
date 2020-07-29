﻿using System;
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
        private readonly int MaxPagesToView = 4;
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

            //TagBuilder currentItem = CreateTag(PageModel.PageNumber, urlHelper);
            TagBuilder item = null;
            //var startIndex = Math.Max(PageModel.PageNumber - MaxPagesToView / 2, 0);
            var startIndex = 0;
            startIndex = PageModel.PageNumber - MaxPagesToView / 2;
            startIndex = startIndex > 0 ? startIndex : MaxPagesToView/2 - PageModel.PageNumber;
            var endIndex = 0;
            endIndex = PageModel.PageNumber + MaxPagesToView / 2;
            endIndex = endIndex <= PageModel.TotalPages ? endIndex : PageModel.TotalPages + MaxPagesToView / 2 - PageModel.PageNumber;
            if (PageModel.TotalPages > MaxPagesToView && PageModel.PageNumber > MaxPagesToView / 2) 
            {
                item  = AddLinkToStart(urlHelper);
                tag.InnerHtml.AppendHtml(item);
            }
            if (PageModel.PageNumber > MaxPagesToView / 2 && PageModel.PageNumber<PageModel.TotalPages-1)
            {
                item = AddMultiPoint(urlHelper);//добавить многоточие слева
                tag.InnerHtml.AppendHtml(item);
            }
            for (int i=startIndex;i<endIndex; i++)
            {
                item = CreateTag(i+1, urlHelper);
                tag.InnerHtml.AppendHtml(item);
            }
            var countRight = PageModel.TotalPages - PageModel.PageNumber - MaxPagesToView / 2;//Для подсчета того, нужно ли добавлять многоточие справа
            countRight = countRight > 0 ? MaxPagesToView:0;
            if (countRight > MaxPagesToView/2)
            {
                item= AddMultiPoint(urlHelper);//добавить многоточие справа
                tag.InnerHtml.AppendHtml(item);
            }
            if (PageModel.TotalPages > MaxPagesToView && PageModel.PageNumber < PageModel.TotalPages) 
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