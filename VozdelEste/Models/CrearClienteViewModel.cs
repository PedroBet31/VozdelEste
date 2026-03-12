using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VozdelEste.Models
{
    public class CrearClienteViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido.")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "La cédula debe ser numérica y tener 8 dígitos.")]
        public string CI { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


    }

}