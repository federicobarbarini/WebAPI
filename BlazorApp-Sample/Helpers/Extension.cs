using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.Web;

namespace BlazorApp_Sample.Helpers
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

        #region --> QueryString

        public static NameValueCollection QueryString(this NavigationManager navigationManager)
        {
            return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
        }

        public static string QueryString(this NavigationManager navigationManager, string key)
        {
            return navigationManager.QueryString()[key];
        }

        #endregion

    }
}
