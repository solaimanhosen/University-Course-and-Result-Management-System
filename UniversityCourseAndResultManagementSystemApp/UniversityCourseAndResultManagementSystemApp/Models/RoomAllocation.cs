using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityCourseAndResultManagementSystemApp.Models
{
    public class RoomAllocation
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string RoomNo { get; set; }
        public string Day { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string Valid { get; set; }
    }
}