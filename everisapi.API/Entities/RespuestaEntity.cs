using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Entities
{
    public class RespuestaEntity
    {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(5)]
    public int Estado { get; set; }

    [StringLength(4000)]
    public string Notas { get; set; }

    [StringLength(4000)]
    public string NotasAdmin { get; set; }

    public int PreguntaId { get; set; }
    [ForeignKey("PreguntaId")]
    public PreguntaEntity PreguntaEntity { get; set; }

    public int EvaluacionId { get; set; }
    [ForeignKey("EvaluacionId")]
    public EvaluacionEntity EvaluacionEntity { get; set; }
  }
}
