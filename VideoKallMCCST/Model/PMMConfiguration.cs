using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoKallMCCST.ViewModel;

namespace VideoKallMCCST.Model
{
    public class PMMConfiguration: NotifyPropertyChanged
    {
        private string _url;
        public string URL { get { return _url; } set { _url = value; OnPropertyChanged("URL"); } }
        private string _userName;
        public string UserName { get { return _userName; } set { _userName = value; OnPropertyChanged("UserName"); } }
        private string _pwd;
        public string PWD { get { return _pwd; } set { _pwd = value; OnPropertyChanged("PWD"); } }
    }
}
