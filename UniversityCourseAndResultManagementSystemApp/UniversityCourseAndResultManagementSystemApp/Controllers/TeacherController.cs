using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversityCourseAndResultManagementSystemApp.Gateway;
using UniversityCourseAndResultManagementSystemApp.Models;

namespace UniversityCourseAndResultManagementSystemApp.Controllers
{
    public class TeacherController : Controller
    {
        private DatabaseGateway newDatabaseGateway = new DatabaseGateway();
        // GET: Teacher
        public List<Department> LoadDeptDropBox()
        {
            List<Department> deptList = new List<Department>();
            newDatabaseGateway.Command.CommandText = "SELECT * FROM DEPARTMENT;";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                Department newDepartment = new Department
                {
                    DeptCode = reader["DeptCode"].ToString(),
                    DeptName = reader["DeptName"].ToString()
                };
                deptList.Add(newDepartment);

            }
            newDatabaseGateway.Connection.Close();

            return deptList;

        }

        public List<string> LoadDesignationDropBox()
        {
            List<string> desgnationList = new List<string>();
            newDatabaseGateway.Command.CommandText = "SELECT * FROM DESIGNATION;";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                desgnationList.Add(reader["DesignationName"].ToString());

            }
            newDatabaseGateway.Connection.Close();

            return desgnationList;
        }

       

        public ActionResult SaveTeacher(Teacher newTeacher)
        {
            int affectedRow = 0;
            if (newTeacher.TeacherName != null)
            {
                /*newDatabaseGateway.Command.CommandText = "INSERT INTO TEACHER(TeacherName, TeacherAddress, TeacherEmail, TeacherContactNo, TeacherDesignation, TeacherDeptCode, TeacherCreditLimit, TeacherCreditRemaining) VALUES('" + newTeacher.TeacherName + "','" +
                                                         newTeacher.TeacherAddress + "','" + newTeacher.TeacherEmail + "','" +
                                                         newTeacher.TeacherContactNo + "','" + newTeacher.TeacherDesignation + "','" +
                                                         newTeacher.TeacherDeptCode + "','"+ newTeacher.TeacherCreditLimit +"', '"+ 
                                                         newTeacher.TeacherCreditLimit +"');";*/
                newDatabaseGateway.Command.CommandText = "INSERT INTO TEACHER(TeacherName, TeacherAddress, TeacherEmail, TeacherContactNo, TeacherDesignation, TeacherDeptCode, TeacherCreditLimit, TeacherCreditRemaining) " +
                                                         "SELECT @teacherName, @teacherAddress, @teacherEmail, @teacherContactNo, @teacherDesignation, @teacherDeptCode, @teacherCreditLimit, @teacherCreditRemaining " +
                                                         "WHERE NOT EXISTS (SELECT * FROM TEACHER WHERE TeacherEmail = @teacherEmail)";
                newDatabaseGateway.Command.Parameters.AddWithValue("@teacherName", newTeacher.TeacherName);
                newDatabaseGateway.Command.Parameters.AddWithValue("@teacherAddress", newTeacher.TeacherAddress);
                newDatabaseGateway.Command.Parameters.AddWithValue("@teacherEmail", newTeacher.TeacherEmail);
                newDatabaseGateway.Command.Parameters.AddWithValue("@teacherContactNo", newTeacher.TeacherContactNo);
                newDatabaseGateway.Command.Parameters.AddWithValue("@teacherDesignation", newTeacher.TeacherDesignation);
                newDatabaseGateway.Command.Parameters.AddWithValue("@teacherDeptCode", newTeacher.TeacherDeptCode);
                newDatabaseGateway.Command.Parameters.AddWithValue("@teacherCreditLimit", newTeacher.TeacherCreditLimit);
                newDatabaseGateway.Command.Parameters.AddWithValue("@teacherCreditRemaining", newTeacher.TeacherCreditRemaining);

                newDatabaseGateway.Connection.Open();
                affectedRow = newDatabaseGateway.Command.ExecuteNonQuery();
                newDatabaseGateway.Connection.Close();

                if (affectedRow == 0)
                {
                    ViewBag.message = "Email already Exists !!!";
                }
                else
                {
                    ViewBag.message = "Saved Successfully !!!";
                }
            }
            ViewBag.deptList = LoadDeptDropBox();
            ViewBag.designationList = LoadDesignationDropBox();

            return View();
        }

        public ActionResult AddTeacher(Teacher newTeacher)
        {
            if (newTeacher.TeacherName != null)
            {
                newDatabaseGateway.Command.CommandText = "INSERT INTO TEACHER(TeacherName, TeacherAddress, TeacherEmail, TeacherContactNo, TeacherDesignation, TeacherDeptCode, TeacherCreditLimit, TeacherCreditRemaining) VALUES('" + newTeacher.TeacherName + "','" +
                                                         newTeacher.TeacherAddress + "','" + newTeacher.TeacherEmail + "','" +
                                                         newTeacher.TeacherContactNo + "','" + newTeacher.TeacherDesignation + "','" +
                                                         newTeacher.TeacherDeptCode + "','" + newTeacher.TeacherCreditLimit + "', '" +
                                                         newTeacher.TeacherCreditLimit + "');";
                newDatabaseGateway.Connection.Open();
                int affectedRow = newDatabaseGateway.Command.ExecuteNonQuery();
                newDatabaseGateway.Connection.Close();
            }
            ViewBag.deptList = LoadDeptDropBox();
            ViewBag.designationList = LoadDesignationDropBox();

            return View();
        }

        public JsonResult GetTeacherByDepartment(string DeptCode)
        {
            List<Teacher> teachers = new List<Teacher>();
            newDatabaseGateway.Command.CommandText = "SELECT * FROM TEACHER WHERE TeacherDeptCode = '"+ DeptCode +"';";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                Teacher newTeacher = new Teacher();

                newTeacher.TeacherId = (int) reader["TeacherId"];
                newTeacher.TeacherName = reader["TeacherName"].ToString();
                newTeacher.TeacherCreditLimit = Convert.ToDouble(reader["TeacherCreditLimit"].ToString());
                newTeacher.TeacherCreditRemaining = Convert.ToDouble(reader["TeacherCreditRemaining"].ToString());
                
                teachers.Add(newTeacher);
            }
            newDatabaseGateway.Connection.Close();
            return Json(teachers);
        }

        public List<Teacher> GeTeacherByTeacherId(int TeacherId)
        {
            List<Teacher> teachers = new List<Teacher>();
            newDatabaseGateway.Command.CommandText = "SELECT * FROM TEACHER WHERE TeacherId = '" + TeacherId + "';";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                Teacher newTeacher = new Teacher();
                newTeacher.TeacherName = reader["TeacherName"].ToString();
                newTeacher.TeacherCreditLimit = Convert.ToDouble(reader["TeacherCreditLimit"].ToString());
                newTeacher.TeacherCreditRemaining = Convert.ToDouble(reader["TeacherCreditRemaining"].ToString());

                teachers.Add(newTeacher);
            }
            newDatabaseGateway.Connection.Close();
            return teachers;
        }

        public JsonResult GetTeacherByTeacherId(int TeacherId)
        {
            return Json(GeTeacherByTeacherId(TeacherId));
        }

        public JsonResult GetCoursesByDepartmentCode(string DeptCode)
        {
            List<Course> courses = new List<Course>();
            newDatabaseGateway.Command.CommandText = "SELECT * FROM COURSE WHERE DeptCode = '"+ DeptCode +"';";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                Course newCourse = new Course
                {
                    CourseCode = reader["CourseCode"].ToString()
                };
                courses.Add(newCourse);

            }
            newDatabaseGateway.Connection.Close();
            return Json(courses);
        }

        public JsonResult GetCourseByCourseCode(string CourseCode)
        {
            List<Course> courses = new List<Course>();
            newDatabaseGateway.Command.CommandText = "SELECT * FROM COURSE WHERE CourseCode = '" + CourseCode + "';";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                Course newCourse = new Course();
                newCourse.CourseName = reader["CourseName"].ToString();
                newCourse.CourseCredit = Convert.ToDouble(reader["CourseCredit"].ToString());

                courses.Add(newCourse);
            }
            newDatabaseGateway.Connection.Close();
            return Json(courses);
        }

        public ActionResult CourseAssignToTeacher(Teacher newTeacher, Course newCourse)
        {
            if (newTeacher.TeacherId != 0 && newCourse.CourseCode != null)
            {
                /*newDatabaseGateway.Command.CommandText = "BEGIN " +
                                                         "DECLARE @maxWeight float, @productKey integer " +
                                                         "SET @maxWeight = 100.00 " +
                                                         "SET @productKey = 424  " +
                                                         "IF @maxWeight <= 1000 " +
                                                         "BEGIN " +
                                                         "PRINT 'There are ' +  ' Touring-3000 bikes.'; " +
                                                         "END; " +
                                                         "ELSE " + 
                                                         "PRINT 'There are ' +  ' Touring-4000 bikes.'; " +
                                                         "END; " +
                                                         "END;";*/
                string str="";
                newDatabaseGateway.Command.CommandText = "SELECT FACULTY FROM COURSE WHERE CourseCode = '"+ newCourse.CourseCode +"';";
                newDatabaseGateway.Connection.Open();
                SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
                while (reader.Read())
                {
                    str = reader["FACULTY"].ToString();
                }
                newDatabaseGateway.Connection.Close();
                if (str == "Not Assigned Yet")
                {
                    List<Teacher> teachers = GeTeacherByTeacherId(newTeacher.TeacherId);
                    newDatabaseGateway.Command.CommandText = "BEGIN " +
                                                             "UPDATE COURSE SET FACULTY = '"+ teachers[0].TeacherName +"' WHERE CourseCode = '"+ newCourse.CourseCode +"'; " +
                                                             "UPDATE TEACHER SET TeacherCreditRemaining = '" + (newTeacher.TeacherCreditRemaining - newCourse.CourseCredit) + "' WHERE TeacherID = '"+ newTeacher.TeacherId +"'; " +
                                                             "END;";
                    newDatabaseGateway.Connection.Open();
                    int affectedRow = newDatabaseGateway.Command.ExecuteNonQuery();
                    newDatabaseGateway.Connection.Close();
                    ViewBag.message = "Course Assigneded Successfully ";
                }
                else
                {
                    ViewBag.message = "Course is Already Assigned To " + str;
                }

            }
            ViewBag.deptList = LoadDeptDropBox();
            return View();
        }
    }
}