namespace WebAPI_Sample2.Helper
{
    public static class HttpExtensioncs
    {

        /// <summary>
        /// Restituisce l'id utente corrente loggato
        /// </summary>
        public static Guid CurrentUserId(this HttpRequest r, IConfiguration configuration)
        {
            ////--> Verifico se il token è compilato
            string? authHeader = r.Headers["Authorization"];
            if (authHeader.ToReal().Length == 0) throw new Exception("Necessaria Autenticazione.");

            //--> Leggo il token dall'header della richiesta http
            string token = authHeader.Replace("Bearer ", string.Empty);

            var a = new BLL.Auth(configuration);
            var claims = a.Validate(token);
            var userId = claims["UserId"];

            return new Guid(userId);
        }

        /// <summary>
        /// Restituisce l'utente corrente loggato
        /// </summary>
        public static Models.UserInfo? CurrentUser(this HttpRequest r, IConfiguration configuration)
        {
            var userId = r.CurrentUserId(configuration);
            var c = new ORM.Context(configuration);
            var users = c.GetUsers();
            return (from x in users where x.UserId == userId select x).FirstOrDefault();
        }


    }
}
