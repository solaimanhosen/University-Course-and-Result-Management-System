using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityCourseAndResultManagementSystemApp.Models
{
    public class Student
    {
        public string StudentRegNo { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentContactNo { get; set; }
        public DateTime StudentRegDate { get; set; }
        public string StudentAddress { get; set; }
        public string StudentDept { get; set; }
    }
}