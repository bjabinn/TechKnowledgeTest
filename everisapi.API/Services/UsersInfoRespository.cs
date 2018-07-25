using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using everisapi.API.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using everisapi.API.Models;

namespace everisapi.API.Services
{
  public class UsersInfoRespository : IUsersInfoRepository
  {

    private AsignacionInfoContext _context;

    //Le damos un contexto en el constructor
    public UsersInfoRespository(AsignacionInfoContext context)
    {
      _context = context;
    }

    //Devuelve un solo proyecto de un usuario
    public ProyectoEntity GetOneProyecto(string userNombre, int proyectoId)
    {
      return _context.Proyectos.Where(p => p.UserNombre == userNombre && p.Id == proyectoId).FirstOrDefault();
    }

    //Recoge todos los proyectos de un usuario
    public IEnumerable<ProyectoEntity> GetProyectosDeUsuario(string userNombre)
    {
      return _context.Proyectos.Where(p => p.UserNombre == userNombre).OrderBy( p => p.Nombre).ToList();
    }

    //Recoge todos los proyectos de todos los usuarios
    public IEnumerable<ProyectoEntity> GetFullProyectos()
    {
      return _context.Proyectos.OrderBy(p => p.Nombre).ToList();
    }

    //Recoge un usuario por su nombre 
    public UserEntity GetUser(string userNombre, bool IncluirProyectos = true)
    {
      if (IncluirProyectos)
      {
        //Si se quiere incluir los proyectos del usuario entrara aquí
        //Incluimos los proyectos del usuario especificada (Include extiende de Microsoft.EntityFrameworkCore)
        return _context.Users.Include(u => u.ProyectosDeUsuario).
            Where(u => u.Nombre == userNombre).FirstOrDefault();
      }
      else
      {
        //Si no es así devolveremos solo el usuario
        return _context.Users.Include(u => u.User_Role).Where(u => u.Nombre == userNombre).FirstOrDefault();
      }
    }

    //Recoge todos los usuarios
    public IEnumerable<UserEntity> GetUsers()
    {
      //Devolvemos todos los usuarios ordenadas por Nombre
      return _context.Users.OrderBy(c => c.Nombre).ToList();
    }

    //Devuelve si el usuario existe
    public bool UserExiste(string userNombre)
    {
      return _context.Users.Any(u => u.Nombre == userNombre);
    }

    //Devuelve todos los roles de usuario
    public IEnumerable<RoleEntity> GetRolesUsuario(UserEntity usuario)
    {
      var RolesAsignados = _context.User_Roles.Where(ur => ur.User == usuario).ToList();
      List<RoleEntity> RolesEntregar = new List<RoleEntity>();
      foreach (User_RoleEntity usuario_roles in RolesAsignados)
      {
        var Resolver = _context.Roles.Where(r => r.Id == usuario_roles.RoleId).FirstOrDefault();
        RolesEntregar.Add(Resolver);

      }
      return RolesEntregar;
    }

    //Devuelve una lista con todos los datos del proyecto por su id
    public ProyectoEntity GetFullProject(int id)
    {
      return _context.Proyectos.Include(p => p.Evaluaciones).
               ThenInclude(Evaluacion => Evaluacion.Respuestas).
               Where(p => p.Id == id).FirstOrDefault();
    }

    //Devuelve si el usuario esta bien logeado o no
    public bool UserAuth(UsersSinProyectosDto UserForAuth)
    {
      return _context.Users.Any(u => u.Nombre == UserForAuth.Nombre && u.Password == UserForAuth.Password);
    }

    /*GUARDAR DATOS EN USUARIO*/
    //Aqui introducimos un nuevo usuario
    public bool AddUser(UserEntity usuario)
    {
      _context.Users.Add(usuario);
      return SaveChanges();
    }

    //Este metodo nos permite persistir los cambios en las entidades
    public bool SaveChanges()
    {
      return (_context.SaveChanges() >= 0);
    }

    /*UPDATE USER*/
    //Nos permite modificar un usuario
    public bool AlterUser(UserEntity usuario)
    {
      var UserAlter = _context.Users.Where(u => u.Nombre == usuario.Nombre).FirstOrDefault();
      UserAlter.Nombre = usuario.Nombre;
      UserAlter.Password = usuario.Password;
      //Le quitamos los anteriores roles
      var ListaRole = _context.Users.Include(u => u.User_Role).Where(u => u.Nombre == usuario.Nombre).FirstOrDefault().User_Role;
      foreach (var role in ListaRole)
      {
        _context.User_Roles.Remove(role);
      }

      //Se los volvemos a incluir
      foreach (var role in usuario.User_Role)
      {
        _context.User_Roles.Add(role);
      }

      return SaveChanges();
    }

    /*ELIMINAR DATOS*/
    //Elimina una pregunta concreta de una asignación
    public bool DeleteUser(UserEntity usuario)
    {
      _context.Users.Remove(usuario);
      return SaveChanges();

    }

    //Elimina todos los proyectos y roles de los que depende el usuario
    public void DeleteRolesOrProjects(UserEntity usuario)
    {
      var Usuario = _context.Users.Include(u => u.User_Role).Include(u => u.ProyectosDeUsuario).ThenInclude(p => p.Evaluaciones).Where(u => u.Nombre == usuario.Nombre).FirstOrDefault();
      if (Usuario.User_Role.Count != 0 && Usuario.ProyectosDeUsuario.Count != 0)
      {
        foreach (var role in Usuario.User_Role)
        {
          _context.User_Roles.Remove(role);
        }

        foreach (var proyecto in Usuario.ProyectosDeUsuario)
        {
          foreach (var evaluacion in proyecto.Evaluaciones)
          {
            _context.Evaluaciones.Remove(evaluacion);
          }
          _context.Proyectos.Remove(proyecto);
        }
        SaveChanges();
      }
    }

    //Aqui introducimos un nuevo proyecto
    public bool AddProj(ProyectoEntity proyecto)
    {
      _context.Proyectos.Add(proyecto);
      return SaveChanges();
    }

    //Nos permite modificar un proyecto
    public bool AlterProj(ProyectoEntity proyecto)
    {
      var AlterProject = _context.Proyectos.Where(p => p.Id == proyecto.Id).FirstOrDefault();

      AlterProject.Nombre = proyecto.Nombre;
      AlterProject.Fecha = proyecto.Fecha;
      AlterProject.UserNombre = proyecto.UserNombre;

      return SaveChanges();
    }

    //Elimina un proyecto
    public bool DeleteProj(ProyectoEntity proyecto)
    {
      _context.Proyectos.Remove(proyecto);
      return SaveChanges();
    }

    //Devuelve si existe un proyecto
    public bool ProyectoExiste(int ProyectoId)
    {
      return _context.Proyectos.Any(p => p.Id == ProyectoId);
    }
  }
}
