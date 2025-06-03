using Microsoft.AspNetCore.Mvc;
using AdvancedAJAX.Data;
using AdvancedAJAX.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace AdvancedAJAX.Controllers
{
    public class CityController : Controller
    {
        private readonly AppDbContext _context;

        public CityController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<City> cities;
            cities = _context.Cities.Include(c => c.Country).ToList();
            return View(cities);
        }

        [HttpGet]
        public IActionResult Create()
        {
            City City = new City();
            ViewBag.Countries = GetCountries();
            return View(City);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(City City)
        {
            _context.Add(City);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult CreateModalForm(int countryId, string countryName)
        {
            City city = new City { CountryId = countryId, CountryName = countryName };
            ViewBag.Countries = GetCountries();
            return PartialView("_CreateModalForm", city);
        }

        [HttpPost]
        public IActionResult CreateModalForm(City city)
        {
            if (ModelState.IsValid)
            {
                _context.Add(city);
                _context.SaveChanges();
                return Json(new { id = city.Id, name = city.Name });
            }
            ViewBag.Countries = GetCountries();
            return PartialView("_CreateModalForm", city);
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            City City = _context.Cities
                .Include(c => c.Country)
                .Where(c => c.Id == Id).FirstOrDefault();

            return View(City);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            City City = _context.Cities
                .Where(c => c.Id == Id).FirstOrDefault();

            ViewBag.Countries = GetCountries();

            return View(City);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(City City)
        {
            _context.Attach(City);
            _context.Entry(City).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int Id)
        {
            City City = _context.Cities
                .Include(c => c.Country)
                .Where(c => c.Id == Id).FirstOrDefault();

            return View(City);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int Id)
        {
            var cityToDelete = _context.Cities.Find(Id);

            if (cityToDelete == null)
            {
                return NotFound();
            }

            try
            {
                _context.Cities.Remove(cityToDelete);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                {
                    _context.Entry(cityToDelete).Reload();
                    _context.Entry(cityToDelete).State = EntityState.Unchanged;

                    ModelState.AddModelError("", "Cannot delete this city because there are customers associated with it. Please delete all associated customers first.");
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while deleting the city. Please try again.");
                }

                var cityToReturn = _context.Cities.Include(c => c.Country).FirstOrDefault(c => c.Id == Id);
                return View(cityToReturn);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                var cityToReturn = _context.Cities.Include(c => c.Country).FirstOrDefault(c => c.Id == Id);
                return View(cityToReturn);
            }
        }

        private List<SelectListItem> GetCountries()
        {
            var lstCountries = new List<SelectListItem>();

            List<Country> Countries = _context.Countries.ToList();

            lstCountries = Countries.Select(ct => new SelectListItem()
            {
                Value = ct.Id.ToString(),
                Text = ct.Name
            }).ToList();

            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "----Select Country----"
            };

            lstCountries.Insert(0, defItem);

            return lstCountries;
        }

    }
}