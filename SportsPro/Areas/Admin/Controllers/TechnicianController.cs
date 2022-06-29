using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportsPro.Models;
using Microsoft.AspNetCore.Authorization;
using SportsPro.Areas.Admin.Models;

namespace SportsPro.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TechnicianController : Controller
    {
     
        private IRepository<Technician> data { get; set; }

      
        public TechnicianController(IRepository<Technician> rep) => data = rep;

     
        public IActionResult Index()
        {
            var technicians = data.List(new QueryOptions<Technician>
            {
                OrderBy = t => t.TechnicianID
            });

            return View(technicians);
        }

      
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Action = "Add";
            return View("Edit", new Technician());
        }

       
        [HttpGet]
        public IActionResult Edit(int id = 11)
        {
            ViewBag.Action = "Edit";
            var technician = data.Get(id);
            return View(technician);
        }

       
        [HttpPost]
        public IActionResult Edit(Technician technician)
        {
            if (technician.TechnicianID == 0)
            {
                string EmailToCheck = nameof(technician.Email);


                string message = CheckEmailTechnician.EmailExists(data, technician.Email);
                if (message != "")
                {
                    ModelState.AddModelError(
                      EmailToCheck, message);
                }
            }
            if (ModelState.IsValid)
            {
                if (technician.TechnicianID == 0)
                {
                    TempData["msgAdd"] = $"{technician.Name} has been added.";
                    data.Insert(technician);
                }
                else
                {
                    TempData["msgEdit"] = $"{technician.Name} has been edited.";
                    data.Update(technician);
                }
                data.Save();
                return RedirectToAction("Index", "Technician");
            }
            else
            {
                ViewBag.Action = (technician.TechnicianID == 0) ? "Add" : "Edit";
                return View(technician);
            }
        }

      
        [HttpGet]
        public IActionResult Delete(int id = 11)
        {
            var technician = data.Get(id);
            return View(technician);
        }

     
        [HttpPost]
        public IActionResult Delete(Technician technician)
        {
            data.Delete(technician);
            data.Save();
            TempData["msgDelete"] = $"{technician.Name} has been deleted.";
            return RedirectToAction("Index", "Technician");
        }
    }
}
