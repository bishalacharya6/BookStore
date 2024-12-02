using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebAppRazorPages.Data;
using WebAppRazorPages.Models;

namespace WebAppRazorPages.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public List<Category> CategoryList { get; set; }
        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task OnGetAsync()
        {
            CategoryList = await _db.Categories.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Retrieve the category by ID
            var category = await _db.Categories.FindAsync(id);

            // If the category is not found, return a not found result
           

            // Remove the category from the database
            _db.Categories.Remove(category);

            // Save the changes to the database
            await _db.SaveChangesAsync();

            // Redirect back to the Index page
            return RedirectToPage("Index");
        }

    }
}
