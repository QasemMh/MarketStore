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
    public class CategorySectionsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public CategorySectionsController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: CategorySections
        public async Task<IActionResult> Index()
        {
            return View(await _context.CategorySections.ToListAsync());
        }

        // GET: CategorySections/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.CategorySections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: CategorySections/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategorySections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Image,FormFile")] CategorySection category)
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

        // GET: CategorySections/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.CategorySections.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: CategorySections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, 
            [Bind("Id,Title,Image,FormFile")] CategorySection category)
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

        // GET: CategorySections/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.CategorySections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: CategorySections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            string rootPath = _webHostEnvironment.WebRootPath;

            var category = await _context.CategorySections.FindAsync(id);
            string fileName = category.Image;

            _context.CategorySections.Remove(category);
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
            return _context.CategorySections.Any(e => e.Id == id);
        }
    }
}
