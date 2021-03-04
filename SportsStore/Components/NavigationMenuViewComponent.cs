using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportsStore.Models;

// komponent widoku generujący menu nawigacyjne i integrujący się z aplikacją przez wywołanie komponentu z
// współdzielonego układu w naszym przypadku z view\shared\_layout.cshtml

    

namespace SportsStore.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private IProductRepository repository;
        public NavigationMenuViewComponent(IProductRepository repo)
        {
            repository = repo;
        }

        // metoda Invoke jest wywoływana w chwili użycia komponentu przez silnik Razor
        public IViewComponentResult Invoke()
        {
            // klasa bazowa ViewComponent podobnie jak Controler przez zestaw właściwości zapewnia
            // dostęp do obiektu kontekstu jedna z nich RouteData dostarcza informacje jaki adres URL zostało bsłużony przez system routinngu
            
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            // do widoku przekazujemy po przez obiekt ViewBg
            // warto by było utworzyć kolejną klasę modelu widoku i za pomocą niej przekazać informację o wybranej kategorii 
            // w przypadku komponentów szukamy widoku 
            // Shared/Components/NavigationMenu/Default.cshtml
            return View(repository.Products
                .Select(x => x.Category)
                .Distinct()
                .OrderBy(x => x));
                
        }
    }
}
