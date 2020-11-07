using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
