using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Clubs
{
    public class DetailsModel : PageModel
    {
        private readonly Ogma3.Data.ApplicationDbContext _context;

        public DetailsModel(Ogma3.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Club Club { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Club = await _context.Clubs.FirstOrDefaultAsync(m => m.Id == id);

            if (Club == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
