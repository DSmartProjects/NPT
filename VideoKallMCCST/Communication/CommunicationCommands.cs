using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoKallMCCST.Communication
{
    public enum DeviceResponseType
    {
    PULSEOXIMETERRESULT = 1,
    PULSEOXIMETERSTATUS=2,
    GLUCORESULT = 3,
    GLUCORESULTSTATUS = 4,
    THERMORESTULT=5,
    THERMORESTULTSTATUS = 6,
    BPRES = 7,
    BPCONCHEC=8,
    BPCONMSG=9
    }

   public struct PodMapping
    {
        public string BPCuffPodID;
        public string ThermoMeterPodID;
        public string OximeterPodID;
        public string GlucomonitorPodID;
        public string DermascopePodID;
        public string OtoscopePodID;
        public string SpirometerPodID;
        public string StethoscopeChestPodID;
        public int TimeOutPeriod;
        public int ReclineStepValue;
        public int SeatUpDownStepValue;
    }

    public struct PodCmdStatus
    {
        public bool IsPodDeployResponseReceived { get; set; } 
        public bool IsPodDeployInProgress { get; set; } 
        public bool isPodDeployOrRetractFailed { get; set; } 
        private bool _isPodRetracted;
        public PodCmdStatus(bool initalvalue = false)
        {
            IsPodDeployResponseReceived = initalvalue;
            IsPodDeployInProgress = initalvalue;
            isPodDeployOrRetractFailed = initalvalue;
            _isPodRetracted = false;
        }

        public bool isPodRetracted()
        {
            if ((IsPodDeployResponseReceived && !isPodDeployOrRetractFailed) && 
                !_isPodRetracted)
                return false;

            return true;
        }

        public bool IsPodDeployed()
        {
            if ((IsPodDeployResponseReceived && !isPodDeployOrRetractFailed)||
                (!IsPodDeployResponseReceived && !isPodDeployOrRetractFailed))
                return false;
            return true;
        }
        public void Reset()
        {
            IsPodDeployResponseReceived = false;
            IsPodDeployInProgress = false;
            isPodDeployOrRetractFailed = false;
            _isPodRetracted = false;
        }
        public void PodSelectionOperationStarted()
        {
            IsPodDeployResponseReceived = false;
            IsPodDeployInProgress = true;
            isPodDeployOrRetractFailed = false;
            _isPodRetracted = false;
        }

        public void PodDeploymentResponseReceived(bool issuccess)
        {
            IsPodDeployResponseReceived = true;
            IsPodDeployInProgress = false;
            isPodDeployOrRetractFailed = !issuccess;
            _isPodRetracted = false;
        }

        public void PodRetractionResponseReceived(bool success)
        {
            IsPodDeployResponseReceived = false;
            IsPodDeployInProgress = false;
            isPodDeployOrRetractFailed = !success;
            _isPodRetracted = true;
        }
         

    }
   public static class CommunicationCommands
    {
        public static readonly string MCCConnectionStatusCheckCmd = "<MCCS>"; //MCC Connection status
        public static readonly string SBCConnectionResponseCmd = "<SMCR>"; //SMC Connection response
        public static readonly string MCCConnection = "<SMCC>"; //SMC Connection 
        public static readonly string SMCPULSEOXIMETERSTART = "<PULSESTART>";
        public static readonly string PUSLEOXIMETERRESULT = "PULSERES>SP:{0}>PR:{1}>DT:{2}";
        public static readonly string PUSLEOXIMETERCONNECTIONMSG = "PULSESTATUS>{0}";
        public static readonly string GLUCORESULTCMD = "<GLUCMD>";
        public static readonly string GLUCORESULTRES = "GLUCMDRES>V:{0}>U:{1}>T:{2}>M:{3}>D:{4}>T>{5}";
        public static readonly string GLUCORESULTRESSTATUS = "GLUCMDRESSTATUS>M:{0}";
        public static readonly string THERMORESULT = "THERMORES>R:{0}>M:{1}>S:{2}>DT:{3}";
        public static readonly string THERMORESULTRESSTATUS = "THERMOCON>{0}";
        public static readonly string THERMORESULTCMD = "<THERMOCMD>";
        public static readonly string THERMORCONNECTIONSTATUS = "THERMOCON>{0}>";
        public static readonly string THERMORError = "THERMOERROR>{0}>";
        public static readonly string THERMOnotpaired = "THERMOnotpaired>{0}>";

        public static readonly string BPCMD = "<BPCMD>";
        public static readonly string BPCONCMD = "<BPCONCMD>";
        public static readonly string BPCONSTATUS = "BPCONN>M:{0}";
        public static readonly string BPCONNECTIONTIME = "BPCONECTED>M:{0}>T:{1}";
        public static readonly string BPRESULT = "BPRES>D:{0}>S:{1}>P:{2}>DT:{3}>T:{4}";

        public static readonly string TAKEPIC = "<PIC>";
        public static readonly string STARTDERMO = "<startdermo>";
        public static readonly string STOPDERMO = "<stopdermo>";
        public static readonly string STARTOTOSCOPE = "<startoto>";
        public static readonly string STOPOTOSCOPE = "<stopoto>";

        public static readonly string STARTSTCHEST = "<startstchecst>";
        public static readonly string STARTSTLUNGS = "<startstlungs>";
        public static readonly string STCHESTRESPONSE = "streadey>{0}";
        public static readonly string STMSG = "stmsg>{0}";

        public static readonly string STPIC = "STPIC>{0}>";
        public static readonly string DERPIC = "DRPIC>{0}>";
        public static readonly string MIROSCOPEPIC = "MRPIC>{0}>";
        public static readonly string MIREXCEPTION = "MREXP>{0}>";
        public static readonly string OTOSAVEIMAGE = "<otosaveImage>";
        public static readonly string DERSAVEIMAGE = "<dersaveImage>";
       
        public static readonly string NotifySAVEDIMAGE = "<imagesaved>";

        public static readonly string SpirometerFVCdata = "SPIROFVC>{0}>";
        public static readonly string SpirometerFVC = "SPIROVC>{0}>";

        public static readonly string StartSpiroFVC = "<startspirofvc>{0}>";

        public static readonly string StartSpiroVC = "<startspirovc>{0}>";
        public static readonly string StopSpiro = "<stopspiro>";

        public static readonly string Spirostatussuccess = "spirostatussucess>";
        public static readonly string SpirostatusFailed = "spirostatusfailed>{0}> ";
        public static readonly string StoppedSpiroMeter = "stoppedspirometer>";

        public static readonly string SpiroVCResults = "spirovcresult>{0}>{1}>";

        public static readonly string SpiroFVCResults = "spirofvcresult>{0}>{1}>";
        public static readonly string SBCShutdown = "<sbcshutdown>";
        public static readonly string SBCStart = "<sbcstart>";

        public static readonly string PODCMD = "pod>{0}>{1}>";//pod>1>d
        public static readonly string SeatBackSTCmd = "stl>{0}>{1}>";//s>11>d
        public static readonly string SeatBackheightCmd = "seatht>{0}>";//b>11>d
        public static readonly string SeatReclineCmd = "seatrec>{0}>";
        public static readonly string HM = "hm>";
        public static readonly string WM = "wm>";
        public static readonly string WT = "wt>";
        public static readonly string CASRES = "casres>{0}>";
    }
}
