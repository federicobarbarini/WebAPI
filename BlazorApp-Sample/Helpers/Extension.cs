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

        #region --> DateTime

        public static int ToDateInt(this DateTime? d)
        {
            if (!d.HasValue) return 0;
            DateTime dt = d.ToReal();
            return dt.ToDateInt();
        }

        public static int ToDateInt(this DateTime d)
        {
            return (d.Year * 10000) + (d.Month * 100) + (d.Day);
        }

        public static DateTime FromDateInt(this int? d)
        {
            return d.ToReal().FromDateInt();
        }

        public static DateTime FromDateInt(this int d)
        {
            if (d == 0) return DateTime.MinValue;

            int year = d / 10000;
            int month= (d - (year*10000)) / 100;
            int day = d - (year * 10000 + month * 100);

            DateTime dt = new DateTime(year, month, day);
            return dt;
        }

        #endregion

    }
}
