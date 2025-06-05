namespace IGAnalyzer;

using Hl7.Fhir.Model;

public class FHIRData
{
    public List<string> Patients { get; set; }
    public List<Tuple<string,string>> OrganizationList { get; set; }
    public List<Tuple<string,string>> PractitionerList { get; set; }

    public FHIRData()
    {
        Patients = new List<string>();
        PractitionerList = new List<Tuple<string, string>>();
        OrganizationList = new List<Tuple<string, string>>();
    }

    public void AddPatient(string patient)
    {
        Patients.Add(patient);
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
