using Ogma3.Data.Models;

namespace Ogma3.Data.DTOs
{
    public class TagDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Namespace { get; set; }

        public string Color { get; set; }

        public static TagDTO FromTag(Tag tag)
        {
            return new TagDTO
            {
                Id = tag.Id,
                Name = tag.Name,
                Slug = tag.Slug,
                Description = tag.Description,
                Namespace = tag.Namespace?.Name,
                Color = tag.Namespace?.Color
            };
        }
    }
}