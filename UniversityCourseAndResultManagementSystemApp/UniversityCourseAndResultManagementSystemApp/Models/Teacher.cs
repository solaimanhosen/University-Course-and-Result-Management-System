using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityCourseAndResultManagementSystemApp.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public String TeacherName { get; set; }
        public String TeacherAddress { get; set; }
        public String TeacherEmail { get; set; }
        public String TeacherContactNo { get; set; }
        public String TeacherDesignation { get; set; }
        public String TeacherDeptCode { get; set; }
        public double TeacherCreditLimit { get; set; }
        public double TeacherCreditRemaining { get; set; }

    }
}