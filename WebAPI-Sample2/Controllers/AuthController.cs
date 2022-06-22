using WebAPI_Sample2.Helper;
using WebAPI_Sample2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel.DataAnnotations;

namespace WebAPI_Sample2.Controllers
{
    [Produces("application/json")]
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
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
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
                var result = c.Validate(token);

                //--> Restituisco la risposta
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            };
        }

        [HttpGet, Route("users"), Authorize]
        [ProducesResponseType(typeof(Models.UserInfo[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<Models.UserInfo>> GetUsers()
        {
            try
            {
                //--> Leggo gli utenti configurati
                var c = new ORM.Context(_configuration);
                var result = c.GetUsers();

                //--> Restituisco la risposta
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            };
        }

        [HttpGet, Route("user"), Authorize]
        [ProducesResponseType(typeof(Models.UserInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public ActionResult<Models.UserInfo> GetUser([Required] string id)
        {
            try
            {
                Guid userID = new Guid(id);
                //--> Leggo l'utente selezionato
                var c = new ORM.Context(_configuration);
                var result =(from x in c.GetUsers() where x.UserId == userID select x).FirstOrDefault();

                //--> Restituisco la risposta
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            };
        }

        [HttpPost, Route("add"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public IActionResult AddUser(UserInfo user)
        {
            try
            {
                //--> Aggiungo l'utente
                var c = new ORM.Context(_configuration);
                c.AddUser(user);

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
