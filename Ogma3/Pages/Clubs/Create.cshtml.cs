using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.FileUploader;
using Utils.Extensions;

namespace Ogma3.Pages.Clubs
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageUploader _uploader;
        private readonly OgmaConfig _config;

        public CreateModel(ApplicationDbContext context, ImageUploader uploader, OgmaConfig config)
        {
            _context = context;
            _uploader = uploader;
            _config = config;
        }

        public IActionResult OnGet()
        {
            Input = new InputModel();
            return Page();
        }

        [BindProperty] 
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Name { get; init; }
            public string Hook { get; init; }
            public string Description { get; init; }

            [DataType(DataType.Upload)] 
            public IFormFile Icon { get; init; }
        }

        public class InputModelValidator : AbstractValidator<InputModel>
        {
            public InputModelValidator()
            {
                RuleFor(m => m.Name)
                    .NotEmpty()
                    .Length(CTConfig.CClub.MinNameLength, CTConfig.CClub.MaxNameLength);
                RuleFor(m => m.Hook)
                    .NotEmpty()
                    .Length(CTConfig.CClub.MinHookLength, CTConfig.CClub.MaxHookLength);
                RuleFor(m => m.Description)
                    .MaximumLength(CTConfig.CClub.MaxDescriptionLength);
                RuleFor(m => m.Icon)
                    .FileSmallerThan(CTConfig.CClub.CoverMaxWeight)
                    .FileHasExtension(new[] {".jpg", ".jpeg", ".png", ".webp"});
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            var club = new Data.Clubs.Club
            {
                Name = Input.Name,
                Slug = Input.Name.Friendlify(),
                Hook = Input.Hook,
                Description = Input.Description,
                ClubMembers = new List<ClubMember>
                {
                    new()
                    {
                        MemberId = (long) uid,
                        Role = EClubMemberRoles.Founder
                    }
                }
            };

            await _context.Clubs.AddAsync(club);
            await _context.SaveChangesAsync();

            if (Input.Icon is not { Length: > 0 }) return RedirectToPage("./Index");
            
            var file = await _uploader.Upload(
                Input.Icon,
                "club-icons",
                $"{club.Id}-{club.Name.Friendlify()}",
                _config.ClubIconWidth,
                _config.ClubIconHeight
            );
            club.IconId = file.FileId;
            club.Icon = file.Path;
            
            // Final save
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}