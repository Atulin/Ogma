using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using Ogma3.Data.Enums;

namespace Ogma3.Data.Models
{
    public class Story : BaseModel
    {
        // public Story()
        // {
        //     Chapters = new List<Chapter>();
        // }

        [Required]
        public User Author { get; set; }

        [Required]
        [MinLength(CTConfig.Story.MinTitleLength)]
        [MaxLength(CTConfig.Story.MaxTitleLength)]
        public string Title { get; set; }

        [Required] 
        public string Slug { get; set; }

        [Required]
        [MinLength(CTConfig.Story.MinDescriptionLength)]
        [MaxLength(CTConfig.Story.MaxDescriptionLength)]
        public string Description { get; set; }

        [Required]
        [MinLength(CTConfig.Story.MinHookLength)]
        [MaxLength(CTConfig.Story.MaxHookLength)]
        public string Hook { get; set; }

        public string? Cover { get; set; }
        public string? CoverId { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        [Required]
        [DefaultValue(false)]
        public bool IsPublished { get; set; }
        
        
        // Chapters
        public  ICollection<Chapter> Chapters { get; set; } //= new List<Chapter>();


        // Tags
        [JsonIgnore]
        public  ICollection<StoryTag> StoryTags { get; set; } //= new List<StoryTag>();
        [NotMapped]
        public IEnumerable<Tag> Tags => 
            StoryTags == null || StoryTags.Count <= 0
                ? new List<Tag>() 
                : StoryTags.Select(st => st.Tag).ToList();
        
        // Rating
        [Required]
        public Rating Rating { get; set; }
        
        // Status
        [Required]
        [DefaultValue(EStoryStatus.InProgress)]
        public EStoryStatus Status { get; set; }
        
        // Votes
        [Required]
        public  VotePool VotesPool { get; set; }
        [Required]
        public long VotesPoolId { get; set; }

        [NotMapped] 
        public int Score => VotesPool.Votes.Count;
    }
}