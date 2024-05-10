using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop_Application
{
    public class Clients
    {
        public string IP_Address { get; set; }
        public string Name { get; set; }
        public int Port_NO { get; set; }
        public int No_Of_Job_Completed { get; set; }
        public string Currently_Doing_Any_Job { get; set; }
        public string Name_Of_Last_Completed_Job { get; set; }
        public string Result_Of_Last_Completed_Job { get; set; }
        public string Connected { get; set; }
    }
}
