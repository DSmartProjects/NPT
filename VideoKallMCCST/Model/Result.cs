
using VideoKallMCCST.ViewModel;

namespace VideoKallMCCST.Model
{
    public class Result<T>: NotifyPropertyChanged
    {
        private string _status=string.Empty;
        public string status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged("status");
            }
        }


        private string _message = string.Empty;

        public string message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged("message");
            }
        }

        private T _data ;

        public T data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                OnPropertyChanged("data");
            }
        }


    }
}
