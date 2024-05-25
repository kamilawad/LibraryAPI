using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs
{
    public class UserDTO
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
