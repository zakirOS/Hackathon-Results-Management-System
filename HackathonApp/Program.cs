using HackathonData.Data;
using HackathonData.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string connectionString = configuration.GetConnectionString("HackathonDB");
        string xmlPath = configuration["Paths:XmlInput"];
        string outputDir = configuration["Paths:JsonOutput"];

        var optionsBuilder = new DbContextOptionsBuilder<HackathonContext>();
        optionsBuilder.UseSqlServer(connectionString);

        using var context = new HackathonContext(optionsBuilder.Options);

        var importService = new ImportService(context);
        var queryService = new LinqQueryService(context);

        importService.DataImported += (inserted, updated, skipped, duration) =>
        {
            Console.WriteLine("=== Import Finished ===");
            Console.WriteLine($"Inserted: {inserted}");
            Console.WriteLine($"Updated : {updated}");
            Console.WriteLine($"Skipped : {skipped}");
            Console.WriteLine($"Duration: {duration.TotalSeconds:F2}s");
            Console.WriteLine("=======================");
        };

        bool exit = false;

        while (!exit)
        {
            Console.WriteLine();
            Console.WriteLine("Hackathon Results Management System");
            Console.WriteLine("1) Import XML to DB");
            Console.WriteLine("2) Run Simple Queries (1–5)");
            Console.WriteLine("3) Run Medium Queries (6–10)");
            Console.WriteLine("4) Run Complex Queries (11–15)");
            Console.WriteLine("5) Export All Queries to JSON");
            Console.WriteLine("6) Exit");
            Console.Write("Choose option: ");

            var key = Console.ReadKey();
            Console.WriteLine();

            switch (key.KeyChar)
            {
                case '1':
                    await importService.ImportFromXmlAsync(xmlPath);
                    break;

                case '2':
                    await RunSimpleQueries(queryService);
                    break;

                case '3':
                    await RunMediumQueries(queryService);
                    break;

                case '4':
                    await RunComplexQueries(queryService);
                    break;

                case '5':
                    await ExportAllQueries(queryService, outputDir);
                    break;

                case '6':
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    static async Task RunSimpleQueries(LinqQueryService s)
    {
        var q1 = await s.Q01_ProjectsByNeuralNovaAsync();
        Console.WriteLine($"Q1 count = {q1.Count}");

        var q2 = await s.Q02_ProjectsOn2025_10_12Async();
        Console.WriteLine($"Q2 count = {q2.Count}");

        var q3 = await s.Q03_ProjectsInAiMlAsync();
        Console.WriteLine($"Q3 count = {q3.Count}");

        var q4 = await s.Q04_ScoreGreater90Async();
        Console.WriteLine($"Q4 count = {q4.Count}");

        var q5 = await s.Q05_Top5OverallAsync();
        Console.WriteLine("Q5 Top 5:");
        foreach (var p in q5)
            Console.WriteLine($"{p.ProjectName} ({p.Score})");
    }

    static async Task RunMediumQueries(LinqQueryService s)
    {
        Console.WriteLine($"Q6 count = {(await s.Q06_ProjectsIn2024Async()).Count}");
        Console.WriteLine($"Q7 count = {(await s.Q07_HealthTechScoreGreater88Async()).Count}");
        Console.WriteLine($"Q8 count = {(await s.Q08_SortedByDateThenScoreAsync()).Count}");

        var q9 = await s.Q09_CountPerCategoryAsync();
        Console.WriteLine("Q9:");
        foreach (var (cat, count) in q9)
            Console.WriteLine($"{cat}: {count}");

        var q10 = await s.Q10_Top3ByByteForgeAsync();
        Console.WriteLine("Q10 Top 3:");
        foreach (var p in q10)
            Console.WriteLine($"{p.ProjectName} ({p.Score})");
    }

    static async Task RunComplexQueries(LinqQueryService s)
    {
        var q11 = await s.Q11_AverageScorePerCategoryAsync();
        Console.WriteLine("Q11:");
        foreach (var x in q11)
            Console.WriteLine($"{x.Category}: {x.AverageScore:F2}");

        var q12 = await s.Q12_SmartCityOrEnergyAboveCategoryAverageAsync();
        Console.WriteLine($"Q12 count = {q12.Count}");

        var q13 = await s.Q13_ProjectNameContainsAiScoreGreater92Async();
        Console.WriteLine($"Q13 count = {q13.Count}");

        var q14 = await s.Q14_Top5PerCategoryAsync();
        Console.WriteLine("Q14:");
        foreach (var block in q14)
        {
            Console.WriteLine(block.Category);
            foreach (var p in block.TopProjects)
                Console.WriteLine($"  {p.ProjectName} ({p.Score})");
        }

        var q15 = await s.Q15_MembersAtLeast5AboveGlobalAverageAsync();
        Console.WriteLine($"Q15 count = {q15.Count}");
    }

    static async Task ExportAllQueries(LinqQueryService s, string outputDir)
    {
        Directory.CreateDirectory(outputDir);

        await File.WriteAllTextAsync(
            Path.Combine(outputDir, "q01.json"),
            System.Text.Json.JsonSerializer.Serialize(await s.Q01_ProjectsByNeuralNovaAsync(), new System.Text.Json.JsonSerializerOptions { WriteIndented = true })
        );

        await File.WriteAllTextAsync(
            Path.Combine(outputDir, "q02.json"),
            System.Text.Json.JsonSerializer.Serialize(await s.Q02_ProjectsOn2025_10_12Async(), new System.Text.Json.JsonSerializerOptions { WriteIndented = true })
        );

        await File.WriteAllTextAsync(
            Path.Combine(outputDir, "q03.json"),
            System.Text.Json.JsonSerializer.Serialize(await s.Q03_ProjectsInAiMlAsync(), new System.Text.Json.JsonSerializerOptions { WriteIndented = true })
        );

        await File.WriteAllTextAsync(
            Path.Combine(outputDir, "q04.json"),
            System.Text.Json.JsonSerializer.Serialize(await s.Q04_ScoreGreater90Async(), new System.Text.Json.JsonSerializerOptions { WriteIndented = true })
        );

        await File.WriteAllTextAsync(
            Path.Combine(outputDir, "q05.json"),
            System.Text.Json.JsonSerializer.Serialize(await s.Q05_Top5OverallAsync(), new System.Text.Json.JsonSerializerOptions { WriteIndented = true })
        );

        
        Console.WriteLine("All JSON files exported.");
    }
}

