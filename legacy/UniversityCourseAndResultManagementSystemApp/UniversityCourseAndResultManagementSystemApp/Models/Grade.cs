using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityCourseAndResultManagementSystemApp.Models
{
    public class Grade
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string CourseGrade { get; set; }
        public double CourseCredit { get; set; }
    }
}