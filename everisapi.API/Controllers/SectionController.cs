using everisapi.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using AutoMapper;
using everisapi.API.Models;
using everisapi.API.Entities;
using Microsoft.AspNetCore.Authorization;

namespace everisapi.API.Controllers
{
  [Authorize]
  [Route("api/sections")]
  public class SectionController : Controller
  {

    //Creamos un logger
    private ILogger<SectionController> _logger;
    private ISectionsInfoRepository _sectionInfoRepository;

    //Utilizamos el constructor para inicializar el logger
    public SectionController(ILogger<SectionController> logger,
      ISectionsInfoRepository sectionInfoRepository)
    {
      _logger = logger;
      _sectionInfoRepository = sectionInfoRepository;
    }

    //Recogemos todas las sections de la base de datos
    [HttpGet()]
    public IActionResult GetSections()
    {
      try
      {
        var SectionEntities = _sectionInfoRepository.GetSections();

        var results = Mapper.Map<IEnumerable<SectionWithoutAreaDto>>(SectionEntities);

        _logger.LogInformation("Mandamos correctamente todas las sections.");

        return Ok(results);
      }
      catch (Exception ex)
      {
        _logger.LogCritical($"Se recogio un error al recibir todos los datos de las sections: " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }

    //Introduciendo el nombre de usuario encuentra todos los datos de este si existe
    [HttpGet("{id}")]
    public IActionResult GetSection(int id, bool IncluirAsignaciones = false)
    {
      try
      {
        //Recoge si existe el usuario si es así la devuelve si no es así muestra un error
        var Section = _sectionInfoRepository.GetSection(id, IncluirAsignaciones);

        if (Section == null)
        {
          _logger.LogInformation("La section con id " + id + " no pudo ser encontrado.");
          return NotFound();
        }

        //Si tenemos como parametro recibir sus proyectos los incluirá
        //sino lo devolverá sin proyectos
        if (IncluirAsignaciones)
        {
          var SectionResult = Mapper.Map<SectionDto>(Section);

          return Ok(SectionResult);

        }
        else
        {
          var SectionWithoutAreaResult = Mapper.Map<SectionWithoutAreaDto>(Section);

          return Ok(SectionWithoutAreaResult);
        }

      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir la section con id " + id + ": " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }

    //Recogemos el numero máximo de preguntas de esta section para un proyecto especifico
    [HttpGet("{id}/evaluacion/{idevaluacion}/preguntas")]
    public IActionResult GetNumPreguntas(int id, int idevaluacion)
    {

      try
      {
        //Comprueba si existe la section y si existe manda un json con la información
        //si no existe mandara un error 404 el error 500 aparecera si el servidor falla
        SectionEntity sectionExist = _sectionInfoRepository.GetSection(id, true);
        if (sectionExist == null)
        {
          _logger.LogInformation($"La section con id " + id + " no pudo ser encontrado.");
          return NotFound();
        }

        //Recogemos un numero de preguntas por id y el id de proyecto
        var NumeroPreguntasResult = _sectionInfoRepository.GetNumPreguntasFromSection(id, idevaluacion);

        return Ok(NumeroPreguntasResult);

      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir el número de preguntas para la section " + id + " y la evaluación " + idevaluacion + ": " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }

    //Recogemos el numero de respuestas positivas filtrado por id de proyecto y id de section
    [HttpGet("{id}/evaluacion/{idevaluacion}/respuestas")]
    public IActionResult GetNumRespuestas(int id, int idevaluacion)
    {

      try
      {
        //Comprueba si existe la section y si existe manda un json con la información
        //si no existe mandara un error 404 el error 500 aparecera si el servidor falla
        SectionEntity sectionExist = _sectionInfoRepository.GetSection(id, true);
        if (sectionExist == null)
        {
          _logger.LogInformation($"La section con id " + id + " no pudo ser encontrado.");
          return NotFound();
        }

        //Recogemos un numero de preguntas por id y el id de proyecto
        var NumeroRespuestasResult = _sectionInfoRepository.GetRespuestasCorrectasFromSection(id, idevaluacion);

        return Ok(NumeroRespuestasResult);

      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir el número de respuestas correctas para la section " + id + " y el evaluacion " + idevaluacion + ": " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }

    //Recogemos la id de la sección y devuelve un objeto con información extendida
    [HttpGet("evaluacion/{id}")]
    public IActionResult GetDatosEvaluacionFromEval(int id)
    {

      try
      {
        //Comprueba si existe la section y si existe manda un json con la información
        //si no existe mandara un error 404 el error 500 aparecera si el servidor falla
        var SectionInfo = _sectionInfoRepository.GetSectionsInfoFromEval(id);
        if (SectionInfo == null)
        {
          _logger.LogInformation($"La section con id de evaluación" + id + " no pudo ser encontrado.");
          return NotFound();
        }

        return Ok(SectionInfo);

      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir el número de respuestas correctas para la section  con la id de evaluación " + id + ": " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }

    //Introduciendo el nombre del usuario recogemos todos sus roles
    [HttpGet("{id}/asignaciones")]
    public IActionResult GetAsignacionesFromSection(int id)
    {

      try
      {
        //Comprueba si existe asignación y si existe manda un json con la información
        //si no existe mandara un error 404 el error 500 aparecera si el servidor falla
        SectionEntity sectionExist = _sectionInfoRepository.GetSection(id, true);
        if (sectionExist == null)
        {
          _logger.LogInformation($"La section con id " + id + " no pudo ser encontrado.");
          return NotFound();
        }

        //Recogemos una lista de preguntas de la asignacion
        var asignacionesDeSection = _sectionInfoRepository.GetAsignacionesFromSection(sectionExist);

        //Transformamos la lista anterior en una nueva con los datos que necesitamos
        //Ya que otros son relevantes
        var AsignacionesResult = Mapper.Map<IEnumerable<AsignacionDto>>(asignacionesDeSection);
        return Ok(AsignacionesResult);

      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir las asignaciones de la section con id " + id + ": " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }

    /*ADD SECTIONS*/
    [HttpPost("add")]
    public IActionResult AddAsignacion([FromBody] SectionWithoutAreaDto SectionAdd)
    {

      //Si los datos son validos los guardara
      if (SectionAdd == null || _sectionInfoRepository.GetSection(SectionAdd.Id, false) != null)
      {
        return BadRequest();
      }

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //Comprueba que se guardo bien y lo envia
      if (_sectionInfoRepository.AddSection(Mapper.Map<SectionEntity>(SectionAdd)))
      {
        return Ok("La asignación fue creada.");
      }
      else
      {
        return BadRequest();
      }
    }

    /*UPDATE SECTIONS*/
    [HttpPut("update")]
    public IActionResult UpdateSection([FromBody] SectionWithoutAreaDto SectionUpdate)
    {
      //Si los datos son validos los guardara
      if (SectionUpdate == null || _sectionInfoRepository.GetSection(SectionUpdate.Id, false) == null)
      {
        return BadRequest();
      }

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //Comprueba que se guardo bien y lo envia
      if (_sectionInfoRepository.AlterSection(Mapper.Map<SectionEntity>(SectionUpdate)))
      {
        return Ok("La asignación fue modificada correctamente.");
      }
      else
      {
        return BadRequest();
      }
    }

    /*UPDATE SECTIONS*/
    [HttpPut("addNotas")]
    public IActionResult AddNotas([FromBody] SectionWithNotasDto SectionUpdate)
    {
      //Si los datos son validos
      if (SectionUpdate == null || _sectionInfoRepository.GetSection(SectionUpdate.SectionId, false) == null)
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
        if (_sectionInfoRepository.AddNotasSection(SectionUpdate))
        {
          return Ok("Se añadio la nota correctamente");
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al añadir las notas: " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }

    /*DELETE SECTIONS*/
    [HttpDelete("delete")]
    public IActionResult DeleteSection([FromBody] SectionWithoutAreaDto SectionDelete)
    {
      //Si los datos son validos los guardara
      if (SectionDelete == null || _sectionInfoRepository.GetSection(SectionDelete.Id, false) == null)
      {
        return BadRequest();
      }

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //Comprueba que se guardo bien y lo envia
      if (_sectionInfoRepository.DeleteSection(Mapper.Map<SectionEntity>(SectionDelete)))
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
