using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CCM.Models;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using CCM.Models.DataModels;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace CCM.Helpers
{
    public static class CCMRequestRestAPI
    {
        public static async Task<string> GetResponse(string baseUrl, string requestcontentbody)
        {
            var Response = "";
            using (var client = new HttpClient())
            {
                // client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage ResponseMessage = await client.GetAsync(requestcontentbody);
                if (ResponseMessage.IsSuccessStatusCode)
                {
                    Response = ResponseMessage.Content.ReadAsStringAsync().Result;
                    // ListBO = JsonConvert.DeserializeObject<List<ListBO>>(Response);
                    // ListBO = ListBO.OrderByDescending(x => x.CreatedOn).ToList();
                }
                return Response;
            }
        }

        public static async Task<string> GetResponsePostRequest(string baseUrl, string requestcontentbody)
        {
            var Response = "";
            using (var client = new HttpClient())
            {
                // client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                var content = new StringContent(requestcontentbody, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage ResponseMessage = await client.PostAsync(baseUrl, content);

                if (ResponseMessage.IsSuccessStatusCode)
                {
                    Response = ResponseMessage.Content.ReadAsStringAsync().Result;
                    // ListBO = JsonConvert.DeserializeObject<List<ListBO>>(Response);
                    // ListBO = ListBO.OrderByDescending(x => x.CreatedOn).ToList();
                }
                return Response;
            }


        }

        #region Initail HTTP Execution Code
        //public static async Task<string> GetResponsePostRequest(string baseUrl, string requestcontentbody)
        // {
        //     var Response = "";
        //     using (var client = new HttpClient())
        //     {
        //         // client.BaseAddress = new Uri(baseUrl);
        //         client.DefaultRequestHeaders.Accept.Clear();
        //         var content = new StringContent(requestcontentbody, Encoding.UTF8, "application/json");
        //         client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //         HttpResponseMessage ResponseMessage = await client.PostAsync(baseUrl, content);

        //         if (ResponseMessage.IsSuccessStatusCode)
        //         {
        //             Response = ResponseMessage.Content.ReadAsStringAsync().Result;
        //             // ListBO = JsonConvert.DeserializeObject<List<ListBO>>(Response);
        //             // ListBO = ListBO.OrderByDescending(x => x.CreatedOn).ToList();
        //         }
        //         return Response;
        //     }


        // }

        #endregion
    }
}