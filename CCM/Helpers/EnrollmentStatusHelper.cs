using CCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Helpers
{
    public static class EnrollmentStatusHelper
    {
        private static readonly ApplicationdbContect _db = new ApplicationdbContect();
        private static List<EnrollmentStatus> enrollmentStatuses = _db.EnrollmentStatuss.ToList();

        public static string NotEnrolled = enrollmentStatuses.FirstOrDefault(x=>x.Name.ToLowerInvariant()== ("Not Enrolled").ToLowerInvariant()).Name;
        public static string Enrolled = enrollmentStatuses.FirstOrDefault(x=>x.Name.ToLowerInvariant()== ("Enrolled").ToLowerInvariant()).Name;
        public static string NoTQualified = enrollmentStatuses.FirstOrDefault(x=>x.Name.ToLowerInvariant()== ("Not Qualified").ToLowerInvariant()).Name;
        public static string InvalidContactinformation = enrollmentStatuses.FirstOrDefault(x=>x.Name.ToLowerInvariant()== ("Invalid Contact Information").ToLowerInvariant()).Name;
        public static string DeEnrolled = enrollmentStatuses.FirstOrDefault(x=>x.Name.ToLowerInvariant()== ("De-Enrolled").ToLowerInvariant()).Name;
        public static string Eligibility = enrollmentStatuses.FirstOrDefault(x=>x.Name.ToLowerInvariant()== ("Eligibility").ToLowerInvariant()).Name;

    }
}