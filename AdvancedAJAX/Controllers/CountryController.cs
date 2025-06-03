using Microsoft.AspNetCore.Mvc;
using AdvancedAJAX.Data;
using AdvancedAJAX.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace AdvancedAJAX.Controllers
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
            return View(_context.Countries.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Country());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Add(country);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = _context.Countries.FirstOrDefault(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = _context.Countries.Find(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Code,Name,CurrencyName")] Country country)
        {
            if (id != country.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Countries.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = _context.Countries.FirstOrDefault(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var countryToDelete = _context.Countries.Find(id);

            if (countryToDelete == null)
            {
                return NotFound();
            }

            try
            {
                _context.Countries.Remove(countryToDelete);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                {
                    _context.Entry(countryToDelete).Reload();
                    _context.Entry(countryToDelete).State = EntityState.Unchanged;

                    ModelState.AddModelError("", "Cannot delete this country because there are cities associated with it. Please delete all associated cities first.");
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while deleting the country. Please try again.");
                }

                var countryToReturn = _context.Countries.FirstOrDefault(c => c.Id == id);
                return View(countryToReturn);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                var countryToReturn = _context.Countries.FirstOrDefault(c => c.Id == id);
                return View(countryToReturn);
            }
        }

        [HttpGet]
        public IActionResult CreateModalForm()
        {
            return PartialView("_CreateModalForm", new Country());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateModalForm(Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Add(country);
                _context.SaveChanges();
                return Json(new { id = country.Id, name = country.Name });
            }
            return PartialView("_CreateModalForm", country);
        }

        private List<SelectListItem> GetCountriesForDropdown()
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