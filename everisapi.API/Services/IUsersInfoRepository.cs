using everisapi.API.Entities;
using everisapi.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Services
{
  public interface IUsersInfoRepository
  {
    //Devuelve todos los usuarios
    IEnumerable<UserEntity> GetUsers();

    //Devuelve un usuario
    UserEntity GetUser(string userNombre, Boolean IncluirProyectos);

    //Devuelve todos los proyectos de un usuario
    IEnumerable<ProyectoEntity> GetProyectosDeUsuario(string userNombre);

    //Devuelve un proyecto de un usuario
    ProyectoEntity GetOneProyecto(string userNombre, int proyectoId);

    //Devuelve si un usuario existe o no
    bool UserExiste(string userNombre);

    //Devuelve si un usuario existe o no y si esta bien logeado
    bool UserAuth(UsersSinProyectosDto UserForAuth);

    //Devuelve todos los roles de este usuario
    IEnumerable<RoleEntity> GetRolesUsuario(UserEntity usuario);

    //Devuelve todos los proyectos de todos los usuarios
    IEnumerable<ProyectoEntity> GetFullProyectos();

    //Devuelve un proyecto con todos sus datos
    ProyectoEntity GetFullProject(int id);

    //Aqui introducimos un nuevo usuario
    bool AddUser(UserEntity usuario);

    //Este metodo nos permite persistir los cambios en las entidades
    bool SaveChanges();

    //Nos permite modificar un usuario
    bool AlterUser(UserEntity usuario);

    //Elimina una pregunta concreta de una asignación
    bool DeleteUser(UserEntity usuario);

    //Aqui introducimos un nuevo proyecto
    bool AddProj(ProyectoEntity proyecto);

    //Nos permite modificar un proyecto
    bool AlterProj(ProyectoEntity proyecto);

    //Elimina un proyecto
    bool DeleteProj(ProyectoEntity proyecto);

    //Elimina todo de lo que depende un usuario
    void DeleteRolesOrProjects(UserEntity usuario);

    //Devuelve si existe un proyecto
    bool ProyectoExiste(int ProyectoId);
  }
}
