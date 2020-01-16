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
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, d.[Name]
                                        FROM Employee e
                                        LEFT JOIN Department d ON e.DepartmentId = d.Id";


                    var reader = cmd.ExecuteReader();
                    var employees = new List<Employee>();
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
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
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id AS EmployeeId, e.FirstName, e.LastName, 
                                        d.Id AS DepartmentId, d.[Name] AS DepartmentName, e.ComputerId, e.Email,
                                        t.Id AS TrainingProgramId, t.[Name] AS TrainingProgramName,
                                        et.Id AS EmployeeTrainingId
                                       FROM Employee e
                                       LEFT JOIN Department d ON e.DepartmentId = d.Id
                                       LEFT JOIN Computer c ON e.ComputerId = c.Id
                                       LEFT JOIN EmployeeTraining et ON e.Id = et.EmployeeId
                                       LEFT JOIN TrainingProgram t ON t.Id = et.TrainingProgramId
                                       WHERE e.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    Employee employee = null;
                    while (reader.Read())

                    {
                        if (employee == null)
                        {
                            employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                TrainingProgram = new List<TrainingProgram>(),
                                Department = new Department(),
                                Computer = new Computer()

                            };
                        }
                        if (employee.Department != null)
                        {
                            employee.Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName"))
                            };
                        }
                        if (employee.Computer != null)
                        {
                            employee.Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),

                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("TrainingProgramId")))
                        {
                            int trainingProgramId = reader.GetInt32(reader.GetOrdinal("TrainingProgramId"));
                            if (!employee.TrainingProgram.Any(t => t.Id == trainingProgramId))
                            {
                                TrainingProgram trainingProgram = new TrainingProgram
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId")),
                                    Name = reader.GetString(reader.GetOrdinal("TrainingProgramName"))
                                };
                            employee.TrainingProgram.Add(trainingProgram);
                            }
                        }
                    };
                    reader.Close();
                    if (employee == null)
                    {
                        reader.Close();
                        return NotFound();
                    }
                    return View(employee);
                }

            }
        }


        // GET: Employees/Create
        public ActionResult Create()
        {
            var departments = GetDepartments().Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString()
            }).ToList();
            var viewModel = new EmployeeViewModel
            {
                Employee = new Employee(),
                Departments = departments
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
                        cmd.Parameters.Add(new SqlParameter("@email", ""));

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
    }
}
