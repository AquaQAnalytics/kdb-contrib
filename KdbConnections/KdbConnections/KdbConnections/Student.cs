using System;

namespace KdbConnections
{
    public class Student
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _gender;
        public string Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        private int _age;
        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }

        private DateTime _dob;
        public DateTime DOB
        {
            get { return _dob; }
            set { _dob = value; }
        }

        private double _outstandingFee;
        public double OutStandingFee
        {
            get { return _outstandingFee; }
            set { _outstandingFee = value; }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public Student MapColumnData(string colName, object data, Student student)
        {
            if (data != null)
            {
                switch (colName)
                {
                    case "name": //string
                        student.Name = (string)data;
                        break;
                    case "age": // int
                        student.Age = (int)data;
                        break;
                    case "gender": //string
                        student.Gender = (string)data;
                        break;
                    case "dob":  //datetime
                        student.DOB = (DateTime)data;
                        break;
                    case "outstandingfee":  //double
                        student.OutStandingFee = (double)data;
                        break;
                    default:
                        break;
                }
            }

            return student;
        }
    }
}
