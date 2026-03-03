using System;
using HackathonData.Data;
using HackathonData.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonData.Services
{
    public class LinqQueryService
    {
        private readonly HackathonContext _context;

        public LinqQueryService(HackathonContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> Q01_ProjectsByNeuralNovaAsync()
        {
            return await _context.Projects
                .Where(p => p.TeamName == "NeuralNova")
                .ToListAsync();
        }

        public async Task<List<Project>> Q02_ProjectsOn2025_10_12Async()
        {
            var target = new DateTime(2025, 10, 12);

            return await (from p in _context.Projects
                          where p.EventDate == target
                          select p).ToListAsync();
        }

        public async Task<List<Project>> Q03_ProjectsInAiMlAsync()
        {
            return await _context.Projects
                .Where(p => p.Category == "AI-ML")
                .ToListAsync();
        }

        public async Task<List<Project>> Q04_ScoreGreater90Async()
        {
            return await _context.Projects
                .Where(p => p.Score > 90m)
                .ToListAsync();
        }

        public async Task<List<Project>> Q05_Top5OverallAsync()
        {
            return await _context.Projects
                .OrderByDescending(p => p.Score)
                .ThenBy(p => p.Id)
                .Take(5)
                .ToListAsync();
        }

        public async Task<List<Project>> Q06_ProjectsIn2024Async()
        {
            var from = new DateTime(2024, 1, 1);
            var to = new DateTime(2024, 12, 31);

            return await _context.Projects
                .Where(p => p.EventDate >= from && p.EventDate <= to)
                .ToListAsync();
        }

        public async Task<List<Project>> Q07_HealthTechScoreGreater88Async()
        {
            return await _context.Projects
                .Where(p => p.Category == "HealthTech" && p.Score > 88m)
                .ToListAsync();
        }

        public async Task<List<Project>> Q08_SortedByDateThenScoreAsync()
        {
            return await _context.Projects
                .OrderBy(p => p.EventDate)
                .ThenByDescending(p => p.Score)
                .ToListAsync();
        }

        public async Task<List<(string Category, int Count)>> Q09_CountPerCategoryAsync()
        {
            var list = await _context.Projects
                .GroupBy(p => p.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderBy(x => x.Category)
                .ToListAsync();

            return list
                .Select(x => (x.Category, x.Count))
                .ToList();
        }


        public async Task<List<Project>> Q10_Top3ByByteForgeAsync()
        {
            return await _context.Projects
                .Where(p => p.TeamName == "ByteForge")
                .OrderByDescending(p => p.Score)
                .Take(3)
                .ToListAsync();
        }

        public async Task<List<CategoryAverageDto>> Q11_AverageScorePerCategoryAsync()
        {
            return await _context.Projects
                .GroupBy(p => p.Category)
                .Select(g => new CategoryAverageDto
                {
                    Category = g.Key,
                    AverageScore = g.Average(p => p.Score)
                })
                .OrderBy(x => x.Category)
                .ToListAsync();
        }

        public async Task<List<Project>> Q12_SmartCityOrEnergyAboveCategoryAverageAsync()
        {
            var averages = await Q11_AverageScorePerCategoryAsync();
            var avgDict = averages.ToDictionary(a => a.Category, a => a.AverageScore);

            string[] categories = { "SmartCity", "Energy" };

            var projectsInCategories = await _context.Projects
                .Where(p => categories.Contains(p.Category))
                .ToListAsync();

            return projectsInCategories
                .Where(p => avgDict.TryGetValue(p.Category, out var avg) && p.Score >= avg)
                .ToList();
        }


        public async Task<List<Project>> Q13_ProjectNameContainsAiScoreGreater92Async()
        {
            return await _context.Projects
                .Where(p =>
                    p.Score > 92m &&
                    EF.Functions.Like(p.ProjectName, "%AI%"))
                .ToListAsync();
        }


        public async Task<List<CategoryTop5Dto>> Q14_Top5PerCategoryAsync()
        {
            var groups = await _context.Projects
                .GroupBy(p => p.Category)
                .ToListAsync();

            var result = new List<CategoryTop5Dto>();

            foreach (var g in groups)
            {
                var top5 = g
                    .OrderByDescending(p => p.Score)
                    .ThenBy(p => p.Id)
                    .Take(5)
                    .Select(p => new ProjectBriefDto
                    {
                        Id = p.Id,
                        TeamName = p.TeamName,
                        ProjectName = p.ProjectName,
                        Score = p.Score
                    })
                    .ToList();

                result.Add(new CategoryTop5Dto
                {
                    Category = g.Key,
                    TopProjects = top5
                });
            }

            return result;
        }

        public async Task<List<Project>> Q15_MembersAtLeast5AboveGlobalAverageAsync()
        {
            var averageScore = await _context.Projects.AverageAsync(p => p.Score);

            return await _context.Projects
                .Where(p => p.Members >= 5 && p.Score > averageScore)
                .ToListAsync();
        }
    }
}

