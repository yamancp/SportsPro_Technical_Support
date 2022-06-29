using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportsPro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace SportsPro.Controllers
{
    [Area("Admin")]
    public class IncidentController : Controller
    {
       
        private ISportsProUnitOfWork data { get; set; }
        public IncidentController(ISportsProUnitOfWork unit) => data = unit;

      
        [Authorize(Roles = "Admin, Technician")]
        public IActionResult Index()
        {
            ViewBag.filter = Request.Query["filter"];
            string filter= Request.Query["filter"];
            var vm = new IncidentListViewModel()
            {
                MyFilter = filter
            };

            vm.Incidents = data.Incidents.List(new QueryOptions<Incident>
            { Includes = "Customer, Product, Technician",
                OrderBy = i => i.DateOpened
            });

            if (filter == "unassigned")
                vm.Incidents = data.Incidents.List(new QueryOptions<Incident>
                { Includes = "Customer, Product, Technician",
                    Where = i => i.TechnicianID == null,
                    OrderBy = i => i.DateOpened
                });
            
            if (filter == "open")
                vm.Incidents = data.Incidents.List(new QueryOptions<Incident>
                { Includes = "Customer, Product, Technician",
                    Where = i => i.DateClosed == null,
                    OrderBy = i => i.DateOpened
                });

            return View(vm);
        }

        
        [Authorize(Roles = "Admin, Technician")]
        [HttpGet]
        public IActionResult Add()
        {
            var vm = new IncidentViewModel
            {
                Customers = data.Customers.List(new QueryOptions<Customer> {OrderBy = c => c.FirstName }),
                Products = data.Products.List(new QueryOptions<Product> { OrderBy = p => p.Name }),
                Technicians = data.Technicians.List(new QueryOptions<Technician> { OrderBy = t => t.Name }),
                DesiredAction = "Add",
                Incident = new Incident()

            };

            return View("Edit", vm);
        }

       
        [Authorize(Roles = "Admin, Technician")]
        [HttpGet]
        public IActionResult Edit(int id = 1)
        {
            var vm = new IncidentViewModel
            {
                Customers = data.Customers.List(new QueryOptions<Customer> { OrderBy = c => c.FirstName }),
                Products = data.Products.List(new QueryOptions<Product> { OrderBy = p => p.Name }),
                Technicians = data.Technicians.List(new QueryOptions<Technician> { OrderBy = t => t.Name }),
                DesiredAction = "Edit",
                Incident = data.Incidents.Get(id)

            };
            return View(vm);
        }

      
        [Authorize(Roles = "Admin, Technician")]
        [HttpPost]
        public IActionResult Edit(IncidentViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Incident.IncidentID == 0)
                {
                    TempData["msgAdd"] = $"{vm.Incident.Title} has been added.";
                    data.Incidents.Insert(vm.Incident);
                }
                else
                {
                    TempData["msgEdit"] = $"{vm.Incident.Title} has been edited.";
                    data.Incidents.Update(vm.Incident);
                }
                data.Save();
                return RedirectToAction("Index", "Incident");
            }
            else
            {
                vm.Customers = data.Customers.List(new QueryOptions<Customer> { OrderBy = c => c.FirstName });
                vm.Products = data.Products.List(new QueryOptions<Product> { OrderBy = p => p.Name });
                vm.Technicians = data.Technicians.List(new QueryOptions<Technician> { OrderBy = t => t.Name });
                vm.DesiredAction = (vm.Incident.IncidentID == 0) ? "Add" : "Edit";
                return View(vm);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(int id = 1)
        {
            var incident = data.Incidents.Get(id);
            return View(incident);
        }

 
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(Incident incident)
        {
            data.Incidents.Delete(incident);
            data.Save();
            TempData["msgDelete"] = $"{incident.Title} has been deleted.";
            return RedirectToAction("Index", "Incident");
        }
    }
}
