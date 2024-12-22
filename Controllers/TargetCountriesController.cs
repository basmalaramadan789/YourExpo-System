using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourExpo.Models;
using YourExpo.Persistence;

namespace YourExpo.Controllers;
[Authorize(Roles ="Admin")]
public class TargetCountriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public TargetCountriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: TargetCountries
    public async Task<IActionResult> Index()
    {
        return View(await _context.TargetCountries.ToListAsync());
    }



    // GET: TargetCountries/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: TargetCountries/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] TargetCountry targetCountry)
    {
        if (ModelState.IsValid)
        {
            _context.Add(targetCountry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(targetCountry);
    }

    // GET: TargetCountries/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var targetCountry = await _context.TargetCountries.FindAsync(id);
        if (targetCountry == null)
        {
            return NotFound();
        }
        return View(targetCountry);
    }

    // POST: TargetCountries/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] TargetCountry targetCountry)
    {
        if (id != targetCountry.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(targetCountry);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TargetCountryExists(targetCountry.Id))
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
        return View(targetCountry);
    }

    // GET: TargetCountries/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var targetCountry = await _context.TargetCountries
            .FirstOrDefaultAsync(m => m.Id == id);
        if (targetCountry == null)
        {
            return NotFound();
        }

        return View(targetCountry);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var targetCountry = await _context.TargetCountries.FindAsync(id);
        if (targetCountry == null)
        {
            return NotFound();
        }

        _context.TargetCountries.Remove(targetCountry);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TargetCountryExists(int id)
    {
        return _context.TargetCountries.Any(e => e.Id == id);
    }
}
