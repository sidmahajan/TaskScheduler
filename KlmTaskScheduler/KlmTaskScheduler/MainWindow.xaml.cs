using KlmTaskScheduler.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KlmTaskScheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DbHelper dbh;
        List<TaskBE> taskList;
        List<EmployeeBE> employees;

        public MainWindow()
        {
            InitializeComponent();
            dbh = new DbHelper();

            employees = dbh.GetEmployees();
            cbAssignedTo.ItemsSource = employees;

            taskList = dbh.GetTasks();
            dgTaskList.ItemsSource = taskList;
        }

        public void Reset()
        {
            txtTaskName.Text = "";
            dpDate.Text = "";
            startTime.Text = "";
            endTime.Text = "";
            cbAssignedTo.SelectedIndex = -1;
            errormessage.Text = "";
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            errormessage.Text = "";
            //Basic checks for data entered
            if (txtTaskName.Text == "" || dpDate.Text == "" || startTime.Text == "" || endTime.Text == "" || cbAssignedTo.SelectedIndex == -1)
            {
                errormessage.Text = "All fields are mandatory!! Please check!";
            }
            else if (Convert.ToDateTime(startTime.Text).TimeOfDay >= Convert.ToDateTime(endTime.Text).TimeOfDay)
            {
                errormessage.Text = "Task Start cannot be greater than equal to Task End";
            }
            else
            {
                errormessage.Text = "";

                TaskBE taskBE = new TaskBE();
                taskBE.TaskName = txtTaskName.Text;
                taskBE.Date = Convert.ToDateTime(dpDate.Text);
                taskBE.StartTime = startTime.Text;
                taskBE.EndTime = endTime.Text;
                taskBE.AssignedTo = Convert.ToInt32(cbAssignedTo.SelectedValue);

                DateTime WeekStart = taskBE.Date.StartOfWeek(DayOfWeek.Monday);
                DateTime WeekEnd = WeekStart.AddDays(6);
                List<DateTime> allDates = new List<DateTime>();
                for (DateTime date = WeekStart; date <= WeekEnd; date = date.AddDays(1))
                    allDates.Add(date);

                //if ((taskBE.Date.DayOfWeek == DayOfWeek.Saturday) || (taskBE.Date.DayOfWeek == DayOfWeek.Sunday))
                //{
                //    errormessage.Text = "Weekend should be off";
                //}
                //else 
                if (taskList.Where(x => x.Date == taskBE.Date && x.AssignedTo == taskBE.AssignedTo).Sum(x => x.Duration) >= 8)
                {
                    errormessage.Text = "User cannot be assigned more than 8 hours of work per day";
                }
                else if (taskList.Where(x => x.AssignedTo == taskBE.AssignedTo).Where(x => allDates.Any(y => y.Equals(x.Date))).Select(x => x.Date).Distinct().Count() == 5
                    && !taskList.Where(x => x.AssignedTo == taskBE.AssignedTo).Where(x => allDates.Any(y => y.Equals(x.Date))).Select(x => x.Date).Distinct().Contains(taskBE.Date))
                {
                    errormessage.Text = "User should have 2 days off";
                }
                else if (TimeBlockIsOverLapping(Convert.ToDateTime(taskBE.StartTime), Convert.ToDateTime(taskBE.EndTime)
                    , taskList.Where(x => x.Date == taskBE.Date && x.AssignedTo == taskBE.AssignedTo)))
                {
                    errormessage.Text = "Task to the same user cannot overlap";
                }
                else
                {
                    dbh.CreateTask(taskBE);
                    taskList = dbh.GetTasks();
                    dgTaskList.ItemsSource = taskList;

                    Reset();
                }


            }

        }

        private static bool TimeBlockIsOverLapping(DateTime newTimeBlockStart, DateTime newTimeBlockEnd, IEnumerable<TaskBE> existingTimeBlocks)
        {
            foreach (var block in existingTimeBlocks)
            {
                // This is only if existingTimeBlocks are ordered. It is only
                // a speedup
                //if (newTimeBlockStart > Convert.ToDateTime(block.EndTime))
                //{
                //    break;
                //}

                // This is the real check. The ordering of the existingTimeBlocks
                // is irrelevant
                if (newTimeBlockStart <= Convert.ToDateTime(block.EndTime) && newTimeBlockEnd >= Convert.ToDateTime(block.StartTime))
                {
                    return true;
                }
            }

            return false;
        }

    }

}
