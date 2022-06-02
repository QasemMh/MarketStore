using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MarketStore.Models;
using MarketStore.constants;

namespace MarketStore.Controllers
{
    public class UserMessagesController : Controller
    {
        private readonly ModelContext _context;

        public UserMessagesController(ModelContext context)
        {
            _context = context;
        }

        // GET: UserMessages
        public async Task<IActionResult> Index(
             int? pageNumber)
        {
            var userMessages = _context.UserMessages.AsQueryable();

            int pageSize = 20;
            return View(await PaginatedList<UserMessage>.CreateAsync(userMessages.AsNoTracking(),
                pageNumber ?? 1, pageSize));
        }

        // GET: UserMessages/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userMessage = await _context.UserMessages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userMessage == null)
            {
                return NotFound();
            }

            return View(userMessage);
        }

      
        
        // GET: UserMessages/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userMessage = await _context.UserMessages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userMessage == null)
            {
                return NotFound();
            }

            return View(userMessage);
        }

        // POST: UserMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var userMessage = await _context.UserMessages.FindAsync(id);
            _context.UserMessages.Remove(userMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserMessageExists(long id)
        {
            return _context.UserMessages.Any(e => e.Id == id);
        }
    }
}
