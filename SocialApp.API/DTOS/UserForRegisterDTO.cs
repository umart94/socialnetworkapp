using System.ComponentModel.DataAnnotations;
using System;
namespace SocialApp.API.DTOS
{
    public class UserForRegisterDTO
    {
        //convert User Objects to simpler objects that we can return through code

        //pass data annotations using System.ComponentModel.DataAnnotations; for the reqd property.

        [Required(ErrorMessage = "YOU MUST ENTER USERNAME")]
        //[DataType(DataType.EmailAddress)]
        // [EmailAddress]

        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "PASSWORD MUST BE BETWEEN 4-8 CHARACTERS ")]

        public string Password { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string KnownAs { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; } // add using System;

        [Required]

        public string City { get; set; }

        [Required]

        public string Country { get; set; }

         public DateTime Created { get; set; } // add using System;

          public DateTime LastActive { get; set; } // add using System;

          public UserForRegisterDTO(){
              Created = DateTime.Now;
              LastActive = DateTime.Now;
          }


    }
}