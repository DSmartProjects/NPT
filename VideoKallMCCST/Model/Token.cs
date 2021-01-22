using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoKallMCCST.ViewModel;

namespace VideoKallMCCST.Model
{
    public class Token: NotifyPropertyChanged
    {
        private string _token = string.Empty;
        public string token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
                OnPropertyChanged("token");
            }
        }
    }
}
