﻿using Newtonsoft.Json;
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
using VideoKallMCCST.Helpers;
using VideoKallMCCST.Model;

namespace VideoKallMCCST.Communication
{
    public class HttpClientManager
    {
        HttpResponseMessage response = null;
        string basePMM_APIUrl =string.Empty;
        string base_APIUrl = "https://localhost:44355/api";

        public HttpClientManager(){
            if (MainPage.mainPage.mainpagecontext.PMMConfig != null)
            {
                var pmm_config = MainPage.mainPage.mainpagecontext.PMMConfig;
                basePMM_APIUrl = !string.IsNullOrWhiteSpace(pmm_config.API_URL) ? pmm_config.API_URL : string.Empty;
            }
            else
            {
                Utility ut = new Utility();
                var pmm_Config = Task.Run(async () => { return await ut.ReadPMMConfigurationFile(); }).Result;
            }           
        }

        public async Task<List<Patient>> PatientsAsync(string user)
        {
            List<Patient> patients =  new List<Patient>();
            var uri = string.Empty;
            if (!string.IsNullOrEmpty(basePMM_APIUrl))
            {
                uri = basePMM_APIUrl + "/v1/patient/searchpatients?name=" + user;
            }
            else
                return patients;
           
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

        #region DevicesAPI     


        public async Task<bool> POST(HeightTestResult heightTest)
        {

            var uri = string.Empty;
            //Converting the object to a json string. NOTE: Make sure the object doesn't contain circular references.

            string json = JsonConvert.SerializeObject(heightTest);

            //Needed to setup the body of the request
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(base_APIUrl))
            {
                uri = base_APIUrl + "/HeightTestResults";
            }
            else
                return false;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
            };

