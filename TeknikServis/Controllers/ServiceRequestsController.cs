using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeknikServis.Data;
using TeknikServis.Models;

namespace TeknikServis.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly AppDbContext _ctx;
        public ServiceRequestsController(AppDbContext ctx) { _ctx = ctx; }

        public IActionResult Index(string search, int? categoryId, string sort)
        {
            var q = _ctx.ServiceRequests
                .Include(r => r.RequestCategory).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                q = q.Where(r => r.CustomerName.Contains(search)
                              || r.DeviceName.Contains(search));

            if (categoryId.HasValue && categoryId > 0)
                q = q.Where(r => r.RequestCategoryId == categoryId);

            q = sort switch
            {
                "date_asc" => q.OrderBy(r => r.RequestDate),
                "date_desc" => q.OrderByDescending(r => r.RequestDate),
                "customer_asc" => q.OrderBy(r => r.CustomerName),
                "customer_desc" => q.OrderByDescending(r => r.CustomerName),
                "status_asc" => q.OrderBy(r => r.Status),
                _ => q.OrderByDescending(r => r.Id)
            };

            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.Sort = sort;
            ViewBag.Categories = new SelectList(
                _ctx.RequestCategories.ToList(), "Id", "Name", categoryId);

            ViewBag.ToplamTalep = _ctx.ServiceRequests.Count();
            ViewBag.Beklemede = _ctx.ServiceRequests.Count(r => r.Status == "Beklemede");
            ViewBag.Islemde = _ctx.ServiceRequests.Count(r => r.Status == "İşlemde");
            ViewBag.Tamamlandi = _ctx.ServiceRequests.Count(r => r.Status == "Tamamlandı");
            ViewBag.Iptal = _ctx.ServiceRequests.Count(r => r.Status == "İptal");

            return View(q.ToList());
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            var r = _ctx.ServiceRequests.Include(x => x.RequestCategory)
                        .FirstOrDefault(x => x.Id == id);
            return r == null ? NotFound() : View(r);
        }

        [Authorize(Roles = "Admin"), HttpGet]
        public IActionResult Add()
        {
            LoadViewBag();
            return View();
        }

        [Authorize(Roles = "Admin"), HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(ServiceRequest m)
        {
            if (!ModelState.IsValid) { LoadViewBag(); return View(m); }
            _ctx.ServiceRequests.Add(m);
            _ctx.SaveChanges();
            TempData["Success"] = "Servis talebi oluşturuldu.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin"), HttpGet]
        public IActionResult Update(int id)
        {
            var r = _ctx.ServiceRequests.Find(id);
            if (r == null) return NotFound();
            LoadViewBag(r.RequestCategoryId);
            return View(r);
        }

        [Authorize(Roles = "Admin"), HttpPost, ValidateAntiForgeryToken]
        public IActionResult Update(ServiceRequest m)
        {
            if (!ModelState.IsValid) { LoadViewBag(); return View(m); }
            _ctx.ServiceRequests.Update(m);
            _ctx.SaveChanges();
            TempData["Success"] = "Talep güncellendi.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Remove(int id)
        {
            var r = _ctx.ServiceRequests.Find(id);
            if (r != null) { _ctx.ServiceRequests.Remove(r); _ctx.SaveChanges(); }
            TempData["Success"] = "Talep silindi.";
            return RedirectToAction("Index");
        }

        private void LoadViewBag(int? selectedCat = null)
        {
            ViewBag.Categories = new SelectList(
                _ctx.RequestCategories.ToList(), "Id", "Name", selectedCat);
            ViewBag.Statuses = new List<string>
                { "Beklemede", "İşlemde", "Tamamlandı", "İptal" };
        }
    }
}