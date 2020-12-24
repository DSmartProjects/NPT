﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoKallMCCST.ViewModel;

namespace VideoKallMCCST.Model
{
    public class BloodPressureTestResult : NotifyPropertyChanged
    {  
        private int _bloodPressureTestResultId;
        public int BloodPressureTestResultId
        {
            get
            {
                return _bloodPressureTestResultId;
            }
            set
            {
                _bloodPressureTestResultId = value;
                OnPropertyChanged("BloodPressureTestResultId");
            }
        }
        private int _patientId;

        public int PatientId
        {
            get
            {
                return _patientId;
            }
            set
            {
                _patientId = value;
                OnPropertyChanged("PatientId");
            }
        }
        private int? _chairId;
        public int? ChairId
        {
            get
            {
                return _chairId;
            }
            set
            {
                _chairId = value;
                OnPropertyChanged("ChairId");
            }
        }
        private Guid? _sessionId;
        public Guid? SessionId
        {
            get
            {
                return _sessionId;
            }
            set
            {
                _sessionId = value;
                OnPropertyChanged("SessionId");
            }
        }
        private int? _cystolic;
        public int? Cystolic
        {
            get
            {
                return _cystolic;
            }
            set
            {
                _cystolic = value;
                OnPropertyChanged("Cystolic");
            }
        }
        private double? _diastolic;
        public double? Diastolic
        {
            get
            {
                return _diastolic;
            }
            set
            {
                _diastolic = value;
                OnPropertyChanged("Diastolic");
            }
        }
        private int? _createdBy;
        public int? CreatedBy
        {
            get
            {
                return _createdBy;
            }
            set
            {
                _createdBy = value;
                OnPropertyChanged("CreatedBy");
            }
        }

        private DateTime? _createdDate;
        public DateTime? CreatedDate
        {
            get
            {
                return _createdDate;
            }
            set
            {
                _createdDate = value;
                OnPropertyChanged("CreatedDate");
            }
        }

        private Patient _patient = null;
        public virtual Patient Patient { get { return _patient; } set { _patient = value; OnPropertyChanged("Patient"); } }
    }
}
