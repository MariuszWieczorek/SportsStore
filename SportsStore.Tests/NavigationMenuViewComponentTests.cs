using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Routing;
using Moq;
using SportsStore.Components;
using SportsStore.Models;
using Xunit;

namespace SportsStore.Tests
{
    public class NavigationMenuViewComponentTests
    {
        // cel utworzenie posortowanej listy pozbawionej duplikatów
        // dlatego używamy danych nieposortowanych zawierających powtarzające się kategorie
        // przekazujemy je klasie atrybutu pomocniczego znacznika
        // i sprawdzamy czy rezultat będzie zgodny z oczekiwaniami

        [Fact]
        public void Can_Select_Categories()
        {
            // Przygotowanie.
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Jabłka"},
                new Product {ProductID = 2, Name = "P2", Category = "Jabłka"},
                new Product {ProductID = 3, Name = "P3", Category = "Śliwki"},
                new Product {ProductID = 4, Name = "P4",
                    Category = "Pomarańcze"},
            }).AsQueryable<Product>());

            NavigationMenuViewComponent target =
                new NavigationMenuViewComponent(mock.Object);

            // Działanie — pobranie zbioru kategorii.
            string[] results = ((IEnumerable<string>)(target.Invoke()
                as ViewViewComponentResult).ViewData.Model).ToArray();

            // Asercje.
            Assert.True(Enumerable.SequenceEqual(new string[] { "Jabłka",
                "Pomarańcze", "Śliwki" }, results));
        }

        // sprawdzenie czy prawidłowo dodaje informacje na temat wybranej kategorii
        // przez przypisanie właściwości ViewBag
        // właściwość ta jest dostępna przez kalsę ViewViewComponentResult - s.256 rozdz. 22 szczegóły
        [Fact]
        public void Indicates_Selected_Category()
        {
            // Przygotowanie.
            string categoryToSelect = "Jabłka";
            Mock<IProductRepository> mock1 = new Mock<IProductRepository>();
            mock1.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Jabłka"},
                new Product {ProductID = 4, Name = "P2", Category = "Pomarańcze"},
            }).AsQueryable<Product>());

            NavigationMenuViewComponent target = new NavigationMenuViewComponent(mock1.Object);

            // test jednostkowy za pomocą ViewComponentContext dostarcza komponent widoku wraz z danymi routingu
            // komponent ten zapewnia dostęp do danych kontekstu konkretnego widoku
            // wykorzystujęc do tego właściwość ViewContext

            target.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    RouteData = new RouteData()
                }
            };

            target.RouteData.Values["category"] = categoryToSelect;

            // Działanie.
            string result = (string)(target.Invoke() as
                   ViewViewComponentResult).ViewData["SelectedCategory"];

            // Asercje.
            Assert.Equal(categoryToSelect, result);
        }
        
    }
}