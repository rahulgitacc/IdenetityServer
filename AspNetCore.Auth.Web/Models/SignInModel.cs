using System.ComponentModel.DataAnnotations;

namespace AspNetCore.Auth.Web.Models
{
    public class SignInModel
    {
        [Required(ErrorMessage = "Have to supply a username")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Have to supply a password")]
        public string Password { get; set; }
    }
}
