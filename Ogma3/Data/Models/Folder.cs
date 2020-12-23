using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Enums;

namespace Ogma3.Data.Models
{
    public class Folder : BaseModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Slug { get; set; }
        public string? Description { get; set; }
        
        [Required]
        public Club Club { get; set; }
        public long ClubId { get; set; }

        public Folder? ParentFolder { get; set; }
        public long? ParentFolderId { get; set; }

        public ICollection<Folder> ChildFolders { get; set; }
        public ICollection<Story> Stories { get; set; }

        [Required]
        [DefaultValue(0)]
        public int StoriesCount { get; set; }
        
        [Required]
        [DefaultValue(EClubMemberRoles.User)]
        public EClubMemberRoles AccessLevel { get; set; }
    }
}