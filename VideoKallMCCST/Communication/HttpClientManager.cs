using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SBCDBModule.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VideoKallMCCST.Model;

namespace VideoKallMCCST.Communication
{
    public class HttpClientManager
    {
        HttpResponseMessage response = null;
        static string baseAPIUrl = "http://183.82.119.28:5003/api";
        public async Task<List<Patient>> PatientsAsync(string user)
        {
            List<Patient> patients = null;
            var uri =baseAPIUrl+ "/v1/patient/searchpatients?name=" + user;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri),
            };
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                patients = new List<Patient>();
                string httpResponseBody = "";
                try
                {
                    HttpResponseMessage response = await client.SendAsync(request);                   
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                    return patients;
                }
    
                var resultObjects = AllChildren(JObject.Parse(httpResponseBody))
                .First(c => c.Type == JTokenType.Array && c.Path.Contains("data"))
                .Children<JObject>();
                Patient patient = null;              
                foreach (JObject data in resultObjects)
                {
                    patient = new Patient { ID = Convert.ToInt32(data["Patient_ID"].ToString()), Name = string.Concat(data["Patient_FirstName"].ToString(),!string.IsNullOrWhiteSpace(data["Patient_MiddleName"].ToString())?" ":"",
                           data["Patient_MiddleName"].ToString()," ",
                         data["Patient_LastName"].ToString()), DOB =Convert.ToDateTime(data["DOB"].ToString())
                    };
                    patients.Add(patient); 
                }
            }
            return patients;
        }
        // recursively yield all children of json
        private static IEnumerable<JToken> AllChildren(JToken json)
        {
            foreach (var c in json.Children())
            {
                yield return c;
                foreach (var cc in AllChildren(c))
                {
                    yield return cc;
                }
            }
        }
    }
}
