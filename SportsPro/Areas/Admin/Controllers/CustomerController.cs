using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportsPro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using SportsPro.Areas.Admin.Models;


namespace SportsPro.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        
        private SportsProContext context { get; set; }
        private Repository<Customer> data { get; set; }

        private Repository<Country> countryData { get; set; }


     
        public CustomerController(SportsProContext ctx)
        {
            data = new Repository<Customer>(ctx);
            countryData = new Repository<Country>(ctx);
            context = ctx;
        }

       
        public ViewResult Index()
        {
            var customers = data.List(new QueryOptions<Customer>
            {
                Includes = "Country",
                OrderBy = c => c.FirstName
            });
                
            return View(customers);
        }

     
        [HttpGet]
        public ViewResult Add()
        {
            ViewBag.Countries = countryData.List(new QueryOptions<Country> 
            { OrderBy = c => c.Name });
            ViewBag.Action = "Add";
            return View("Edit", new Customer());
        }

   
        [HttpGet]
        public ViewResult Edit(int id = 1002)
        {
            ViewBag.Countries = countryData.List(new QueryOptions<Country>
            { OrderBy = c => c.Name });
            ViewBag.Action = "Edit";
            var customer = data.Get(id);
            return View(customer);
        }
       

        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (customer.CustomerID == 0)
            {
                string EmailToCheck = nameof(customer.Email);

                string message = CheckEmail.EmailExists(data, customer.Email);
                if (message != "")
                {
                    ModelState.AddModelError(
                      EmailToCheck, message);
                }
            }

            if (ModelState.IsValid)
            {
                if (customer.CustomerID == 0)
                {
                    TempData["msgAdd"] = $"{customer.FullName} has been added.";
                    data.Insert(customer);
                }
                else
                {
                    TempData["msgEdit"] = $"{customer.FullName} has been edited.";
                    data.Update(customer);
                }
                data.Save();
                return RedirectToAction("Index", "Customer");
            }
            else
            {
                ViewBag.Action = (customer.CustomerID == 0) ? "Add" : "Edit";
                ViewBag.Countries = countryData.List(new QueryOptions<Country>
                { OrderBy = c => c.Name }); 
                return View(customer);
            }
        }


        [HttpGet]
        public ViewResult Delete(int id = 1002)
        {
            Customer customer = data.Get(id);
            return View(customer);
        }


        [HttpPost]
        public IActionResult Delete(Customer customer)
        {
            
            data.Delete(customer);
            data.Save();
            TempData["msgDelete"] = $"{customer.FirstName} {customer.LastName} has been deleted.";
            return RedirectToAction("Index", "Customer");
        }
    }
}
