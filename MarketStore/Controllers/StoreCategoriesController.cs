using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MarketStore.Models;
using MarketStore.constants;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace MarketStore.Controllers
{
    public class StoreCategoriesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StoreCategoriesController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: StoreCategories
        public async Task<IActionResult> Index(string currentFilter, string searchString, int? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var storeCategory = _context.StoreCategories.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                storeCategory = storeCategory.Where(s => s.Name.Contains(searchString));
            }
            int pageSize = 20;
            return View(await PaginatedList<StoreCategory>.CreateAsync(storeCategory.AsNoTracking(),
                pageNumber ?? 1, pageSize));
        }

        // GET: StoreCategories/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeCategory = await _context.StoreCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storeCategory == null)
            {
                return NotFound();
            }

            return View(storeCategory);
        }

        // GET: StoreCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StoreCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Image,FormFile")] StoreCategory storeCategory)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                string[] allowedExe = new string[] { ".png", ".jpeg", ".jpg" };

                if (storeCategory.FormFile != null)
                {        
                    string fileExe = Path.GetExtension(storeCategory.FormFile.FileName);
                    string fileName = Guid.NewGuid().ToString() + fileExe;// get name of image without path

                    //validation
                    if (!allowedExe.Contains(fileExe.ToLower()))
                    {
                        ModelState.AddModelError("Image", "Not Allowed exe");
                        return View(storeCategory);
                    }
                    if (storeCategory.FormFile.Length > 10485760)
                    {
                        ModelState.AddModelError("Image", "File Must be Less Than 10mb");
                        return View(storeCategory);
                    }


                    string path = Path.Combine(rootPath + "/images/", fileName);
                    //create virtual object in wwwrootpath
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await storeCategory.FormFile.CopyToAsync(fileStream);
                    }
                    storeCategory.Image = fileName;

                    _context.Add(storeCategory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ModelState.AddModelError("Image", "File Not Uploaded, please try again");
            return View(storeCategory);
        }

        // GET: StoreCategories/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeCategory = await _context.StoreCategories.FindAsync(id);
            if (storeCategory == null)
            {
                return NotFound();
            }
            return View(storeCategory);
        }

        // POST: StoreCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Image,FormFile")] StoreCategory storeCategory)
        {
            if (id != storeCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string rootPath = _webHostEnvironment.WebRootPath;
                    string[] allowedExe = new string[] { ".png", ".jpeg", ".jpg" };

                    if (storeCategory.FormFile != null)
                    {
                        string fileExe = Path.GetExtension(storeCategory.FormFile.FileName);
                        string fileName = Guid.NewGuid().ToString() + fileExe;// get name of image without path

                        //validation
                        if (!allowedExe.Contains(fileExe.ToLower()))
                        {
                            ModelState.AddModelError("Image", "Not Allowed exe");
                            return View(storeCategory);
                        }
                        if (storeCategory.FormFile.Length > 10485760)
                        {
                            ModelState.AddModelError("Image", "File Must be Less Than 10mb");
                            return View(storeCategory);
                        }
                        string path = Path.Combine(rootPath + "/images/", fileName);
                        //create virtual object in wwwrootpath
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await storeCategory.FormFile.CopyToAsync(fileStream);
                        }
               
                        //delete old image:
                        if (System.IO.File.Exists(Path.Combine(rootPath + "/images/", storeCategory.Image)))
                        {
                            // If file found, delete it    
                            System.IO.File.Delete(Path.Combine(rootPath + "/images/", storeCategory.Image));
                        }

                        storeCategory.Image = fileName;


                    }

                    _context.Update(storeCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreCategoryExists(storeCategory.Id))
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

            ModelState.AddModelError("Image", "Please, try again");
            return View(storeCategory);
        }

        // GET: StoreCategories/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeCategory = await _context.StoreCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storeCategory == null)
            {
                return NotFound();
            }

            return View(storeCategory);
        }

        // POST: StoreCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            string rootPath = _webHostEnvironment.WebRootPath;

            var storeCategory = await _context.StoreCategories.FindAsync(id);
            string fileName = storeCategory.Image;

            _context.StoreCategories.Remove(storeCategory);           
            await _context.SaveChangesAsync();

            //delete old image:
            if (System.IO.File.Exists(Path.Combine(rootPath + "/images/", fileName)))
            {
                // If file found, delete it    
                System.IO.File.Delete(Path.Combine(rootPath + "/images/", fileName));
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StoreCategoryExists(long id)
        {
            return _context.StoreCategories.Any(e => e.Id == id);
        }
    }
}
