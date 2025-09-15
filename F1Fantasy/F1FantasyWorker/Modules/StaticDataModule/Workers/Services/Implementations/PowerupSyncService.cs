using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Implementations;

public class PowerupSyncService : IPowerupSyncService
{
    public async Task<List<PowerupDto>> GetPowerupsFromStaticResourcesAsync()
    {
        var result = new List<PowerupDto>();
        var filePath = Path.Combine(AppContext.BaseDirectory, "Static", "powerups.csv");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"powerups.csv not found at {filePath}");

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
            if (fields.Count < 4)
                continue;

            var id = Int32.Parse(fields[0].Trim());
            var type = fields[1].Trim();
            var description = fields[2].Trim();
            var imgUrl = fields[3].Trim();
                

            if (!string.IsNullOrEmpty(type) &&
                !string.IsNullOrEmpty(description) &&
                !string.IsNullOrEmpty(imgUrl))
                result.Add(new PowerupDto(id, type, description, imgUrl));
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