using System;
using System.Linq;
using System.Threading.Tasks;
using B2Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Clubs
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IB2Client _b2Client;
        private readonly OgmaConfig _config;

        public DeleteModel(ApplicationDbContext context, IB2Client b2Client, OgmaConfig config)
        {
            _context = context;
            _b2Client = b2Client;
            _config = config;
        }

        [BindProperty]
        public GetData Club { get; set; }

        public class GetData
        {
            public long Id { get; init; }
            public string Name { get; init; }
            public string Slug { get; set; }
            public string Hook { get; init; }
            public DateTime CreationDate { get; init; }
            public long FounderId { get; init; }
        }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id is null) return NotFound();
            
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            
            Club = await _context.Clubs
                .Where(c => c.Id == id)
                .Select(c => new GetData
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    Hook = c.Hook,
                    CreationDate = c.CreationDate,
                    FounderId = c.ClubMembers.FirstOrDefault(m => m.Role == EClubMemberRoles.Founder).MemberId
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (Club is null) return NotFound();
            if (Club.FounderId != uid) return Unauthorized();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id is null) return NotFound();
            
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            
            var club = await _context.Clubs
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
            
            if (club is null) return NotFound();

            var founderId = await _context.ClubMembers
                .Where(cm => cm.ClubId == club.Id && cm.Role == EClubMemberRoles.Founder)
                .Select(cm => cm.MemberId)
                .FirstOrDefaultAsync();

            if (founderId != uid) return Unauthorized();
            
            _context.Clubs.Remove(club);

            if (club.Icon is not null && club.IconId is not null)
            {
                await _b2Client.Files.Delete(club.IconId, club.Icon.Replace(_config.Cdn, ""));
            }

            await _context.SaveChangesAsync();
            
            return RedirectToPage("./Index");
        }
    }
}
