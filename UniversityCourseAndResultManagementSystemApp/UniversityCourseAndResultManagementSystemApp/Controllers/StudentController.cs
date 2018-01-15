using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using UniversityCourseAndResultManagementSystemApp.Gateway;
using UniversityCourseAndResultManagementSystemApp.Models;

namespace UniversityCourseAndResultManagementSystemApp.Controllers
{
    public class StudentController : Controller
    {
        
        private DatabaseGateway newDatabaseGateway = new DatabaseGateway();

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
        public ActionResult RegisterStudent(Student newStudent)
        {
            int affectedRow = 0;
            if (newStudent.StudentName != null)
            {
                int total = 0;
                string regNo = newStudent.StudentDept + "-" + newStudent.StudentRegDate.Year + "-";
                newDatabaseGateway.Command.CommandText = "SELECT COUNT(*) AS TOTAL FROM STUDENT WHERE StudentRegNo like '" + regNo + "%';";
                newDatabaseGateway.Connection.Open();
                SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
                while (reader.Read())
                {
                    total = (int)reader["TOTAL"];
                }
                newDatabaseGateway.Connection.Close();
                total++;
                if (total < 10)
                {
                    regNo += "00" + total;
                }
                else if (total < 100)
                {
                    regNo += "0" + total;
                }
                else
                {
                    regNo += total;
                }
                newStudent.StudentRegNo = regNo;

                /*newDatabaseGateway.Command.CommandText = "INSERT INTO STUDENT VALUES('" + newStudent.StudentRegNo + "','" +
                                                         newStudent.StudentName + "','" +
                                                         newStudent.StudentEmail + "','" +
                                                         newStudent.StudentContactNo + "','" +
                                                         newStudent.StudentRegDate.Date.ToString("MM/dd/yyyy") + "','" +
                                                         newStudent.StudentAddress + "','" +
                                                         newStudent.StudentDept + "');";*/
                newDatabaseGateway.Command.CommandText = "INSERT INTO STUDENT(StudentRegNo, StudentName, StudentEmail, StudentContactNo, StudentRegDate, StudentAddress, StudentDept) " +
                                                         "SELECT @StudentRegNo, @StudentName, @StudentEmail, @StudentContactNo, @StudentRegDate, @StudentAddress, @StudentDept " +
                                                         "WHERE NOT EXISTS (SELECT * FROM STUDENT WHERE StudentEmail = @StudentEmail)";
                newDatabaseGateway.Command.Parameters.AddWithValue("@StudentRegNo", newStudent.StudentRegNo);
                newDatabaseGateway.Command.Parameters.AddWithValue("@StudentName", newStudent.StudentName);
                newDatabaseGateway.Command.Parameters.AddWithValue("@StudentEmail", newStudent.StudentEmail);
                newDatabaseGateway.Command.Parameters.AddWithValue("@StudentContactNo", newStudent.StudentContactNo);
                newDatabaseGateway.Command.Parameters.AddWithValue("@StudentRegDate", newStudent.StudentRegDate);
                newDatabaseGateway.Command.Parameters.AddWithValue("@StudentAddress", newStudent.StudentAddress);
                newDatabaseGateway.Command.Parameters.AddWithValue("@StudentDept", newStudent.StudentDept);

                newDatabaseGateway.Connection.Open();
                affectedRow = newDatabaseGateway.Command.ExecuteNonQuery();
                newDatabaseGateway.Connection.Close();

                if (affectedRow == 0)
                {
                    ViewBag.message = "Email Already Exists!!";

                    
                }
                else
                {
                    ViewBag.message = "Successfully Saved!!!!!";

                    using (StringWriter sw = new StringWriter())
                    {
                        using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                        {
                            StringBuilder sb = new StringBuilder();

                            //Generate Invoice (Bill) Header.

                            sb.Append("<h2>ADMISSION INFORMATION</h2>");
                            sb.Append("<br/><br/>");

                            sb.Append("<table>");
                            sb.Append("<tr><td>REG. NO</td><td>:</td><td>");
                            sb.Append(newStudent.StudentRegNo);
                            sb.Append("</td></tr>");
                            sb.Append("<tr><td>NAME</td><td>:</td><td>");
                            sb.Append(newStudent.StudentName);
                            sb.Append("</td></tr>");
                            sb.Append("<tr><td>EMAIL</td><td>:</td><td>");
                            sb.Append(newStudent.StudentEmail);
                            sb.Append("</td></tr>");
                            sb.Append("<tr><td>CONTACT NO</td><td>:</td><td>");
                            sb.Append(newStudent.StudentContactNo);
                            sb.Append("</td></tr>");
                            sb.Append("<tr><td>REG. DATE</td><td>:</td><td>");
                            sb.Append(newStudent.StudentRegDate);
                            sb.Append("</td></tr>");
                            sb.Append("<tr><td>ADDRESS</td><td>:</td><td>");
                            sb.Append(newStudent.StudentAddress);
                            sb.Append("</td></tr>");
                            sb.Append("<tr><td>DEPARTMENT</td><td>:</td><td>");
                            sb.Append(newStudent.StudentDept);
                            sb.Append("</td></tr>");
                            sb.Append("</table>");

                            //sb.Append("<h2>hi</h2>");


                            //Export HTML String as PDF.
                            StringReader sr = new StringReader(sb.ToString());
                            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                            pdfDoc.Open();
                            htmlparser.Parse(sr);
                            pdfDoc.Close();
                            Response.ContentType = "application/pdf";
                            Response.AddHeader("content-disposition", "attachment;filename=Invoice_" + ".pdf");
                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            Response.Write(pdfDoc);
                            Response.End();
                        }
                    }
                }






            }
            ViewBag.date = DateTime.Today.ToString("dd-MM-yyyy");
            ViewBag.deptList = LoadDeptDropBox();
            return View();
        }

