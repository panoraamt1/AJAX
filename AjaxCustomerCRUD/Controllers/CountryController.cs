using AjaxCustomerCRUD.Data;
using AjaxCustomerCRUD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AjaxCustomerCRUD.Controllers
{
    public class CountryController : Controller
    {
        private readonly AppDbContext _context;

        public CountryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Country> countries;
            countries = _context.Countries.ToList();
            return View(countries);
        }

        [HttpGet]
        public IActionResult Create()
        {
            Country country = new Country();
            return View(country);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(Country country)
        {
            _context.Add(country);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Details(int Id)
        {
            Country country = GetCountry(Id);
            return View(country);
        }

        [HttpGet]

        public IActionResult Edit(int Id)
        {
            Country country = GetCountry(Id);
            return View(country);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]

        public IActionResult Edit(Country country)
        {
            _context.Attach(country);
            _context.Entry(country).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private Country GetCountry(int id)
        {
            Country country;
            country = _context.Countries
                .Where(c => c.Id == id).FirstOrDefault();
            return country;
        }

        [HttpGet]

        public IActionResult Delete(int id)
        {
            Country country = GetCountry(id);
            return View(country);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]

        public IActionResult Delete(Country country)
        {
            try
            {
                _context.Attach(country);
                _context.Entry(country).State = EntityState.Deleted;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _context.Entry(country).Reload();
                ModelState.AddModelError("", ex.InnerException.Message);
                return View(country);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
