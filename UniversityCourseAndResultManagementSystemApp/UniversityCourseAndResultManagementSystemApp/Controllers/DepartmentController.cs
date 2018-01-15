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
    public class DepartmentController : Controller
    {
        private DatabaseGateway newDataBaseGateway = new DatabaseGateway();
        // GET: Department
        public ActionResult SaveDepartment(Department newdepartment)
        {
            int affectedRow = 0;
            if (newdepartment.DeptCode != null && newdepartment.DeptName != null)
            {
                //newDataBaseGateway.Command.CommandText = "INSERT INTO DEPARTMENT VALUES('" + newdepartment.DeptCode + "','" + newdepartment.DeptName + "');";
                newDataBaseGateway.Command.CommandText = "INSERT INTO DEPARTMENT(DeptCode, DeptName) SELECT @deptCode, @deptName WHERE NOT EXISTS(SELECT * FROM DEPARTMENT WHERE DeptCode = @deptCode or DeptName = @deptName);";
                newDataBaseGateway.Command.Parameters.AddWithValue("@deptCode", newdepartment.DeptCode);
                newDataBaseGateway.Command.Parameters.AddWithValue("@deptName", newdepartment.DeptName);
                newDataBaseGateway.Connection.Open();
                affectedRow = newDataBaseGateway.Command.ExecuteNonQuery();
                newDataBaseGateway.Connection.Close();
                if (affectedRow == 0)
                {
                    ViewBag.message = "Department Code or Name already Exsts !!!";
                }
                else
                {
                    ViewBag.message = "Successfully Saved...!!!";
                }
            }
            

            return View();
        }

        public ActionResult newDepartment(Department newdepartment)
        {
            int affectedRow = 0;
            if (newdepartment.DeptCode != null && newdepartment.DeptName != null)
            {
                //newDataBaseGateway.Command.CommandText = "INSERT INTO DEPARTMENT VALUES('" + newdepartment.DeptCode + "','" + newdepartment.DeptName + "');";
                newDataBaseGateway.Command.CommandText = "INSERT INTO DEPARTMENT(DeptCode, DeptName) SELECT @deptCode, @deptName WHERE NOT EXISTS(SELECT * FROM DEPARTMENT WHERE DeptCode = @deptCode or DeptName = @deptName);";
                newDataBaseGateway.Command.Parameters.AddWithValue("@deptCode", newdepartment.DeptCode);
                newDataBaseGateway.Command.Parameters.AddWithValue("@deptName", newdepartment.DeptName);
                newDataBaseGateway.Connection.Open();
                affectedRow = newDataBaseGateway.Command.ExecuteNonQuery();
                newDataBaseGateway.Connection.Close();
                if (affectedRow == 0)
                {
                    ViewBag.message = "Department Code or Name already Exsts !!!";
                }
                else
                {
                    ViewBag.message = "Successfully Saved...!!!";
                }
            }
            

            return View();
        }

        public ActionResult ViewDepartment()
        {
            newDataBaseGateway.Command.CommandText = "SELECT * FROM DEPARTMENT;";
            newDataBaseGateway.Connection.Open();
            SqlDataReader reader = newDataBaseGateway.Command.ExecuteReader();

            List<Department> deptList = new List<Department>();

            while (reader.Read())
            {
                Department newDepartment = new Department
                {
                    DeptCode = reader["DeptCode"].ToString(),
                    DeptName = reader["DeptName"].ToString()
                };
                deptList.Add(newDepartment);

            }
            newDataBaseGateway.Connection.Close();

            ViewBag.deptList = deptList;
            return View();
        }
    }
}