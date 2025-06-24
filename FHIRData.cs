namespace IGAnalyzer;

using Hl7.Fhir.Model;

public class FHIRData
{
    public string? Patient { get; set; }

    public List<Tuple<string,string, string>> IGData { get; set; } = new List<Tuple<string,string, string>>();


    public FHIRData()
    {
        
        
    }

    public void AddPatient(string patient)
    {
        Patient = patient;
    }


    
    public void AddIGData(string role,string profile, string reference)
    {
        IGData.Add(new Tuple<string, string, string>(role, profile, reference));
    }

}