            using (var client = new HttpClient())
            {

                string httpResponseBody = "";
                try
                {
                    HttpResponseMessage response = await client.PostAsync(uri, data);
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    Toast.ShowToast("", "Successfully Saved.");
                }
                catch (Exception ex)
                {
                    Toast.ShowToast("", "Failed");
                    httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> POST(WeightTestResult weightTest)
        {

            var uri = string.Empty;
            //Converting the object to a json string. NOTE: Make sure the object doesn't contain circular references.

            string json = JsonConvert.SerializeObject(weightTest);

            //Needed to setup the body of the request
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(base_APIUrl))
            {
                uri = base_APIUrl + "/WeightTestResults";
            }
            else
                return false;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
            };

            using (var client = new HttpClient())
            {

                string httpResponseBody = "";
                try
                {
                    HttpResponseMessage response = await client.PostAsync(uri, data);
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    Toast.ShowToast("", "Successfully Saved.");
                }
                catch (Exception ex)
                {
                    Toast.ShowToast("", "Failed");
                    httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> POST(BloodPressureTestResult bpTest)
        {

            var uri = string.Empty;
            //Converting the object to a json string. NOTE: Make sure the object doesn't contain circular references.

            string json = JsonConvert.SerializeObject(bpTest);

            //Needed to setup the body of the request
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(base_APIUrl))
            {
                uri = base_APIUrl + "/BloodPressureTestResults";
            }
            else
                return false;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
            };

            using (var client = new HttpClient())
            {

                string httpResponseBody = "";
                try
                {
                    HttpResponseMessage response = await client.PostAsync(uri, data);
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    Toast.ShowToast("", "Successfully Saved.");
                }
                catch (Exception ex)
                {
                    Toast.ShowToast("", "Failed");
                    httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> POST(PulseOximeterTestResult pulseTest)
        {

            var uri = string.Empty;
            //Converting the object to a json string. NOTE: Make sure the object doesn't contain circular references.

            string json = JsonConvert.SerializeObject(pulseTest);

            //Needed to setup the body of the request
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(base_APIUrl))
            {
                uri = base_APIUrl + "/PulseOximeterTestResults";
            }
            else
                return false;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
            };

            using (var client = new HttpClient())
            {

                string httpResponseBody = "";
                try
                {
                    HttpResponseMessage response = await client.PostAsync(uri, data);
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    Toast.ShowToast("", "Successfully Saved.");
                }
                catch (Exception ex)
                {
                    Toast.ShowToast("", "Failed");
                    httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> POST(ThermometerTestResult tempTest)
        {

            var uri = string.Empty;
            //Converting the object to a json string. NOTE: Make sure the object doesn't contain circular references.

            string json = JsonConvert.SerializeObject(tempTest);

            //Needed to setup the body of the request
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(base_APIUrl))
            {
                uri = base_APIUrl + "/ThermometerTestResults";
            }
            else
                return false;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
            };

            using (var client = new HttpClient())
            {

                string httpResponseBody = "";
                try
                {
                    HttpResponseMessage response = await client.PostAsync(uri, data);
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    Toast.ShowToast("", "Successfully Saved.");
                }
                catch (Exception ex)
                {
                    Toast.ShowToast("", "Failed");
                    httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                    return false;
                }
            }
            return true;
        }
        public async Task<bool> POST(DermatoscopeTestResult dermoTest)
        {

            var uri = string.Empty;
            //Converting the object to a json string. NOTE: Make sure the object doesn't contain circular references.

            string json = JsonConvert.SerializeObject(dermoTest);

            //Needed to setup the body of the request
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(base_APIUrl))
            {
                uri = base_APIUrl + "/DermatoscopeTestResults";
            }
            else
                return false;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
            };

            using (var client = new HttpClient())
            {

                string httpResponseBody = "";
                try
                {
                    HttpResponseMessage response = await client.PostAsync(uri, data);
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    Toast.ShowToast("", "Successfully Saved.");
                }
                catch (Exception ex)
                {
                    Toast.ShowToast("", "Failed");
                    httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> POST(OtoscopeTestResult otoTest)
        {

            var uri = string.Empty;
            //Converting the object to a json string. NOTE: Make sure the object doesn't contain circular references.

            string json = JsonConvert.SerializeObject(otoTest);

            //Needed to setup the body of the request
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(base_APIUrl))
            {
                uri = base_APIUrl + "/OtoscopeTestResults";
            }
            else
                return false;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
            };

            using (var client = new HttpClient())
            {

                string httpResponseBody = "";
                try
                {
                    HttpResponseMessage response = await client.PostAsync(uri, data);
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    Toast.ShowToast("", "Successfully Saved.");
                }
                catch (Exception ex)
                {
                    Toast.ShowToast("", "Failed");
                    httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> POST(SpirometerTestResult spiroTest)
        {

            var uri = string.Empty;
            //Converting the object to a json string. NOTE: Make sure the object doesn't contain circular references.

            string json = JsonConvert.SerializeObject(spiroTest);

            //Needed to setup the body of the request
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(base_APIUrl))
            {
                uri = base_APIUrl + "/SpirometerTestResults";
            }
            else
                return false;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
            };

            using (var client = new HttpClient())
            {

                string httpResponseBody = "";
                try
                {
                    HttpResponseMessage response = await client.PostAsync(uri, data);
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    Toast.ShowToast("", "Successfully Saved.");
                }
                catch (Exception ex)
                {
                    Toast.ShowToast("", "Failed");
                    httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> POST(GlucoseMonitorTestResult glucoTest)
        {

            var uri = string.Empty;
            //Converting the object to a json string. NOTE: Make sure the object doesn't contain circular references.

            string json = JsonConvert.SerializeObject(glucoTest);

            //Needed to setup the body of the request
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(base_APIUrl))
            {
                uri = base_APIUrl + "/GlucoseMonitorTestResults";
            }
            else
                return false;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
            };

            using (var client = new HttpClient())
            {

                string httpResponseBody = "";
                try
                {
                    HttpResponseMessage response = await client.PostAsync(uri, data);
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    Toast.ShowToast("", "Successfully Saved.");
                }
                catch (Exception ex)
                {
                    Toast.ShowToast("", "Failed");
                    httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> POST(ChestStethoscopeTestResult chestTest)
        {

            var uri = string.Empty;
            //Converting the object to a json string. NOTE: Make sure the object doesn't contain circular references.

            string json = JsonConvert.SerializeObject(chestTest);

            //Needed to setup the body of the request
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(base_APIUrl))
            {
                uri = base_APIUrl + "/ChestStethoscopeTestResults";
            }
            else
                return false;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
            };

            using (var client = new HttpClient())
            {

                string httpResponseBody = "";
                try
                {
                    HttpResponseMessage response = await client.PostAsync(uri, data);
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    Toast.ShowToast("", "Successfully Saved.");
                }
                catch (Exception ex)
                {
                    Toast.ShowToast("", "Failed");
                    httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> POST(SeatBackStethoscopeTestResult seatTest)
        {

            var uri = string.Empty;
            //Converting the object to a json string. NOTE: Make sure the object doesn't contain circular references.

            string json = JsonConvert.SerializeObject(seatTest);

            //Needed to setup the body of the request
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(base_APIUrl))
            {
                uri = base_APIUrl + "/SeatBackStethoscopeTestResults";
            }
            else
                return false;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
            };

            using (var client = new HttpClient())
            {

                string httpResponseBody = "";
                try
                {
                    HttpResponseMessage response = await client.PostAsync(uri, data);
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    Toast.ShowToast("", "Successfully Saved.");
                }
                catch (Exception ex)
                {
                    Toast.ShowToast("", "Failed");
                    httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                    return false;
                }
            }
            return true;
        }

        #endregion

    }
}
