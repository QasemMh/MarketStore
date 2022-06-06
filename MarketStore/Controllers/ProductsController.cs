using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MarketStore.Models;
using MarketStore.constants;
using MarketStore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace MarketStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        // GET: Products
        public async Task<IActionResult> Index(string currentFilter,
            string searchString, int? pageNumber, long? storeId)
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
            ViewData["StoreId"] = storeId;
            ViewBag.Stores = new SelectList(await _context.Stores.ToListAsync(), "Id", "Name", storeId);

            var products = _context.Products
                .Include(s => s.Store).Include(c => c.Category)
                .AsQueryable();

            if (storeId.HasValue)
            {
                products = products.Where(p => p.StoreId == storeId);
            }



            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s =>
                s.Name.Contains(searchString) ||
                s.Store.Name.StartsWith(searchString)
                || s.Category.Name.StartsWith(searchString)
                );
            }
            int pageSize = 20;
            return View(await PaginatedList<Product>.CreateAsync(products.AsNoTracking(),
                pageNumber ?? 1, pageSize));

        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Store)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var productImages = await _context.ProductImages
                .Where(p => p.ProductId == product.Id).ToListAsync();

            ProductViewModel viewModel = new ProductViewModel();
            viewModel.Product = product;
            viewModel.ProductImages = productImages;


            return View(viewModel);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Cost,Quantitiy,ExpireDate,CategoryId,StoreId,FormFiles")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (!IsNumber(product.Price.ToString()))
                {
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                    ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
                    ModelState.AddModelError("Price", "Price Must be Number only");
                    return View(product);
                }
                if (product.Cost != null)
                    if (!IsNumber(product.Cost.ToString()))
                    {
                        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                        ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
                        ModelState.AddModelError("Cost", "Cost Must be Number only");
                        return View(product);
                    }
                if (product.Quantitiy != null)
                    if (!IsNumber(product.Quantitiy.ToString()))
                    {
                        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                        ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
                        ModelState.AddModelError("Quantitiy", "Quantitiy Must be Number only");
                        return View(product);
                    }


                //validation for images
                string rootPath = _webHostEnvironment.WebRootPath;
                string[] allowedExe = new string[] { ".png", ".jpeg", ".jpg" };
                List<string> imageFileNames = new List<string>();
                if (product.FormFiles != null && product.FormFiles.Count < 6)
                {
                    string fileExe, fileName, path;

                    foreach (var item in product.FormFiles)
                    {
                        fileExe = Path.GetExtension(item.FileName);
                        fileName = Guid.NewGuid().ToString() + fileExe;// get name of image without path

                        //validation
                        if (!allowedExe.Contains(fileExe.ToLower()))
                        {
                            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
                            ModelState.AddModelError("FormFiles", "Not Allowed exe");
                            return View(product);
                        }
                        if (item.Length > 10485760)
                        {
                            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
                            ModelState.AddModelError("FormFiles", "File Must be Less Than 10mb");
                            return View(product);
                        }

                        path = Path.Combine(rootPath + "/images/", fileName);
                        //create virtual object in wwwrootpath
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }

                        imageFileNames.Add(fileName);

                    }

                    var x = imageFileNames.Select(img => new ProductImage
                    {
                        Image = img,
                        Product = product,
                    });


                    _context.ProductImages.AddRange(x);


                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
            ModelState.AddModelError("FormFiles", "File Not Uploaded, please try again");
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel();
            viewModel.Product = await _context.Products.FindAsync(id);

            if (viewModel.Product == null)
            {
                return NotFound();
            }

            var productImages = await _context.ProductImages
                .Where(p => p.ProductId == viewModel.Product.Id).ToListAsync();

            viewModel.ProductImages = productImages;

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", viewModel.Product.CategoryId);
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", viewModel.Product.StoreId);
            return View(viewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ProductViewModel viewModel)
        {
            if (id != viewModel.Product.Id)
            {
                return NotFound();
            }

            var product = viewModel.Product;

            if (ModelState.IsValid)
            {
                try
                {
                    if (!IsNumber(product.Price.ToString()))
                    {
                        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                        ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
                        ModelState.AddModelError("Product.Price", "Price Must be Number only");
                        return View(viewModel);
                    }
                    if (product.Cost != null)
                        if (!IsNumber(product.Cost.ToString()))
                        {
                            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
                            ModelState.AddModelError("Product.Cost", "Cost Must be Number only");
                            return View(viewModel);
                        }
                    if (product.Quantitiy != null)
                        if (!IsNumber(product.Quantitiy.ToString()))
                        {
                            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
                            ModelState.AddModelError("Product.Quantitiy", "Quantitiy Must be Number only");
                            return View(viewModel);
                        }


                    //validation for images
                    string rootPath = _webHostEnvironment.WebRootPath;
                    string[] allowedExe = new string[] { ".png", ".jpeg", ".jpg" };
                    List<string> imageFileNames = new List<string>();
                    if (product.FormFiles != null)
                    {
                        if (product.FormFiles.Count > 6)
                        {
                            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
                            ModelState.AddModelError("Product.FormFiles", "You must choose 5 image at most");
                            return View(viewModel);
                        }

                        string fileExe, fileName, path;

                        foreach (var item in product.FormFiles)
                        {
                            fileExe = Path.GetExtension(item.FileName);
                            fileName = Guid.NewGuid().ToString() + fileExe;// get name of image without path

                            //validation
                            if (!allowedExe.Contains(fileExe.ToLower()))
                            {
                                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                                ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
                                ModelState.AddModelError("Product.FormFiles", "Not Allowed exe");
                                return View(viewModel);
                            }
                            if (item.Length > 10485760)
                            {
                                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                                ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
                                ModelState.AddModelError("Product.FormFiles", "File Must be Less Than 10mb");
                                return View(viewModel);
                            }

                            path = Path.Combine(rootPath + "/images/", fileName);
                            //create virtual object in wwwrootpath
                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                await item.CopyToAsync(fileStream);
                            }

                            imageFileNames.Add(fileName);

                        }

                        //remove old image
                        var imagesDB = await _context.ProductImages
                            .Where(p => p.ProductId == product.Id).ToListAsync();
                        if (imagesDB != null)
                        {
                            foreach (var item in imagesDB)
                            {
                                if (System.IO.File.Exists(Path.Combine(rootPath + "/images/", item.Image)))
                                {
                                    // If file found, delete it    
                                    System.IO.File.Delete(Path.Combine(rootPath + "/images/", item.Image));
                                }
                            }

                            _context.ProductImages.RemoveRange(imagesDB);
                            await _context.SaveChangesAsync();
                        }

                        var productImages = imageFileNames.Select(img => new ProductImage
                        {
                            Image = img,
                            ProductId = product.Id,
                        });

                        _context.ProductImages.AddRange(productImages);
                        await _context.SaveChangesAsync();
                    }

                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(viewModel.Product.Id))
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


            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
            ModelState.AddModelError("Product.FormFiles", "Product Not updated Please try again");
            return View(viewModel);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                 .Include(p => p.Category)
                 .Include(p => p.Store)
                 .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var productImages = await _context.ProductImages
                .Where(p => p.ProductId == product.Id).ToListAsync();

            ProductViewModel viewModel = new ProductViewModel();
            viewModel.Product = product;
            viewModel.ProductImages = productImages;

            return View(viewModel);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            string rootPath = _webHostEnvironment.WebRootPath;
            var product = await _context.Products.FindAsync(id);
            var imagesDB = await _context.ProductImages
                .Where(p => p.ProductId == product.Id).ToListAsync();

            //remove image from server
            if (imagesDB != null)
            {
                foreach (var item in imagesDB)
                {
                    if (System.IO.File.Exists(Path.Combine(rootPath + "/images/", item.Image)))
                    {
                        // If file found, delete it    
                        System.IO.File.Delete(Path.Combine(rootPath + "/images/", item.Image));
                    }
                }
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        private bool IsNumber(string input)
        {
            return new Regex(@"^(\d+(\.\d+)?)$").IsMatch(input);
        }
    }
}
