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
    public class CategoriesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public CategoriesController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Categories
        public async Task<IActionResult> Index(
            string currentFilter, string searchString, int? pageNumber)
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

            var categories = _context.Categories.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(s => s.Name.Contains(searchString));
            }


            int pageSize = 20;
            return View(await PaginatedList<Category>.CreateAsync(categories.AsNoTracking(),
                pageNumber ?? 1, pageSize));
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Image,FormFile")] Category category)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                string[] allowedExe = new string[] { ".png", ".jpeg", ".jpg" };

                if (category.FormFile != null)
                {
                    string fileExe = Path.GetExtension(category.FormFile.FileName);
                    string fileName = Guid.NewGuid().ToString() + fileExe;// get name of image without path

                    //validation
                    if (!allowedExe.Contains(fileExe.ToLower()))
                    {
                        ModelState.AddModelError("Image", "Not Allowed Extension");
                        return View(category);
                    }
                    if (category.FormFile.Length > 10485760)
                    {
                        ModelState.AddModelError("Image", "File Must be Less Than 10mb");
                        return View(category);
                    }


                    string path = Path.Combine(rootPath + "/images/", fileName);
                    //create virtual object in wwwrootpath
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await category.FormFile.CopyToAsync(fileStream);
                    }
                    category.Image = fileName;

                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ModelState.AddModelError("Image", "File Not Uploaded, please try again");
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Image,FormFile")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string rootPath = _webHostEnvironment.WebRootPath;
                    string[] allowedExe = new string[] { ".png", ".jpeg", ".jpg" };

                    if (category.FormFile != null)
                    {
                        string fileExe = Path.GetExtension(category.FormFile.FileName);
                        string fileName = Guid.NewGuid().ToString() + fileExe;// get name of image without path

                        //validation
                        if (!allowedExe.Contains(fileExe.ToLower()))
                        {
                            ModelState.AddModelError("Image", "Not Allowed exe");
                            return View(category);
                        }
                        if (category.FormFile.Length > 10485760)
                        {
                            ModelState.AddModelError("Image", "File Must be Less Than 10mb");
                            return View(category);
                        }
                        string path = Path.Combine(rootPath + "/images/", fileName);
                        //create virtual object in wwwrootpath
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await category.FormFile.CopyToAsync(fileStream);
                        }

                        //delete old image:
                        if (System.IO.File.Exists(Path.Combine(rootPath + "/images/", category.Image)))
                        {
                            // If file found, delete it    
                            System.IO.File.Delete(Path.Combine(rootPath + "/images/", category.Image));
                        }

                        category.Image = fileName;


                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);

        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            string rootPath = _webHostEnvironment.WebRootPath;

            var category = await _context.Categories.FindAsync(id);
            string fileName = category.Image;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            //delete old image:
            if (System.IO.File.Exists(Path.Combine(rootPath + "/images/", fileName)))
            {
                // If file found, delete it    
                System.IO.File.Delete(Path.Combine(rootPath + "/images/", fileName));
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(long id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
