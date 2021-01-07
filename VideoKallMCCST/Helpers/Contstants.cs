
namespace VideoKallMCCST.Helpers
{
    public static class Constants
    {
        #region DateFormaters
        public const string US_DATE_MM_DD_YYYY = "MM/dd/yyyy";
        public const string US_DATE_FORMATE = "en-us";
        #endregion

        public const string MSG_SEARCH_NOT_FOUND = "Patient not found";
        public const string Success = "Success";
        public const string Failure = "Failure";
        public const string OK = "OK";
        public const double MIN_Media_Duration = 10;
        public const double MAX_Media_Duration = 10000;

        public const string Deployment_Recline_Inprogress = @"Seat movement in progress";
        public const string Wait_Time = @"Please wait for {0} sec.";
        public const string MsgDevicenotDeployed = "Device is not deployed yet. Please redeploy or wait until device deployed.";
        public const string MsgPodNotRetracted = "Device is not retracted yet. Please ask user to put the device in the Pod. Do you want to continue? ";
        public const string Measure_Height_First = @"Please measure height first.";
        public const string InValid_UNAME_PWD = @"Please provide valid username and password";
        public const string StatusCode_Success ="200";
        public const string Login_Success_MSG = "Login Successfull";
        public const string Admin_UNAME = "Admin";
        public const string Admin_PWD = "ClinicStop@2021";
        public const int Admin_ID =1;

        #region device name
        public const string Height ="Height";
        public const string Weight ="Weight";
        public const string BP_CUFF = "Blood Pressure";
        public const string PulseOximeter = "Pulse Oximeter";
        public const string Thermometer = "Thermometer";
        public const string Dermatoscope = "Dermatoscope";
        public const string Otoscope = "Otoscope";
        public const string Spirometer = "Spirometer";
        public const string GlucoseMonitor= "Glucose Monitor";
        public const string ChestStethoscope = "Chest Stethoscope";
        public const string SeatBackStethoscope = "Seat Back Stethoscope";
        #endregion device name

    }
}
