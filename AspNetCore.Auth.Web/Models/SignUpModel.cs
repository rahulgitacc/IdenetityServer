using System.ComponentModel.DataAnnotations;

namespace AspNetCore.Auth.Web.Models
{
    public class SignUpModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string RepeatPassword { get; set; }
    }
}
