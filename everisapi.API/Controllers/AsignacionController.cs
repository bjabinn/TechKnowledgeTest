using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using everisapi.API.Models;
using everisapi.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using everisapi.API.Entities;
using Microsoft.AspNetCore.Authorization;

namespace everisapi.API.Controllers
{
  [Authorize]
  [Route("api/asignaciones")]
  public class AsignacionController : Controller
  {

    //Creamos un logger
    private ILogger<AsignacionController> _logger;
    private IAsignacionInfoRepository _asignacionInfoRepository;

    public AsignacionController(ILogger<AsignacionController> logger, IAsignacionInfoRepository asignacionInfoRepository)
    {
      _logger = logger;
      _asignacionInfoRepository = asignacionInfoRepository;

    }

    //Introduciendo la petición de la route devuelve todas las asignaciones y sus preguntas 
    [HttpGet()]
    public IActionResult GetAsignaciones()
    {
      try
      {
        var AsignacionEntities = _asignacionInfoRepository.GetAsignaciones();

        var results = Mapper.Map<IEnumerable<AsignacionSinPreguntasDto>>(AsignacionEntities);

        return Ok(results);
      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir todos los datos de las asignaciones: " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }

    }


    //Recoge una lista de asignaciones con todas sus preguntas y su respuesta filtrada por id de una evaluación
    [HttpGet("evaluacion/{id}")]
    public IActionResult GetAsignacionFromEval(int id)
    {
      try
      {
        var AsignacionesWithInfo = _asignacionInfoRepository.GetAsignFromEval(id);

        return Ok(AsignacionesWithInfo);
      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir todos los datos de las asignaciones: " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }

    }

    //Recoge una asignación con todas sus preguntas y su respuesta filtrada por id de evaluación y otra de asignación
    [HttpGet("evaluacion/{idEval}/asignacion/{idAsig}")]
    public IActionResult GetAsignacionFromEval(int idEval, int idAsig)
    {
      try
      {
        var AsignacionesWithInfo = _asignacionInfoRepository.GetAsignFromEvalAndAsig(idEval, idAsig);

        return Ok(AsignacionesWithInfo);
      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir todos los datos de las asignaciones: " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }

    }

    //Recoge una asignación con todas sus preguntas y su respuesta filtrada por id de evaluación y otra de asignación
    [HttpGet("evaluacion/{idEval}/section/{idSection}")]
    public IActionResult GetAsignacionFromSection(int idEval, int idSection)
    {
      try
      {
        var AsignacionesWithInfo = _asignacionInfoRepository.GetAsignFromEvalAndSection(idEval, idSection);

        return Ok(AsignacionesWithInfo);
      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir todos los datos de las asignaciones: " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }

    }

    //Introduciendo la id de la asignacion devuelve una asignación especifica
    [HttpGet("{id}")]
    public IActionResult GetAsignacion(int id, bool IncluirPreguntas = false)
    {
      try
      {
        //Recoge si existe la asignación si es asi la devuelve si no es así muestra un error
        var Asignacion = _asignacionInfoRepository.GetAsignacion(id, IncluirPreguntas);

        if (Asignacion == null)
        {
          _logger.LogInformation("La asignación con id " + id + " no pudo ser encontrado.");
          return NotFound();
        }

        //Si tenemos como parametro recibir sus preguntas las incluira
        //sino lo devolvera sin preguntas
        if (IncluirPreguntas)
        {
          var AsignacionResult = Mapper.Map<AsignacionDto>(Asignacion);

          return Ok(AsignacionResult);

        }
        else
        {
          var AsignacionSinPregunta = Mapper.Map<AsignacionSinPreguntasDto>(Asignacion);

          return Ok(AsignacionSinPregunta);
        }

      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir la asignación con id " + id + ": " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }

    /*ADD ASIGNACIONES*/
    [HttpPost("add")]
    public IActionResult AddAsignacion([FromBody] AsignacionCreateUpdateDto AsignacionAdd)
    {

      //Si los datos son validos los guardara
      if (AsignacionAdd == null || _asignacionInfoRepository.AsignacionExiste(AsignacionAdd.Id))
      {
        return BadRequest();
      }

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //Comprueba que se guardo bien y lo envia
      if (_asignacionInfoRepository.AddAsig(Mapper.Map<AsignacionEntity>(AsignacionAdd)))
      {
        return Ok("La asignación fue creada.");
      }
      else
      {
        return BadRequest();
      }
    }

    /*UPDATE ASIGNACIONES*/
    [HttpPut("update")]
    public IActionResult UpdateAsignacion([FromBody] AsignacionCreateUpdateDto AsignacionUpdate)
    {
      //Si los datos son validos los guardara
      if (AsignacionUpdate == null || !_asignacionInfoRepository.AsignacionExiste(AsignacionUpdate.Id))
      {
        return BadRequest();
      }

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //Comprueba que se guardo bien y lo envia
      if (_asignacionInfoRepository.AlterAsig(Mapper.Map<AsignacionEntity>(AsignacionUpdate)))
      {
        return Ok("La asignación fue modificada correctamente.");
      }
      else
      {
        return BadRequest();
      }
    }


    //Añadir notas a una asignacion
    [HttpPut("addNotas")]
    public IActionResult AddNotasAsignacion([FromBody] AsignacionUpdateNotasDto AsignacionUpdate)
    {
      //Si los datos son validos los guardara
      if (AsignacionUpdate == null || !_asignacionInfoRepository.AsignacionExiste(AsignacionUpdate.Id))
      {
        return BadRequest();
      }

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {

        //Comprueba que se guardo bien y lo envia
        if (_asignacionInfoRepository.AddNotas(AsignacionUpdate))
        {
          return Ok("La asignación fue modificada correctamente.");
        }
        else
        {
          return BadRequest();
        }

      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir todos los datos de las asignaciones: " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }

    }

    //Recoge una lista de asignaciones con todas sus preguntas y su respuesta filtrada por id de una evaluación
    [HttpGet("evaluacion/{id}/notas")]
    public IActionResult GetNotasAsignaciones(int id)
    {
      try
      {
        var AsignacionesWithInfo = _asignacionInfoRepository.GetAsignConNotas(id);

        return Ok(AsignacionesWithInfo);
      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir todos los datos de las asignaciones: " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }

    }

    /*DELETE ASIGNACIONES*/
    [HttpDelete("delete")]
    public IActionResult DeleteAsignacion([FromBody] AsignacionCreateUpdateDto AsignacionDelete)
    {
      //Si los datos son validos los guardara
      if (AsignacionDelete == null || !_asignacionInfoRepository.AsignacionExiste(AsignacionDelete.Id))
      {
        return BadRequest();
      }

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //Comprueba que se guardo bien y lo envia
      if (_asignacionInfoRepository.DeleteAsig(Mapper.Map<AsignacionEntity>(AsignacionDelete)))
      {
        return Ok("La asignación fue eliminada correctamente.");
      }
      else
      {
        return BadRequest();
      }
    }
  }
}