        List<string> GetAllRegNo()
        {
            List<string> regNoList = new List<string>();
            newDatabaseGateway.Command.CommandText = "SELECT StudentRegNo FROM STUDENT;";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                regNoList.Add(reader["StudentRegNo"].ToString());
            }
            newDatabaseGateway.Connection.Close();
            return regNoList;
        }

        public JsonResult GetStudentByStudentRegNo(string StudentRegNo)
        {
            Student newStudent = new Student();
            newDatabaseGateway.Command.CommandText =
                "SELECT StudentName, StudentEmail, StudentDept FROM STUDENT WHERE StudentRegNo = '" + StudentRegNo + "';";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                newStudent.StudentName = reader["StudentName"].ToString();
                newStudent.StudentEmail = reader["StudentEmail"].ToString();
                newStudent.StudentDept = reader["StudentDept"].ToString();
            }
            newDatabaseGateway.Connection.Close();
            ViewBag.courses = GetCoursesByDepartment2(newStudent.StudentDept);
            return Json(newStudent);
        }

        public JsonResult GetCoursesByDepartment(string StudentDept)
        {
            List<Course> courses = new List<Course>();
            newDatabaseGateway.Command.CommandText =
                "SELECT CourseCode FROM COURSE WHERE DeptCode = '" + StudentDept + "';";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                Course newCourse = new Course();
                newCourse.CourseCode = reader["CourseCode"].ToString();
                courses.Add(newCourse);
            }
            newDatabaseGateway.Connection.Close();
            return Json(courses);
        }

        public List<Course> GetCoursesByDepartment2(string StudentDept)
        {
            List<Course> courses = new List<Course>();
            newDatabaseGateway.Command.CommandText =
                "SELECT CourseCode FROM COURSE WHERE DeptCode = '" + StudentDept + "';";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                Course newCourse = new Course();
                newCourse.CourseCode = reader["CourseCode"].ToString();
                courses.Add(newCourse);
            }
            newDatabaseGateway.Connection.Close();
            return courses;
        }

        public ActionResult EnrollCourse(EnrollCourse data)
        {
            int affectedRow = 0;
            string Grade = "Not Graded Yet";
            data.CourseGrade = Grade;
            if (data.CourseCode != null && data.StudentRegNo != null)
            {
                newDatabaseGateway.Command.CommandText = "INSERT INTO ENDROLLMENT(StudentRegNo, CourseCode, Grade, EnrollDate) SELECT @studentRegNo, @courseCode, @grade, @enrollDate WHERE NOT EXISTS (SELECT * FROM ENDROLLMENT WHERE StudentRegNo = @studentRegNo and CourseCode = @courseCode)";
                newDatabaseGateway.Command.Parameters.AddWithValue("@studentRegNo", data.StudentRegNo);
                newDatabaseGateway.Command.Parameters.AddWithValue("@courseCode", data.CourseCode);
                newDatabaseGateway.Command.Parameters.AddWithValue("@grade", data.CourseGrade);
                newDatabaseGateway.Command.Parameters.AddWithValue("@enrollDate", data.EnrollDate);
                //var bal = "INSERT INTO TEST(TEST_NAME, TEST_FEE, TYPE_ID) SELECT @testName, @testFee, @testType WHERE NOT EXISTS(SELECT * FROM TEST WHERE TEST_NAME = @testName);";
                newDatabaseGateway.Connection.Open();
                affectedRow = newDatabaseGateway.Command.ExecuteNonQuery();
                newDatabaseGateway.Connection.Close();
                if (affectedRow == 0)
                {
                    ViewBag.message = "Error !!!";
                }
                else
                {
                    ViewBag.message = "Successfully Enrolled. !!!";
                }
            }
            ViewBag.regNoList = GetAllRegNo();
            return View();
        }

        void GeneratePDF(Student newStudent)
        {
            
        }
    }
}