using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlmTaskScheduler.Entities
{
    class TaskBE
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public DateTime Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public Double Duration { get; set; }
        public int AssignedTo { get; set; }
    }
}
