using WebAPI_Sample1.Helper;
using WebAPI_Sample1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace WebAPI_Sample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region "--> Dichiarazioni"

        private IConfiguration _configuration;

        #endregion

        #region "--> Costruttori"

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion

        #region "--> Metodi"

        [HttpGet, Route("login"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetLogin([FromQuery] Models.LoginInfo info)
        {
            try
            {
                //--> Genero il token jwt
                var c = new BLL.Auth(_configuration);
                var token = c.Login(info);

                //--> Restituisco la risposta
                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            };
        }

        [HttpGet, Route("validate"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetValidate()
        {
            try
            {
                ////--> Verifico se il token è compilato
                string? authHeader = Request.Headers["Authorization"];
                if (authHeader.ToReal().Length == 0) return Unauthorized("Necessaria Autenticazione.");

                //--> Leggo il token dall'header della richiesta http
                string token =  authHeader.Replace("Bearer ", string.Empty);

                //--> Decripto il token jwt
                var c = new BLL.Auth(_configuration);
                var r = c.Validate(token);

                //--> Restituisco la risposta
                return Ok(r);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            };
        }

        [HttpPost, Route("add"), Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult AddUser(UserInfo user)
        {
            try
            {
                //--> Aggiungo l'utente
                var c = new BLL.Auth(_configuration);
                c.Add(user);

                //--> Restituisco la risposta
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            };
        }
        #endregion

    }
}
