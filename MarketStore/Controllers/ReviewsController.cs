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
    public class ReviewsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ReviewsController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            return View(await _context.Reviews.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,JobTitle,Review1,FormFile")] Review review)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                string[] allowedExe = new string[] { ".png", ".jpeg", ".jpg" };

                if (review.FormFile != null)
                {
                    string fileExe = Path.GetExtension(review.FormFile.FileName);
                    string fileName = Guid.NewGuid().ToString() + fileExe;// get name of image without path

                    //validation
                    if (!allowedExe.Contains(fileExe.ToLower()))
                    {
                        ModelState.AddModelError("Image", "Not Allowed Extension");
                        return View(review);
                    }
                    if (review.FormFile.Length > 10485760)
                    {
                        ModelState.AddModelError("Image", "File Must be Less Than 10mb");
                        return View(review);
                    }


                    string path = Path.Combine(rootPath + "/images/", fileName);
                    //create virtual object in wwwrootpath
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await review.FormFile.CopyToAsync(fileStream);
                    }
                    review.Image = fileName;

                    _context.Add(review);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ModelState.AddModelError("Image", "File Not Uploaded, please try again");
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,JobTitle,Review1,Image,FormFile")]
        Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string rootPath = _webHostEnvironment.WebRootPath;
                    string[] allowedExe = new string[] { ".png", ".jpeg", ".jpg" };

                    if (review.FormFile != null)
                    {
                        string fileExe = Path.GetExtension(review.FormFile.FileName);
                        string fileName = Guid.NewGuid().ToString() + fileExe;// get name of image without path

                        //validation
                        if (!allowedExe.Contains(fileExe.ToLower()))
                        {
                            ModelState.AddModelError("Image", "Not Allowed exe");
                            return View(review);
                        }
                        if (review.FormFile.Length > 10485760)
                        {
                            ModelState.AddModelError("Image", "File Must be Less Than 10mb");
                            return View(review);
                        }
                        string path = Path.Combine(rootPath + "/images/", fileName);
                        //create virtual object in wwwrootpath
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await review.FormFile.CopyToAsync(fileStream);
                        }

                        //delete old image:
                        if (System.IO.File.Exists(Path.Combine(rootPath + "/images/", review.Image)))
                        {
                            // If file found, delete it    
                            System.IO.File.Delete(Path.Combine(rootPath + "/images/", review.Image));
                        }

                        review.Image = fileName;


                    }

                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
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
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var review = await _context.Reviews.FindAsync(id);

            //delete old image:
            if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath + "/images/", review.Image)))
            {
                // If file found, delete it    
                System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath + "/images/", review.Image));
            }


            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(long id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
}
