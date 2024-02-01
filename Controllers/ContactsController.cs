// ContactsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web_based.Data;
using web_based.Models;
using web_based.ViewModels; // Include the ViewModel namespace
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;

public class ContactsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(ApplicationDbContext context, ILogger<ContactsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Contacts
    public async Task<IActionResult> Index()
    {
        var contacts = await _context.Contacts
                                 .Select(c => new ContactViewModel
                                 {
                                     Id = c.Id,
                                     FirstName = c.FirstName,
                                     LastName = c.LastName,
                                     Phone = c.Phone,
                                     Email = c.Email,
                                     CategoryName = _context.Categories.FirstOrDefault(cat => cat.Id == c.CategoryId).Name
                                 })
                                 .ToListAsync();
        return View(contacts);
    }

    // GET: Contacts/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            _logger.LogWarning("Details action called with a null ID.");
            return NotFound();
        }

        var contactViewModel = await _context.Contacts
            .Where(c => c.Id == id)
            .Select(c => new ContactViewModel
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Phone = c.Phone,
                Email = c.Email,
                CategoryName = _context.Categories.FirstOrDefault(cat => cat.Id == c.CategoryId).Name,
                DateAdded = c.DateAdded
            })
            .FirstOrDefaultAsync();

        if (contactViewModel == null)
        {
            _logger.LogWarning($"Details action called with a non-existing contact ID: {id}");
            return NotFound();
        }

        return View(contactViewModel);
    }

    // GET: Contacts/Create
    public IActionResult Create()
    {
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        return View("CreateEdit", new Contact());
    }

    // POST: Contacts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Phone,Email,CategoryId")] Contact contact)
    {
        // Remove ModelState errors related to Category
        ModelState.Remove("Category");

        if (ModelState.IsValid)
        {
            contact.DateAdded = DateTime.Now;

            _context.Add(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        else
        {
            // If ModelState is not valid, repopulate CategoryId SelectList and return to view
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", contact.CategoryId);
            return View("CreateEdit", contact);
        }
    }


    // GET: Contacts/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            _logger.LogWarning("Edit action called with a null ID.");
            return NotFound();
        }

        var contact = await _context.Contacts.FindAsync(id);
        if (contact == null)
        {
            _logger.LogWarning($"Edit action called with a non-existing contact ID: {id}");
            return NotFound();
        }

        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", contact.CategoryId);
        return View("CreateEdit", contact);
    }



    // POST: Contacts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Phone,Email,CategoryId")] Contact contact)
    {
        if (id != contact.Id)
        {
            _logger.LogWarning($"Edit action called with mismatched ID: {id} and contact ID: {contact.Id}");
            return NotFound();
        }

        // Remove ModelState errors related to Category
        ModelState.Remove("Category");

        if (ModelState.IsValid)
        {
            var existingContact = await _context.Contacts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (existingContact == null)
            {
                _logger.LogWarning($"Contact with ID: {id} not found for editing.");
                return NotFound();
            }

            contact.DateAdded = existingContact.DateAdded;

            _context.Update(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        else
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", contact.CategoryId);
            return View("CreateEdit", contact);
        }
    }



    // GET: Contacts/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            _logger.LogWarning("Delete action called with a null ID.");
            return NotFound();
        }

        var contactViewModel = await _context.Contacts
            .Where(c => c.Id == id)
            .Select(c => new ContactViewModel
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Phone = c.Phone,
                Email = c.Email,
                CategoryName = _context.Categories.FirstOrDefault(cat => cat.Id == c.CategoryId).Name
            })
            .FirstOrDefaultAsync();

        if (contactViewModel == null)
        {
            _logger.LogWarning($"Delete action called with a non-existing contact ID: {id}");
            return NotFound();
        }

        return View(contactViewModel);
    }

    // POST: Contacts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact == null)
        {
            _logger.LogWarning($"DeleteConfirmed action called with a non-existing contact ID: {id}");
            return NotFound();
        }

        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ContactExists(int id)
    {
        return _context.Contacts.Any(e => e.Id == id);
    }
}
