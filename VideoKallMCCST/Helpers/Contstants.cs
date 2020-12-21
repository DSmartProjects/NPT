
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
        public const double MAX_Media_Duration = 10000000000000;

        public const string Deployment_Recline_Inprogress = @"Seat back movement in progress";
        public const string Wait_Time = @"Please wait for {0} sec.";
        public const string Measure_Height_First = @"Please Measure Height First.";
        public const string MsgDevicenotDeployed = "Device is not deployed yet. Please redeploy or wait until device deployed.";
        public const string MsgPodNotRetracted = "Device is not retracted yet. Please ask user to put the device in the Pod. Do you want to continue? ";


    }
}
