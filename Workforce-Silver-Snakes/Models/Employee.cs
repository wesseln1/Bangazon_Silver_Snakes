using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Workforce_Silver_Snakes.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Email { get; set; }
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        [Display(Name = "Supervisor")]
        public bool IsSupervisor { get; set; }
        [Display(Name = "Computer")]
        public int ComputerId { get; set; }
      
        public Computer Computer { get; set; }
        public Department Department { get; set; }
        public List<String> TrainingPrograms { get; set; } = new List<String>();
        public List<TrainingSelect> TrainingList { get; set; } = new List<TrainingSelect>();


    }
}
