using System.Collections.Generic;
using kx;

namespace KdbConnections
{
    public class Queries
    {
        public Queries()
        {

        }

        /// <summary>
        /// Loads a c.Flip containing the fictional db version number to the q session as a table
        /// </summary>
        /// <param name="flipData"></param>
        public void LoadDBVersionData(c.Flip flipData)
        {
            if (DBConnection.Connection != null && DBConnection.Connection.Connected)
            {
              object x =  DBConnection.Connection.k("{[x;y]x set y}","dbversion", flipData);
            }
        }

        /// <summary>
        /// Loads a c.Flip containing the student data to the q session as a table
        /// </summary>
        /// <param name="flipData"></param>
        public void LoadStudentsToDB(c.Flip flipData)
        {
            if (DBConnection.Connection != null && DBConnection.Connection.Connected)
            {
              object x =  DBConnection.Connection.k("{[x;y]x set y}","student", flipData);
            }
        }

        /// <summary>
        /// Hardcoded query to return a fiction db version number from a table, single piece of data
        /// </summary>
        /// <returns></returns>
        public c.Flip GetDBVersion()
        {
            if (DBConnection.Connection != null && DBConnection.Connection.Connected)
            {
                object obj = DBConnection.Connection.k("select from dbversion");
                return c.td(obj);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns c.Flip from a hardcoded "select from student query"
        /// containing all students in the student table that has been loaded
        /// </summary>
        /// <returns></returns>
        public c.Flip SelectFromStudentQuery()
        {
            if (DBConnection.Connection != null && DBConnection.Connection.Connected)
            {
                object obj = DBConnection.Connection.k("select from student");
                return c.td(obj);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a c.Flip containing the student data where gender = f
        /// </summary>
        /// <returns></returns>
        public c.Flip SelectFromStudentGenderQuery()
        {
            if (DBConnection.Connection != null && DBConnection.Connection.Connected)
            {
                object obj = DBConnection.Connection.k("select from student where gender=`f");
                return c.td(obj);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns c.Flip of data when a free-formatted query is entered into the text box.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public c.Flip ExecuteStudentFreeFormatQuery(string query)
        {
            if (DBConnection.Connection != null && DBConnection.Connection.Connected)
            {
                object obj = DBConnection.Connection.k(query);
                return c.td(obj);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a c.Flip from a parameterised query, passing in an object
        /// array containing the query statement and a single name to query against.
        /// </summary>
        /// <param name="selectedName"></param>
        /// <returns></returns>
        public c.Flip QueryWithSingleNameAsParam(string selectedName)
        {
            object[] queryParams = new object[2];

            string query = "{[x]select from student where name=x}";
            queryParams[0] = query.ToCharArray();
            queryParams[1] = selectedName;

            if (DBConnection.Connection != null && DBConnection.Connection.Connected)
            {
                object obj = DBConnection.Connection.k(queryParams);
                return c.td(obj);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns c.Flip from a parameterised query. Submitted as an object array
        /// containing the query as a char array, and a two age values to query between.
        /// </summary>
        /// <param name="fromAge"></param>
        /// <param name="toAge"></param>
        /// <returns></returns>
        public c.Flip QueryAgeRange(int fromAge, int toAge)
        {
            object[] queryParams = new object[3];
            string query = "{[fromAge;toAge]select from student where age >= fromAge,age <= toAge}";
            char[] queryarray = query.ToCharArray();

            queryParams[0] = queryarray;
            queryParams[1] = fromAge;
            queryParams[2] = toAge;

            if (DBConnection.Connection != null && DBConnection.Connection.Connected)
            {
                object obj = DBConnection.Connection.k(queryParams);
                return c.td(obj);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns back c.Flip object from parameterised query
        /// passing in an object array containing the query as a char array,
        /// and a list of names to query against.
        /// </summary>
        /// <param name="queryInputs"></param>
        /// <returns></returns>
        public c.Flip QueryStudentListOfNames(List<string> namesToQuery)
        {
            object[] queryParams = new object[2];

            string query = "{[names]select from student where name in names}";
            char[] queryarray = query.ToCharArray();

            string[] names = new string[namesToQuery.Count];

            for (int a = 0; a < namesToQuery.Count; a++)
            {
                names[a] = namesToQuery[a];
            }

            queryParams[0] = queryarray;
            queryParams[1] = names;

            if (DBConnection.Connection != null && DBConnection.Connection.Connected)
            {
                object obj = DBConnection.Connection.k(queryParams);
                return c.td(obj);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns c.Flip from parameterised query containing the query as
        /// char array, a list of names AND a lower age range to query against.
        /// </summary>
        /// <param name="namesToQuery"></param>
        /// <param name="fromAge"></param>
        /// <returns></returns>
        public c.Flip QueryStudentListOfNamesAndAge(List<string> namesToQuery, int fromAge)
        {
            object[] queryParams = new object[3];

            string query = "{[names;fromAge]select from student where name in names,age >= fromAge}";
            char[] queryarray = query.ToCharArray();

            string[] names = new string[namesToQuery.Count];

            for (int a = 0; a < namesToQuery.Count; a++)
            {
                names[a] = namesToQuery[a];
            }

            queryParams[0] = queryarray;
            queryParams[1] = names;
            queryParams[2] = fromAge;

            if (DBConnection.Connection != null && DBConnection.Connection.Connected)
            {
                object obj = DBConnection.Connection.k(queryParams);
                return c.td(obj);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns c.Flip from parameterised query containing the query as char array;
        /// a list of names,  a list of ages and a gender to query against.
        /// </summary>
        /// <param name="namesToQuery"></param>
        /// <param name="agesToQuery"></param>
        /// <param name="genderToQuery"></param>
        /// <returns></returns>
        public c.Flip QueryNamesAgesAndGender(List<string> namesToQuery, List<int> agesToQuery, string genderToQuery)
        {
            object[] queryParams = new object[4];

            string query = "{[names;ages;genders]select from student where name in names,age in ages,gender in genders}";
            char[] queryarray = query.ToCharArray();

            string[] names = new string[namesToQuery.Count];
            int[] ages = new int[agesToQuery.Count];

            string[] gender = new string[1];

            for (int a = 0; a < namesToQuery.Count; a++)
            {
                names[a] = namesToQuery[a];
            }

            for (int b = 0; b < agesToQuery.Count; b++)
            {
                ages[b] = agesToQuery[b];
            }

            queryParams[0] = queryarray;
            queryParams[1] = names;
            queryParams[2] = ages;
            queryParams[3] = genderToQuery;

            if (DBConnection.Connection != null && DBConnection.Connection.Connected)
            {
                object obj = DBConnection.Connection.k(queryParams);
                return c.td(obj);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an int which is the number of rows of returned data in a c.Flip
        /// </summary>
        /// <param name="flipData"></param>
        /// <returns></returns>
        public int GetNumberOfRows(c.Flip flipData)
        {
            if (flipData != null)
            {
                return c.n(flipData.y[0]);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns the column names as an array of strings from a c.Flip
        /// </summary>
        /// <param name="flipData"></param>
        /// <returns></returns>
        public string[] GetColumnNames(c.Flip flipData)
        {
            string[] colNames = new string[0];

            if (flipData != null)
            {
                colNames = flipData.x;
            }

            return colNames;
        }
    }
}
