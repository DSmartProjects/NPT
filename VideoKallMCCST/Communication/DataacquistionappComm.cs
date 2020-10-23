using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoKallMCCST.Communication;
 
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace VideoKallMCCST.Communication
{
   public class DataacquistionappComm
    {
        DatagramSocket selfhost = null;
        uint inboundBufferSize = 2048; 
        public string DataacquistionPortno { get; set; } =  "9854" ;
        public event EventHandler<CommunicationMsg> MessageReceived;

        public  void Initialize()
        {
            selfhost = new DatagramSocket();
            selfhost.MessageReceived += MessageFromDataacqAppReceived;
            // Refer to the DatagramSocketControl class' MSDN documentation for the full list of control options.
            selfhost.Control.InboundBufferSizeInBytes = inboundBufferSize;

            // Set the IP DF (Don't Fragment) flag.
            // Refer to the DatagramSocketControl class' MSDN documentation for the full list of control options.
            selfhost.Control.DontFragment = true;
        }

        private void MessageFromDataacqAppReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            try
            {
                uint stringLength = args.GetDataReader().UnconsumedBufferLength; 
                byte[] buffer = new byte[stringLength];
                args.GetDataReader().ReadBytes(buffer);
                string str = Encoding.Unicode.GetString(buffer);
                MessageReceived?.Invoke(this, new CommunicationMsg(str));
            }
            catch (Exception e)
            {
                string s = e.ToString();
                MainPage.mainPage.LogExceptions(e.Message);
                MainPage.mainPage.NotifyStatusCallback(e.Message);
            }
        }

        public async void Connect()
        {
            try
            { 
                HostName host = new HostName(MainPage.mainPage.SMCCommChannel.IPAddress);
                await selfhost.ConnectAsync(host, DataacquistionPortno);

                 SendMessageToDataacquistionapp("ConnectionTest");
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                MainPage.mainPage.LogExceptions(ex.Message);
                MainPage.mainPage.NotifyStatusCallback(ex.Message);
            }
         //   return 0;
        }

        DataWriter writer = null;
      public void  SendMessageToDataacquistionapp(string msg)
        {
            if (selfhost != null)
            {
                try
                {
                    if(writer == null)
                      writer    = new DataWriter(selfhost.OutputStream);
                    string stringToSend = msg;
                    writer.WriteString(stringToSend);

                    writer?.StoreAsync();
                   // writer.Dispose();
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                    MainPage.mainPage.LogExceptions(e.Message);
                    MainPage.mainPage.NotifyStatusCallback(e.Message);
                }
                
            }
        }

    }
}
