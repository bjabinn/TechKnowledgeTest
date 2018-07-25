using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Entities
{
    public class AsignacionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Required]
        public ICollection<PreguntaEntity> PreguntasDeAsignacion { get; set; }
        = new List<PreguntaEntity>();

        [Required]
        [MaxLength(50)]
        public int Peso { get; set; }

        public int SectionId { get; set; }
        //SectionEntity esta relacionando la asignación con la section
        //Mediante esta Foreign Key estamos relacionando SectionEntity con su Id
        [ForeignKey("SectionId")]
        public SectionEntity SectionEntity { get; set; }
  }
}
