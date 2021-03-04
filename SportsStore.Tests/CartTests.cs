using System.Linq;
using SportsStore.Models;
using Xunit;

namespace SportsStore.Tests
{
    public class CartTests
    {

        // test czy dodanie do koszyka po raz pierwszy danego produktu spowoduje dodanie nowej pozycji
        [Fact]
        public void Can_Add_New_Lines()
        {
            // Przygotowanie — utworzenie produktów testowych.
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            // Przygotowanie — utworzenie nowego koszyka.
            Cart target = new Cart();

            // Działanie.
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            CartLine[] results = target.Lines.ToArray();

            // Asercje.
            Assert.Equal(2, results.Length);
            Assert.Equal(p1, results[0].Product);
            Assert.Equal(p2, results[1].Product);
            Assert.Equal(p2, results[1].Product);
        }

        // sprawdzamy czy dodanie do koszyka takiego produktu,
        // który już w nim jest spowoduje zwiększenie ilości a nie dodanie nowej pozycji
        [Fact]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            // Przygotowanie — tworzenie produktów testowych.
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            // Przygotowanie — utworzenie nowego koszyka.
            Cart target = new Cart();

            // Działanie.
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines
                .OrderBy(c => c.Product.ProductID).ToArray();

            // Asercje.
            Assert.Equal(2, results.Length);
            Assert.Equal(11, results[0].Quantity);
            Assert.Equal(1, results[1].Quantity);
        }

        [Fact]
        public void Can_Remove_Line()
        {
            // Przygotowanie — tworzenie produktów testowych.
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            // Przygotowanie — utworzenie nowego koszyka.
            Cart target = new Cart();

            // Przygotowanie — dodanie kilku produktów do koszyka.
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            // Działanie.
            target.RemoveLine(p2);

            // Asercje.
            Assert.Equal(0, target.Lines.Where(c => c.Product == p2).Count());
            Assert.Equal(2, target.Lines.Count());
        }

        [Fact]
        public void Calculate_Cart_Total()
        {
            // Przygotowanie — tworzenie produktów testowych.
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            // Przygotowanie — utworzenie nowego koszyka.
            Cart target = new Cart();

            // Działanie.
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();

            // Asercje.
            Assert.Equal(450M, result);
        }

        [Fact]
        public void Can_Clear_Contents()
        {
            // Przygotowanie — tworzenie produktów testowych.
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            // Przygotowanie — utworzenie nowego koszyka.
            Cart target = new Cart();

            // Przygotowanie — dodanie kilku produktów do koszyka.
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);


            // Działanie — czyszczenie koszyka.
            target.Clear();

            // Asercje.
            Assert.Equal(0, target.Lines.Count());
        }
    }
}