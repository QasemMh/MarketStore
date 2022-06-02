using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MarketStore.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace MarketStore.Controllers
{
    public class SlidersController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SlidersController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Sliders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sliders.ToListAsync());
        }

        // GET: Sliders/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Sliders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // GET: Sliders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sliders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,FormFile")] Slider slider)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                string[] allowedExe = new string[] { ".png", ".jpeg", ".jpg" };

                if (slider.FormFile != null)
                {
                    string fileExe = Path.GetExtension(slider.FormFile.FileName);
                    string fileName = Guid.NewGuid().ToString() + fileExe;// get name of image without path

                    //validation
                    if (!allowedExe.Contains(fileExe.ToLower()))
                    {
                        ModelState.AddModelError("Image", "Not Allowed Extension");
                        return View(slider);
                    }
                    if (slider.FormFile.Length > 10485760)
                    {
                        ModelState.AddModelError("Image", "File Must be Less Than 10mb");
                        return View(slider);
                    }


                    string path = Path.Combine(rootPath + "/images/", fileName);
                    //create virtual object in wwwrootpath
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await slider.FormFile.CopyToAsync(fileStream);
                    }
                    slider.Image = fileName;

                    _context.Add(slider);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ModelState.AddModelError("Image", "File Not Uploaded, please try again");
            return View(slider);
        }

        // GET: Sliders/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
            {
                return NotFound();
            }
            return View(slider);
        }

        // POST: Sliders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Title,Description,FormFile,Image")] Slider slider)
        {
            if (id != slider.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string rootPath = _webHostEnvironment.WebRootPath;
                    string[] allowedExe = new string[] { ".png", ".jpeg", ".jpg" };

                    if (slider.FormFile != null)
                    {
                        string fileExe = Path.GetExtension(slider.FormFile.FileName);
                        string fileName = Guid.NewGuid().ToString() + fileExe;// get name of image without path

                        //validation
                        if (!allowedExe.Contains(fileExe.ToLower()))
                        {
                            ModelState.AddModelError("Image", "Not Allowed exe");
                            return View(slider);
                        }
                        if (slider.FormFile.Length > 10485760)
                        {
                            ModelState.AddModelError("Image", "File Must be Less Than 10mb");
                            return View(slider);
                        }
                        string path = Path.Combine(rootPath + "/images/", fileName);
                        //create virtual object in wwwrootpath
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await slider.FormFile.CopyToAsync(fileStream);
                        }

                        //delete old image:
                        if (System.IO.File.Exists(Path.Combine(rootPath + "/images/", slider.Image)))
                        {
                            // If file found, delete it    
                            System.IO.File.Delete(Path.Combine(rootPath + "/images/", slider.Image));
                        }

                        slider.Image = fileName;


                    }

                    _context.Update(slider);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SliderExists(slider.Id))
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
            return View(slider);

        }

        // GET: Sliders/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Sliders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // POST: Sliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            string rootPath = _webHostEnvironment.WebRootPath;

            var slider = await _context.Sliders.FindAsync(id);

            //delete old image:
            if (System.IO.File.Exists(Path.Combine(rootPath + "/images/", slider.Image)))
            {
                // If file found, delete it    
                System.IO.File.Delete(Path.Combine(rootPath + "/images/", slider.Image));
            }

            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SliderExists(long id)
        {
            return _context.Sliders.Any(e => e.Id == id);
        }
    }
}
