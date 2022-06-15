using WebAPI_Sample2.Helper;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace WebAPI_Sample2.BLL
{
    public class Auth
    {

        #region "--> Dichiarazioni"

        private IConfiguration _configuration;
        private ORM.Context _context;

        #endregion

        #region "--> Costruttori"

        public Auth(IConfiguration configuration)
        {
            _configuration = configuration;
            _context = new ORM.Context(configuration);
        }

        #endregion

        #region "--> Metodi"

        public JwtSecurityToken Login(Models.LoginInfo info)
        {
            if (info == null) throw new ArgumentNullException("info", "Info is Required.");
            //if (string.IsNullOrEmpty(info.UserName)) throw new ArgumentNullException("UserName", "UserName is Required.");

            //--> Leggo gli utenti
            var users = _context.GetUsers().ToList();

            //--> Verico Utente e Password
            var user = (from x in users
                       where (x.UserName.ToReal().ToLowerInvariant() == info.UserName.ToReal().ToLowerInvariant()
                           || x.Email.ToReal().ToLowerInvariant() == info.UserName.ToReal().ToLowerInvariant())
                           && x.Password.ToReal() == info.Password.ToReal()
                      select x).FirstOrDefault();
            if (user == null) throw new Exception(string.Format(@"UserName ""{0}"" or Password is incorrect.", info.UserName));

            //--> Creo il payload 
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim("DisplayName", user.DisplayName.ToReal()),
                        new Claim("UserName", user.UserName.ToReal()),
                        new Claim("Email", user.Email.ToReal()) };

            //--> Genero il Token JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], 
                                             _configuration["Jwt:Audience"], 
                                             claims,
                                             expires: DateTime.UtcNow.AddMinutes(10), 
                                             signingCredentials: signIn);
            return token;
        }

        public Dictionary<string, string> Validate(string token)
        {

            //--> Decripto il pazload del token jwt
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var claims = jwtSecurityToken.Claims.ToArray();

            //--> Creo il dizionario contente il payload
            var TokenInfo = new Dictionary<string, string>();
            foreach (var claim in claims)
            {
                TokenInfo.Add(claim.Type, claim.Value);
            };
            return TokenInfo;
        }

        #endregion

    }
}
