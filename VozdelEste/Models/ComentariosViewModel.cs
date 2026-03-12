using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VozdelEste.Models
{
    public class ComentarioDetalleViewModel
    {
        public int Id { get; set; }
        public string ClienteCI { get; set; }
        public string ClienteNombre { get; set; }
        public string Comentario { get; set; }
        public DateTime Fecha { get; set; }
        public int ProgramaId { get; set; }
    }

    public class ComentariosViewModel
    {
        public Programas Programa { get; set; }
        public List<ComentarioDetalleViewModel> Comentarios { get; set; }
        public ComentariosProgramas NuevoComentario { get; set; }
    }
}