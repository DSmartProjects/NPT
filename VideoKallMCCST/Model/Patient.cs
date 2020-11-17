using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoKallMCCST.ViewModel;

namespace VideoKallMCCST.Model
{
    public class Patient: NotifyPropertyChanged
    {
        private int _id ;
        public int ID { get { return _id; } set { _id = value; OnPropertyChanged("ID"); } }
        private string _name;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        private DateTime _dob;
        public DateTime DOB { get { return _dob; } set { _dob = value; OnPropertyChanged("DOB"); } }      
    }

}
