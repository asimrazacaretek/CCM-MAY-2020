using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CCM.Ability
{

    public class Rootobject
    {
        public Eligibilityrequest eligibilityRequest { get; set; }
    }

    public class Eligibilityrequest
    {
        public Provider provider { get; set; }
        public Subscriber subscriber { get; set; }
        public Dependent dependent { get; set; }
        public Servicedates serviceDates { get; set; }
        public Servicetypecodes serviceTypeCodes { get; set; }
        public int payerIdentifier { get; set; }
    }

    public class Provider
    {
        public int npi { get; set; }
        public string lastName { get; set; }
    }

    public class Subscriber
    {
        public string memberIdentifier { get; set; }
    }

    public class Dependent
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string dateOfBirth { get; set; }
    }

    public class Servicedates
    {
        public string start { get; set; }
    }

    public class Servicetypecodes
    {
        public string[] serviceTypeCode { get; set; }
    }

    public class AbilityEligibility
    {
        public string ClientUsername { get; set; }
        public string ClientPassword { get; set; }

        public string AbilityUserName { get; set; }
        public string AbilityPassword { get; set; }

        public class AccessToken
        {
            public string access_token { get; set; }
            public string expires_in { get; set; }

            public string token_type { get; set; }
        }
        public AbilityEligibility()
        {
            ClientUsername = "";
            ClientPassword = "";
            AbilityUserName = "";
            AbilityPassword = "";
        }
        public async Task<string> GetAccessTokenAsync()
        {
          
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(ClientUsername + ":" + ClientPassword));
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://idp.myabilitynetwork.com/connect/token"))
                {
                    
                    request.Headers.TryAddWithoutValidation("Authorization", "Basic "+ encoded);

                    request.Content = new StringContent("grant_type=password&username="+ AbilityUserName +"&password=" + AbilityPassword +"&scope=openid%20ability%3Aaccessapi", Encoding.UTF8, "application/x-www-form-urlencoded");

                    var response = await httpClient.SendAsync(request);
                }
            }
            return "";


        }
        public async Task<string> TestEcho()
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.abilitynetwork.com/v1/test/echo"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer <authentication token>");

                    request.Content = new StringContent("{ \"test\" : \"test1\" }", Encoding.UTF8, "application/json");

                    var response = await httpClient.SendAsync(request);
                }
            }
            return "";
        }
        public string CheckPatientEligibility(int PatientID)
        {
            try
            {

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return "";
        }
    }
}