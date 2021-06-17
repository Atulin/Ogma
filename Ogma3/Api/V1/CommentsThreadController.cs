using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.AuthorizationData;
using Ogma3.Data.Clubs;
using Ogma3.Data.ModeratorActions;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(CommentsThreadController))]
    [ApiController]
    public class CommentsThreadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public CommentsThreadController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("permissions/{id:long}")]
        public async Task<bool> GetPermissionsAsync(long id)
        {
            if (User.Identity is {IsAuthenticated: false}) return false;
            
            var uid = User.GetNumericId();
            if (uid is null) return false;
            
            bool canLock;

            if (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Moderator))
            {
                canLock = true;
            } 
            else
            {
                var roles = new[] {EClubMemberRoles.Founder, EClubMemberRoles.Admin, EClubMemberRoles.Moderator};
                
                canLock = await _context.CommentThreads
                    .Where(ct => ct.Id == id)
                    .Where(ct => ct.ClubThreadId != null)
                    .Select(ct => ct.ClubThread.Club.ClubMembers
                        .Where(cm => cm.MemberId == uid)
                        .Any(cm => roles.Contains(cm.Role)))
                    .FirstOrDefaultAsync();
            }
            
            return canLock;
        }

        [HttpGet("lock/status/{id:long}")]
        public async Task<bool> GetLockStatusAsync(long id) 
            => await _context.CommentThreads
                .Where(ct => ct.Id == id)
                .Select(ct => ct.LockDate != null)
                .FirstOrDefaultAsync();

        // POST
        [HttpPost("lock")]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LockClubThreadAsync([FromBody]PostData data)
        {
            var canLock = await GetPermissionsAsync(data.Id);

            if (!canLock) return Unauthorized();
            
            var thread = await _context.CommentThreads.FindAsync(data.Id);
            thread.LockDate = thread.LockDate is null ? DateTime.Now : null;

            if (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Moderator))
            {
                var uid = User.GetNumericId();
                if (uid is null) return Unauthorized();

                string type;
                if (thread.BlogpostId is not null) type = "blogpost";
                else if (thread.ChapterId is not null) type = "chapter";
                else if (thread.ClubThreadId is not null) type = "club";
                else if (thread.UserId is not null) type = "user profile";
                else type = "unknown";

                var typeId = thread.BlogpostId ?? thread.ChapterId ?? thread.ClubThreadId ?? thread.UserId ?? 0;

                _context.ModeratorActions.Add(new ModeratorAction
                {
                    StaffMemberId = (long) uid,
                    Description = thread.LockDate is null
                        ? ModeratorActionTemplates.ThreadUnlocked(type, typeId, thread.Id, User.GetUsername())
                        : ModeratorActionTemplates.ThreadLocked(type, typeId, thread.Id, User.GetUsername())
                });
            }

            await _context.SaveChangesAsync();
                
            return Ok(thread.LockDate is not null);
        }

        
        // Don't delete or this whole controller will break
        [HttpGet] public string Ping() => "Pong";

        public sealed record PostData(long Id);
    }
}