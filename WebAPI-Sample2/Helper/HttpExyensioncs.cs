﻿namespace WebAPI_Sample2.Helper
{
    public static class HttpExyensioncs
    {

        /// <summary>
        /// Restituisce l'id utente corrente loggato
        /// </summary>
        public static Guid CurrentUserId(this HttpRequest r, IConfiguration configuration)
        {
            ////--> Verifico se il token è compilato
            string? authHeader = r.Headers["Authorization"];
            if (authHeader.ToReal().Length == 0) throw new Exception("Necessaria Autenticazione.");
            var a = new BLL.Auth(configuration);
            var claims = a.Validate(authHeader);
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
