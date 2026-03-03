using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HackathonData.Entities;
using System.Collections.Generic;

namespace HackathonData.Services
{
    
    public class CategoryAverageDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal AverageScore { get; set; }
    }

    
    public class CategoryTop5Dto
    {
        public string Category { get; set; } = string.Empty;
        public List<ProjectBriefDto> TopProjects { get; set; } = new();
    }

    
    public class ProjectBriefDto
    {
        public int Id { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public decimal Score { get; set; }
    }
}

