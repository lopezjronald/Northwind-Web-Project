﻿using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Packt.Shared; // NorthwindContext
using Microsoft.EntityFrameworkCore; // Include extension

namespace Northwind.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NorthwindContext db;


        public HomeController(ILogger<HomeController> logger, NorthwindContext injectedContext)
        {
            _logger = logger;
            db = injectedContext;
        }

        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
        public IActionResult Index()
        {
            _logger.LogError("This is a seriosu error (not really!)");
            _logger.LogWarning("This is your first warning!");
            _logger.LogWarning("Second warning!");
            _logger.LogInformation("I am in the Index method of the HomeController");

            HomeIndexViewModel model = new
            (
                VisitorCount: (new Random()).Next(1, 1001),
                Categories: db.Categories.ToList(),
                Products: db.Products.ToList()
            );

            return View(model);
        }

        public IActionResult ProductDetail(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("You must pass a product ID in the route, for example, /Home/ProductDetail/21");
            }

            Product? model = db.Products.SingleOrDefault(p => p.ProductId == id);

            if (model == null)
            {
                return NotFound($"ProductId {id} not found.");
            }

            return View(model); // pass model to view and then return result
        }

        public IActionResult ModelBinding()
        {
            return View(); // the page iwth a form to submit
        }

        [HttpPost]
        public IActionResult ModelBinding(Thing thing)
        {
            HomeModelBindingViewModel model = new(
                thing,
                !ModelState.IsValid,
                ModelState.Values.SelectMany(state => state.Errors).Select(error => error.ErrorMessage));
            return View(model); // show the model bound thing
        }

        [Route("private")]
        [Authorize(Roles = "Administrators")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}