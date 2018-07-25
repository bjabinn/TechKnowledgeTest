using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace everisapi.API.Models
{
    public class PreguntaCreateDto
    {
        [Required]
        [MaxLength(120)]
        public string Pregunta { get; set; }

        [Required]
        public bool Respuesta { get; set; }
    }
}
