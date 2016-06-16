using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using kx;

namespace KdbConnections
{
    public partial class MainWindow : Window
    {
        #region Properties
        private List<Student> _studentNames = new List<Student>();
        public List<Student> StudentNames
        {
            get { return _studentNames; }
            set { _studentNames = value; }
        }

        private List<Student> _studentAges = new List<Student>();
        public List<Student> StudentAges
        {
            get { return _studentAges; }
            set { _studentAges = value; }
        }

        private List<Student> _returnedStudentData = new List<Student>();
        public List<Student> ReturnedStudentData
        {
            get { return _returnedStudentData; }
            set { _returnedStudentData = value; }
        }

        #endregion

        #region Class Members

        private int _port; 
        private string _host;
        private Queries _queries;
        List<int> agesToQuery = new List<int>();
        List<string> namesToQuery = new List<string>();

        #endregion

        #region Constructor 
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            _queries = new Queries();
        }

        #endregion

        #region Event Handlers

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            _host = txtServer.Text.ToString();
            _port = Convert.ToInt32(txtPort.Text);

            ConnectToKDB();

            comboNameSelection.Items.Refresh();
            datagridStudentAges.Items.Refresh();
            datagridStudentNames.Items.Refresh();
            datagridStudentNames2.Items.Refresh();
            datagridStudentNames3.Items.Refresh();
        }

        private void btnGetDBVersion_Click(object sender, RoutedEventArgs e)
        {
            GetDBVersion();
        }

        private void btnSelectStudentTable_Click(object sender, RoutedEventArgs e)
        {
            ExecuteSelectFromStudentQuery();
        }

        private void btnGenderQuery_Click(object sender, RoutedEventArgs e)
        {
            QueryGender();
        }

        private void btnGenderAge_Click(object sender, RoutedEventArgs e)
        {
            QueryGenderAndAge();
        }

        private void btnSingleName_Click(object sender, RoutedEventArgs e)
        {
            QuerySingleName();
        }

        private void btnAgeRange_Click(object sender, RoutedEventArgs e)
        {
            QueryAgeRange();
        }

        private void btnQueryListOfNames_Click(object sender, RoutedEventArgs e)
        {
            QueryWithListOfNames();
        }

        private void btnQueryListNamesAndAgeValue_Click(object sender, RoutedEventArgs e)
        {
            QueryListOfNamesAndAge();
        }

        private void checkStudentSelect_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            Student stud = checkBox.DataContext as Student;

            namesToQuery.Add(stud.Name);
        }

        private void checkStudentSelect_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            Student stud = checkBox.DataContext as Student;

            if (namesToQuery.Contains(stud.Name))
            {
                namesToQuery.Remove(stud.Name);
            }
        }

        private void checkAge_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            Student stud = checkBox.DataContext as Student;

            agesToQuery.Add(stud.Age);
        }

        private void checkAge_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            Student stud = checkBox.DataContext as Student;

            if (agesToQuery.Contains(stud.Age))
            {
                agesToQuery.Remove(stud.Age);
            }
        }

        private void btnQueryNamesAndAges_Click(object sender, RoutedEventArgs e)
        {
            QueryNamesAgesAndGender();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReturnedStudentData.Clear();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BuildStudentList();
            BuildAgeList();
            dataGridStudentsTable.Items.Refresh();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            ConvertListIntoFlip();
            ConvertDBVersionDataIntoFlip();
        }

        #endregion

        #region private Methods

        private void ConnectToKDB()
        {
            try
            {
                bool connected = DBConnection.ConnectToKDB(_host, _port);

                if (connected)
                {
                    statusConnectionLight.Fill = Brushes.Green;
                    txtErrorMessage.Text = "Connected to " + _host + " on port " + _port;
                }
            }
            catch (SocketException e)
            {
                statusConnectionLight.Fill = Brushes.Red;
                txtErrorMessage.Text = "Cannot connect to " + _host + " on port " + _port + " " + e.Message;
            }
            catch (Exception exc)
            {
                statusConnectionLight.Fill = Brushes.Red;
                txtErrorMessage.Text = "Cannot connect to " + _host + " on port " + _port + " " + exc.Message;
            }
        }

        /// <summary>
        /// Creates a c.Flip object only containing the version number of a fictional
        /// db, then loads it into the q session.
        /// </summary>
        private void ConvertDBVersionDataIntoFlip()
        {
            string[] colnames = new string[1];
            colnames[0] = "dbversion";

            object[] colData = new object[1];
            string[] dbVersion = new string[1];

            dbVersion[0] = "dbversion";

            colData[0] = dbVersion;

            c.Dict dict = new c.Dict(colnames, colData);
            c.Flip flipData = new c.Flip(dict);
            _queries.LoadDBVersionData(flipData);
        }

        /// <summary>
        /// Converts the list of students into a c.Flip object, to be inserted as a table in the db.
        /// </summary>
        private void ConvertListIntoFlip()
        {
            string[] colNames = new string[5];
            colNames[0] = "name";
            colNames[1] = "gender";
            colNames[2] = "age";
            colNames[3] = "dob";
            colNames[4] = "outstandingfee";

            object[] colData = new object[5];

            string[] names = new string[StudentNames.Count];
            string[] genders = new string[StudentNames.Count];
            int[] ages = new int[StudentNames.Count];
            DateTime[] dobs = new DateTime[StudentNames.Count];
            double[] outstandingFees = new double[StudentNames.Count];

            int a = 0;
            foreach (Student stud in StudentNames)
            {
                names[a] = stud.Name;
                genders[a] = stud.Gender;
                ages[a] = stud.Age;
                dobs[a] = stud.DOB;
                outstandingFees[a] = stud.OutStandingFee;

                a++;
            }

            colData[0] = names;
            colData[1] = genders;
            colData[2] = ages;
            colData[3] = dobs;
            colData[4] = outstandingFees;

            c.Dict dict = new c.Dict(colNames, colData);
            c.Flip flipData = new c.Flip(dict);

            _queries.LoadStudentsToDB(flipData);
        }

        /// <summary>
        /// Creates a list of Student objects that will be loaded into database
        /// </summary>
        private void BuildStudentList()
        {
            Student s1 = new Student();
            s1.Name = "joe";
            s1.Gender = "m";
            s1.Age = 21;
            s1.DOB = DateTime.Now.AddYears(-21);
            s1.OutStandingFee = 1223.56;

            Student s2 = new Student();
            s2.Name = "tim";
            s2.Gender = "m";
            s2.Age = 22;
            s2.DOB = DateTime.Now.AddYears(-22);
            s2.OutStandingFee = 3070.22;

            Student s3 = new Student();
            s3.Name = "mia";
            s3.Gender = "f";
            s3.Age = 20;
            s3.DOB = DateTime.Now.AddYears(-22);
            s3.OutStandingFee = 2999.10;

            Student s4 = new Student();
            s4.Name = "tara";
            s4.Gender = "f";
            s4.Age = 19;
            s4.DOB = DateTime.Now.AddYears(-19);
            s4.OutStandingFee = 1500.00;

            Student s5 = new Student();
            s5.Name = "sam";
            s5.Gender = "m";
            s5.Age = 23;
            s5.DOB = DateTime.Now.AddYears(-23);
            s5.OutStandingFee = 1750.26;

            Student s6 = new Student();
            s6.Name = "jim";
            s6.Gender = "m";
            s6.Age = 24;
            s6.DOB = DateTime.Now.AddYears(-23);
            s6.OutStandingFee = 1223.32;

            Student s7 = new Student();
            s7.Name = "bob";
            s7.Gender = "m";
            s7.Age = 24;
            s7.DOB = DateTime.Now.AddYears(-23);
            s7.OutStandingFee = 346.23;

            _studentNames.Add(s1);
            _studentNames.Add(s2);
            _studentNames.Add(s3);
            _studentNames.Add(s4);
            _studentNames.Add(s5);
            _studentNames.Add(s6);
            _studentNames.Add(s7);
        }

        /// <summary>
        /// Creates a list of ages that are displayed in a grid to be selected to query against
        /// </summary>
        private void BuildAgeList()
        {
            _studentAges.Clear();

             Student s1 = new Student();
             s1.Age = 18;

             Student s2 = new Student();
             s2.Age = 19;

             Student s3 = new Student();
             s3.Age = 20;

             Student s4 = new Student();
             s4.Age = 21;

             Student s5 = new Student();
             s5.Age = 22;

             Student s6 = new Student();
             s6.Age = 23;

             Student s7 = new Student();
             s7.Age = 24;

             _studentAges.Add(s1);
             _studentAges.Add(s2);
             _studentAges.Add(s3);
             _studentAges.Add(s4);
             _studentAges.Add(s5);
             _studentAges.Add(s6);
             _studentAges.Add(s7);

        }

        private void GetDBVersion()
        {
            try
            {
                c.Flip flipData = _queries.GetDBVersion();
                this.lblDBVersion.Content = c.at(flipData.y[0], 0).ToString();
            }
            catch (Exception ex)
            {
                lblErrorMessage.Content = "Error: " + ex.Message;
            }
        }

        private void ExecuteSelectFromStudentQuery()
        {
            try
            {
                c.Flip flipData = _queries.SelectFromStudentQuery();
                MapReturnedData(flipData);
                dataGridResult1.Items.Refresh();
            }

            catch (Exception ex)
            {
                lblErrorMessage.Content = "Error: " + ex.Message;
            }
        }

        private void QueryGender()
        {
            try
            {
                c.Flip flipData = _queries.SelectFromStudentGenderQuery();
                MapReturnedData(flipData);
                dataGridResult1.Items.Refresh();
            }

            catch (Exception ex)
            {
                lblErrorMessage.Content = "Error: " + ex.Message;
            }
        }

        private void QueryGenderAndAge()
        {
            {
                try
                {
                    string query = txtQueryGenderAge.Text.ToString();
                    c.Flip flipData = _queries.ExecuteStudentFreeFormatQuery(query);

                    MapReturnedData(flipData);
                    dataGridResult1.Items.Refresh();
                }

                catch (Exception ex)
                {
                    lblErrorMessage.Content = "Error: " + ex.Message;
                }
            }
        }

        private void QuerySingleName()
        {
            if (comboNameSelection.SelectedValue != null)
            {
                try
                {
                    string selectedName = comboNameSelection.SelectedValue.ToString();
                    c.Flip flipData = _queries.QueryWithSingleNameAsParam(selectedName);
                    MapReturnedData(flipData);
                    dataGridResults2.Items.Refresh();
                }
                catch (Exception ex)
                {
                    lblErrorMessage.Content = "Error: " + ex.Message;
                }
            }
        }

        private void QueryAgeRange()
        {
            try
            {
                int fromAge = Convert.ToInt32(txtFromAge.Text);
                int toAge = Convert.ToInt32(txtToAge.Text);

                c.Flip flipData = _queries.QueryAgeRange(fromAge, toAge);
                MapReturnedData(flipData);
                dataGridResultsAgeRange.Items.Refresh();
            }
            catch (Exception ex)
            {
                lblErrorMessage.Content = "Error: " + ex.Message;
            }
        }

        private void QueryWithListOfNames()
        {
            try
            {
                c.Flip flipData = _queries.QueryStudentListOfNames(namesToQuery);
                MapReturnedData(flipData);
                dataGridResults3.Items.Refresh();
            }
            catch (Exception ex)
            {
                lblErrorMessage.Content = "Error: " + ex.Message;
            }
        }

        private void QueryListOfNamesAndAge()
        {
            namesToQuery.Clear();

            foreach (Student s in _studentNames)
            {
                if (s.IsSelected)
                {
                    namesToQuery.Add(s.Name);
                }
            }

            try
            {
                int fromAge = Convert.ToInt32(txtFromAge2.Text);
                object obj = _queries.QueryStudentListOfNamesAndAge(namesToQuery, fromAge);
                c.Flip flipData = c.td(obj);

                MapReturnedData(flipData);
                dataGridResults5.Items.Refresh();
            }
            catch (Exception ex)
            {
                lblErrorMessage.Content = "Error: " + ex.Message;
            }
        }

        private void QueryNamesAgesAndGender()
        {
            string gender = "";

            if (radioMale.IsChecked == true)
            {
               gender = "m";
            }
            else if (radioFemale.IsChecked == true)
            {
               gender = "f";
            }

            try
            {
                object obj = _queries.QueryNamesAgesAndGender(namesToQuery, agesToQuery, gender);
                c.Flip flipData = c.td(obj);

                MapReturnedData(flipData);
                dataGridResults4.Items.Refresh();
            }
            catch (Exception ex)
            {
                lblErrorMessage.Content = "Error: " + ex.Message;
            }
        }

        private void MapReturnedData(c.Flip flipData)
        {
            ReturnedStudentData.Clear();

            int rows = _queries.GetNumberOfRows(flipData);
            string[] colNames = _queries.GetColumnNames(flipData);

            for (int a = 0; a < rows; a++)
            {
                Student student = new Student();

                for (int col = 0; col < colNames.Length; col++)
                {
                    string colname = colNames[col];
                    object data = c.at(flipData.y[col], a);

                    student.MapColumnData(colname, data, student);
                }

                ReturnedStudentData.Add(student);
            }
        }

        #endregion
    }
}
