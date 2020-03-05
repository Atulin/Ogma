using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Ogma3.Data.Models;
using Utils;

namespace Ogma3.Data.DTOs
{
    public class ShelfFromApiDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; }
        public bool IsQuick { get; set; }
        public string Color { get; set; }
        public IEnumerable<Story> Stories;
        public int Count { get; set; }

        public static ShelfFromApiDTO FromShelf(Shelf shelf)
            => new ShelfFromApiDTO
            {
                Id = shelf.Id,
                Name = shelf.Name,
                Description = shelf.Description,
                IsDefault = shelf.IsDefault,
                IsPublic = shelf.IsPublic,
                IsQuick = shelf.IsQuickAdd,
                Color = shelf.Color,
                Stories = shelf.Stories,
                Count = shelf.ShelfStories.Count
            };
    }
}