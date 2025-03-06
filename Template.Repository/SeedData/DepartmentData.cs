using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Domain.Entities;

namespace Template.Utilities.SeedData
{
   public class DepartmentData
    {
        public static HashSet<Department> LoadDepartments()
        {
            return new HashSet<Department>
            {
                new Department { Name = "HR" },
                new Department { Name = "IT" }
            };
        }
    }
}
