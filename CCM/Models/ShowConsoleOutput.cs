using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace CCM.Models
{
    public static class ShowConsoleOutput
    {
       
        public static void ConsoleMassage(this object obj,string text)
        {
            Debug.WriteLine(text+" - "+Newtonsoft.Json.JsonConvert.SerializeObject(obj));
        }
        public static int? GetNullableInteger(this string text)
        {
            return string.IsNullOrEmpty(text) == false ? Convert.ToInt32(text) : (int?)null;
        }
        public static int GetInteger(this string text)
        {
            return  Convert.ToInt32(text);
        }
        public static int GetInteger(this int? text)
        {
            return (int)text;
        }
    }
}