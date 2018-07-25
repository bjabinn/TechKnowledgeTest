using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace everisapi.API.Models
{
    public class EvaluacionCreateUpdateDto
    {
    public int Id { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    public bool Estado { get; set; }
    
    public string NotasObjetivos { get; set; }

    public string NotasEvaluacion { get; set; }

    public double Puntuacion { get; set; }

    [Required]
    public int ProyectoId { get; set; }

  }
}
