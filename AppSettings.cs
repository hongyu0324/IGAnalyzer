namespace IGAnalyzer;

using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

public class AppSettings
{

    [JsonPropertyName("DataDirectory")]
    public string? DataDirectory { get; set; }

    [JsonPropertyName("FHIRServer")]
    public string? FHIRServer { get; set; }

    [JsonPropertyName("IGName")]
    public string? IGName { get; set; }

    [JsonPropertyName("ICD10PCSVersion")]
    public string? ICD10PCSVersion { get; set; }

    [JsonPropertyName("MasterData")]
    public List<MasterDataItem> MasterData { get; set; } = new List<MasterDataItem>();

    [JsonPropertyName("BindingsAdd")]
    public List<BindingAddItem> BindingsAdd { get; set; } = new List<BindingAddItem>();


    [JsonPropertyName("LogicModelAdd")]
    public List<LogicModelAddItem> LogicModelAdd { get; set; } = new List<LogicModelAddItem>();

    [JsonPropertyName("PathUpdate")]
    public List<PathUpdateItem> PathUpdate { get; set; } = new List<PathUpdateItem>();

    [JsonPropertyName("BindingIgnore")]
    public List<string> BindingIgnore { get; set; } = new List<string>();

    [JsonPropertyName("Constraints")]
    public List<Constraint> Constraints { get; set; } = new List<Constraint>();

    


    public class MasterDataItem
    {
        [JsonPropertyName("Name")]
        public string? Name { get; set; }

        [JsonPropertyName("ResourceType")]
        public string? ResourceType { get; set; }

        [JsonPropertyName("Role")]
        public string? Role { get; set; }
    }

    public class BindingAddItem
    {
        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("valueSet")]
        public string? ValueSet { get; set; }
    }

    public class PathUpdateItem
    {
        [JsonPropertyName("before")]
        public string? Before { get; set; }

        [JsonPropertyName("after")]
        public string? After { get; set; }
    }

    public class LogicModelAddItem
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("profile")]
        public string? Profile { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }


    public AppSettings()
    {
        DataDirectory = null;
        FHIRServer = null;
        IGName = null;
    }

    public class Constraint
    {
        [JsonPropertyName("profileName")]
        public string? ProfileName { get; set; }

        [JsonPropertyName("profileType")]
        public string? ProfileType { get; set; }

        [JsonPropertyName("implySource")]
        public string? ImplySource { get; set; }

        [JsonPropertyName("implyTarget")]
        public string? ImplyTarget { get; set; }

        [JsonPropertyName("rule")]
        public List<Rule> Rules { get; set; } = new List<Rule>();
    }

    public class Rule
    {
        [JsonPropertyName("sourceValue")]
        public string? SourceValue { get; set; }

        // Note: JSON key is "TargetType" with a capital 'T'
        [JsonPropertyName("targetType")]
        public string? TargetType { get; set; }

        [JsonPropertyName("targetValue")]
        public object? TargetValue { get; set; } // Will be JsonElement after initial deserialization
    }

    // Represents the targetValue structure when TargetType is "integer"
    public class TargetValueRange
    {
        // Note: JSON key is "Max" with a capital 'M'
        [JsonPropertyName("Max")]
        public int Max { get; set; }

        // Note: JSON key is "Min" with a capital 'M'
        [JsonPropertyName("Min")]
        public int Min { get; set; }
    }

    public string GetTargetString(string profileName, string implySource, string sourceValue)
    {
        string targetValue = string.Empty;
        foreach (var constraint in Constraints)
        {
            if (constraint.ProfileName == profileName)
            {
                foreach (var rule in constraint.Rules)
                {
                    if (rule.SourceValue == sourceValue && rule.TargetType == "string")
                    {
                        targetValue = rule.TargetValue?.ToString() ?? string.Empty;
                        if (targetValue != string.Empty)
                        {
                            targetValue = targetValue.Replace("[", "").Replace("]", "");
                            List<string> values = targetValue.Split(',').Select(v => v.Trim()).ToList();
                            targetValue = string.Join(", ", values);
                            targetValue = targetValue.Replace("\"", "");
                        }
                    }
                    else if (rule.SourceValue == sourceValue && rule.TargetType == "integer")
                    {
                        if (rule.TargetValue is JsonElement jsonElement)
                        {
                            // Deserialize the JSON element to TargetValueRange
                            var range = JsonSerializer.Deserialize<TargetValueRange>(jsonElement.GetRawText());
                            if (range != null)
                            {
                                for(int i = range.Min; i <= range.Max; i++)
                                {
                                    targetValue += i.ToString() + ",";
                                }
                                targetValue = targetValue.TrimEnd(',').Trim();
                            }
                        }
                    }
                }
            }
        }
        
        return targetValue;
    }

    public void Load()
    {
        string configFile = "Application.json";
        if (File.Exists(configFile))
        {
            string json = File.ReadAllText(configFile);
            AppSettings? config = JsonSerializer.Deserialize<AppSettings>(json);
            if (config != null)
            {
                DataDirectory = config.DataDirectory;
                FHIRServer = config.FHIRServer;
                IGName = config.IGName;
                MasterData = config.MasterData;
                BindingsAdd = config.BindingsAdd;
                PathUpdate = config.PathUpdate;
                Constraints = config.Constraints;
                LogicModelAdd = config.LogicModelAdd;
                BindingIgnore = config.BindingIgnore;
            }
        }
        else
        {
            throw new FileNotFoundException($"Configuration file '{configFile}' not found.");
        }
    }

}