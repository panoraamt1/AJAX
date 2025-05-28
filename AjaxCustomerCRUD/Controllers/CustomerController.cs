using AjaxCustomerCRUD.Data;
using AjaxCustomerCRUD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AjaxCustomerCRUD.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
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
            return View(Customer);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(Customer Customer)
        {
            _context.Add(Customer);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Details(int Id)
        {
            Customer customer = _context.Customers.Where(c => c.Id == Id).FirstOrDefault();

            return View(customer);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            Customer customer = _context.Customers.Where(c => c.Id == Id).FirstOrDefault();

            ViewBag.Countries = GetCountries();
            return View(customer);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]

        public IActionResult Edit(Customer customer)
        {
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
    }
}
