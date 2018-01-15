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
    public class ResultController : Controller
    {
        private DatabaseGateway newDatabaseGateway = new DatabaseGateway();

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

        List<string> GetAllGrade()
        {
            List<string> gradeLetters = new List<string>();
            newDatabaseGateway.Command.CommandText = "SELECT GradeLetter FROM GRADE;";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                gradeLetters.Add(reader["GradeLetter"].ToString());
            }
            newDatabaseGateway.Connection.Close();
            return gradeLetters;
        }

        public JsonResult GetCoursesByStudent(string StudentRegNo)
        {
            List<Course> courses = new List<Course>();
            newDatabaseGateway.Command.CommandText = "SELECT CourseCode FROM ENDROLLMENT WHERE StudentRegNo = '" + StudentRegNo + "';";
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
            ViewBag.student = newStudent;
            return Json(newStudent);
        }

        public JsonResult GetResultByStudent(string StudentRegNo)
        {
            TempData["StudentRegNo"] = StudentRegNo;
            List<Grade> grades = new List<Grade>();
            newDatabaseGateway.Command.CommandText =
                "SELECT COURSE.CourseCode, CourseName, Grade FROM COURSE, ENDROLLMENT WHERE COURSE.CourseCode = ENDROLLMENT.CourseCode AND StudentRegNo = '"+ StudentRegNo +"';";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                Grade newGrade = new Grade
                {
                    CourseCode = reader["CourseCode"].ToString(),
                    CourseName = reader["CourseName"].ToString(),
                    CourseGrade = reader["Grade"].ToString()
                };
                grades.Add(newGrade);
            }
            newDatabaseGateway.Connection.Close();
            return Json(grades);
        }

        public ActionResult SaveStudentResult(Student student, string CourseCode, string GradeLetter)
        {
            int affectedRow = 0;
            if (student.StudentRegNo != null && CourseCode != null && GradeLetter != null)
            {
                newDatabaseGateway.Command.CommandText = "UPDATE ENDROLLMENT SET Grade = '" + GradeLetter + "' WHERE StudentRegNo = '" + student.StudentRegNo + "' and CourseCode = '" + CourseCode + "';";
                newDatabaseGateway.Connection.Open();
                affectedRow = newDatabaseGateway.Command.ExecuteNonQuery();
                newDatabaseGateway.Connection.Close();
                if (affectedRow == 0)
                {
                    ViewBag.message = "Error !!!";
                }
                else
                {
                    ViewBag.message = "Successfully Saved. !!!";
                }
            }
            ViewBag.regNoList = GetAllRegNo();
            ViewBag.gradeLetters = GetAllGrade();
            return View();
        }

        public ActionResult ViewResult()
        {
            TempData["StudentRegNo"] = "";
            ViewBag.regNoList = GetAllRegNo();
            return View();
        }



        public void GenerateResultPDF()
        {
            string StudentRegNo = (string) TempData["StudentRegNo"];

            Student student = new Student();
            newDatabaseGateway.Command.CommandText =
                "SELECT StudentName, StudentEmail, StudentDept FROM STUDENT WHERE StudentRegNo = '" + StudentRegNo + "';";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                student.StudentName = reader["StudentName"].ToString();
                student.StudentEmail = reader["StudentEmail"].ToString();
                student.StudentDept = reader["StudentDept"].ToString();
            }
            newDatabaseGateway.Connection.Close();

            List<Grade> grades = new List<Grade>();
            newDatabaseGateway.Command.CommandText =
                "SELECT COURSE.CourseCode, CourseName, Grade, CourseCredit FROM COURSE, ENDROLLMENT WHERE COURSE.CourseCode = ENDROLLMENT.CourseCode AND StudentRegNo = '" + StudentRegNo + "';";
            newDatabaseGateway.Connection.Open();
            reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                Grade newGrade = new Grade
                {
                    CourseCode = reader["CourseCode"].ToString(),
                    CourseName = reader["CourseName"].ToString(),
                    CourseGrade = reader["Grade"].ToString(),
                    CourseCredit = Convert.ToDouble(reader["CourseCredit"].ToString())
                };
                grades.Add(newGrade);
            }
            newDatabaseGateway.Connection.Close();

            Dictionary<string, double> gradePoint = new Dictionary<string, double>();
            gradePoint["A+"] = 4.00;
            gradePoint["A"] = 4.00;
            gradePoint["A-"] = 3.70;
            gradePoint["B+"] = 3.30;
            gradePoint["B"] = 3.00;
            gradePoint["B-"] = 2.70;
            gradePoint["C+"] = 2.30;
            gradePoint["C"] = 2.00;
            gradePoint["C-"] = 1.70;
            gradePoint["D+"] = 1.30;
            gradePoint["D"] = 1.00;
            gradePoint["F"] = 0.00;
            gradePoint["Not Graded Yet"] = 0.00;

            double totalGp = 0.00;
            double totalCr = 0.00;
            double cgpa = 0.00;

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();
                    

                    //Generate Invoice (Bill) Header.
                    /*sb.Append("<table width='90%' border='0' align='center' cellpadding='0' cellspacing='0'>");
                    sb.Append("<tr><td><table width='100%' border='0' cellspacing='0' cellpadding='0' ><tr>");
                    sb.Append("<td width='50%'>Student ID: <strong>2014-3-60-044</strong><br />Student Name: <strong>Md. Solaiman Hosen</strong><br />Objective: <strong>B.Sc. in  Computer Science and Engineering</strong></td>");
                    sb.Append("<td width='50%' align='left' valign='top'><strong> <u> Status</u></strong><br /></td></tr></table><hr/></td></tr>");
		
                    sb.Append("<tr><td align='left'><table width='80%' border='0' cellspacing='0' cellpadding='0' >");
                    sb.Append("<tr><td width='9%'><strong>Course</strong></td><td width='43%'><strong>Title of the course </strong></td><td width='10%' align='center'><strong>Credit</strong></td>");
                    sb.Append("<td width='12%' align='center'><strong>Grade</strong></td><td width='16%' align='center'><strong>Grade Point </strong></td><td width='10%' align='center'><strong>gpacr</strong></td></tr>");
	  
                    //sb.Append("_____________________________________________________________________________________________");

                    sb.Append("<tr><td>MAT101</td>");
                    sb.Append("<td>Differential & Integral Calculus</td>");
                    sb.Append("<td align='center'>3</td>");
                    sb.Append("<td align='center'>A-</td>");
                    sb.Append("<td align='center'>3.7</td>");
                    sb.Append("<td align='center'>3</td></tr>");
		
                   // sb.Append("______________________________________________________________________________");
                    sb.Append("</tr>");
                    sb.Append("<tr><td>&nbsp;</td>");
                    sb.Append("<td align='left'><strong>CGPA: 3.7</strong>&nbsp;</td>");
                    sb.Append("<td colspan='2' align='right'>Term GPA:&nbsp; </td>");
                    sb.Append("<td align='center'><strong>3.7</strong></td><td>&nbsp;</td></tr>	");  
	  
                    //sb.Append("____________________________________________________________________________________");
			  
                    sb.Append("  </table></td></tr><tr><td><table width='100%' border='0' cellspacing='0' cellpadding='0'>");
                    sb.Append("  <tr><td width='24%'><strong>Summary: CGPA 3.65 (out of 4.00) </strong></td>");
                    sb.Append("  <td width='11%'><strong>gpacr: 117</strong></td>");
                    sb.Append("  <td width='24%'><strong>Credit Earned: 117</strong></td>");
                    sb.Append("  <td width='41%'><strong>Minimum Credit Required: 140</strong></td></tr>");
                    sb.Append("  </table></td></tr><tr><td>&nbsp;</td></tr>");
                    sb.Append("  </table>");*/

                    sb.Append("<h2 align='center'>EAST WEST UNIVERSITY</h2>");
                    sb.Append("<br/>");
                    sb.Append("<table>");
                    sb.Append("<tr><td>Reg. No :</td><td>");
                    sb.Append(StudentRegNo);
                    
                    sb.Append("</td><td>&nbsp;</td></tr>");
                    sb.Append("<tr><td>Student Name : </td><td>");
                    sb.Append(student.StudentName);
                    sb.Append("</td><td>&nbsp;</td></tr>");
                    sb.Append("<tr><td>Objective : </td><td>B.Sc. in ");
                    sb.Append(student.StudentDept);
                    sb.Append("</td><td>&nbsp;</td></tr>");
                    sb.Append("</table>");
                    sb.Append("___________________________________________________________________________________");

                    sb.Append("<table width='80%' border='0' cellspacing='0' cellpadding='0' >");
                    sb.Append("<tr><th width='15%'>Course</th><th width='37%'>Title of the course</th><th width='10%' align='center'>Credit</th>");
                    sb.Append("<th width='12%' align='center'>Grade</th><th width='16%' align='center'>Grade Point</th><th width='10%' align='center'>gpacr</th></tr></table>");
                    sb.Append("___________________________________________________________________________________");


                    sb.Append("<table width='80%' border='0' cellspacing='0' cellpadding='0' >");
                    foreach (Grade grade in grades)
                    {
                        sb.Append("<tr><td width='15%'>");
                        sb.Append(grade.CourseCode);
                        //CSE105
                        sb.Append("</td>");
                        sb.Append("<td width='37%'>");
                        sb.Append(grade.CourseName);
                        //Structured Programming
                        sb.Append("</td>");
                        sb.Append("<td width='10%' align='center'>");
                        sb.Append(grade.CourseCredit);
                        //4
                        sb.Append("</td>");
                        sb.Append("<td width='12%' align='center'>");
                        sb.Append(grade.CourseGrade);
                        //A
                        sb.Append("</td>");
                        sb.Append("<td width='16%' align='center'>");
                        sb.Append(gradePoint[grade.CourseGrade]);
                        //4.00
                        sb.Append("</td>");
                        sb.Append("<td width='10%' align='center'>");
                        sb.Append(grade.CourseCredit);
                        //4
                        sb.Append("</td></tr>");

                        totalCr += grade.CourseCredit;
                        totalGp += (gradePoint[grade.CourseGrade]*grade.CourseCredit);
                    }

                    cgpa = totalGp / totalCr;
                    
                    sb.Append("</table>");
                    sb.Append("___________________________________________________________________________________");

                    sb.Append("<table>");
                    sb.Append("<tr><td>&nbsp;</td>");
                    sb.Append("<td align='left'>CGPA: ");
                    sb.Append(cgpa);
                    //3.7
                    sb.Append("&nbsp;</td>");
                    sb.Append("<td colspan='2' align='right'>Term GPA:&nbsp; </td>");
                    sb.Append("<td align='center'>");
                    sb.Append(cgpa);
                    //3.7
                    sb.Append("</td><td>&nbsp;</td></tr>");
                    sb.Append("</table>");

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
}