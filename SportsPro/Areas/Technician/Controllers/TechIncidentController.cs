using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportsPro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace SportsPro.Controllers
{
    [Area("Technician")]
    [Authorize(Roles = "Admin, Technician")]
    public class TechIncidentController : Controller
    {
        
        private SportsProContext context { get; set; }
        public TechnicianListViewModel viewModel;

        
        public TechIncidentController(SportsProContext ctx)
        {
            context = ctx;
            viewModel = new TechnicianListViewModel();
        }

        
        public IActionResult Index()
        {

            var data = new TechnicianListViewModel()
            { Technician = new Technician { TechnicianID = 1 } };
           

            IQueryable<Technician> query = context.Technicians;
            

            data.Technicians = query.ToList();
            return View(data);
        }

        [HttpPost]
       
        public IActionResult Index(TechnicianListViewModel selectedTechnician)
        {
            var session = new MySession(HttpContext.Session);
            var sessionTech = session.GetTechnician();
            sessionTech = context.Technicians.Find(selectedTechnician.Technician.TechnicianID);
            session.SetTechnician(sessionTech);
         

            return RedirectToAction("Success", "TechIncident");
        }

        
        public IActionResult Success()
        {

            var data = new IncidentListViewModel();
            var session = new MySession(HttpContext.Session);
            var sessionTech = session.GetTechnician();


            IQueryable<Incident> query = context.Incidents;
            query = query.Include(c => c.Customer)
                .Include(p => p.Product)
                .Include(t => t.Technician)
                .OrderBy(i => i.DateOpened);

           
            query = query.Where(
                i => i.TechnicianID == sessionTech.TechnicianID)
                .Where(
                i => i.DateClosed == null);
            
            data.Incidents = query.ToList();
           
            if (data.Incidents.Count() == 0)
            {
                TempData["message"] = $"{sessionTech.Name} has no open incidents.";
                return RedirectToAction("Index", "TechIncident"); }
            return View(data);
       
        }

      
        [HttpGet]
        public IActionResult Add()
        {
            var data = new IncidentViewModel
            {
                Customers = context.Customers.OrderBy(c => c.FirstName).ToList(),
                Products = context.Products.OrderBy(p => p.Name).ToList(),
                Technicians = context.Technicians.OrderBy(t => t.Name).ToList(),
                DesiredAction = "Add",
                Incident = new Incident()

            };

            return View("Edit", data);
        }

       
        [HttpGet]
        public IActionResult Edit(int id = 1)
        {
            var data = new IncidentViewModel
            {
                Customers = context.Customers.OrderBy(c => c.FirstName).ToList(),
                Products = context.Products.OrderBy(p => p.Name).ToList(),
                Technicians = context.Technicians.OrderBy(t => t.Name).ToList(),
                DesiredAction = "Edit",
                Incident = context.Incidents.Find(id)

            };
            return View(data);
        }

       
        [HttpPost]
        public IActionResult Edit(Incident incident)
        {
            if (ModelState.IsValid)
            {
                if (incident.IncidentID == 0)
                {
                    
                    context.Incidents.Add(incident);
                }
                else
                {
                    context.Incidents.Update(incident);
                }
                context.SaveChanges();
                return RedirectToAction("Index", "Incident");
            }
            else
            {
                ViewBag.Action = (incident.IncidentID == 0) ? "Add" : "Edit";
                return View(incident);
            }
        }

       
        [HttpGet]
        public IActionResult Delete(int id = 1)
        {
            var incident = context.Incidents.Find(id);
            return View(incident);
        }

       
        [HttpPost]
        public IActionResult Delete(Incident incident)
        {
            context.Incidents.Remove(incident);
            context.SaveChanges();
            return RedirectToAction("Index", "Incident");
        }
    }
}
