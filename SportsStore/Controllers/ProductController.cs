using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository repository;
        public int PageSize = 3;

        public ProductController(IProductRepository repo)
        {
            repository = repo;
        }

        /*
        public ViewResult List(int productPage = 1) => View(repository.Products
            .OrderBy(p => p.ProductID)
            .Skip((productPage - 1) * PageSize)
            .Take(PageSize)
            );
        */

        // dodawanie danych do widoku
        // można to zrealizowaćza pomocą mechanizmu ViewBag
        // ale lepszym rozwiązaniem jest opakowanie wszystkich danych wysyłanych z kontrolera do Widoku
        // pojedynczą klasą modelu widoku ProductListViewModel 
        public ViewResult List(string category, int productPage = 1) 
            => View( new ProductsListViewModel
            {
            Products = repository.Products
            .Where(p => category == null || p.Category == category)
            .OrderBy(p => p.ProductID)
            .Skip((productPage - 1) * PageSize)
            .Take(PageSize),

            PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ? repository.Products.Count() : repository.Products.Where(x=>x.Category == category).Count()

                },

                CurrentCategory = category
            });

        // -------------------------------------------------------------------------------------------------------------------------
        // metoda List zapisana w sposób tradycyjny, a nie za pomocą metody wcielającej wyrażenie
        // public ViewResult List(string category, int productPage = 1)
        // {
        //    return View(...)
        // }
        // -------------------------------------------------------------------------------------------------------------------------

    }

   
}
