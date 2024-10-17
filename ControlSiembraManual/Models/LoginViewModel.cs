using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace GestionSiembra.Models
{
    public class LoginViewModel
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage ="El campo {0} debe ser un correo valido")]
        public string Email { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name ="Recuerdame")]
        public bool Recuerdame { get; set; }


    }
}
