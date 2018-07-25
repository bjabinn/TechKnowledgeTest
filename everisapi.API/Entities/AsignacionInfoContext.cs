using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Entities
{
    //Creamos una clase para almacenar la información de la base de datos
    public class AsignacionInfoContext : DbContext
    {


        //Modifica la base de datos que utilizaremos
        public AsignacionInfoContext(DbContextOptions<AsignacionInfoContext> options)
            : base(options)
        {
            //Esto lo utilizamos para crear la migración
            Database.EnsureCreated();
            //Este metodo es utilizado para utilizar la migración una vez creado el contexto
            //Database.Migrate();
        }

        public DbSet<AsignacionEntity> Asignaciones { get; set; }

        public DbSet<EvaluacionEntity> Evaluaciones { get; set; }

        public DbSet<PreguntaEntity> Preguntas { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<ProyectoEntity> Proyectos { get; set; }

        public DbSet<RoleEntity> Roles { get; set; }

        public DbSet<User_RoleEntity> User_Roles { get; set; }

        public DbSet<SectionEntity> Sections { get; set; }

        public DbSet<RespuestaEntity> Respuestas { get; set; }

        public DbSet<NotasSectionsEntity> NotasSections { get; set; }

        public DbSet<NotasAsignacionesEntity> NotasAsignaciones { get; set; }


  }
}
