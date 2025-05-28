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

    [JsonPropertyName("MasterData")]
    public List<MasterDataItem> MasterData { get; set; } = new List<MasterDataItem>();

    [JsonPropertyName("BindingsAdd")]
    public List<BindingAddItem> BindingsAdd { get; set; } = new List<BindingAddItem>();

    [JsonPropertyName("PathUpdate")]
    public List<PathUpdateItem> PathUpdate { get; set; } = new List<PathUpdateItem>();

    [JsonPropertyName("Constraints")]
    public List<Constraint> Constraints { get; set; } = new List<Constraint>();


    public class MasterDataItem
    {
        [JsonPropertyName("Name")]
        public string? Name { get; set; }

        [JsonPropertyName("ResourceType")]
        public string? ResourceType { get; set; }
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
        [JsonPropertyName("TargetType")]
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
            }
        }
        else
        {
            throw new FileNotFoundException($"Configuration file '{configFile}' not found.");
        }
    }

}