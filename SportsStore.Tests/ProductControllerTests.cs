using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using Xunit;


namespace SportsStore.Tests
{
    public class ProductControllerTests
    {
        [Fact]
        public void Can_Paginate()
        {
            // Przygotowanie.
            // tworzymy imitację repozytorium
            Mock<IProductRepository> mockx = new Mock<IProductRepository>();

            mockx.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"},
                new Product {ProductID = 5, Name = "P6"},
                new Product {ProductID = 5, Name = "P7"}
            }).AsQueryable<Product>());

            // wstrzykujemy powyższą imitację repozytorium do konstruktora klasy ProductController
            ProductController controller = new ProductController(mockx.Object);
            controller.PageSize = 3;

            // Działanie.
            // następnie wykonujemy metodę List dla określonej strony

            // IEnumerable<Product> result = controller.List(2).ViewData.Model as IEnumerable<Product>;
            // po opakowaniu w klasę ProductsListViewModel
            ProductsListViewModel result = controller.List(null,2).ViewData.Model as ProductsListViewModel;

            // Asercje.
            // porównujemy obiekty Product jakie otrzymamy z tymi jakich się spodziewaliśmy
            Product[] prodArray = result.Products.ToArray();

            Assert.True(prodArray.Length == 3);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {

            // Musimy się upewnić, że kontroler przesyła do widoku prawidłowe dane stronicowania

            // Przygotowanie.
            // tworzymy imitację repozytorium
            Mock<IProductRepository> mockx = new Mock<IProductRepository>();

            mockx.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"},
                new Product {ProductID = 6, Name = "P6"},
                new Product {ProductID = 7, Name = "P7"}
            }).AsQueryable<Product>());

            // Przygotowanie.
            // wstrzykujemy powyższą imitację repozytorium do konstruktora klasy ProductController
            // ustawiamy pole controller.PageSize = 3 w inny sposób 
            ProductController controller = new ProductController(mockx.Object) { PageSize = 3 };
            
            // Działanie.
            // ustawiamy się na 2-giej stronie
            ProductsListViewModel result = controller.List(null,2).ViewData.Model as ProductsListViewModel;

            // Asercje.
            // sprawdzamy czy kontroler zapisał prawidłowe dane do PagingInfo
            // czy wyliczył poszczególne właściwości zgodnie z oczekiwaniami

            PagingInfo pageInfo = result.PagingInfo;
            // sprawdzamy czy rzeczywiście ustawiona jest 2-ga strona zgodnie z parametrem przekazanym do metody List()
            Assert.Equal(2, pageInfo.CurrentPage);
            // czy mamy skonfigurowane 3 elementy do wyświetlenia na stronie
            Assert.Equal(3, pageInfo.ItemsPerPage);

            Assert.Equal(7, pageInfo.TotalItems);
            // czy w sumie wyjdzie nam 3 strony do wyświetlenia
            Assert.Equal(3, pageInfo.TotalPages);
        }

        [Fact]
        public void Can_Filter_Products()
        {
            // Przygotowanie.
            // Utworzenie imitacji repozytorium.
            Mock<IProductRepository> mockX = new Mock<IProductRepository>();
            mockX.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            }).AsQueryable<Product>());

            // Przygotowanie — utworzenie kontrolera i ustawienie 3-elementowej strony.
            // wstrzykujemy powyższą imitację repozytorium do konstruktora klasy ProductController
            ProductController controller = new ProductController(mockX.Object);
            controller.PageSize = 3;

            // Działanie.
            Product[] result =
                (controller.List("Cat2", 1).ViewData.Model as ProductsListViewModel)
                    .Products.ToArray();

            // Asercje.
            // z kategorią Cat2 powinny byś dwa produkty
            Assert.Equal(2, result.Length);
            // sprawdzamu czy są to obiekty o nazwie P2 i P4
            Assert.True(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.True(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        // kontrola, czy prawidłowo działa zliczanie produktów w przypadku filtrowania
        [Fact]
        public void Generate_Category_Specific_Product_Count()
        {
            // Przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            }).AsQueryable<Product>());

            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            Func<ViewResult, ProductsListViewModel> GetModel;
            GetModel = result => result?.ViewData?.Model as ProductsListViewModel;

            // Działanie.
            int? res1 = GetModel(target.List("Cat1"))?.PagingInfo.TotalItems;
            int? res2 = GetModel(target.List("Cat2"))?.PagingInfo.TotalItems;
            int? res3 = GetModel(target.List("Cat3"))?.PagingInfo.TotalItems;
            int? resAll = GetModel(target.List(null))?.PagingInfo.TotalItems;

            // Asercje.
            Assert.Equal(2, res1);
            Assert.Equal(2, res2);
            Assert.Equal(1, res3);
            Assert.Equal(5, resAll);
        }
    }
}