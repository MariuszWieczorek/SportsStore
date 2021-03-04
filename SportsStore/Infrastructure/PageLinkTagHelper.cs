using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SportsStore.Models.ViewModels;
using System.Collections.Generic;

// klasa atrybutu pomocniczego znacznika
// umieszcza on w elemencie <div> znaczniki <a> odpowiadające stronom produktów
// interfejs IUrlHelper i metody Action() jest najwygodniejszym sposobem generowania łączy wychodzących
// w atrybucie pomocniczym znacznika

namespace SportsStore.Infrastructure
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }

        public string PageAction { get; set; }


        // sposobem na otrzymywanie informacji dodatkowych z widoku bez konieczności dodawania kolejnych właściwości
        // do klasy atrybutu pomocniczego jest wykorzystanie przyjaznej funkcji pozwalającej 
        // wszystkie właściwości o takim samym prefiksie utrzymywać w jednej kolekcji

        // oznaczenie właściwości atrybutu pomocniczego znacznika
        // atrybutem HtmlAttributeName pozwala na określenie prefixu dla nazw atrybutu elementu w omawianym przykładzie to page-url-
        // Wartość dowolnego atrybutu, którego nazwa rozpoczyna się od podanego prefiksu do słownika przypisanego właściwości PageUrlValues
        // Ten słownik zostanie przekazany metodzie IUrlHelper.Action()

        /* poniższy kod w pliku List.cshtml odpowiedzialny za wstawienie numeracji stron,
         * dzięki powyższemu możemy wpisać: page-url-category="@Model.CurrentCategory"
         * gdy użytkownik kliknie na łącze kategoria będzie przekazana do metody akcji list() 
         * i filter zostanie zachowany
         * 
         <div page-model="@Model.PagingInfo" page-action="List" page-classes-enabled="true"
             page-class="btn"
             page-class-normal="btn-secondary"
             page-class-selected="btn-primary"
             page-url-category="@Model.CurrentCategory"
             class="btn-group btn-group-sm m-1">
        </div>
        */


        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

 
        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }
 

        // tworzymy w pętli znaczniki <a> - tyle znaczników ile
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");
            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                PageUrlValues["productPage"] = i;
                // Metoda Action() dostarczana przez interfejs IUrlHelper jest najwygodniejszym sposobem generowania łączy wychodzących
                tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);

                
                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage
                        ? PageClassSelected : PageClassNormal);
                }
                

                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}