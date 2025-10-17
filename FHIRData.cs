namespace IGAnalyzer;

using Hl7.Fhir.Model;

public class FHIRData
{
    public string? Patient { get; set; }

    public List<IGDataItem> IGDataList { get; set; } = new List<IGDataItem>();

    public class IGDataItem
    {
        public string Role { get; set; }
        public string Profile { get; set; }
        public string Reference { get; set; }

        public IGDataItem(string role, string profile, string reference)
        {
            Role = role;
            Profile = profile;
            Reference = reference;
        }
    }

    public FHIRData()
    {


    }

    public void LoadIGDataFromFile(string filePath)
    {
        // read the Json string from a file
        try
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file {filePath} does not exist.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error reading the file: {ex.Message}");
        }
        var igDataJson = System.IO.File.ReadAllText(filePath);
        try
        {
            // deserialize the Json string to a list of IGDataItem
            IGDataList = System.Text.Json.JsonSerializer.Deserialize<List<IGDataItem>>(igDataJson) ?? new List<IGDataItem>();
        }
        catch (System.Text.Json.JsonException ex)
        {
            MessageBox.Show($"Error deserializing the file: {ex.Message}");
        }
    }
    


    public void SaveIGDataToFile(string filePath)
    {
        // transfer the IGDataList to a Json string
        var igDataJson = System.Text.Json.JsonSerializer.Serialize(IGDataList);
        // write the Json string to a file
        System.IO.File.WriteAllText(filePath, igDataJson);
    }

    public void AddPatient(string patient)
    {
        Patient = patient;
    }

    public void AddIGData(string role, string profile, string reference)
    {
        IGDataList.Add(new IGDataItem(role, profile, reference));
    }

    public void Clear()
    {
        Patient = null;
        IGDataList.Clear();
    }
}
