using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityCourseAndResultManagementSystemApp.Models
{
    public class Course
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public double CourseCredit { get; set; }
        public string CourseDes { get; set; }
        public string DeptCode { get; set; }
        public string CourseSemester { get; set; }
        public string CourseAssignedTo { get; set; }
    }
}