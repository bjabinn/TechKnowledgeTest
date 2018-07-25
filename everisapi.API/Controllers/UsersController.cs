using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using everisapi.API.Services;
using everisapi.API.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using everisapi.API.Entities;
using System.Text;
using System.Security.Cryptography;

namespace everisapi.API.Controllers
{
  [Authorize]
  [Route("api/users")]
  public class UsersController : Controller
  {

    //Creamos un logger
    private ILogger<UsersController> _logger;
    private IUsersInfoRepository _userInfoRepository;

    public UsersController(ILogger<UsersController> logger, IUsersInfoRepository userInfoRepository)
    {
      _logger = logger;
      _userInfoRepository = userInfoRepository;

    }

    //Introduciendo la petición de la route devuelve todos los usuarios
    [Authorize]
    [HttpGet()]
    public IActionResult GetUsers()
    {
      try
      {
        var UsersEntities = _userInfoRepository.GetUsers();

        var results = Mapper.Map<IEnumerable<UsersSinProyectosDto>>(UsersEntities);

        _logger.LogInformation("Mandamos correctamente todos los usuarios");

        return Ok(results);
      }
      catch (Exception ex)
      {
        _logger.LogCritical($"Se recogio un error al recibir todos los datos de los usuarios: " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }

    }

    //Introduciendo el nombre de usuario encuentra todos los datos de este si existe
    [HttpGet("{Nombre}")]
    public IActionResult GetUser(String Nombre, bool IncluirProyectos = false)
    {
      try
      {
        //Recoge si existe el usuario si es así la devuelve si no es así muestra un error
        var Usuario = _userInfoRepository.GetUser(Nombre, IncluirProyectos);

        if (Usuario == null)
        {
          _logger.LogInformation("El usuario con nombre " + Nombre + " no pudo ser encontrado.");
          return NotFound();
        }

        //Si tenemos como parametro recibir sus proyectos los incluirá
        //sino lo devolverá sin proyectos
        if (IncluirProyectos)
        {
          var UserResult = Mapper.Map<UsersDto>(Usuario);

          return Ok(UserResult);

        }
        else
        {
          var UserSinProyectosResult = Mapper.Map<UsersSinProyectosDto>(Usuario);

          return Ok(UserSinProyectosResult);
        }

      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir el usuario con nombre " + Nombre + ": " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }

    //Introduciendo el nombre del usuario recogemos todos sus roles
    [HttpGet("{Nombre}/roles")]
    public IActionResult GetRoles(String Nombre)
    {

      try
      {
        //Recoge si existe el usuario y si no es así devolvera un error
        var Usuario = _userInfoRepository.GetUser(Nombre, false);

        if (Usuario == null)
        {
          _logger.LogInformation("El usuario con nombre " + Nombre + " no pudo ser encontrado recogiendo roles.");
          return NotFound();
        }
        //Recoge todos los roles para este usuario en específico
        var RolesAsignados = _userInfoRepository.GetRolesUsuario(Usuario);

        //Devolvera sus roles aunque esten vacios
        var RolesResult = Mapper.Map<List<RoleDto>>(RolesAsignados);

        return Ok(RolesResult);

      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir los roles de usuario con nombre " + Nombre + ": " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }

    /*ADD USUARIOS*/
    [HttpPost("add")]
    public IActionResult AddUsers([FromBody] UsersWithRolesDto UsuarioAdd)
    {
      //Si los datos son validos los guardara
      if (UsuarioAdd == null || _userInfoRepository.UserExiste(UsuarioAdd.Nombre))
      {
        return BadRequest();
      }

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //Encriptamos la contraseña
      using (var sha256 = SHA256.Create())
      {
        // Le damos la contraseña
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(UsuarioAdd.Password));
        // Recogemos el hash como string
        var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        // Y se lo damos 
        UsuarioAdd.Password = hash;
      }

      //Comprueba que se guardo bien y lo envia
      if (_userInfoRepository.AddUser(Mapper.Map<UserEntity>(UsuarioAdd)))
      {
        return Ok("El usuario fue creado.");
      }
      else
      {
        return BadRequest();
      }
    }

    /*UPDATE USUARIOS*/
    [HttpPut("update")]
    public IActionResult UpdateUsers([FromBody] UsersWithRolesDto UsuarioUpdate)
    {
      //Si los datos son validos los guardara
      if (UsuarioUpdate == null)
      {
        return BadRequest();
      }

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //Encriptamos la contraseña
      using (var sha256 = SHA256.Create())
      {
        // Le damos la contraseña
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(UsuarioUpdate.Password));
        // Recogemos el hash como string
        var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        // Y se lo damos 
        UsuarioUpdate.Password = hash;
      }

      //Comprueba que se guardo bien y lo envia
      if (_userInfoRepository.AlterUser(Mapper.Map<UserEntity>(UsuarioUpdate)))
      {
        return Ok("El usuario fue modificado correctamente.");
      }
      else
      {
        return BadRequest();
      }
    }

    /*DELETE USUARIOS*/
    [HttpDelete("delete")]
    public IActionResult DeleteUsers([FromBody] UsersSinProyectosDto usuarioDelete)
    {
      //Si los datos son validos los guardara
      if (usuarioDelete == null)
      {
        return BadRequest();
      }

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //Encriptamos la contraseña
      using (var sha256 = SHA256.Create())
      {
        // Le damos la contraseña
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(usuarioDelete.Password));
        // Recogemos el hash como string
        var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        // Y se lo damos 
        usuarioDelete.Password = hash;
      }

      //Comprueba que se guardo bien y lo envia
      if (_userInfoRepository.DeleteUser(Mapper.Map<UserEntity>(usuarioDelete)))
      {
        return Ok("Eliminación completada");
      }
      else
      {
        return BadRequest();
      }
    }
  }
}
