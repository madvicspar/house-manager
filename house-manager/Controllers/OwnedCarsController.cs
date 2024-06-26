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
    public class OwnedCarsController : Controller
    {
        private readonly ApplicationContext _context;

        public OwnedCarsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: OwnedCars
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.OwnedCars.Include(o => o.Car).Include(o => o.Owner);
            return View(await applicationContext.ToListAsync());
        }

        // GET: OwnedCars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ownedCar = await _context.OwnedCars
                .Include(o => o.Car)
                .Include(o => o.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ownedCar == null)
            {
                return NotFound();
            }

            return View(ownedCar);
        }

        // GET: OwnedCars/Create
        public IActionResult Create()
        {
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Brand");
            ViewData["OwnerId"] = new SelectList(_context.Lodgers, "Id", "PassportNumber");
            return View();
        }

        // POST: OwnedCars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CarId,OwnerId")] OwnedCar ownedCar)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ownedCar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Brand", ownedCar.CarId);
            ViewData["OwnerId"] = new SelectList(_context.Lodgers, "Id", "PassportNumber", ownedCar.OwnerId);
            return View(ownedCar);
        }

        // GET: OwnedCars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ownedCar = await _context.OwnedCars.FindAsync(id);
            if (ownedCar == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Brand", ownedCar.CarId);
            ViewData["OwnerId"] = new SelectList(_context.Lodgers, "Id", "PassportNumber", ownedCar.OwnerId);
            return View(ownedCar);
        }

        // POST: OwnedCars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CarId,OwnerId")] OwnedCar ownedCar)
        {
            if (id != ownedCar.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ownedCar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnedCarExists(ownedCar.Id))
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
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Brand", ownedCar.CarId);
            ViewData["OwnerId"] = new SelectList(_context.Lodgers, "Id", "PassportNumber", ownedCar.OwnerId);
            return View(ownedCar);
        }

        // GET: OwnedCars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ownedCar = await _context.OwnedCars
                .Include(o => o.Car)
                .Include(o => o.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ownedCar == null)
            {
                return NotFound();
            }

            return View(ownedCar);
        }

        // POST: OwnedCars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ownedCar = await _context.OwnedCars.FindAsync(id);
            if (ownedCar != null)
            {
                _context.OwnedCars.Remove(ownedCar);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OwnedCarExists(int id)
        {
            return _context.OwnedCars.Any(e => e.Id == id);
        }
    }
}
