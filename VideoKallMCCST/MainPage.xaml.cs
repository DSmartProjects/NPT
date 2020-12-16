using System;
using System.Threading.Tasks;
using VideoKallMCCST.Communication;
using VideoKallMCCST.View;
using VideoKallMCCST.ViewModel;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VideoKallMCCST
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
    
        public MainPage()
        {
            this.InitializeComponent();
            mainPage = this;
            this.DataContext = mainpagecontext;
            VideoCallVM = new VideoCallViewModel();
            RightPanelHolder.Navigate(typeof(VideoCall));
            pagePlaceHolder.Navigate(typeof(LogoPage));
            NotifyStatusCallback += UpdateNotification;          
        }
      
        public StorageFolder rootImageFolder { get; set; }
        async void UpdateNotification(string s, int code)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //StatusTxt.Text = s;
            });
        }

        public bool TestIsInProgress { get; set; } = false;
        private   void Page_Loading(FrameworkElement sender, object args)
        {
            
        }
        private static Windows.Media.MediaExtensionManager mediaExtensionMgr;
        public void EnsureMediaExtensionManager()
        {
            if (mediaExtensionMgr == null)
            {
                mediaExtensionMgr = new Windows.Media.MediaExtensionManager();
                mediaExtensionMgr.RegisterSchemeHandler("Microsoft.Samples.SimpleCommunication.StspSchemeHandler", "stsp:");
            }
        }

        private void SMCCommChannel_MessageReceived(object sender, CommunicationMsg msg)
        {
            string[] cmd = msg.Msg.ToLower().Split('>');
             switch (cmd[0])
            {
                case "<smcr":
                     mainpagecontext.IsSMCConnected = true;
                    break;
                case "<smpulr":
                    break;
                case "pulseres":
                    mainpagecontext.NotifyResult?.Invoke(this, new CommunicationMsg(DeviceResponseType.PULSEOXIMETERRESULT, msg.Msg.ToLower())) ;
                    break;
                case "pulsestatus":
                    mainpagecontext.NotifyResult?.Invoke(this, new CommunicationMsg(DeviceResponseType.PULSEOXIMETERSTATUS, msg.Msg.ToLower()));
                    break;
                case "glucmdres":
                    mainpagecontext.NotifyResult?.Invoke(this, new CommunicationMsg(DeviceResponseType.GLUCORESULT, msg.Msg.ToLower()));
                    break;
                case "glucoresultresstatus":
                    mainpagecontext.NotifyResult?.Invoke(this, new CommunicationMsg(DeviceResponseType.GLUCORESULTSTATUS, msg.Msg.ToLower()));
                    break;
                case "thermores":
                    mainpagecontext.NotifyResult?.Invoke(this, new CommunicationMsg(DeviceResponseType.THERMORESTULT, msg.Msg));
                    break;
                case "thermocon":
                case "thermoerror":
                case "thermonotpaired":
                    mainpagecontext.NotifyResult?.Invoke(this, new CommunicationMsg(DeviceResponseType.THERMORESTULTSTATUS, msg.Msg.ToLower()));
                    break;
                case "bpres":
                    mainpagecontext.NotifyResult?.Invoke(this, new CommunicationMsg(DeviceResponseType.BPRES, msg.Msg));
                    break;
                case "bpconected"://BPCONECTED
                    mainpagecontext.NotifyResult?.Invoke(this, new CommunicationMsg(DeviceResponseType.BPCONCHEC, msg.Msg));
                    break;
                case "bpconn": //BPCONN BPCONECTED>M:False>T:
                    mainpagecontext.NotifyResult?.Invoke(this, new CommunicationMsg(DeviceResponseType.BPCONMSG, msg.Msg));
                    break;
                case "streadey":
                    StethoscopeNotification?.Invoke(msg.Msg.ToLower().Split('>')[1]);
                    if(msg.Msg.ToLower().Split('>')[1].Contains(("Ready for streaming at").ToLower()))
                    {
                        isStethoscopeReadystreaming = true;
                        StethoscopeStartStop?.Invoke("Ready",1);
                        
                    }
                    break;
                case "stmsg":
                    StethoscopeNotification?.Invoke(msg.Msg.ToLower().Split('>')[1]);
                    if (msg.Msg.ToLower().Split('>')[1].Contains(("streaming stopped").ToLower()))
                    {
                        
                        isStethoscopeReadystreaming = false;
                    }
                    break;
                case "stpic":
                   // ImageDisplay?.Invoke(msg.Msg);
                    break;
                case "derpic":
                   // ImageDisplay?.Invoke(msg.Msg);
                    break;
                case "mrpic":
                    ImageDisplay?.Invoke(msg.Msg);
                    break;
                case "mrexp":
                    UpdateNotification(cmd[1], 0);

                     // ImageDisplay?.Invoke(msg.Msg);
                    break;
                case "<imagesaved":
                    ImageDisplay?.Invoke("imagesaved");
                    UpdateNotification("Image Saved.", 0);
                    break;
                case "connectedmcc":
                    isDataAcquitionappConnected = true;

                    DQConnectionCallback?.Invoke(true);
                    break;
                case "spirofvc":
                    isDataAcquitionappConnected = true;
                    SPirotResults?.Invoke(msg.Msg);
                    break;
                case "spirovc":
                    isDataAcquitionappConnected = true;
                    SPirotResults?.Invoke(msg.Msg);
                    break;
                case "spirofvcvt":
                    isDataAcquitionappConnected = true;
                    SPirotResults?.Invoke(msg.Msg);
                    break;
                case "spirostatussucess":
                case "stoppedspirometer":
                    isDataAcquitionappConnected = true;
                    SPirotResults?.Invoke(msg.Msg);
                    break;
                case "spirostatusfailed":
                    isDataAcquitionappConnected = true;
                    SPirotResults?.Invoke(msg.Msg);
                    break;

                case "spirovcresult":
                case "spirofvcresult":
                    isDataAcquitionappConnected = true;
                    SPirotResults?.Invoke(msg.Msg);
                    break;
                case "<sbcshutdown":
                    OnSBCDisconnection();
                    break;
                case "<sbcstart":
                    OnSBCStart();
                    break;
                case "casres":
                    ProcessCASResponse(msg.Msg);
                    break;
            }
        }
        string incompletemsg = "";
        void ProcessCASResponse(string msg)
        {
            string st = msg; //ack from daq to mcc
            if (st.ToLower().Contains(("Cmd sent").ToLower()))
                return;
            string  resp = msg.Split('>')[1];
            if (incompletemsg.Length > 0)
                resp = incompletemsg + resp;

            string[] responses = resp.Split(']');
            if (resp.EndsWith(']'))
            {
                incompletemsg = ""; 
                foreach(var rsp in responses)
                {
                    if (rsp.Length > 0)
                        ParseCASResponses(rsp);
                }
            }
            else
            {
                incompletemsg = responses[responses.Length - 1];
                if(responses.Length>1)
                {
                    for(int i=0; i< responses.Length-1; i++ )
                    {
                        if(responses[i].Length>0)
                        ParseCASResponses(responses[i]);
                    }
                        
                }
            }
        }

        void ParseCASResponses(string resp)
        {
            string res = resp.ToUpper();
            string strMSG = "";
            string status = "";
            switch (res.Substring(0,2))
            {
                case "[W":
                     strMSG = resp;
                    if(res.Equals("[WM"))
                    {
                        strMSG = "Acknowldgement received(WM).";
                    }
                    else if(res.Equals("[WT"))
                    {
                        strMSG = "Acknowldgement received(WT).";
                    }
                    else if (res.Equals("[WTG"))
                    {
                        strMSG = "Tear Weight completed.(WT).";
                    }
                    else if (res.Equals("[WTB"))
                    {
                        strMSG = "Tear Weight failed.(WT).";
                    }
                    else
                    {
                        try {
                            if (res.Length > 3)
                            {
                                string op = res.Substring(0, 3);
                                  status = res.Substring(res.Length - 1);
                                if (status.Equals("G") && op.Equals("[WM"))
                                {
                                    strMSG = "Weight Measure completed.";
                                    string val = res.Substring(3, res.Length - 4);
                                    decimal weight = Int32.Parse(val);
                                    if (WeightMeasureUnit == 0)
                                    {
                                        weight = weight * (decimal)0.02205;
                                        strMSG = string.Format("{0:0.##}{1}", Decimal.Round( weight),"lb");

                                    }
                                    else
                                    {
                                        weight = weight * (decimal) 0.01;
                                        strMSG = string.Format("{0:0.##}{1}", decimal.Round( weight), "kg");

                                    }
                                    CASResult?.Invoke(strMSG, 2, 0);
                                }
                                else if (status.Equals("B") && op.Equals("[WM"))
                                {
                                    strMSG = "Weight Measure failed.";
                                }
                                else
                                {
                                    strMSG = "Un known response: " + res;
                                }

                            }


                        } catch(Exception ex) {
                            string s = ex.Message; }
                      
                    }

                    break;
                case "[H":
                    //0.0833333
                    string ht = "{0}’ {1:0.##}” ";
                      strMSG = resp;
                    if (res.Equals("[HM"))
                    {
                        strMSG = "Acknowldgement received(HM).";
                        MainPage.mainPage.CASResult?.Invoke(strMSG, 1,1);
                    }
                    try
                    {
                        if (res.Length > 3)
                        {
                            string op = res.Substring(0, 3);
                              status = res.Substring(res.Length - 1);
                            if (status.Equals("G") && op.Equals("[HM"))
                            {
                                strMSG = "Height Measure completed.";
                                CASResult?.Invoke(strMSG, 1, 1);
                                string val = res.Substring(3, res.Length - 4);
                                double height = Int32.Parse(val);
                                if (HeightMeasureUnit == 0)
                                    strMSG = height.ToString()+" cm";
                                else
                                {
                                    height = (height / 2.54) * 0.0833333;

                                    strMSG = string.Format(ht, height.ToString().Split('.')[0], height.ToString().Split('.')[1].Substring(0,1));

                                }

                                CASResult?.Invoke(strMSG, 1, 0);
                            }
                            else if (status.Equals("B") && op.Equals("[HM"))
                            {
                                strMSG = "Height Measure failed.";
                            }
                            else
                            {
                                strMSG = "Un known response: " + res;
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        string s = ex.Message;
                    }

                    break;
                case "[P":
                    status = res.Substring(res.Length - 1);
                    if(status.Equals("D") || status.Equals("R"))
                    {
                        strMSG = "Acknowldgement received.";
                        CASResult?.Invoke(strMSG, 4, 1);
                    }
                    else if(res.Substring(res.Length-2).Equals("DG"))
                    {
                        strMSG = "Deploy Completed.";
                        PoddeployretractcmdStatus.PodSelectionOperationResponseiSSuccess( true);
                        CASResult?.Invoke(strMSG, 4, 1);
                    }
                    else if (res.Substring(res.Length - 2).Equals("RG"))
                    {
                        PoddeployretractcmdStatus.PodSelectionOperationResponseiSSuccess(true);
                        strMSG = "Retract Completed.";
                        CASResult?.Invoke(strMSG, 4, 1);
                    }
                    else if (res.Substring(res.Length - 2).Equals("DB"))
                    {
                        PoddeployretractcmdStatus.PodSelectionOperationResponseiSSuccess(false);
                        strMSG = "Deploy Failed.";
                        CASResult?.Invoke(strMSG, 4, 1);
                    }
                    else if (res.Substring(res.Length - 2).Equals("RB"))
                    {
                        PoddeployretractcmdStatus.PodSelectionOperationResponseiSSuccess (false);
                        strMSG = "Retract Failed.";
                        CASResult?.Invoke(strMSG, 4, 1);
                    }

                    break;
                case "[S":
                    status = res.Substring(res.Length - 1);
                    if (status.Equals("D") || status.Equals("R"))
                    {
                        strMSG = "Acknowldgement received.";
                        CASResult?.Invoke(strMSG, 3, 3);
                    }
                    else if (res.Substring(res.Length - 2).Equals("DG"))
                    {
                        isSTDeployed = true; 
                        strMSG = "Deploy Completed.";
                        CASResult?.Invoke(strMSG, 3, 1);
                    }
                    else if (res.Substring(res.Length - 2).Equals("RG"))
                    { 
                        isSTDeployed = false;
                        strMSG = "Retract Completed.";
                        CASResult?.Invoke(strMSG, 3, 1);
                    }
                    else if (res.Substring(res.Length - 2).Equals("DB"))
                    {
                        strMSG = "Deploy Failed.";
                        CASResult?.Invoke(strMSG, 3, 1);
                    }
                    else if (res.Substring(res.Length - 2).Equals("RB"))
                    {
                        strMSG = "Retract Failed.";
                        CASResult?.Invoke(strMSG, 3, 1);
                    }

                    break;
                case "[R":
                    status = res.Substring(res.Length - 1);
                    if (status.Equals("G") )
                    {
                        PoddeployretractcmdStatus.PodSelectionOperationResponseiSSuccess(true);
                        strMSG = "Reclined. "+ res.Substring(2);
                    }
                    if (status.Equals("B"))
                    {
                        PoddeployretractcmdStatus.PodSelectionOperationResponseiSSuccess(false);
                        strMSG = "Reclined failed.";
                    }
                    break;
                case "[B":
                     status = res.Substring(res.Length - 1);
                    if (status.Equals("G"))
                    {
                        PoddeployretractcmdStatus.PodSelectionOperationResponseiSSuccess(true);
                        strMSG = "Seat height Adjusted."+ res.Substring(2);
                        CASResult?.Invoke(strMSG, 3, 1);
                    }
                    if (status.Equals("B"))
                    {
                        PoddeployretractcmdStatus.PodSelectionOperationResponseiSSuccess(false);
                        strMSG = "Seat height Adjustment Failed.";
                        CASResult?.Invoke(strMSG, 3, 1);
                    }
                    break; 
            }
        }
         void OnSBCDisconnection()
        {
            
            mainpagecontext.IsSMCConnected = false;
            isStethoscopeReadystreaming = false;
        }
        void OnSBCStart()
        {
            mainpagecontext.IsSMCConnected = true;
            isStethoscopeReadystreaming = false;


        }
        public bool isDataAcquitionappConnected { get; set; } = false;
        public bool isStethoscopeStreaming { get; set; } = false;

        public bool isStethoscopeReadystreaming { get; set; } = false;
        void UpdateSMCStatus()
        {
            //TxtSMCStatus.Text = mainpagecontext.IsSMCConnected ? "Available" : "Not Available";
            
            //BorderStatus.Background = mainpagecontext.IsSMCConnected ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen): new SolidColorBrush(Windows.UI.Colors.Red);
        }

        private void Watchdog_Tick(object sender, object e)
        {
            watchdog.Stop();
            if(statuscheckinterval>1 || mainpagecontext.IsSMCConnected)
            UpdateSMCStatus();

            if (!mainpagecontext.IsSMCConnected && intervalcount >= 5)
            {
                intervalcount = 0;
                SMCCommChannel.IPAddress = mainpagecontext.TxtIpAddress;// "192.168.0.33";
                SMCCommChannel.PortNo = mainpagecontext.TxtProtNo; // "9856"
                SMCCommChannel.Connect();
                SMCCommChannel.SendMessage(CommunicationCommands.MCCConnection);
            }
            else if (mainpagecontext.IsSMCConnected && statuscheckinterval >= 5)
            {
                statuscheckinterval = 0;
                mainpagecontext.IsSMCConnected = false;
                SMCCommChannel.SendMessage(CommunicationCommands.MCCConnectionStatusCheckCmd);
            }
            else if(!mainpagecontext.IsSMCConnected && intervalcount%2 == 0)
            {
                SMCCommChannel.SendMessage(CommunicationCommands.MCCConnectionStatusCheckCmd);
            }

            if (intervalcount > 6)
                intervalcount = 0;
          //  if(!isDataAcquitionappConnected && statuscheckinterval>2)
           //     CommToDataAcq.SendMessageToDataacquistionapp("ConnectionTest");

            intervalcount++;
           
            statuscheckinterval++;
           // mainpagecontext.UpdateStatus(mainpagecontext.IsSMCConnected);
            watchdog.Start();
        }

        public CommunicationChannel SMCCommChannel { get; private set; }

        int intervalcount = 0;
        int statuscheckinterval = 0;
        public void WriteMessage(string msg)
        {
            SMCCommChannel?.SendMessage(msg);
        }

        private   void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SMCCommChannel = new CommunicationChannel();
            SMCCommChannel.Initialize();

            Utility ut = new Utility();

            var result = Task.Run(async () => { return await ut.ReadIPaddress(); }).Result;
            var pmm_Config = Task.Run(async () => { return await ut.ReadPMMConfigurationFile(); }).Result;
            SMCCommChannel.IPAddress = mainpagecontext.TxtIpAddress;// "192.168.0.33";
            SMCCommChannel.PortNo = mainpagecontext.TxtProtNo;// "9856";
            SMCCommChannel.MessageReceived += SMCCommChannel_MessageReceived;
            mainpagecontext.IsSMCConnected = false;
            SMCCommChannel.Connect();
            SMCCommChannel.SendMessage(CommunicationCommands.MCCConnection);
            watchdog = new DispatcherTimer();
            watchdog.Tick += Watchdog_Tick;
            watchdog.Interval = new TimeSpan(0, 0, 1);
            watchdog.Start();
            CommToDataAcq.MessageReceived += SMCCommChannel_MessageReceived;
            CommToDataAcq.Initialize();
                                                                                                             CommToDataAcq.Connect();
        }

        public void GettingSMCStatus() {

            SMCCommChannel = new CommunicationChannel();
            SMCCommChannel.Initialize();

            Utility ut = new Utility();

            var result = Task.Run(async () => { return await ut.ReadIPaddress(); }).Result;
            SMCCommChannel.IPAddress = mainpagecontext.TxtIpAddress;// "192.168.0.33";
            SMCCommChannel.PortNo = mainpagecontext.TxtProtNo;// "9856";
            SMCCommChannel.MessageReceived += SMCCommChannel_MessageReceived;
            mainpagecontext.IsSMCConnected = false;
            SMCCommChannel.Connect();
            SMCCommChannel.SendMessage(CommunicationCommands.MCCConnection);
            watchdog = new DispatcherTimer();
            watchdog.Tick += Watchdog_Tick;
            watchdog.Interval = new TimeSpan(0, 0, 1);
            watchdog.Start();
            CommToDataAcq.MessageReceived += SMCCommChannel_MessageReceived;
            CommToDataAcq.Initialize();
            CommToDataAcq.Connect();
        }
        public DataacquistionappComm CommToDataAcq { get; set; } = new DataacquistionappComm();

     public   async void LogExceptions(string exception)
        {
          
            try
            {
                string msg =DateTime.Now.Date.ToString()+DateTime.Now.TimeOfDay.ToString()+ exception + Environment.NewLine;
                // msg = Environment.NewLine + msg + Environment.NewLine;
                string filename = "Exceptionlogs.txt";
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile pinfofile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                //  await Windows.Storage.FileIO.AppendTextAsync(pinfofile, msg, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                await Windows.Storage.FileIO.WriteTextAsync(pinfofile, msg, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            catch (Exception)
            { }
        }
        public bool IsUserLogedin { get; set; }
        public static VideoCallViewModel VideoCallVM = null;
        public static MainPage mainPage;
        public MainPageViewModel mainpagecontext = new MainPageViewModel();
        public delegate void NotifyStatus(string message, int code = 0);
        public delegate void CommandDelegate();
        public delegate void BoolDelegate(bool isDermascope);
        public delegate void BoolstatusDelegate(bool isDermascope, int devicetypes);
        public BoolDelegate DigitalMicroscope;
        public NotifyStatus NotifyStatusCallback;
        DispatcherTimer watchdog = null;
        public NotifyStatus StethoscopeNotification;
        public NotifyStatus StethoscopelungsNotification;
        public CommandDelegate StethoscopeRecord;
        public CommandDelegate OtoscopeComm;
        public NotifyStatus StethoscopeStartStop;
        public delegate void ImageDisplayNotification(string img);
        public ImageDisplayNotification ImageDisplay;
        public CommandDelegate NextPatient;
        public BoolDelegate StethoscopeStatus;
        public BoolDelegate MicroscopeStatus;
        public delegate void UpdateResults(string msg);
        public UpdateResults SPirotResults;
        public CommandDelegate Spirometercallback;
        public BoolDelegate Spirometrystatus;
        public CommandDelegate ResetSpirometr;
        public BoolstatusDelegate Thermostatusdelegate;
        public BoolDelegate DQConnectionCallback;
        public CommandDelegate ResetSTLungs;
        public CommandDelegate ResetGluco;
        public CommandDelegate SaveSTConfig;
        public NotifyStatus HM_WMEvents;
        public bool IsStethescopeChest { get; set; } = false;
        public PodMapping Podmapping { get; set; }
        public int WeightMeasureUnit = 0; // pound;
        public int HeightMeasureUnit = 0; // Cm;
        public CasNotification CASResult;
        public bool isSTDeployed = false;
        public PodCmdStatus PoddeployretractcmdStatus = new PodCmdStatus();
        public BoolDelegate SeatReclineMsg;
        public BoolDelegate SeatHeightAdjust;
        public delegate void CasNotification(string message, int devicecode , int isresultornotificationmsg);
        public delegate void REQ_MSG_Visibility(string status);
        public REQ_MSG_Visibility REQ_MSG_VisibilityCompleted;

    }
}
