using everisapi.API.Models;
using everisapi.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace everisapi.API.Controllers
{
    [Route("api/Token")]
    public class TokenController: Controller
    {

    private IConfiguration Configuration;
    private IUsersInfoRepository _usersInfoRespository;

    public TokenController(IConfiguration config, IUsersInfoRepository usersInfoRespository)
    {
      Configuration = config;
      _usersInfoRespository = usersInfoRespository;
    }

    [HttpPost()]
    public IActionResult CreateToken([FromBody] UsersSinProyectosDto UserAuth)
    {
      //In real example use LoginModel, this is just for dummy purpose so that
      //we can focus on relevant code

      //Comprueba que el body del json es correcto sino devolvera null
      //Si esto ocurre devolveremos un error
      if (UserAuth == null)
      {
        return BadRequest();
      }

      //Si no cumple con el modelo de creación devuelve error
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //Encriptamos la contraseña
      using (var sha256 = SHA256.Create())
      {
        // Le damos la contraseña
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(UserAuth.Password));
        // Recogemos el hash como string
        var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        // Y se lo damos 
        UserAuth.Password = hash;
      }

      IActionResult response = Unauthorized();
      if (_usersInfoRespository.UserAuth(UserAuth))
      {
        //create jwt token here and send it with response
        var jwtToken = JwtTokenBuilder();
        response = Ok(new { access_token = jwtToken });
      }

      return response;
    }

    //Metodo para crear tokens
    private object JwtTokenBuilder()
    {
      //Preparamos la clave y las credenciales
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:key"]));

      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var jwtToken = new JwtSecurityToken(issuer:Configuration["JWT:issuer"],
        audience: Configuration["JWT:audience"], signingCredentials:credentials,
        expires:DateTime.Now.AddYears(19));

      return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
  }
}
