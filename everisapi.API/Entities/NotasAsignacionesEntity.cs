using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Entities
{
    public class NotasAsignacionesEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(4000)]
        public string Notas { get; set; }

        public int AsignacionId { get; set; }
        [ForeignKey("AsignacionId")]
        public AsignacionEntity AsignacionEntity { get; set; }

        public int EvaluacionId { get; set; }
        [ForeignKey("EvaluacionId")]
        public EvaluacionEntity EvaluacionEntity { get; set; }
    }
}
