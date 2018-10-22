using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AppName.Web.Models
{
    public class ContactViewModel
    {
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [MinLength(20)]
        [MaxLength(500)]
        public string Message { get; set; }
        
        [Required]
        [BindProperty(Name = "g-recaptcha-response")]  
        public string Recaptcha { get; set; }
    }
}