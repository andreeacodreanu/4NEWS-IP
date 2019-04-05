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
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
        public DateTime Date { get; set; }


        //[RegularExpression(".(jpg|png|gif)", ErrorMessage = "Please use an image with an extension of .jpg, .png, .gif, .bmp")]
        [Required(ErrorMessage = "Image is required")]
        //[RegularExpression(@".+(jpg|png|gif)$", ErrorMessage = "Please use an image with an extension of .jpg, .png, .jpeg")]       
        public string Image { get; set; }
        [RegularExpression(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$", ErrorMessage = "Invalid URL")]
        public string Link { get; set; }


        public string Approved { get; set; }
        public int NrComm { get; set; }

        public string Resume { get; set; }

        public virtual ICollection<Comment> Comment { get; set; }

       
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }


        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }


        // Se aduaga acest atribut pentru a putea prelua toate categoriile unui model in helper
        
        public IEnumerable<SelectListItem> Categories { get; set; }

    }

    
}