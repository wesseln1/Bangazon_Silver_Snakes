using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Workforce_Silver_Snakes.Models;
using Workforce_Silver_Snakes.Models.ViewModels;

namespace Workforce_Silver_Snakes.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;
        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }
        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Employees
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.FirstName, e.LastName, e.DepartmentId, d.[Name]
                                        FROM Employee e
                                        LEFT JOIN Department d ON e.DepartmentId = d.Id";


                    var reader = cmd.ExecuteReader();
                    var employees = new List<Employee>();
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {

                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            }
                        });
                    }
                    reader.Close();
                    return View(employees);
                }

            }
        }

        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            var departments = GetDepartments().Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString()
            }).ToList();
            var computers = GetAvalibleComputers().Select(computers => new SelectListItem
            {
                Text = computers.Make + "" + computers.Model,
                Value = computers.Id.ToString()
            }).ToList();

            var viewModel = new EmployeeViewModel
            {
                Employee = new Employee(),
                Departments = departments,
                Computers = computers
            };
            return View(viewModel);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee 
                                            (FirstName, LastName, DepartmentId, IsSupervisor, ComputerId, Email)
                                            VALUES (@firstName, @lastName, @departmentId, @isSupervisor, @computerId, @email)";

                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@computerId", employee.ComputerId));
                        cmd.Parameters.Add(new SqlParameter("@email", employee.Email));

                        //use an excute non query for inserts bc we are not asking for anything back.
                        //it is a non query- shows that the rows were affected.
                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        private List<Department> GetDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id,[Name] FROM Department";
                    var reader = cmd.ExecuteReader();

                    var departments = new List<Department>();

                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }
                    reader.Close();
                    return departments;
                }

            }
        }

        private List<Computer> GetAvalibleComputers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id AS ComputerId, c.Make, 
                                        c.Model, c.PurchaseDate, 
                                        c.DecomissionDate
                                        FROM Computer c
                                        WHERE NOT EXISTS
                                        (SELECT e.ComputerId AS EmployeeComputerId
                                        FROM Employee e
                                        WHERE e.ComputerId = c.Id)";
                    var reader = cmd.ExecuteReader();
                    var computers = new List<Computer>();

                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId")))
                        {
                            int computerId = reader.GetInt32(reader.GetOrdinal("ComputerId"));
                            if (!computers.Any(c => c.Id == computerId))
                            {
                                Computer computer = new Computer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Model = reader.GetString(reader.GetOrdinal("Model"))
                                };
                                computers.Add(computer);
                            }
                        }
                    }

                    reader.Close();
                    return computers;
                }
            }
        }
    }
}
