using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VozdelEste.Models
{
    public class EditarRolViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public List<PermisoCheckbox> PermisosDisponibles { get; set; } = new List<PermisoCheckbox>();
    }

    public class PermisoCheckbox
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Seleccionado { get; set; }
    }
}