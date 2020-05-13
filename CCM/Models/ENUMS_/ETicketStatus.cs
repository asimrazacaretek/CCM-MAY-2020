using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ENUMS_
{
    public static class ETicketStatus
    {
        public const string OPEN = "Open";   
        public const string IN_PROGRESS = "In Progress";   
        public const string PENDING = "Pending";   
        public const string RESOLVED = "Resolved";
        public const string JustWatching = "watch";
        public const string JustWatchingPending = "watchPending";
        public const string UNRESOLVED = "UnResolved";

    }

    public static class RPMServices
    {
        public const int BloodPressureID = 1;
        public const int BloodSugarID = 2;
        public const int WeightMeasurement = 1002;
    }


}