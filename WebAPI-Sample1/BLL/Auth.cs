using WebAPI_Sample1.Helper;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace WebAPI_Sample1.BLL
{
    public class Auth
    {

        #region "--> Dichiarazioni"

        private string dataPath;
        private IConfiguration _configuration;

        #endregion

        #region "--> Costruttori"

        public Auth(IConfiguration configuration)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            dataPath = Path.Combine(path, "users.json");
            _configuration = configuration; 
        }

        #endregion

        #region "--> Metodi"

        public JwtSecurityToken Login(Models.LoginInfo info)
        {
            if (info == null) throw new ArgumentNullException("info", "Info is Required.");
            if (string.IsNullOrEmpty(info.UserName)) throw new ArgumentNullException("UserName", "UserName is Required.");

            //--> Leggo gli utenti
            var users = this.GetUsers().ToList();

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
                        new Claim(JwtRegisteredClaimNames.Jti, user.UserId.ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim("DisplayName", user.DisplayName.ToReal()),
                        new Claim("UserName", user.UserName.ToReal()),
                        new Claim("Email", user.Email.ToReal())
            };

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

        public IEnumerable<Models.UserInfo> GetUsers()
        {
            if (!File.Exists(dataPath)) return Enumerable.Empty<Models.UserInfo>();
            var js = File.ReadAllText(dataPath);
            if (string.IsNullOrEmpty(js)) return Enumerable.Empty<Models.UserInfo>();

            var r = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Models.UserInfo>>(js);
            if (r==null) return Enumerable.Empty<Models.UserInfo>();
            return r;
        }

        public void Save(IEnumerable<Models.UserInfo> data)
        {
            var d = data.ToArray();
            if (File.Exists(dataPath)) File.Delete(dataPath);   

            var js = Newtonsoft.Json.JsonConvert.SerializeObject(d);
            File.WriteAllText(dataPath, js);
        }

        public void Add(Models.UserInfo data) 
        {
            var users = this.GetUsers().ToList();
            if ((from x in users where x.UserName.ToReal().ToLowerInvariant() == data.UserName.ToReal().ToLowerInvariant() select x).Any()) throw new Exception(string.Format(@"UserName ""{0}"" already exist.", data.UserName));
            if ((from x in users where x.Email.ToReal().ToLowerInvariant() == data.Email.ToReal().ToLowerInvariant() select x).Any()) throw new Exception(string.Format(@"Email ""{0}"" already exist.", data.Email));
            users.Add(data);
            this.Save(users);
        }

        #endregion

    }
}
