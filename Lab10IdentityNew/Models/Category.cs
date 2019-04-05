using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProiectIP.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public string CategoryName { get; set; }

        public virtual ICollection<Article> Article { get; set; }

    }
}