using WebAPI_Sample2.Helper;

namespace WebAPI_Sample2.ORM
{
    public class Context
    {
        #region "--> Dichiarazioni"

        private string dataPath;
        private IConfiguration _configuration;

        #endregion

        #region "--> Costruttori"

        public Context(IConfiguration configuration)
        {
            var path = configuration["DataPath"];
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            dataPath = path;
            _configuration = configuration;
        }

        #endregion

        #region --> Metodi

        #region --> Users

        public string GetUsersPath()
        {
            var filePath = Path.Combine(dataPath, "users.json");
            return filePath;
        }


        public IEnumerable<Models.UserInfo> GetUsers()
        {
            var filePath = GetUsersPath();
            if (!File.Exists(filePath)) return Enumerable.Empty<Models.UserInfo>();
            var js = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(js)) return Enumerable.Empty<Models.UserInfo>();

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Models.UserInfo>>(js);
            if (result == null) return Enumerable.Empty<Models.UserInfo>();
            return result;
        }

        public void SaveUsers(IEnumerable<Models.UserInfo> data)
        {
            var filePath = GetUsersPath();
            if (File.Exists(filePath)) File.Delete(filePath);

            var d = data.ToArray();
            var js = Newtonsoft.Json.JsonConvert.SerializeObject(d);
            File.WriteAllText(filePath, js);
        }

        public void AddUser(Models.UserInfo data)
        {
            data.UserId = Guid.NewGuid();
            var users = this.GetUsers().ToList();
            if ((from x in users where x.UserName.ToReal().ToLowerInvariant() == data.UserName.ToReal().ToLowerInvariant() select x).Any())
                throw new Exception(string.Format(@"UserName ""{0}"" already exist.", data.UserName));
            if ((from x in users where x.Email.ToReal().ToLowerInvariant() == data.Email.ToReal().ToLowerInvariant() select x).Any())
                throw new Exception(string.Format(@"Email ""{0}"" already exist.", data.Email));

            users.Add(data);
            this.SaveUsers(users);
        }

        #endregion

        #endregion

    }
}
