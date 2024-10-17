using System.ComponentModel.DataAnnotations;

namespace GestionSiembra.Models
{
    public class OlvideMiPasswordViewModel
    {
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [EmailAddress(ErrorMessage ="El campo {0} debe ser un correo valido")]
        public string Email { get; set; }
    }
}
