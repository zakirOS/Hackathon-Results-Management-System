using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonData.Entities
{
    public class Project
    {
        public int Id { get; set; }

        public string TeamName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public decimal Score { get; set; }

        public int Members { get; set; }

        public string Captain { get; set; } = string.Empty;
    }
}

