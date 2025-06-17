namespace IGAnalyzer;

using Hl7.Fhir.Model;

public class FHIRData
{
    public string? Patient { get; set; }
    public List<Tuple<string,string>> OrganizationList { get; set; }
    public List<Tuple<string,string>> PractitionerList { get; set; }


    public FHIRData()
    {
        PractitionerList = new List<Tuple<string, string>>();
        OrganizationList = new List<Tuple<string, string>>();
    }

    public void AddPatient(string patient)
    {
        Patient = patient;
    }

    public void AddOrganization(string role,string organization)
    {
        OrganizationList.Add(new Tuple<string, string>(role, organization));
    }

    public void AddPractitioner(string role, string practitioner)
    {
        PractitionerList.Add(new Tuple<string, string>(role, practitioner));
    }

}
