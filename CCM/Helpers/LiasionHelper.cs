using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Helpers
{
    public static class LiasionHelper
    {
        private static string LiasionPasswordKey = "#LS1013";// + liaisonId;
        internal static string GenrateLiasionPassword(string combine)
        {
            return combine + LiasionPasswordKey; 
        }
    }
}