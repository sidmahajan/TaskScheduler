using KlmTaskScheduler.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlmTaskScheduler
{
    class DbHelper
    {
        SqlConnection connection;
        private void ConnectDb()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            string connectionString = ConfigurationManager.ConnectionStrings["DataFormConnection"].ConnectionString;
            //builder.DataSource = "DESKTOP-OQ6LPEQ\\SQLDEV";
            //builder.IntegratedSecurity = true;
            //builder.InitialCatalog = "TestDB";
            try
            {
                //connection = new SqlConnection(builder.ConnectionString);
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CloseConnection()
        {
            if (connection != null && connection.State == ConnectionState.Open)
                connection.Close();
        }

        public List<EmployeeBE> GetEmployees()
        {
            List<EmployeeBE> employees = new List<EmployeeBE>();
            ConnectDb();
            SqlCommand cmd = new SqlCommand("Select * from Employee", connection);
            cmd.CommandType = CommandType.Text;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        EmployeeBE empBE = new EmployeeBE();
                        empBE.EmployeeId = reader.GetInt32(reader.GetOrdinal("ID"));
                        empBE.FirstName = reader.GetString(reader.GetOrdinal("FIRST_NAME"));
                        empBE.LastName = reader.GetString(reader.GetOrdinal("LAST_NAME"));
                        empBE.FullName = empBE.FirstName + " " + empBE.LastName;
                        employees.Add(empBE);
                    }
                }
            }

            CloseConnection();
            return employees;
        }

        public void CreateTask(TaskBE taskBE)
        {
            ConnectDb();

            SqlCommand cmd = new SqlCommand("sp_CreateTask", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("TaskName", taskBE.TaskName);
            cmd.Parameters.AddWithValue("TaskDate", taskBE.Date);
            cmd.Parameters.AddWithValue("StartTime", taskBE.StartTime);
            cmd.Parameters.AddWithValue("EndTime", taskBE.EndTime);
            cmd.Parameters.AddWithValue("AssignedUser", taskBE.AssignedTo);
            int k = cmd.ExecuteNonQuery();

            CloseConnection();
        }

        public List<TaskBE> GetTasks()
        {
            List<TaskBE> taskList = new List<TaskBE>();
            ConnectDb();
            SqlCommand cmd = new SqlCommand("Select * from Tasks", connection);
            cmd.CommandType = CommandType.Text;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TaskBE taskBE = new TaskBE();
                        taskBE.TaskId = reader.GetInt32(reader.GetOrdinal("ID"));
                        taskBE.TaskName = reader.GetString(reader.GetOrdinal("NAME"));
                        taskBE.Date = reader.GetDateTime(reader.GetOrdinal("DATE"));
                        taskBE.StartTime = reader.GetString(reader.GetOrdinal("STARTTIME"));
                        taskBE.EndTime = reader.GetString(reader.GetOrdinal("ENDTIME"));
                        taskBE.AssignedTo = reader.GetInt32(reader.GetOrdinal("USERID"));

                        taskBE.Duration = TimeSpan.Parse(taskBE.EndTime).TotalHours - TimeSpan.Parse(taskBE.StartTime).TotalHours;

                        taskList.Add(taskBE);
                    }
                }
            }

            CloseConnection();
            return taskList;
        }

    }
}
