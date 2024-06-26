using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using house_manager;
using house_manager.Models;

namespace house_manager.Controllers
{
    public class OwnedApartmentsController : Controller
    {
        private readonly ApplicationContext _context;

        public OwnedApartmentsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: OwnedApartments
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.OwnedApartments.Include(o => o.Apartment).Include(o => o.Owner);
            return View(await applicationContext.ToListAsync());
        }

        // GET: OwnedApartments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ownedApartment = await _context.OwnedApartments
                .Include(o => o.Apartment)
                .Include(o => o.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ownedApartment == null)
            {
                return NotFound();
            }

            return View(ownedApartment);
        }

        // GET: OwnedApartments/Create
        public IActionResult Create()
        {
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "Id", "Id");
            ViewData["OwnerId"] = new SelectList(_context.Lodgers, "Id", "PassportNumber");
            return View();
        }

        // POST: OwnedApartments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApartmentId,OwnerId,OwnershipPercentage")] OwnedApartment ownedApartment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ownedApartment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "Id", "Id", ownedApartment.ApartmentId);
            ViewData["OwnerId"] = new SelectList(_context.Lodgers, "Id", "PassportNumber", ownedApartment.OwnerId);
            return View(ownedApartment);
        }

        // GET: OwnedApartments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ownedApartment = await _context.OwnedApartments.FindAsync(id);
            if (ownedApartment == null)
            {
                return NotFound();
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "Id", "Id", ownedApartment.ApartmentId);
            ViewData["OwnerId"] = new SelectList(_context.Lodgers, "Id", "PassportNumber", ownedApartment.OwnerId);
            return View(ownedApartment);
        }

        // POST: OwnedApartments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApartmentId,OwnerId,OwnershipPercentage")] OwnedApartment ownedApartment)
        {
            if (id != ownedApartment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ownedApartment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnedApartmentExists(ownedApartment.Id))
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
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "Id", "Id", ownedApartment.ApartmentId);
            ViewData["OwnerId"] = new SelectList(_context.Lodgers, "Id", "PassportNumber", ownedApartment.OwnerId);
            return View(ownedApartment);
        }

        // GET: OwnedApartments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ownedApartment = await _context.OwnedApartments
                .Include(o => o.Apartment)
                .Include(o => o.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ownedApartment == null)
            {
                return NotFound();
            }

            return View(ownedApartment);
        }

        // POST: OwnedApartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ownedApartment = await _context.OwnedApartments.FindAsync(id);
            if (ownedApartment != null)
            {
                _context.OwnedApartments.Remove(ownedApartment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OwnedApartmentExists(int id)
        {
            return _context.OwnedApartments.Any(e => e.Id == id);
        }
    }
}
