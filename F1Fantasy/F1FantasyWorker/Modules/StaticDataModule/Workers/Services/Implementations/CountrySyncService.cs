using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Implementations;

public class CountrySyncService : ICountrySyncService
{
    public async Task<List<CountryDto>> GetCountriesFromStaticResourcesAsync()
    {
        var result = new List<CountryDto>();
        var filePath = Path.Combine(AppContext.BaseDirectory, "Static", "countries.csv");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"countries.csv not found at {filePath}");

        using var reader = new StreamReader(filePath);
        string? line;
        bool isFirstLine = true;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (isFirstLine)
            {
                isFirstLine = false; // skip header
                continue;
            }

            // Use a simple CSV parser to handle quoted fields with commas
            var fields = ParseCsvLine(line);
            if (fields.Count < 5)
                continue;

            var alpha3 = fields[2].Trim();
            var shortName = fields[3].Trim();
            var nationalityField = fields[4].Trim();

            // Remove surrounding quotes if present
            if (nationalityField.StartsWith("\"") && nationalityField.EndsWith("\""))
                nationalityField = nationalityField[1..^1];

            // Split by comma, then by "or", and trim each name
            var nationalities = nationalityField
                .Split(',')
                .SelectMany(n => n.Split(new[] { " or " }, StringSplitOptions.RemoveEmptyEntries))
                .Select(n => n.Trim())
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();

            if (!string.IsNullOrEmpty(alpha3) && nationalities.Count > 0)
                result.Add(new CountryDto(alpha3, nationalities, shortName));
        }

        return result;
    }
    
    private static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        bool inQuotes = false;
        var field = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '\"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(field.ToString());
                field.Clear();
            }
            else
            {
                field.Append(c);
            }
        }
        fields.Add(field.ToString());
        return fields;
    }

}