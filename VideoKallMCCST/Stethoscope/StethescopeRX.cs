using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoKallMCCST.Stethoscope
{
    class StethescopeRX
    {
        [DllImport("ssoipRXLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static void ReadConfigurationFile();

        [DllImport("ssoipRXLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static void SetLogFolder(String filewithfolder);

        [DllImport("ssoipRXLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static void Connect();
        [DllImport("ssoipRXLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static void Disconnect();

        [DllImport("ssoipRXLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static void RegisterRXCallback(IntPtr callback);
        [DllImport("ssoipRXLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static void Record();
        
        //TX
        [UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall, SetLastError = true)]
        public delegate void CallbackDelegate(string x);

        [DllImport("ssoipTXLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static void ReadTXConfigurationFile();

        [DllImport("ssoipTXLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static void GenerateRecordingDeviceFile();

        [DllImport("ssoipTXLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static void StartStreaming(int stethoscopeIndx, bool lungs);

        [DllImport("ssoipTXLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static void StopStreaming();

        [DllImport("ssoipTXLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static void RegisterCallback(IntPtr callbackfunc);

        [DllImport("ssoipTXLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static void SetTXLogFolder(String filewithfolder);

        string RXLogfile = "RXlog.txt";
        public event EventHandler<EventArgs> TXevents;
        IntPtr handle;
        CallbackDelegate NotificationHandle;
        public StethescopeRX()
        {
               
        }
        public void Initialize()
        {
            if (File.Exists(RXLogfile))
                File.Delete(RXLogfile);
           
            NotificationHandle = new CallbackDelegate(CallBackTX);
            handle = Marshal.GetFunctionPointerForDelegate(NotificationHandle);
            RegisterRXCallback(handle);
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            RXLogfile = localFolder.Path;
            SetLogFolder(RXLogfile);
            ReadConfigurationFile(); 
        }

        public void ConnectTX()
        {
            Connect();
            //await Task.Run(() =>
            //{
            //    Connect();
            //});
            // RegisterRXCallback(handle);


        }
        public void DisconnectTX()
        {
            Disconnect();
        }

        public void RecordRx()
        {
            Record();
        }
        void CallBackTX(string message)
        {
            TXevents?.Invoke(message, null);
        }
    }
}
