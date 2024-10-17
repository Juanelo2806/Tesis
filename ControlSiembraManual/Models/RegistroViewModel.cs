using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace GestionSiembra.Models
{
    public class RegistroViewModel
    {
       
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage ="El campo {0} es requerido")]
        public string Email { get; set; }
        
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
