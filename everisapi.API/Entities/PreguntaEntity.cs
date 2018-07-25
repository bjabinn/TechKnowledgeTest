using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Entities
{
    public class PreguntaEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Pregunta { get; set; }

        public String Correcta { get; set; }

        public int AsignacionId { get; set; }
        //AsignacionEntity esta relacionando la pregunta con la asignación
        //Mediante esta Foreign Key estamos relacionando AsignacionEntity con su Id
        [ForeignKey("AsignacionId")]
        public AsignacionEntity AsignacionEntity { get; set; }

    }
}
