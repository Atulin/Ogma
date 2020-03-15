#nullable enable 

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;

namespace Ogma3.Data.Models
{
    public class Shelf : BaseModel
    {
        [Required]
        [MinLength(CTConfig.Shelf.MinNameLength)]
        [MaxLength(CTConfig.Shelf.MaxNameLength)]
        public string Name { get; set; }

        [MaxLength(CTConfig.Shelf.MaxDescriptionLength)]
        public string Description { get; set; } = "";
        
        [Required]
        public User Owner { get; set; }
        
        [Required]
        [DefaultValue(false)]
        public bool IsDefault { get; set; } = false;
        
        [Required]
        [DefaultValue(false)]
        public bool IsPublic { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsQuickAdd { get; set; }
        
        [DefaultValue(null)]
        [MinLength(7)]
        [MaxLength(7)]
        public string? Color { get; set; }

        [DefaultValue(null)]
        public Icon Icon { get; set; }
        
        // Stories
        [JsonIgnore]
        public  ICollection<ShelfStory> ShelfStories { get; set; } = new List<ShelfStory>();
        [NotMapped]
        public IEnumerable<Story> Stories =>
            ShelfStories == null || ShelfStories.Count <= 0
                ? new List<Story>()
                : ShelfStories.Select(ss => ss.Story).ToList();
    }
}