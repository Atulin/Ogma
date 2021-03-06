﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.ModeratorActions;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Comparers;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Serilog;
using Utils;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(UsersController))]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OgmaUserManager _userManager;
        private readonly long? _uid;

        public UsersController(ApplicationDbContext context, OgmaUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
            _uid = User?.GetNumericId();
        }

        // GET: api/Users/signin/John
        [HttpGet("signin/{name}")]
        public async Task<ActionResult<SignInData>> GetSignInData(string name)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.NormalizedUserName == name.Normalize().ToUpperInvariant())
                .Select(u => new { u.Avatar, u.Title, u.Email })
                .FirstOrDefaultAsync();

            return user is null
                ? new SignInData
                {
                    Avatar = Lorem.Picsum(200),
                    Title = string.Empty,
                    // HasMfa = false
                }
                : new SignInData
                {
                    Avatar = user.Avatar ?? Lorem.Gravatar(user.Email),
                    Title = user.Title,
                    // HasMfa = user.TwoFactorEnabled
                };
        }

        // api/Users/block
        [HttpPost("block")]
        public async Task<ActionResult<bool>> BlockUser(BlockPostData data)
        {
            var uid = User.GetNumericId();
            if (!uid.HasValue) return Unauthorized();

            var targetUserId = await _context.Users
                .Where(u => u.NormalizedUserName == data.Name.Normalize().ToUpper())
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            var existing = await _context.BlacklistedUsers
                .Where(bu => bu.BlockingUserId == targetUserId)
                .Where(bu => bu.BlockedUserId == uid)
                .FirstOrDefaultAsync();
            
            if (existing is null)
            {
                _context.BlacklistedUsers.Add(new UserBlock
                {
                    BlockingUserId = targetUserId,
                    BlockedUserId = (long) uid 
                });
                await _context.SaveChangesAsync();
                return true;
            }

            _context.BlacklistedUsers.Remove(existing);
            await _context.SaveChangesAsync();
            return false;
        }

        // api/Users/follow
        [HttpPost("follow")]
        public async Task<ActionResult<bool>> FollowUser(BlockPostData data)
        {
            var uid = User.GetNumericId();
            if (!uid.HasValue) return Unauthorized();

            var targetUserId = await _context.Users
                .Where(u => u.NormalizedUserName == data.Name.Normalize().ToUpper())
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            var existing = await _context.FollowedUsers
                .Where(bu => bu.FollowingUserId == uid)
                .Where(bu => bu.FollowedUserId == targetUserId)
                .FirstOrDefaultAsync();
            
            if (existing is null)
            {
                _context.FollowedUsers.Add(new UserFollow
                {
                    FollowingUserId = (long) uid ,
                    FollowedUserId = targetUserId
                });
                await _context.SaveChangesAsync();
                return true;
            }

            _context.FollowedUsers.Remove(existing);
            await _context.SaveChangesAsync();
            return false;
        }
        
        [HttpPost("ban")]
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Moderator)]
        public async Task<ActionResult> BanUser(BanData data)
        {
            var (userId, days) = data;
            
            // Check if user is logged in
            if (_uid is null) return Unauthorized();
            var uid = (long)_uid;

            // Get user to be banned
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
            if (user is null) return NotFound();

            // Ban/unban user
            if (days.HasValue)
            {
                user.BannedUntil = DateTime.Now.AddDays((double) days);
                await _userManager.UpdateSecurityStampAsync(user);
                _context.ModeratorActions.Add(new ModeratorAction
                {
                    StaffMemberId = uid,
                    Description = ModeratorActionTemplates.UserBan(user, User.GetUsername(), (DateTime) user.BannedUntil)
                });
            }
            else if (user.BannedUntil.HasValue)
            {
                _context.ModeratorActions.Add(new ModeratorAction
                {
                    StaffMemberId = uid,
                    Description = ModeratorActionTemplates.UserUnban(user, User.GetUsername(), (DateTime) user.BannedUntil)
                });
                user.BannedUntil = null;
            }
            else
            {
                return BadRequest();
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPost("mute")]
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Moderator)]
        public async Task<ActionResult> MuteUser(BanData data)
        {
            var (userId, days) = data;
            
            // Check if user is logged in
            if (_uid is null) return Unauthorized();
            var uid = (long)_uid;
            
            // Get user to be muted
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
            if (user is null) return NotFound();

            // Mute/unmute user
            if (days.HasValue && !user.MutedUntil.HasValue)
            {
                user.MutedUntil = DateTime.Now.AddDays((double) days);
                _context.ModeratorActions.Add(new ModeratorAction
                {
                    StaffMemberId = uid,
                    Description = ModeratorActionTemplates.UserMute(user, User.GetUsername(), (DateTime) user.MutedUntil)
                });
            }
            else if (user.MutedUntil.HasValue)
            {
                _context.ModeratorActions.Add(new ModeratorAction
                {
                    StaffMemberId = uid,
                    Description = ModeratorActionTemplates.UserUnmute(user, User.GetUsername(), (DateTime) user.MutedUntil)
                });
                user.MutedUntil = null;
            }
            else
            {
                return BadRequest();
            }
            
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("roles")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult> ManageRoles(RoleData data)
        {
            var (userId, roles) = data;
            
            // Check if user is logged in
            if (_uid is null) return Unauthorized();
            var uid = (long)_uid;
            
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync();
            
            var newRoles = await _context.OgmaRoles
                .Where(ur => roles.Contains(ur.Id))
                .ToListAsync();

            // Handle role removal
            var removedRoles = user.Roles.Except(newRoles, new OgmaRoleComparer()).ToList();
            foreach (var role in removedRoles)
            {
                user.Roles.Remove(role);
            }
            await _context.ModeratorActions.AddRangeAsync(removedRoles.Select(r => new ModeratorAction
            {
                StaffMemberId = uid,
                Description = ModeratorActionTemplates.UserRoleRemoved(user, User.GetUsername(), r.Name)
            }));
            
            // Handle role adding
            var addedRoles = newRoles.Except(user.Roles, new OgmaRoleComparer()).ToList();
            foreach (var role in addedRoles)
            {
                user.Roles.Add(role);
            }
            await _context.ModeratorActions.AddRangeAsync(addedRoles.Select(r => new ModeratorAction
            {
                StaffMemberId = uid,
                Description = ModeratorActionTemplates.UserRoleAdded(user, User.GetUsername(), r.Name)
            }));

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception occurred when staff member {Staff} tried adding roles {Role} to user {User}", uid, roles, userId);
                return BadRequest(e.Message);
            }
            return Ok();
        }
        

        // Don't delete or this whole controller will break
        [HttpGet] public string Ping() => "Pong";
    }

    public sealed record BanData(long UserId, double? Days);
    public sealed record RoleData(long UserId, IEnumerable<long> Roles);
    public sealed record SignInData
    {
        public string Avatar { get; init; }
        public string Title { get; init; }
        // public bool HasMfa { get; init; }
    }

    public sealed record BlockPostData(string Name);
}
