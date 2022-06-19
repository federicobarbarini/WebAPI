using Microsoft.OpenApi.Models;

namespace WebAPI_Sample2.Helper
{
    public static class Extension
    {

        #region --> ToReal

        public static string ToReal(this string? value)
        {
            if (value == null) return string.Empty;
            return value.ToString();
        }

        public static Guid ToReal(this Guid? value)
        {
            if (value == null) return Guid.Empty;
            return value.Value;
        }

        public static int ToReal(this int? value)
        {
            if (value == null) return 0;
            return value.Value;
        }

        public static decimal ToReal(this decimal? value)
        {
            if (value == null) return 0;
            return value.Value;
        }

        public static bool ToReal(this bool? value)
        {
            if (value == null) return false;
            return value.Value;
        }

        public static DateTime ToReal(this DateTime? value)
        {
            if (value == null) return DateTime.MinValue;
            return value.Value;
        }

        public static TimeSpan ToReal(this TimeSpan? value)
        {
            if (value == null) return TimeSpan.MinValue;
            return value.Value;
        }

        #endregion

        #region --> SwaggerJWTAuth

        public static void AddSwaggerJWTAuth(this WebApplicationBuilder builder)
        {
            
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
        }

        #endregion

    }
}
