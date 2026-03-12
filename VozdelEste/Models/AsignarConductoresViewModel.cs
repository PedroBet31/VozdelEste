using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VozdelEste.Models
{
    public class AsignarConductoresViewModel
    {
        public int ProgramaId { get; set; }
        public string ProgramaNombre { get; set; }
        public List<ConductorAsignado> ConductoresDisponibles { get; set; }
    }

    public class ConductorAsignado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Seleccionado { get; set; }
    }

}