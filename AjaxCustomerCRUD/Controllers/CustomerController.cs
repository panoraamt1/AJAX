using AjaxCustomerCRUD.Data;
using AjaxCustomerCRUD.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AjaxCustomerCRUD.Controllers
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
            List<Customer> Cities;
            Cities = _context.Customers.ToList();
            return View(Cities);
        }

        [HttpGet]
        public IActionResult Create()
        {
            Customer Customer = new Customer();
            ViewBag.Countries = GetCountries();
            ViewBag.Cities = new List<SelectListItem>();
            return View(Customer);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            string uniqueFileName = GetProfilePhotoFileName(customer);
            customer.PhotoUrl = uniqueFileName;

            _context.Add(customer);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Details(int Id)
        {
            Customer customer = _context.Customers.Include(cty => cty.City).Include(cou => cou.City.Country).Where(c => c.Id == Id).FirstOrDefault();

            return View(customer);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            Customer customer = _context.Customers
                .Include(co => co.City)
                .Where(c => c.Id == Id).FirstOrDefault();
            customer.CountryId = customer.City.CountryId;

            ViewBag.Countries = GetCountries();
            ViewBag.Cities = GetCities(customer.CountryId);
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
            Customer customer = _context.Customers.Where(c => c.Id == Id).FirstOrDefault();

            return View(customer);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]

        public IActionResult Delete(Customer customer)
        {
            _context.Attach(customer);
            _context.Entry(customer).State = EntityState.Deleted;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
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


        [HttpGet]
        public JsonResult GetCitiesByCountry(int countryId)
        {
            List<SelectListItem> cities = _context.Cities
                .Where(c => c.CountryId == countryId)
                .OrderBy(n => n.Name).
                Select(n => new SelectListItem 
                { 
                    Value = n.Id.ToString(), 
                    Text = n.Name 
                }).ToList();
            return Json(cities);
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

        private List<SelectListItem> GetCities(int countryId)
        {
            List<SelectListItem> cities = _context.Cities.Where(c => c.CountryId == countryId).OrderBy(n => n.Name).Select(n =>
            new SelectListItem
            {
                Value = n.Id.ToString(),
                Text = n.Name
            }).ToList();

            return cities;
        }


    }
}
