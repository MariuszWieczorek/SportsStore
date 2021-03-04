using System.Collections.Generic;
using System.Linq;

// klasa Cart korzysta z zdefinowanej w tym samym pliku klasy CartLine
// klasa CartLine - do reprezentowania produktu wybranego przez klienta
// metody pozwalające na
// dodanie elementu do koszyka
// usunięcie elementu z koszyka
// obliczenie całkowitej wartości towarów
// wyzerowanie poprzez usunięcie wszystkich towarów
// udostępniamy właściwość dającą dostęp do zawartości koszyka

namespace SportsStore.Models
{
    public class Cart
    {
        private List<CartLine> lineCollection = new List<CartLine>();

    
        public virtual void AddItem(Product product, int quantity)
        {
            CartLine line = lineCollection
                .Where(p => p.Product.ProductID == product.ProductID)
                .FirstOrDefault();

            if (line == null)
            {
                lineCollection.Add(new CartLine
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public virtual void RemoveLine(Product product) =>
            lineCollection.RemoveAll(l => l.Product.ProductID ==
                product.ProductID);

        public virtual decimal ComputeTotalValue() =>
            lineCollection.Sum(e => e.Product.Price * e.Quantity);

        public virtual void Clear() => lineCollection.Clear();

        public virtual IEnumerable<CartLine> Lines => lineCollection;
    }

    public class CartLine
    {
        public int CartLineID { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}