using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityCourseAndResultManagementSystemApp.Models
{
    public class EnrollCourse
    {
        public string StudentRegNo { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentDept { get; set; }
        public string CourseCode { get; set; }
        public DateTime EnrollDate { get; set; }
        public string CourseGrade { get; set; }
    }
}