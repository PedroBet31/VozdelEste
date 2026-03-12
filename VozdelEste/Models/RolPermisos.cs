using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace VozdelEste.Models
{
    public class RolPermisos
    {
        [Column(Order = 0)]
        public int RolId { get; set; }

        [Column(Order = 1)]
        public int PermisoId { get; set; }

        public virtual Roles Roles { get; set; }
        public virtual Permisos Permisos { get; set; }


    }
}