using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class EmployeeDataViewModel
    {
        public int RecordNumber { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Employee_Surname { get; set; }
        public string ContactName { get; set; }
        public string Password { get; set; }
        public string Department { get; set; }
        public string User_Type { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string DCXKIM { get; set; }
        public string DCXLDOMAIL { get; set; }
        public string LocationCode { get; set; }
        public bool IsChangePassword { get; set; }
    }
}