using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportsPro.Models;
using Microsoft.AspNetCore.Authorization;

namespace SportsPro.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
   
        private IRepository<Product> data { get; set; }

       
        public ProductController(IRepository<Product> rep) => data = rep;
       

     
        public ViewResult Index()
        {
            var products = data.List(new QueryOptions<Product>
            { OrderBy = p => p.ProductCode });
            return View(products);
        }

        
        [HttpGet]
        public ViewResult Add()
        {
            ViewBag.Action = "Add";
            return View("Edit", new Product());
        }

      
        [HttpGet]
        public ViewResult Edit(int id = 1)
        {
            ViewBag.Action = "Edit";
            var product = data.Get(id);
            
            return View(product);
        }

      
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ProductID == 0)
                {
                    TempData["msgAdd"] = $"{product.Name} has been added.";
                    data.Insert(product);
                }   
                else
                {
                    TempData["msgEdit"] = $"{product.Name} has been edited.";
                    data.Update(product);
                }
                data.Save();
                return RedirectToAction("Index", "Product");
            }
            else
            {
                ViewBag.Action = (product.ProductID == 0) ? "Add" : "Edit";
                return View(product);
            }
        }

       
        [HttpGet]
        public ViewResult Delete(int id = 1)
        {
            var product = data.Get(id);
            return View(product);
        }

      
        [HttpPost]
        public RedirectToActionResult Delete(Product product)
        {
            data.Delete(product);
            data.Save();
            TempData["msgDelete"] = $"{product.Name} has been deleted.";
            return RedirectToAction("Index", "Product");
        }
    }
}
