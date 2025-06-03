using Microsoft.AspNetCore.Mvc;
using AdvancedAJAX.Data;
using AdvancedAJAX.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace AdvancedAJAX.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public CustomerController(AppDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public IActionResult Index()
        {
            List<Customer> Customers;
            Customers = _context.Customers.ToList();
            return View(Customers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            Customer Customer = new Customer();
            ViewBag.Countries = LoadCountries();
            ViewBag.Cities = new List<SelectListItem>();
            return View(Customer);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            string uniqueFileName = GetProfilePhotoFileName(customer);
            customer.PhotoUrl = uniqueFileName;

            try
            {
                _context.Add(customer);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                {
                    ViewBag.Countries = LoadCountries();
                    
                    int selectedCountryId = customer.CountryId; // Removed ?? 0 if CountryId is int. If it's int?, keep it.

                    
                    ViewBag.Cities = LoadCities(selectedCountryId); // Fix CS1061 and CS1503

                    ModelState.AddModelError("", "The selected City is invalid or does not exist. Please select a valid City.");
                }
                else
                {
                    ModelState.AddModelError("", "An unexpected error occurred while creating the customer. Please try again.");
                }

                return View(customer);
            }
            catch (Exception ex)
            {
                ViewBag.Countries = LoadCountries();
                // Fix CS0104:
                ViewBag.Cities = LoadCities(customer.CountryId); // Removed ?? 0 if CountryId is int.
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                return View(customer);
            }
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            Customer customer = _context.Customers
                .Include(cty => cty.City)
                .Include(cou => cou.City.Country)
                .Where(c => c.Id == Id).FirstOrDefault();

            return View(customer);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            Customer customer = _context.Customers
                .Include(co => co.City)
                .Where(c => c.Id == Id).FirstOrDefault();

            customer.CountryId = customer.City.CountryId;

            ViewBag.Countries = LoadCountries();
            ViewBag.Cities = LoadCities(customer.CountryId);

            return View(customer);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (customer.ProfilePhoto != null)
            {
                string uniqueFileName = GetProfilePhotoFileName(customer);
                customer.PhotoUrl = uniqueFileName;
            }

            _context.Attach(customer);
            _context.Entry(customer).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int Id)
        {
            Customer customer = _context.Customers
                .Include(c => c.City)
                    .ThenInclude(city => city.Country)
                .Where(c => c.Id == Id)
                .FirstOrDefault();

            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var customerToDelete = _context.Customers.Find(id);

            if (customerToDelete == null)
            {
                return NotFound();
            }

            try
            {
                if (!string.IsNullOrEmpty(customerToDelete.PhotoUrl) && customerToDelete.PhotoUrl != "noimage.png")
                {
                    string filePath = Path.Combine(_webHost.WebRootPath, "images", customerToDelete.PhotoUrl);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Customers.Remove(customerToDelete);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                {
                    _context.Entry(customerToDelete).Reload();
                    _context.Entry(customerToDelete).State = EntityState.Unchanged;

                    ModelState.AddModelError("", "Cannot delete this customer because there are associated records in other tables (e.g., Orders). Please ensure no other data depends on this customer.");
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while deleting the customer. Please try again.");
                }

                var customerToReturn = _context.Customers.Include(c => c.City).ThenInclude(city => city.Country).FirstOrDefault(c => c.Id == id);
                return View(customerToReturn);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                var customerToReturn = _context.Customers.Include(c => c.City).ThenInclude(city => city.Country).FirstOrDefault(c => c.Id == id);
                return View(customerToReturn);
            }
        }

        [HttpGet]
        private List<SelectListItem> LoadCountries()
        {
            var lstCountries = _context.Countries
                .Select(ct => new SelectListItem()
                {
                    Value = ct.Id.ToString(),
                    Text = ct.Name
                })
                .ToList();

            lstCountries.Insert(0, new SelectListItem()
            {
                Value = "",
                Text = "----Select Country----"
            });

            return lstCountries;
        }

        [HttpGet]
        public JsonResult GetCountries()
        {
            return Json(LoadCountries());
        }

        private string GetProfilePhotoFileName(Customer customer)
        {
            string uniqueFileName = null;
            if (customer.ProfilePhoto != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + customer.ProfilePhoto.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    customer.ProfilePhoto.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        private List<SelectListItem> LoadCities(int countryId)
        {
            var cities = _context.Cities
                .Where(c => c.CountryId == countryId)
                .OrderBy(n => n.Name)
                .Select(n => new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.Name
                }).ToList();

            if (cities.Any())
            {
                cities.Insert(0, new SelectListItem() { Value = "", Text = "----Select City----" });
            }
            else if (_context.Countries.Any(c => c.Id == countryId))
            {
                cities.Insert(0, new SelectListItem() { Value = "", Text = "----No Cities Available----" });
            }
            else
            {
                cities.Insert(0, new SelectListItem() { Value = "", Text = "----Select City----" });
            }

            return cities;
        }

        [HttpGet]
        public JsonResult GetCitiesByCountry(int countryId)
        {
            return Json(LoadCities(countryId));
        }
    }
}