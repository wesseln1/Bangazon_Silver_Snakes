﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Workforce_Silver_Snakes.Models
{
    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Budget { get; set; }

        [Display(Name = "Employee Count")]
        public int EmployeeCount { get; set; }
        
        public List<Employee> Employees { get; set; }
        public Employee Employee { get; set; }
    }
}
