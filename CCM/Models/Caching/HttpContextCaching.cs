using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace CCM.Models.Caching
{
    public static class HttpContextCaching
    {
        internal static object GetCachData(string key)
        {
            return HttpContext.Current.Cache.Get("enrolmentstatus");
        }

        internal static void AddDataToCache(string key, object data,int timeinminuts, TimeSpan slidingExpiration)
        {
            HttpContext.Current.Cache.Insert(key, data, null, DateTime.Now.AddMinutes(timeinminuts), slidingExpiration);
        }
    }
}