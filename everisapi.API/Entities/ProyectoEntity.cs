using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Entities
{
    public class ProyectoEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        public ICollection<EvaluacionEntity> Evaluaciones { get; set; }
        = new List<EvaluacionEntity>();

        public string UserNombre { get; set; }
        //AsignacionEntity esta relacionando la pregunta con la asignación
        //Mediante esta Foreign Key estamos relacionando AsignacionEntity con su Id
        [ForeignKey("UserNombre")]
        public UserEntity UserEntity { get; set; }
    }
}
