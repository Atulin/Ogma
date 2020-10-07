using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Comment : BaseModel
    {
        [Required] 
        public long CommentsThreadId { get; set; }

        [Required] 
        public  OgmaUser Author { get; set; }
        public long AuthorId { get; set; }

        [Required] 
        public DateTime DateTime { get; set; } = DateTime.Now;

        [DefaultValue(null)]
        public DateTime? LastEdit { get; set; }

        [Required]
        [MinLength(CTConfig.CComment.MinBodyLength)]
        [MaxLength(CTConfig.CComment.MaxBodyLength)]
        public string Body { get; set; }

    }
}