using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using communityWeb.Models;

namespace communityWeb.Controllers
{
    public class DownVotesController : Controller
    {
        private readonly ProjectContext _context;

        public DownVotesController(ProjectContext context)
        {
            _context = context;
        }

        // GET: DownVotes
        public async Task<IActionResult> Index()
        {
            var projectContext = _context.DownVotes.Include(d => d.Post).Include(d => d.User);
            return View(await projectContext.ToListAsync());
        }

        // GET: DownVotes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DownVotes == null)
            {
                return NotFound();
            }

            var downVote = await _context.DownVotes
                .Include(d => d.Post)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (downVote == null)
            {
                return NotFound();
            }

            return View(downVote);
        }

        // GET: DownVotes/Create
        public IActionResult Create()
        {
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: DownVotes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,PostId")] DownVote downVote)
        {
            if (ModelState.IsValid)
            {
                _context.Add(downVote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", downVote.PostId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", downVote.UserId);
            return View(downVote);
        }

        // GET: DownVotes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DownVotes == null)
            {
                return NotFound();
            }

            var downVote = await _context.DownVotes.FindAsync(id);
            if (downVote == null)
            {
                return NotFound();
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", downVote.PostId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", downVote.UserId);
            return View(downVote);
        }

        // POST: DownVotes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,PostId")] DownVote downVote)
        {
            if (id != downVote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(downVote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DownVoteExists(downVote.Id))
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
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", downVote.PostId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", downVote.UserId);
            return View(downVote);
        }

        // GET: DownVotes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DownVotes == null)
            {
                return NotFound();
            }

            var downVote = await _context.DownVotes
                .Include(d => d.Post)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (downVote == null)
            {
                return NotFound();
            }

            return View(downVote);
        }

        // POST: DownVotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DownVotes == null)
            {
                return Problem("Entity set 'ProjectContext.DownVotes'  is null.");
            }
            var downVote = await _context.DownVotes.FindAsync(id);
            if (downVote != null)
            {
                _context.DownVotes.Remove(downVote);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DownVoteExists(int id)
        {
          return (_context.DownVotes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
