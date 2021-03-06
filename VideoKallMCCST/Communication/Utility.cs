﻿using System;
using System.Threading.Tasks;
using VideoKallMCCST.Model;

namespace VideoKallMCCST.Communication
{
    class Utility
    {
        public async Task<bool> ReadIPaddress()
        {
            try
            {
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile IpAddressFile = await localFolder.GetFileAsync("SMCIPAddress.txt");
                var alltext = await Windows.Storage.FileIO.ReadLinesAsync(IpAddressFile);
                string ipadd = "";
                string portid = "";
                foreach (var line in alltext)
                {
                    string[] data = line.Split(':');
                   
                    switch (data[0])
                    {
                        case "IP":
                            ipadd = data[1];
                            break;
                        case "PORT":
                            portid = data[1];
                            break;
                        case "TEMP":
                            MainPage.mainPage.mainpagecontext.ThermometerUnitF = data[1].Equals("1");
                            break;
                       
                        default:
                            break;
                    }
                }


                MainPage.mainPage.mainpagecontext.UpadateIPaddress(ipadd, portid);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> ReadPMMConfigurationFile()
        {
            string filename = "PMM_Config.txt";
            try
            {
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile PmmConfigFile = await localFolder.GetFileAsync("PMM_Config.txt");
                var fileTextData = await Windows.Storage.FileIO.ReadLinesAsync(PmmConfigFile);              
                string url = string.Empty;
                string api_url = string.Empty;
                string test_result_api_url = string.Empty;
                foreach (var line in fileTextData)
                {
                    var urlData = line;
                    if (urlData.Contains("TestResult_API_URL"))
                        test_result_api_url = urlData.Substring(urlData.IndexOf("http") + 0);
                    else if (urlData.Contains("API_URL"))
                        api_url = urlData.Substring(urlData.IndexOf("http") + 0);
                    else if (urlData.Contains("URL"))
                        url = urlData.Substring(urlData.IndexOf("http") + 0);
                  
                    //url = JsonConvert.DeserializeObject<string>(urlData.Substring(urlData.IndexOf("http")+0));                    
                }
                PMMConfiguration pmmConfig = new PMMConfiguration();
                pmmConfig.URL =!string.IsNullOrWhiteSpace(url)?url.ToString().Trim():string.Empty;
                pmmConfig.API_URL = !string.IsNullOrWhiteSpace(api_url) ? api_url.ToString().Trim() : string.Empty;
                pmmConfig.TestResultAPI_URL = !string.IsNullOrWhiteSpace(test_result_api_url) ? test_result_api_url.ToString().Trim() : string.Empty;
                //MainPage.mainPage.mainpagecontext.PMMConfig = pmmConfig;
                VideoKallLoginPage.LoginPage._loginVM.PMMConfig = pmmConfig;
            }
            catch (Exception ex)
            {
                try
                {
                    var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    Windows.Storage.StorageFile pinfofile = await localFolder.CreateFileAsync(filename);
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
               
            }
            return true;
        }
    }
}
