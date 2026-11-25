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
    public class ClassRoomController : Controller
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

        List<int> GetAllRooms()
        {
            List<int> rooms = new List<int>();
            newDatabaseGateway.Command.CommandText = "SELECT * FROM ROOM;";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                rooms.Add(Convert.ToInt32(reader["RoomNo"].ToString()));
            }
            newDatabaseGateway.Connection.Close();
            return rooms;
        }

        List<string> GetAllDay()
        {
            List<string> days = new List<string>();
            newDatabaseGateway.Command.CommandText = "SELECT * FROM WEEKDAY;";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                days.Add(reader["DAY"].ToString());
            }
            newDatabaseGateway.Connection.Close();
            return days;
        }

        public RoomAllocation GetAllocationInfo(string CourseCode)
        {
            RoomAllocation prevAllocation = new RoomAllocation();
            newDatabaseGateway.Command.CommandText = "SELECT * FROM ROOM_SCHEDULE WHERE CourseCode = '"+ CourseCode +"' AND Valid = 'True';";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                prevAllocation.Day = reader["Day"].ToString();
                prevAllocation.RoomNo = reader["RoomNo"].ToString();
                prevAllocation.FromTime = reader["FromTime"].ToString();
                prevAllocation.ToTime = reader["ToTime"].ToString();
            }
            newDatabaseGateway.Connection.Close();
            return prevAllocation;
        }

        public bool LessThan(string str1, string str2)
        {
            int num1, num2, num3, num4;
            num1 = (str1[0] - '0')*10 + (str1[1] - '0');
            num2 = (str1[3] - '0')*10 + (str1[4] - '0');
            num3 = (str2[0] - '0')*10 + (str2[1] - '0');
            num4 = (str2[3] - '0')*10 + (str2[4] - '0');
            if (num1 < num3)
                return true;
            else if (num1 == num3)
            {
                if (num2 < num4)
                    return true;
            }
            return false;
        }

        public bool NoTimeOverlap(string FromTime, string ToTime, RoomAllocation prevAllocation)
        {
            if (LessThan(prevAllocation.FromTime, FromTime))
            {
                if (LessThan(FromTime, prevAllocation.ToTime))
                    return true;
                
            }
            else
            {
                if (LessThan(prevAllocation.FromTime, ToTime))
                    return true;
            }
            return false;
        }

        public ActionResult AllocateClassRoom(string CourseCode, string RoomNo, string Day, string FromTime, string ToTime)
        {
            
            int affectedRow = 0;
            if (CourseCode != null)
            {
                RoomAllocation prevAllocation = GetAllocationInfo(CourseCode);
                if (prevAllocation.Day != Day || NoTimeOverlap(FromTime, ToTime, prevAllocation) == false)
                {
                    newDatabaseGateway.Command.CommandText = "INSERT INTO ROOM_SCHEDULE VALUES('" + CourseCode + "','" + RoomNo + "','" + Day + "','" + FromTime + "','" + ToTime + "','True');";
                    newDatabaseGateway.Connection.Open();
                    affectedRow = newDatabaseGateway.Command.ExecuteNonQuery();
                    newDatabaseGateway.Connection.Close();
                    
                }
                if (affectedRow == 0)
                    ViewBag.message = "Error Found";
                else
                {
                    ViewBag.message = "Successfully Saved!!!!!";
                }
                
            }
            ViewBag.deptList = LoadDeptDropBox();
            ViewBag.rooms = GetAllRooms();
            ViewBag.days = GetAllDay();
            return View();
        }

        public JsonResult GetRoomScheduleByDepartment(string DeptCode)
        {
            List<RoomAllocation> allocations = new List<RoomAllocation>();
            string roomNo;
            newDatabaseGateway.Command.CommandText = "SELECT COURSE.CourseCode, CourseName, RoomNo, Day, FromTime, ToTime " +
                                                     "FROM COURSE FULL OUTER JOIN ROOM_SCHEDULE ON COURSE.CourseCode = ROOM_SCHEDULE.CourseCode " +
                                                     "WHERE DeptCode = '"+ DeptCode +"'  AND (Valid = 'True' OR Valid is NULL);";
            newDatabaseGateway.Connection.Open();
            SqlDataReader reader = newDatabaseGateway.Command.ExecuteReader();
            while (reader.Read())
            {
                RoomAllocation allocation = new RoomAllocation();

                allocation.CourseCode = reader["CourseCode"].ToString();
                allocation.CourseName = reader["CourseName"].ToString();
                
                
                allocation.RoomNo = reader["RoomNo"].ToString();
                
                allocation.Day = reader["Day"].ToString();
                allocation.FromTime = reader["FromTime"].ToString();
                allocation.ToTime = reader["ToTime"].ToString();
                
                allocations.Add(allocation);
            }
            newDatabaseGateway.Connection.Close();
            string CourseCode = "";
            List<AllocationInfo> allocationList = new List<AllocationInfo>();
            AllocationInfo allocationInfo = new AllocationInfo();
            foreach (RoomAllocation roomAllocation in allocations)
            {
                if (CourseCode != roomAllocation.CourseCode)
                {
                    CourseCode = roomAllocation.CourseCode;
                    if (allocationInfo.CourseCode != null)
                    {
                        if (allocationInfo.CourseSchedule == null && allocationInfo.CourseCode != null && allocationInfo.CourseName != null)
                        {
                            allocationInfo.CourseSchedule = "Not Schedule Yet";
                        }
                        if(allocationInfo.CourseCode != null && allocationInfo.CourseName != null && allocationInfo.CourseSchedule != null)
                            allocationList.Add(allocationInfo);
                        allocationInfo = new AllocationInfo();
                    }
                    
                    allocationInfo.CourseCode = roomAllocation.CourseCode;
                    allocationInfo.CourseName = roomAllocation.CourseName;
                    if (roomAllocation.RoomNo != "")
                    {
                        allocationInfo.CourseSchedule =
                            "R.No:" + roomAllocation.RoomNo + "," + roomAllocation.Day + "," +
                            roomAllocation.FromTime + "-" + roomAllocation.ToTime;
                    }
                    
                    
                }
                else
                {
                    allocationInfo.CourseSchedule += "\n" + "R.No:" + roomAllocation.RoomNo + "," + roomAllocation.Day + "," +
                                                     roomAllocation.FromTime + "-" + roomAllocation.ToTime;
                }
            }
            if (allocationInfo.CourseSchedule == null && allocationInfo.CourseCode != null && allocationInfo.CourseName != null)
            {
                allocationInfo.CourseSchedule = "Not Schedule Yet";
            }
            if (allocationInfo.CourseCode != null && allocationInfo.CourseName != null && allocationInfo.CourseSchedule != null)
                allocationList.Add(allocationInfo);
            return Json(allocationList);
        }

        public ActionResult ClassRoomInfo()
        {
            ViewBag.deptList = LoadDeptDropBox();
            return View();
        }

        public ActionResult UnAllocateClassRoom()
        {
            int affectedRow = 0;
            newDatabaseGateway.Command.CommandText = "UPDATE ROOM_SCHEDULE SET Valid = 'False'";
            newDatabaseGateway.Connection.Open();
            newDatabaseGateway.Command.ExecuteNonQuery();
            newDatabaseGateway.Connection.Close();
            if (affectedRow == 0)
            {
                ViewBag.message = "Error!!";
            }
            else
            {
                ViewBag.message = "Successfully UnAllocated.!!!";
            }
            return View();
        }
    }
}