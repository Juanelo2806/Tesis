using System.ComponentModel.DataAnnotations;

namespace GestionSiembra.Models
{
    public class RecuperarPasswordViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo {0} debe ser un correo valido")]
        public string Email { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
       
        

        public string CodigoReseteo { get; set; }
    }
}
