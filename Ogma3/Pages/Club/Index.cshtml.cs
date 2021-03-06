using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Club
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ClubRepository _clubRepo;

        public IndexModel(ApplicationDbContext context, ClubRepository clubRepo)
        {
            _context = context;
            _clubRepo = clubRepo;
        }
        
        public ClubBar ClubBar { get; private set; }
        public IList<ThreadCard> ThreadCards { get; private set; }
        
        public async Task<IActionResult> OnGetAsync(long id, string? slug)
        {
            ClubBar = await _clubRepo.GetClubBar(id);
            if (ClubBar is null) return NotFound();

            ThreadCards = await _context.ClubThreads
                .Where(ct => ct.ClubId == id)
                .OrderByDescending(ct => ct.CreationDate)
                .Take(3)
                .Select(ct => new ThreadCard
                {
                    Id = ct.Id,
                    Title = ct.Title,
                    ClubId = ct.ClubId,
                    IsPinned = ct.IsPinned,
                    CreationDate = ct.CreationDate,
                    AuthorName = ct.Author.UserName,
                    AuthorAvatar = ct.Author.Avatar,
                    CommentsCount = ct.CommentsThread.Comments.Count
                })
                .ToListAsync();

            return Page();
        }
    }
}