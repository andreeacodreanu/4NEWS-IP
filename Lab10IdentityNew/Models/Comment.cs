using ProiectIP2019.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace ProiectIP.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        public string CommentContent { get; set; }
        public DateTime CommentDate { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int ArticleId { get; set; }
        public virtual ApplicationUser Article { get; set; }



    }
}