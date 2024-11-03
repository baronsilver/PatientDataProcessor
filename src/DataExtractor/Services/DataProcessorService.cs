using System.Text.RegularExpressions;
using System.Text.Json;
using DataExtractor.Models;

namespace DataExtractor.Services;

public interface IDataProcessorService
{
    IEnumerable<Patient> ProcessData(string firstFormat, string secondFormat);
}

public class DataProcessorService : IDataProcessorService
{
    private static string CleanNHSNumber(string nhsNumber)
    {
        return new string(nhsNumber.Where(char.IsDigit).ToArray());
    }

    public IEnumerable<Patient> ProcessData(string firstFormat, string secondFormat)
    {
        var patients1 = ProcessFirstFormat(firstFormat);
        var patients2 = ProcessSecondFormat(secondFormat);

        return patients1.Concat(patients2)
            .GroupBy(p => p.NHSNumber)
            .Select(g => g.First())
            .OrderBy(p => p.Name)
            .ToList();
    }

    private List<Patient> ProcessFirstFormat(string input)
    {
        var patients = new List<Patient>();
        
        var normalizedText = Regex.Replace(input, @"\[\[(?i)new-line\]\]", "\n");

        // Extract Name: followed by NHS Number: pairs
        var nameNhsPattern = @"Name:\s*(\w+(?:\s+\w+)?)\s+NHS\s*(?:Number|NUmber):\s*([^\s\n]+)";
        var nameNhsMatches = Regex.Matches(normalizedText, nameNhsPattern);
        foreach (Match match in nameNhsMatches)
        {
            patients.Add(new Patient
            {
                Name = match.Groups[1].Value.Trim(),
                NHSNumber = CleanNHSNumber(match.Groups[2].Value.Trim())
            });
        }

        // Process NHS Number: followed by name pattern
        var nhsWithNamePattern = @"NHS\s*Number:\s*(\d+)\s+(\w+(?:\s+\w+)?)(?=\s*(?:\[\[|NHS|\n|$))";
        var nhsWithNameMatches = Regex.Matches(normalizedText, nhsWithNamePattern);
        foreach (Match match in nhsWithNameMatches)
        {
            patients.Add(new Patient
            {
                Name = match.Groups[2].Value.Trim(),
                NHSNumber = CleanNHSNumber(match.Groups[1].Value.Trim())
            });
        }

        // Process NHS Numbers that are alone
        var nhsAlonePattern = @"NHS\s*Number:\s*(\d+)";
        var nhsAloneMatches = Regex.Matches(normalizedText, nhsAlonePattern);
        foreach (Match match in nhsAloneMatches)
        {
            patients.Add(new Patient
            {
                Name = "Unknown",
                NHSNumber = CleanNHSNumber(match.Groups[1].Value.Trim())
            });
        }
        
        return patients;
    }

    private List<Patient> ProcessSecondFormat(string input)
    {
        try
        {
            input = input.Replace("{[", "[").Replace("]}", "]");
            var jsonPatients = JsonSerializer.Deserialize<List<JsonPatient>>(input);
            
            return jsonPatients?.Select(jp => new Patient
            {
                Name = string.IsNullOrWhiteSpace(jp.Name) ? "Unknown" : jp.Name,
                NHSNumber = CleanNHSNumber(jp.NHSNumber.ToString())
            }).ToList() ?? new List<Patient>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON format: {ex.Message}");
            return new List<Patient>();
        }
    }
}