using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using HackathonData.Data;
using HackathonData.Entities;
using Microsoft.EntityFrameworkCore;

namespace HackathonData.Services
{
    public delegate void DataImportedHandler(int inserted, int updated, int skipped, TimeSpan duration);

    public class ImportService
    {
        private readonly HackathonContext _context;

        public ImportService(HackathonContext context)
        {
            _context = context;
        }

        public event DataImportedHandler? DataImported;

        public async Task ImportFromXmlAsync(string xmlPath)
        {
            var sw = Stopwatch.StartNew();
            int inserted = 0;
            int updated = 0;
            int skipped = 0;
            var now = DateTime.Today;

            if (!System.IO.File.Exists(xmlPath))
            {
                Console.WriteLine($"XML file not found: {xmlPath}");
                return;
            }

            try
            {
                var doc = XDocument.Load(xmlPath);
                var projectElements = doc.Root?.Elements("Project") ?? Enumerable.Empty<XElement>();

                foreach (var elem in projectElements)
                {
                    try
                    {
                        int id = (int)elem.Element("Id")!;
                        string teamName = (string?)elem.Element("TeamName") ?? string.Empty;
                        string projectName = (string?)elem.Element("ProjectName") ?? string.Empty;
                        string category = (string?)elem.Element("Category") ?? string.Empty;
                        DateTime eventDate = (DateTime)elem.Element("EventDate")!;
                        decimal score = (decimal)elem.Element("Score")!;
                        int members = (int)elem.Element("Members")!;
                        string captain = (string?)elem.Element("Captain") ?? string.Empty;

                        if (id <= 0 ||
                            string.IsNullOrWhiteSpace(teamName) ||
                            string.IsNullOrWhiteSpace(projectName) ||
                            string.IsNullOrWhiteSpace(category) ||
                            string.IsNullOrWhiteSpace(captain) ||
                            members < 1 || members > 15 ||
                            score < 0m || score > 100m ||
                            eventDate > now)
                        {
                            skipped++;
                            Console.WriteLine($"Skipped invalid record with Id={id}.");
                            continue;
                        }

                        var existing = await _context.Projects
                            .SingleOrDefaultAsync(p => p.Id == id);

                        if (existing == null)
                        {
                            var project = new Project
                            {
                                Id = id,
                                TeamName = teamName,
                                ProjectName = projectName,
                                Category = category,
                                EventDate = eventDate,
                                Score = score,
                                Members = members,
                                Captain = captain
                            };

                            _context.Projects.Add(project);
                            inserted++;
                        }
                        else
                        {
                            existing.TeamName = teamName;
                            existing.ProjectName = projectName;
                            existing.Category = category;
                            existing.EventDate = eventDate;
                            existing.Score = score;
                            existing.Members = members;
                            existing.Captain = captain;

                            updated++;
                        }
                    }
                    catch (Exception ex)
                    {
                        skipped++;
                        Console.WriteLine("Error parsing a record: " + ex.Message);
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Import failed: " + ex.Message);

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine("Inner: " + ex.InnerException.Message);
                    }
                    else
                    {
                        Console.WriteLine("Full exception:");
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected import error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner: " + ex.InnerException.Message);
                }
            }

            sw.Stop();
            DataImported?.Invoke(inserted, updated, skipped, sw.Elapsed);
        }
    }
}

