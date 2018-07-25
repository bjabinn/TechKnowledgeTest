using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Entities
{
    public class EvaluacionEntity
    {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; }

    [Required]
    public bool Estado { get; set; }

    [Required]
    public ICollection<RespuestaEntity> Respuestas { get; set; }
    = new List<RespuestaEntity>();

    
    [StringLength(4000)]
    public string NotasObjetivos { get; set; }

    [StringLength(4000)]
    public string NotasEvaluacion { get; set; }

    public double Puntuacion { get; set; }


    public int ProyectoId { get; set; }
    //AsignacionEntity esta relacionando la pregunta con la asignación
    //Mediante esta Foreign Key estamos relacionando AsignacionEntity con su Id
    [ForeignKey("ProyectoId")]
    public ProyectoEntity ProyectoEntity { get; set; }
  }
}
