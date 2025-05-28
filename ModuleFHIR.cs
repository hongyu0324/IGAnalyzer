namespace ModuleFHIR;

using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Firely.Fhir.Packages;
using System.Drawing.Text;
using Hl7.Fhir.ElementModel;

public class FHIROperator
{
    
    Dictionary<string, string> urlMap = new Dictionary<string, string>();
    // Constructor to initialize the FHIR operator
    public FHIROperator()
    {
        // Initialize any necessary components or variables here

    }
    // Function to load FHIR IG Package
    public void LoadIGPackage(string igPath)
    {
        // Load the IG package from the specified path
        // Implement the logic to load and parse the IG package here
    }

    // Function to validate FHIR resources
    public void ValidateResources()
    {
        // Implement the logic to validate FHIR resources here
    }

    // Function to display validation results
    public void DisplayResults()
    {
        // Implement the logic to display validation results here
    }

    
    public String ShowMessage(String input){
        string igName = "pas";
        string profilePath = "..\\..\\data\\profiles\\" + igName;
        string profileName = "https://twcore.mohw.gov.tw/ig/" + igName + "/StructureDefinition";
        MessageBox.Show(profileName);
        return profileName;
    }

    public string GenerateQuestionnaire(string title,List<Tuple<string, string, string, string,string>> qList)
    {
        string questionnaire_Json = "";
        // Create a new Questionnaire resource
        var questionnaire = new Questionnaire();

        questionnaire.Name = "Observation Diagnostic TWPAS";
        questionnaire.Title = "Observation Diagnostic TWPAS";
        questionnaire.Status = PublicationStatus.Active;
        questionnaire.Publisher = "大同醫護股份有限公司";

        questionnaire.Description =  title + "問卷";

        questionnaire.Contact = new List<ContactDetail>();
        var contact = new ContactDetail();
        contact.Name = "大同醫護股份有限公司";
        contact.Telecom = new List<ContactPoint>();
        var telecom = new ContactPoint();
        telecom.System = ContactPoint.ContactPointSystem.Url;
        telecom.Value = "https://tmhtc.net/";
        contact.Telecom.Add(telecom);
        questionnaire.Contact.Add(contact);

        var item1 = new Questionnaire.ItemComponent();
        item1.LinkId = title;
        item1.Text = title;
        item1.Type = Questionnaire.QuestionnaireItemType.Group;
        item1.Item = new List<Questionnaire.ItemComponent>();
        questionnaire.Item.Add(item1);

        string itemText = "";
        foreach (var q in qList){

            var item = new Questionnaire.ItemComponent();
            if(itemText != q.Item1){
                itemText = q.Item1;
            }else{ 
                if (item1.Item.Count > 0){
                    item = item1.Item[item1.Item.Count - 1];
                    if(q.Item4 == "CodeableConcept" || q.Item4 == "code"){
                        item.Type = Questionnaire.QuestionnaireItemType.Choice;
                    }
                    if (item.Type == Questionnaire.QuestionnaireItemType.Choice && q.Item5 != string.Empty && item.Text == q.Item1){
                        item.LinkId = q.Item2 + "." + q.Item3;
                        item.AnswerValueSet = q.Item5;
                        item1.Item[item1.Item.Count - 1] = item;
                    } 
                }
                continue;
            }

            //item.LinkId = "1." + (item1.Item.Count + 1).ToString();
            item.LinkId =  q.Item2 +"." + q.Item3;
            item.Text = q.Item1;

            // 根據item4內容，判斷Type
            if(q.Item4 == "BackboneElement"){
                continue;
            }
            else if (q.Item4 == "decimal"){
                item.Type = Questionnaire.QuestionnaireItemType.Decimal;
            }
            else if (q.Item4 == "date"){
                item.Type = Questionnaire.QuestionnaireItemType.Date;
            }
            else if (q.Item4 == "time"){
                item.Type = Questionnaire.QuestionnaireItemType.Time;
            }
            else

            if (q.Item4 == "string"){
                item.Type = Questionnaire.QuestionnaireItemType.String;
            }
            else if (q.Item4 == "dateTime"){
                item.Type = Questionnaire.QuestionnaireItemType.DateTime;
            }
            else if (q.Item4 == "boolean"){
                item.Type = Questionnaire.QuestionnaireItemType.Boolean;
            }
            else if (q.Item4 == "integer"){
                item.Type = Questionnaire.QuestionnaireItemType.Integer;
            }
            else if (q.Item4 == "decimal"){
                item.Type = Questionnaire.QuestionnaireItemType.Decimal;
            }
            else if (q.Item4 == "Attachment"){
                item.Type = Questionnaire.QuestionnaireItemType.Attachment;
            }
            else if (q.Item4 == "CodeableConcept" || q.Item4 == "code"){
                item.Type = Questionnaire.QuestionnaireItemType.Choice;  
                if(q.Item5 != string.Empty) item.AnswerValueSet = q.Item5;
            }
            else{
                item.Type = Questionnaire.QuestionnaireItemType.String;
            }
            item.Required = true;
            item1.Item.Add(item);
        }
        FhirJsonSerializer serializer = new FhirJsonSerializer(new SerializerSettings()
        {
            Pretty = true,
        });
        questionnaire_Json = serializer.SerializeToString(questionnaire);
        return questionnaire_Json; // Ensure a return value in all code paths
    }

    public List<Tuple<string,string, string>> ParsingValueSet(ValueSet valueSet,string type)
    {
        List<Tuple<string, string, string>> listResult = new List<Tuple<string, string, string>>();
        Tuple<string, string, string> result = new Tuple<string, string, string>("", "", "");

        if (type == "expand")
        {
            var expansion = valueSet.Expansion;
            if (expansion != null)
            {
                foreach (var contains in expansion.Contains)
                {
                    string system = contains.System;
                    string code = contains.Code;
                    string display = contains.Display;
                    result = new Tuple<string, string, string>(system, code, display);
                    listResult.Add(result);
                }
            }
            else
            {
                Console.WriteLine("No expansion found.");
            }
        }
        else{
            var compose = valueSet.Compose;
            if (compose != null)
            {
                foreach (var include in compose.Include)
                {
                    string system = include.System;
                    foreach (var concept in include.Concept)
                    {
                        string code = concept.Code;
                        string display = concept.Display;
                        result = new Tuple<string, string, string>(system, code, display);
                        listResult.Add(result);
                    }
                }
            }
            else
            {
                Console.WriteLine("No compose found.");
            }

        }
        
        //sort the list by system and code
        listResult = listResult.OrderBy(x => x.Item1).ThenBy(x => x.Item2).ToList();
        return listResult;
    }
}

/// <summary>
/// FHIR Module for IG Analyzer
/// This module provides functionality to analyze FHIR Implementation Guides (IGs).
/// It includes methods to load and parse FHIR resources, validate them, and display the results.
/// </summary>

// function to load FHIR IG Package


