using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SportsStore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SportsStore
{
    public class Startup
    {

        // konstruktor wczytujący ustawienia konfiguracyjne z pliku appsettings.json
        // przypisuje on obiekt do właściwości Configuration aby mógł byćwykorzystany w dalszej części klasy startup
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => Configuration = configuration;

        // This method gets called by the runtime.
        // Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        // jest używana w celu skonfigurowania współdzielonych obiektów
        // które będą mogły być używane w aplikacji za pomocą mechanizmu wstrzykiwania zależności
        public void ConfigureServices(IServiceCollection services)
        {
            // odczyt connectinon string
            // sekwencja metod odpowiedzilna za  konfigurację Entity Framework
            // AddDbContext() - przygotowuje usługi przygotowwane przez Entity Framework Core do użycia wraz z klasą kontekstu bazy danych
            // argumentem jest wyrażenia lambda które skonfiguruje bazę danych pod kątem kontekstu za pomocą metody UseSqlServer()
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                Configuration["Data2:SportStoreProducts:ConnectionString"]));
                            

            // wstrzykiwanie zależności - ustawienie jaka klasa ma obsługiwać interfejs IProductRepository
            // może to być EFProductRepository lub FakeProductRepository
            services.AddTransient<IProductRepository, EFProductRepository>();
            
            // 1. metoda rozszerzenia odpowiedzialna za skonfigurowanie obiektów współdzielonych używanych w aplikacjach MVC
            services.AddMvc();
        }

        // This method gets called by the runtime. 
        // Use this method to configure the HTTP request pipeline.
        // metoda do skonfigurowania potoku żądania
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            // konfigurowanie domyślenj trasy
            // ruting jest odpowiedzialny za przetwarzanie adresów URL i ustalenie docelowego fragmentu aplikacji
            // informujemy, że żądania dotyczące katalogu głównego witryny
            // powinny być kierowane do metody List klasy ProductController
            // trasy są przetwarzane w kolejności definiowania
            // System rutingu jest używany przez MVC do obsługi żądań przychodzących od klientów
            // ale również do generowania wychodzącycha adresów URL
            // /            - wyświetla pierwszą stronę we wszystkich kategoriach
            // /Strona2     - Wyświetla 2 strone dla wszystkich kategorii
            // /Szachy      - Wyświetla 1 stronę kategorii Szachy
            // /Szachy/Strona2 wyświetla drugą stronę w kategorii Szachy


            app.UseMvc(routes => {

                routes.MapRoute(
                    name: null,
                    template: "{category}/Strona{productPage:int}",
                    defaults: new { controller = "Product", action = "List"
                    });

                routes.MapRoute(
                    name: null,
                    template: "Strona{productPage:int}",
                    defaults: new { controller = "Product", action = "List", productPage = 1}
                    );

                routes.MapRoute(
                    name: null,
                    template: "{category}",
                    defaults: new { controller = "Product", action = "List", productPage = 1}
                    );

                // wzorzec składanych adresów URL
                // URL http://localhost/Strona2
                routes.MapRoute(
                    name: null,
                    template: "",
                    defaults: new { controller = "Product", action = "List", productPage = 1}
                    );
                
                // URL http://localhost/?productPage=2
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}"
                    );

        
            });

            SeedData.EnsurePopulated(app);
            // Domyślnie tworzy bazy danych LocalDB .mdf pliki C:/Users / {user} katalogu.

        }
    }
}
