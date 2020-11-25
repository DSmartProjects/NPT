using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using VideoKallMCCST.Model;
using Windows.Storage;

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
                foreach (var line in fileTextData)
                {
                    var urlData = line;
                    if (urlData.Contains("URL"))
                        url = urlData.Substring(urlData.IndexOf("http") + 0);
                    //url = JsonConvert.DeserializeObject<string>(urlData.Substring(urlData.IndexOf("http")+0));                    
                }
                PMMConfiguration pmmConfig = new PMMConfiguration();
                pmmConfig.URL =!string.IsNullOrWhiteSpace(url)?url.ToString().Trim():string.Empty;
                MainPage.mainPage.mainpagecontext.PMMConfig = pmmConfig;
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
