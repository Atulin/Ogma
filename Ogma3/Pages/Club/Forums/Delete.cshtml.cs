using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Club.Forums
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GetData ClubThread { get; set; }
        
        public class GetData
        {
            public long Id { get; init; }
            public long ClubId { get; init; }
            public long? AuthorId { get; init; }
            public string Title { get; init; }
            public DateTime CreationDate { get; init; }
            public int Replies { get; init; }
        }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            ClubThread = await _context.ClubThreads
                .Where(ct => ct.Id == id)
                .Select(ct => new GetData
                {
                    Id = ct.Id,
                    ClubId = ct.ClubId,
                    AuthorId = ct.AuthorId,
                    Title = ct.Title,
                    CreationDate = ct.CreationDate,
                    Replies = ct.CommentsThread.CommentsCount
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if (ClubThread is null) return NotFound();
            if (!await CanDelete(ClubThread.AuthorId, ClubThread.ClubId)) return Unauthorized();
            
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(long id)
        {
            var thread = await _context.ClubThreads
                .Where(ct => ct.Id == id)
                .FirstOrDefaultAsync();

            if (thread is null) return NotFound();
            if (!await CanDelete(thread.AuthorId, thread.ClubId)) return Unauthorized();
            
            _context.ClubThreads.Remove(thread);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { id = thread.ClubId });
        }

        private async Task<bool> CanDelete(long? authorId, long clubId)
        {
            var uid = User.GetNumericId();
            if (uid is null) return false;

            if (authorId == uid) return true;
            
            return await _context.ClubMembers
                .Where(cm => cm.ClubId == clubId)
                .Where(cm => cm.MemberId == uid)
                .Where(cm => new[]
                {
                    EClubMemberRoles.Founder, 
                    EClubMemberRoles.Admin, 
                    EClubMemberRoles.Moderator
                }.Contains(cm.Role))
                .AnyAsync();
        }
    }
}
