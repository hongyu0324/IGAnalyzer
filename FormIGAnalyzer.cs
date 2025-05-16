using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Firely.Fhir.Packages;
using Hl7.Fhir.FhirPath;
using Newtonsoft.Json;
using Hl7.Fhir.Specification.Terminology;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;

using System.Resources;


namespace IGAnalyzer;
public partial class FormIGAnalyzer : Form
{
    private string profilePath = string.Empty;
    private string profileName = string.Empty;

    private string igName = string.Empty;

    private string igSubName = string.Empty;

    private Dictionary<string, string> urlList = new Dictionary<string, string>();
    private ModuleFHIR.FHIROperator fhirOperator = new ModuleFHIR.FHIROperator();
    FhirPackageSource? resolver;
    private List<Tuple<string, string, string, string>> qList = new List<Tuple<string, string, string, string>>();

    private List<string> profiles = new List<String>();
    private List<string> bundles = new List<String>();

    private string profileDirectory = "profiles\\";

    private string stagingDirectory = "staging\\";

    //private string applyModelDirectory = "applymodel\\";

    //private string questionnaireDirectory = "questionnaire\\";

    //private string fumeDirectory = "fume\\";

    private ResourceManager rm = new ResourceManager("IGAnalyzer.FormIGAnalyzer", typeof(FormIGAnalyzer).Assembly);

    private FhirClient? client;
    private string applyModel = "";
    public FormIGAnalyzer()
    {
        InitializeComponent();
        igName = rm.GetString("IGName") ?? string.Empty;
        igName = "pas";
        profileDirectory = "D:\\Hongyu\\Project\\data\\IGAnalyzer";
        //read profilePath from resource From IGAnalyzer.Properties.Resources
        txtDataDirectory.Text = rm.GetString("DataDirectory") ?? string.Empty;
        profilePath = txtDataDirectory.Text + profileDirectory + igName;
        profileName = "https://twcore.mohw.gov.tw/ig/" + igName + "/StructureDefinition";
        FormListView form = new FormListView();
        //form.Show(); // Use the private member by showing the form
    }

    private void ReadDefaultProfile()
    {

    }

    private void Initial()
    {
        urlList.Clear();
        qList.Clear();
        profiles.Clear();
        bundles.Clear();
        applyModel = string.Empty;
        txtMsg.Text = string.Empty;
        txtPackage.Text = string.Empty;
        txtQuestionnaire.Text = string.Empty;
        lbProfile.Items.Clear();
        lbApplyModel.Items.Clear();
        lvElement.Items.Clear();
        lbProfile.Items.Clear();
        lbStaging.Items.Clear();
        lbBundleList.Items.Clear();
        lvProfile.Items.Clear();
        lvApplyModel.Items.Clear();
        lvElement.Items.Clear();
        string fhirServer = rm.GetString("FHIRServer") ?? string.Empty;
        //string fhirServer = "http://localhost:8080/fhir/";
        client = new FhirClient(fhirServer);
    }
    private void cmbIG_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cmbIG.SelectedItem == null)
        {
            MessageBox.Show("Please select an IG.");
            return;
        }

        string ig = cmbIG.SelectedItem?.ToString() ?? string.Empty;
        if (ig.StartsWith("emr"))
        {
            igName = ig.Split('-')[0]; // IG名稱
            igSubName = ig.Split('-')[1]; // IG子名稱
            profilePath = txtDataDirectory.Text + profileDirectory + igName;
            profileName = "https://twcore.mohw.gov.tw/ig-emr/twcore/StructureDefinition";
        }
        else
        {
            igName = cmbIG.Text; // IG名稱
            profilePath = txtDataDirectory.Text + profileDirectory + igName;
            profileName = "https://twcore.mohw.gov.tw/ig/" + igName + "/StructureDefinition";
        }

    }

    private void btnClose_Click(object? sender, EventArgs e)
    {
        this.Close();
    }

    private void GenerateUrlListFromBinding(Hl7.Fhir.Model.ElementDefinition element, string profileName, Dictionary<string, string> urlList)
    {
        if (element.Binding != null)
        {
            var valueSetUri = element.Binding.ValueSet?.ToString() ?? string.Empty;
            try
            {
                List<string> listPath = element.Path.Split('.').ToList();
                if (listPath.Count > 1)
                {
                    listPath[0] = profileName;
                }
                string code = string.Join(".", listPath);
                if (urlList.ContainsKey(code))
                {
                    return; // Or continue if called from a loop, but return is fine for a helper
                }
                urlList.Add(code, valueSetUri);
            }
            catch (Exception err)
            {
                txtMsg.Text = txtMsg.Text + $"Error processing {profileName} {element.Path}: {err.Message}" + Environment.NewLine;
            }
        }
    }

    private string GetLogicName(string ig, string igSub)
    {
        string logicName = ig;

        if (logicName == "pas") logicName = "Apply";
        if (logicName == "emr")
        {
            if (igSub == "pms") logicName = "PMS";
            if (igSub == "ep") logicName = "EP";
            if (igSub == "ic") logicName = "IC";
            if (igSub == "dms") logicName = "DMS";
        }
        return logicName;
    }

    private async void btnSelect_ClickAsync(object? sender, EventArgs e)
    {
        ofdPackage.InitialDirectory = profilePath;
        ofdPackage.Filter = "IG Package (*.tgz)|*.tgz";
        ofdPackage.FileName = "package.tgz";
        ofdPackage.ShowDialog();
        if (ofdPackage.FileName == "")
        {
            MessageBox.Show("Please select a package file.");
            return;
        }

        Initial();
        string tw_ig = ofdPackage.FileName;
        txtPackage.Text = tw_ig;
        resolver = new(ModelInfo.ModelInspector, new string[] { tw_ig });
        var names = resolver.ListCanonicalUris();

        foreach (var n in names)
        {
            string logicName = GetLogicName(igName, igSubName);

            try
            {
                if (n.StartsWith(profileName))
                {
                    var profile = n.Split("/").Last();
                    if (profile.Contains("Bundle"))
                    {
                        bundles.Add(profile);
                    }
                    else if (profile.Contains(logicName) && profile.Contains("Model"))
                    {
                        applyModel = profile;
                    }
                    else
                    {
                        if (igName == "emr")
                        {
                            if (profile.Contains(logicName))
                            {
                                profiles.Add(profile);
                            }
                        }
                        else
                        {
                            profiles.Add(profile);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                txtMsg.Text = txtMsg.Text + $"Error processing {n}: {ex.Message}" + Environment.NewLine;
            }
        }
        // Display the results in the ListBox
        lbProfile.Items.Clear();
        foreach (var profile in profiles)
        {
            lbProfile.Items.Add(profile);
            lbStaging.Items.Add(profile);
        }

        foreach (var bundle in bundles)
        {
            lbBundleList.Items.Add(bundle);
        }


        urlList.Add("Observation-diagnostic-twpas.valueCodeableConcept", "http://loinc.org/vs/LL1971-2");
        //urlList.Add("Observation-diagnostic-twpas.interpretation", "http://terminology.hl7.org/ValueSet/v3-ObservationInterpretation");
        urlList.Add("Observation-cancer-stage-twpas.valueCodeableConcept", "https://twcore.mohw.gov.tw/ig/pas/ValueSet/cancer-stage-score");
        urlList.Add("MedicationRequest-treat-twpas.dosageInstruction.doseAndRate.doseQuantity.code", "http://hl7.org/fhir/ValueSet/ucum-common");
        urlList.Add("MedicationRequest-apply-twpas.dosageInstruction.doseAndRate.doseQuantity.unit", "http://hl7.org/fhir/ValueSet/ucum-common");
        urlList.Add("Claim-twpas.item.quantity.unit", "http://hl7.org/fhir/ValueSet/ucum-common");
        urlList.Add("MedicationRequest-treat-twpas.status", "http://hl7.org/fhir/ValueSet/medicationrequest-status");
        urlList.Add("MedicationRequest-treat-twpas.dosageInstruction.timing.code.text", "https://twcore.mohw.gov.tw/ig/twcore/ValueSet/medication-frequency-hl7-tw");
        urlList.Add("MedicationRequest-apply-twpas.dosageInstruction.timing.repeat.count", "https://twcore.mohw.gov.tw/ig/twcore/ValueSet/medication-frequency-hl7-tw");
        urlList.Add("MedicationRequest-apply-twpas.medicationCodeableConcept", "http://hl7.org/fhir/ValueSet/medication-codes");
        urlList.Add("Claim-twpas.procedure.procedureCodeableConcept.coding", "https://twcore.mohw.gov.tw/ig/twcore/ValueSet/icd-10-pcs-2023-tw");

        foreach (string profileName in profiles)
        {

            var sd = await resolver.ResolveByUriAsync("StructureDefinition/" + profileName) as StructureDefinition;
            if (sd is not StructureDefinition resolvedProfile)
            {
                txtMsg.Text = txtMsg.Text + "Failed to resolve the StructureDefinition." + Environment.NewLine;
                return;
            }

            foreach (var element in sd.Differential.Element)
            {
                GenerateUrlListFromBinding(element, profileName, urlList);
            }

            foreach (var element in sd.Snapshot.Element)
            {
                GenerateUrlListFromBinding(element, profileName, urlList);
            }

        }


        var resolvedDefinition = await resolver.ResolveByUriAsync("StructureDefinition/" + applyModel);
        if (resolvedDefinition is not StructureDefinition applyModelDef)
        {
            txtMsg.Text = txtMsg.Text + "Failed to resolve the ApplyModel definition." + Environment.NewLine;
            return;
        }

        lbApplyModel.Items.Clear();
        foreach (var ele in applyModelDef.Differential.Element)
        {
            var elementList = ele.Path.Split('.').ToList();
            if (elementList.Count > 1)
            {
                if (elementList.Count == 2)
                {
                    lbApplyModel.Items.Add(ele.Path + " | " + ele.Definition);
                }

                var map = ele.Mapping;
                foreach (var m in map)
                {
                    // get profile name from profiles list 
                    if (elementList.Count > 2)
                    {
                        string q3 = m.Map;
                        if (q3.Contains("where"))
                        {
                            q3 = m.Map;
                        }
                        else
                        {
                            q3 = q3.Split(" ")[0];
                            // 如果以"coding.code"結束，則刪除"coding.code"
                            if (q3.EndsWith(".coding.code"))
                            {
                                q3 = q3.Substring(0, q3.Length - 12);
                            }
                        }
                        //  IG Package問題，未來修改
                        q3 = ModifyByIGPackage(q3);
                        string q1 = ele.Short;
                        if (ele.Short.Contains("，"))
                        {
                            q1 = ele.Short.Split("，")[0];
                        }
                        //  IG Package問題，未來修改
                        var q = new Tuple<string, string, string, string>(q1 + " | " + ele.Path, GetProfileName(m.Identity, profiles), q3, ele.Type.FirstOrDefault()?.Code ?? string.Empty);
                        qList.Add(q);
                    }
                }
            }

        }

    }

    private string ModifyByIGPackage(string igPath)
    {
        string q3 = igPath;
        if (q3.Contains("基因突變類型"))
        {
            q3 = "component.interpretation";
        }

        return q3;
    }


    private void tbJSON_TextChanged(object sender, EventArgs e)
    {

    }

    private void tabPage1_Click(object sender, EventArgs e)
    {

    }
    private string GetProfileName(string name, List<string> profiles)
    {
        name = name.ToLower();
        if (name.Length > 13) name = name.Replace("patient", "pat"); // 縮寫不一致問題
        name = name.Replace("treatment", "tx"); // 縮寫不一致問題
        foreach (var n in profiles)
        {
            var p = n.Replace("-", "");
            p = p.Replace("twpas", "");
            p = "twpas" + p;
            p = p.ToLower();
            if (p == name)
            {
                return n;
            }
        }
        return name;
    }

    private async void lbProfile_SelectedIndexChanged(object sender, EventArgs e)
    {
        string profileName = lbProfile.SelectedItem?.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(profileName))
        {
            MessageBox.Show("No item selected.");
            return;
        }
        if (resolver == null)
        {
            MessageBox.Show("Please select a package file.");
            return;
        }

        lvProfile.Items.Clear();
        // add header to listview
        lvProfile.Columns.Clear();
        lvProfile.Columns.Add("Path", 400);
        lvProfile.Columns.Add("Strength", 100);
        lvProfile.Columns.Add("ValueSet", 800);

        lvElement.Items.Clear();
        lvElement.Columns.Clear();
        lvElement.Columns.Add("Path", 400);
        lvElement.Columns.Add("Type", 100);
        lvElement.Columns.Add("Definition", 500);
        lvElement.Columns.Add("Min", 100);
        lvElement.Columns.Add("Max", 100);
        if (rbApplyModel.Checked)
        {
            lvElement.Columns.Add("ApplyModel", 500);
        }

        rbDifferential.Visible = true;
        rbSnapshot.Visible = true;
        rbApplyModel.Visible = true;

        List<ElementDefinition> listElemnt = new List<ElementDefinition>();
        var applyModelDefResult = await resolver.ResolveByUriAsync("StructureDefinition/" + applyModel);
        if (applyModelDefResult is not StructureDefinition applyModelDef)
        {
            MessageBox.Show("Failed to resolve the ApplyModel definition.");
            return;
        }

        if (resolver != null && await resolver.ResolveByUriAsync("StructureDefinition/" + profileName) is StructureDefinition sd)
        {
            if (rbDifferential.Checked || rbApplyModel.Checked)
            {
                listElemnt = sd.Differential.Element;
            }
            else if (rbSnapshot.Checked)
            {
                listElemnt = sd.Snapshot.Element;
            }
            foreach (var element in listElemnt)
            {
                try
                {
                    ListViewItem itemElement = new ListViewItem(element.Path);
                    itemElement.SubItems.Add(element.Type.FirstOrDefault()?.Code ?? string.Empty);
                    itemElement.SubItems.Add(element.Short ?? string.Empty);
                    itemElement.SubItems.Add(element.Min.ToString() ?? string.Empty);
                    if (element.Max is not null)
                    {
                        itemElement.SubItems.Add(element.Max.ToString() ?? string.Empty);
                    }
                    else
                    {
                        itemElement.SubItems.Add(string.Empty);
                    }
                    if (rbApplyModel.Checked)
                    {
                        foreach (var ele in applyModelDef.Differential.Element)
                        {
                            var map = ele.Mapping;
                            foreach (var m in map)
                            {
                                var type = ele.Type.FirstOrDefault()?.Code;
                                if (type == "BackboneElement") continue;
                                var p1 = GetProfileName(m.Identity, profiles);
                                if (p1 == profileName)
                                {
                                    List<string> pathList = element.Path.Split('.').ToList();
                                    // remove the first element of the list
                                    pathList.RemoveAt(0);
                                    // join the list to a string
                                    string path = string.Join(".", pathList);
                                    string m_str = m.Map;
                                    if (m_str.EndsWith(".coding.code"))
                                    {
                                        m_str = m_str.Substring(0, m_str.Length - 12);
                                    }
                                    // remove the first element of the list
                                    if (m_str.Contains(path))
                                    {
                                        itemElement.SubItems.Add(ele.Path);
                                    }
                                }
                            }
                        }
                    }
                    lvElement.Items.Add(itemElement);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {element.Path}: {ex.Message}");
                }
                if (element.Binding != null)
                {
                    // add item to listview
                    ListViewItem item = new ListViewItem(element.Path);
                    item.SubItems.Add(element.Binding.Strength.ToString());
                    item.SubItems.Add(element.Binding.ValueSet);
                    lvProfile.Items.Add(item);
                }

            }

        }
        else
        {
            MessageBox.Show("Failed to resolve the StructureDefinition.");
        }
    }
    private void lbApplyModel_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Clear the Panel2 as selected item changes
        this.splitQuestionnaire.Panel2.Controls.Clear();
        this.splitQuestionnaire.Panel2.AutoScroll = true;

        List<Tuple<string, string, string, string, string>> qItemList = new List<Tuple<string, string, string, string, string>>();
        // Clear ListView
        lvApplyModel.Items.Clear();
        lvApplyModel.Columns.Clear();
        lvApplyModel.Columns.Add("Name", 200);
        lvApplyModel.Columns.Add("ApplyModel", 100);
        lvApplyModel.Columns.Add("Profile", 200);
        lvApplyModel.Columns.Add("Path", 400);
        lvApplyModel.Columns.Add("Type", 200);
        lvApplyModel.Columns.Add("ValueSet", 600);
        if (lbApplyModel.SelectedItem != null)
        {
            string itemName = lbApplyModel.SelectedItem?.ToString()?.Split('|')[0].Trim() ?? string.Empty;
            // Display the results in the textBox

            foreach (var q in qList)
            {
                if (q.Item1.Contains(itemName))
                {
                    //txtApplyModel.Text += q.Item1 + " | " + q.Item2 + " | " + q.Item3 + " | " + q.Item4 + Environment.NewLine;
                    // add item to listview
                    ListViewItem item = new ListViewItem(q.Item1.Split('|')[0].Trim());
                    //item.SubItems.Add(q.Item1.Split('|')[1].Trim().Replace(itemName + ".", ""));
                    item.SubItems.Add(q.Item1.Split('|')[1].Trim());
                    item.SubItems.Add(q.Item2);
                    item.SubItems.Add(q.Item3);
                    item.SubItems.Add(q.Item4);
                    string code = q.Item2 + "." + q.Item3;
                    //check if urlMap contains code
                    string url = string.Empty;
                    if (urlList.ContainsKey(code))
                    {
                        url = urlList[code];
                        item.SubItems.Add(url);
                    }
                    //  URL問題，未來修改

                    if (q.Item3.Contains("http") && !q.Item3.Contains("StructureDefinition"))
                    {
                        // extract the URL from the string
                        var splitList = q.Item3.Split("\'").ToList();

                        foreach (var s in splitList)
                        {
                            if (s.Contains("http"))
                            {
                                url = s;
                                url = url.Replace("CodeSystem", "ValueSet");
                                //url = url.Replace("StructureDefinition", "ValueSet");
                            }
                        }

                        item.SubItems.Add(url);
                    }

                    // URL問題，未來修改
                    lvApplyModel.Items.Add(item);
                    var qItem = new Tuple<string, string, string, string, string>(q.Item1, q.Item2, q.Item3, q.Item4, url);

                    qItemList.Add(qItem);
                }

            }
            string title = lbApplyModel.SelectedItem?.ToString()?.Split('|')[1].Trim() ?? string.Empty;
            string json = fhirOperator.GenerateQuestionnaire(title, qItemList);
            txtQuestionnaire.Text = json;
        }
        else
        {
            MessageBox.Show("No item selected.");
        }
    }

    private void listView1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void rbDifferential_CheckedChanged(object sender, EventArgs e)
    {
        if (rbDifferential.Checked)
        {
            rbSnapshot.Checked = false;
            rbApplyModel.Checked = false;
        }
        else if (rbSnapshot.Checked)
        {
            rbDifferential.Checked = false;
            rbApplyModel.Checked = false;
        }
        else
        {
            rbDifferential.Checked = false;
            rbSnapshot.Checked = false;
        }

    }
    private async Task<ValueSet?> EpandValueSetByUrlAsync(FhirClient client, string valueSetUri)
    {

        var valueSet = new ValueSet();
        var svc = new ExternalTerminologyService(client);

        var parameters = new ExpandParameters()
            .WithValueSet(url: valueSetUri);
        try
        {
            // Call the Expand operation on the FHIR server
            valueSet = await svc.Expand(parameters) as ValueSet;
            if (valueSet == null)
            {
                {
                    txtMsg.Text = txtMsg.Text + $"Failed to parse ValueSet for {valueSetUri}" + Environment.NewLine;
                    return null;
                }
            }
            // Check if the expansion was successful
            if (valueSet.Expansion == null || valueSet.Expansion.Contains == null || valueSet.Expansion.Contains.Count == 0)
            {
                txtMsg.Text = txtMsg.Text + $"Failed to parse ValueSet for {valueSetUri}" + Environment.NewLine;
                return null;
            }
        }
        catch (Exception ex)
        {
            txtMsg.Text = txtMsg.Text + $"Error processing {valueSetUri}: {ex.Message}" + Environment.NewLine;
            return null;
        }


        return valueSet;

    }
    private async Task<ValueSet?> GetValueSetByUrlAsync(FhirClient client, string valueSetUri)
    {

        var searchParams = new SearchParams();
        var valueSet = new ValueSet();
        searchParams.Add("url", valueSetUri);
        try
        {
            var bundle = await client.SearchAsync<ValueSet>(searchParams);
            if (bundle == null || bundle.Entry.Count == 0)
            {

                txtMsg.Text = txtMsg.Text + $"Failed to parse ValueSet for {valueSetUri}" + Environment.NewLine;
                return null;
            }
            else
            {
                valueSet = bundle.Entry[0].Resource as ValueSet;
            }

            if (valueSet == null)
            {
                txtMsg.Text = txtMsg.Text + $"Failed to parse ValueSet for {valueSetUri}" + Environment.NewLine;
                return null;
            }
            return valueSet;
        }
        catch (Exception ex)
        {
            txtMsg.Text = txtMsg.Text + $"Error processing {valueSetUri}: {ex.Message}" + Environment.NewLine;
            return null;
        }
    }
    private void btnRendering_Click(object sender, EventArgs e)
    {
        // Save the questionnaire data to a file or perform any other action
        btnRebdering_ClickAsync(sender, e);

    }
    private async void btnRebdering_ClickAsync(object sender, EventArgs e)
    {
        // connect to the FHIR server

        // Read the JSON string from the TextBox
        string jsonString = txtQuestionnaire.Text;
        // Deserialize the JSON string into a Questionnaire object
        var questionnaire = new FhirJsonParser().Parse<Questionnaire>(jsonString);

        // parsing the questionnaire
        var questionnaireItem = questionnaire.Item[0].Item;

        int yPos = 10; // Starting Y position for the labels
        int xPos = 10; // Starting X position for the labels
        int height = 0; // Height of the labels
        // Clear the Panel2 before adding new labels
        this.splitQuestionnaire.Panel2.Controls.Clear();

        // Iterate through the items in the questionnaire and create labels for each one

        foreach (var item in questionnaireItem)
        {
            // Display the LinkId and Text of each item
            // Create a new Label for each item in the questionnaire
            Label label = new Label();
            label.Text = $"{item.Text}: ";
            label.AutoSize = true;
            label.Location = new Point(10, yPos);
            this.splitQuestionnaire.Panel2.Controls.Add(label);
            if (xPos < 10 + label.Width + 10)
            {
                xPos = 10 + label.Width + 10; // Set the location of the TextBox
            }
            yPos += label.Height + 10; // Move down for the next label
            height = label.Height; // Height of the label
        }

        yPos = 10; // Move down for the next label
        foreach (var item in questionnaireItem)
        {
            // Create a TextBox for each item in the questionnaire
            string name = item.Text.Split('|').Last().Trim();

            name = name.Replace("ApplyModel.", ""); // Replace spaces with underscores in the name
            if (item.Type == Questionnaire.QuestionnaireItemType.Choice)
            {

                // Create a ComboBox for choice items
                ComboBox comboBox = new ComboBox();
                comboBox.Name = name; // Set the name of the ComboBox
                comboBox.Width = 400; // Set the width of the ComboBox
                comboBox.Location = new Point(xPos, yPos); // Set the location of the ComboBox
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList; // Set the ComboBox to be a drop-down list
                if (item.AnswerValueSet is not null && item.AnswerValueSet != string.Empty)
                {
                    // Get the value set from the FHIR server
                    string type = "expand";
                    if (client == null)
                    {
                        txtMsg.Text = txtMsg.Text + "FHIR client is not initialized." + Environment.NewLine;
                        continue;
                    }
                    var valueSet = await EpandValueSetByUrlAsync(client, item.AnswerValueSet);
                    if (valueSet == null)
                    {
                        valueSet = await GetValueSetByUrlAsync(client, item.AnswerValueSet);
                        type = "compose";
                    }
                    // Serialize the value set to JSON
                    if (valueSet != null)
                    {
                        var json = new FhirJsonSerializer(new SerializerSettings()
                        {
                            Pretty = true,
                        }).SerializeToString(valueSet);
                        var vsMap = fhirOperator.ParsingValueSet(valueSet, type);
                        foreach (var m in vsMap)
                        {
                            // Add the display name to the ComboBox
                            comboBox.Items.Add(m.Item2 + " | " + m.Item3);
                        }
                    }

                }
                this.splitQuestionnaire.Panel2.Controls.Add(comboBox);
                yPos += height + 10; // Move down for the next label 
            }
            else if (item.Type == Questionnaire.QuestionnaireItemType.Boolean)
            {
                // Create a CheckBox for boolean items
                CheckBox checkBox = new CheckBox();
                checkBox.Name = name; // Set the name of the CheckBox
                checkBox.Text = item.Text;
                checkBox.AutoSize = true; // Set the CheckBox to auto-size based on its content
                checkBox.Location = new Point(xPos, yPos); // Set the location of the CheckBox
                this.splitQuestionnaire.Panel2.Controls.Add(checkBox);
                yPos += height + 10; // Move down for the next label 
            }
            else if (item.Type == Questionnaire.QuestionnaireItemType.Integer)
            {
                // Create a NumericUpDown for integer items
                NumericUpDown numericUpDown = new NumericUpDown();
                numericUpDown.Name = name; // Set the name of the NumericUpDown
                numericUpDown.Width = 400; // Set the width of the NumericUpDown
                numericUpDown.Location = new Point(xPos, yPos); // Set the location of the NumericUpDown
                this.splitQuestionnaire.Panel2.Controls.Add(numericUpDown);
                yPos += height + 10; // Move down for the next label 
            }
            else if (item.Type == Questionnaire.QuestionnaireItemType.Decimal)
            {
                // Create a TextBox for decimal items
                TextBox textBox = new TextBox();
                textBox.Name = name; // Set the name of the TextBox
                textBox.Width = 400; // Set the width of the TextBox
                textBox.Location = new Point(xPos, yPos); // Set the location of the TextBox
                this.splitQuestionnaire.Panel2.Controls.Add(textBox);
                yPos += height + 10; // Move down for the next label 
            }
            else if (item.Type == Questionnaire.QuestionnaireItemType.DateTime || item.Type == Questionnaire.QuestionnaireItemType.Date)
            {
                // Create a DateTimePicker for dateTime items
                DateTimePicker dateTimePicker = new DateTimePicker();
                dateTimePicker.Name = name; // Set the name of the DateTimePicker
                dateTimePicker.Width = 400; // Set the width of the DateTimePicker
                dateTimePicker.Location = new Point(xPos, yPos); // Set the location of the DateTimePicker
                this.splitQuestionnaire.Panel2.Controls.Add(dateTimePicker);
                yPos += height + 10; // Move down for the next label 
            }
            else
            {
                // Create a TextBox for other types of items
                TextBox textBox = new TextBox();
                textBox.Name = name; // Set the name of the TextBox
                textBox.Width = 400; // Set the width of the TextBox
                textBox.Location = new Point(xPos, yPos); // Set the location of the TextBox
                this.splitQuestionnaire.Panel2.Controls.Add(textBox);
                yPos += height + 10; // Move down for the next label 
            }
        }

        // add a button to save the questionnaire
        Button btnSave = new Button();
        btnSave.Text = "Save Staging File";
        btnSave.Tag = lbApplyModel.SelectedItem?.ToString()?.Split('|')[1].Trim() ?? string.Empty;
        btnSave.Location = new Point(10, yPos); // Set the location of the Button
        btnSave.AutoSize = true; // Set the Button to auto-size based on its content
        btnSave.BackColor = Color.LightBlue; // Set the background color of the Button
        btnSave.Click += CreateStagingFile; // Add a click event handler to the Button
        this.splitQuestionnaire.Panel2.Controls.Add(btnSave);
    }
    private void CreateStagingFile(object? sender, EventArgs e)
    {
        // Save the questionnaire data to a file or perform any other action
        Dictionary<string, string> questionnaireData = new Dictionary<string, string>();
        // Iterate through the controls in the Panel2 and save their values
        string json = string.Empty;

        foreach (var item in splitQuestionnaire.Panel2.Controls)
        {
            if (item is TextBox textBox)
            {
                questionnaireData.Add(textBox.Name.Replace(".", ""), textBox.Text);
            }
            else if (item is ComboBox comboBox)
            {
                questionnaireData.Add(comboBox.Name.Replace(".", ""), comboBox.SelectedItem?.ToString() ?? string.Empty);
            }
            else if (item is CheckBox checkBox)
            {
                questionnaireData.Add(checkBox.Name.Replace(".", ""), checkBox.Checked.ToString());
            }
            else if (item is NumericUpDown numericUpDown)
            {
                questionnaireData.Add(numericUpDown.Name.Replace(".", ""), numericUpDown.Value.ToString());
            }
            else if (item is DateTimePicker dateTimePicker)
            {
                string date = dateTimePicker.Value.ToString("yyyy-MM-dd");
                // Convert to the desired format to yyyy-mm-dd

                questionnaireData.Add(dateTimePicker.Name.Replace(".", ""), date);
            }
        }

        // Serialize the questionnaire data to JSON可顯示中文


        //json = System.Text.Json.JsonSerializer.Serialize(questionnaireData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

        // Optionally, you can use Newtonsoft.Json for more advanced serialization options
        json = Newtonsoft.Json.JsonConvert.SerializeObject(questionnaireData, Newtonsoft.Json.Formatting.Indented);
        // Display the JSON string in a MessageBox or save it to a file
        // For demonstration, we'll show it in a MessageBox 

        // save the JSON to a file profilePath + "staging\\" + Tag + ".json"
        string fileName = txtDataDirectory.Text + stagingDirectory + cmbIG.Text + ".json";
        string type = (sender as Button)?.Tag?.ToString() ?? string.Empty;
        SaveStagingFile(json, type, fileName);
        // Show the JSON string in a MessageBox
        MessageBox.Show(json.ToString(), "Staging Data Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void SaveStagingFile(string json, string type, string fileName)
    {
        // Check if the directory exists, if not create it
        if (!System.IO.Directory.Exists(txtDataDirectory.Text + stagingDirectory))
        {
            System.IO.Directory.CreateDirectory(txtDataDirectory.Text + stagingDirectory);
        }

        //serialize the json to a dictionary
        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);


        // Check tjhe file exists, if so append the content to the file
        if (System.IO.File.Exists(fileName))
        {
            //read the file
            string fileContent = System.IO.File.ReadAllText(fileName);

            //serialize the file content to a dictionary
            var fileData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent);

            if (data != null)
            {
                foreach (var item in data)
                {
                    if (fileData != null && fileData.ContainsKey(item.Key))
                    {
                        fileData[item.Key] = item.Value;
                    }
                    else if (fileData != null)
                    {
                        fileData.Add(item.Key, item.Value);
                    }
                }
            }
            //serialize the file data to json
            string newJson = Newtonsoft.Json.JsonConvert.SerializeObject(fileData, Newtonsoft.Json.Formatting.Indented);
            // write the file
            System.IO.File.WriteAllText(fileName, newJson);
        }
        else
        {
            // write the file
            System.IO.File.WriteAllText(fileName, json);
        }
    }

    private void lvApplyModel_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void lblHosp_Click(object sender, EventArgs e)
    {

    }

    private void lvApplyModel_DoubleClick(object sender, EventArgs e)
    {
        //Label label = new Label();
        int yPos = 10; // Starting Y position for the labels
        int xPos = 10; // Starting X position for the labels
        int height = 0; // Height of the labels
        FormListView form = new FormListView();
        form.FormBorderStyle = FormBorderStyle.FixedDialog; // Set the form border style to fixed dialog
        form.MaximizeBox = false; // Disable the maximize button
        form.MinimizeBox = false; // Disable the minimize button
        form.Controls.Clear(); // Clear the form controls before adding new ones
        // Clear the Panel2 before adding new labels
        List<string> nameList = new List<string>();
        int total = lvApplyModel.Columns.Count;
        foreach (var col in lvApplyModel.Columns)
        {
            // Display the LinkId and Text of each item
            // Create a new Label for each item in the questionnaire
            Label label = new Label();
            var splitResult = col.ToString()?.Split(":");
            label.Text = splitResult != null && splitResult.Length > 2 ? $"{splitResult[2]}: " : "Invalid Data: ";
            nameList.Add(label.Text);
            label.AutoSize = true;
            label.Location = new Point(10, yPos);
            form.Controls.Add(label);
            if (xPos < 10 + label.Width + 10)
            {
                xPos = 10 + label.Width + 10; // Set the location of the TextBox
            }
            yPos += label.Height + 10; // Move down for the next label
            height = label.Height; // Height of the label
        }

        yPos = 10; // Move down for the next label
        int cnt = 0;
        List<TextBox> textBoxList = new List<TextBox>();
        for (int i = 0; i < total; i++)
        {

            TextBox textBox = new TextBox();
            if (lvApplyModel.SelectedItems[0].SubItems.Count > i)
            {
                var item = lvApplyModel.SelectedItems[0].SubItems[i].Text;
                textBox.Text = lvApplyModel.SelectedItems[0].SubItems[cnt].Text; // Set the name of the TextBox
            }
            //textBox.Text = (item as ListViewItem.ListViewSubItem)?.Text ?? string.Empty; 
            textBox.Width = 600; // Set the width of the TextBox
            textBox.AutoSize = true; // Set the TextBox to auto-size based on its content
            textBox.Location = new Point(xPos, yPos); // Set the location of the TextBox
            textBoxList.Add(textBox);
            form.Controls.Add(textBox);
            yPos += height + 10; // Move down for the next label
            cnt++;
        }

        Button btnLVClose = new Button();
        btnLVClose.Text = "Close";
        btnLVClose.Click += LVClose; // Add a click event handler to the Button
        btnLVClose.AutoSize = true; // Set the Button to auto-size based on its content
        btnLVClose.Location = new Point(xPos + 600 - btnLVClose.Width, yPos + 10); // Set the location of the Button
        form.Controls.Add(btnLVClose);

        Button btnLVSave = new Button();
        btnLVSave.Text = "Save";
        btnLVSave.Tag = textBoxList; // Store the textBoxList in the Tag property
        btnLVSave.Click += LVSave; // Add a click event handler to the Button
        btnLVSave.AutoSize = true; // Set the Button to auto-size based on its content
        btnLVSave.Location = new Point(10, yPos + 10); // Set the location of the Button
        form.Controls.Add(btnLVSave);
        form.Text = lbApplyModel.SelectedItem?.ToString()?.Split('|')[1].Trim() ?? string.Empty;
        form.Size = new Size(xPos + 640, total * (height + 10) + 120); // Set the size of the form
        form.StartPosition = FormStartPosition.CenterParent;
        form.ShowDialog();
    }
    private void LVClose(object? sender, EventArgs e)
    {
        // Save the questionnaire data to a file or perform any other action
        // Close the form or perform any other action
        Form.ActiveForm?.Close();
    }

    private void LVSave(object? sender, EventArgs e)
    {
        // Retrieve the textBoxList from the sender's Tag property
        if (sender is Button button && button.Tag is List<TextBox> textBoxList)
        {
            // Get the selected item from the ListView
            var item = lvApplyModel.SelectedItems[0];

            if (item.SubItems.Count == textBoxList.Count)
            {
                // Update the ListView item with the values from the TextBoxes
                for (int i = 0; i < textBoxList.Count; i++)
                {
                    item.SubItems[i].Text = textBoxList[i].Text;
                }
            }
            else
            {
                // Update existing subitems and add new ones if necessary
                for (int i = 0; i < textBoxList.Count; i++)
                {
                    if (i < item.SubItems.Count)
                    {
                        item.SubItems[i].Text = textBoxList[i].Text;
                    }
                    else
                    {
                        item.SubItems.Add(textBoxList[i].Text);
                    }
                }
            }
            lvApplyModel.Refresh();
        }
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
        List<Tuple<string, string, string, string, string>> qList = new List<Tuple<string, string, string, string, string>>();

        // copy lvApplyModel to qList
        foreach (ListViewItem item in lvApplyModel.Items)
        {
            if (item.SubItems.Count < 6)
            {
                item.SubItems.Add(string.Empty);
            }
            var q = new Tuple<string, string, string, string, string>(item.SubItems[0].Text + "|" + item.SubItems[1].Text, item.SubItems[2].Text, item.SubItems[3].Text, item.SubItems[4].Text, item.SubItems[5].Text);
            qList.Add(q);
        }
        // clear txtQuestionnaire
        txtQuestionnaire.Text = string.Empty;

        // generate questionnaire
        if (lbApplyModel.SelectedItem != null)
        {
            string title = lbApplyModel.SelectedItem?.ToString()?.Split('|')[1].Trim() ?? string.Empty;
            string json = fhirOperator.GenerateQuestionnaire(title, qList);
            // display json in txtQuestionnaire
            txtQuestionnaire.Text = json;
        }
        else
        {
            MessageBox.Show("No item selected in ApplyModel.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Load JSON file to Questionnaire.");
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Save Questionnaire to JSON file.");
    }
    private void btnExport_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Export Questionnaire to FHIR.");
    }
    private async void lbStaging_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Clear the Panel2 as selected item changes
        //this.splitQuestionnaire.Panel2.Controls.Clear();
        //this.splitQuestionnaire.Panel2.AutoScroll = true;

        // Clear ListView
        lvStaging.Items.Clear();
        lvStaging.Columns.Clear();
        txtFHIRData.Text = string.Empty;
        
        lvStaging.Columns.Add("Name", 400);
        lvStaging.Columns.Add("ApplyModel", 500);
        lvStaging.Columns.Add("Path", 400);
        lvStaging.Columns.Add("Type", 200);
        if (lbStaging.SelectedItem != null)
        {
            string itemName = lbStaging.SelectedItem?.ToString()?.Split('|')[0].Trim() ?? string.Empty;

            /*
            StructureDefinition sd = new StructureDefinition();
            sd = await GetStructureDefinition(itemName);
            if (sd == null)
            {
                MessageBox.Show("Failed to resolve the StructureDefinition.");
                return;
            }
            foreach(var element in sd.Differential.Element)
            {
                try
                {
                    foreach (var q in qList)
                    {
                        var pathList = element.Path.Split(".").ToList();
                        // remove the first element of the list
                        pathList.RemoveAt(0);
                        // join the list to a string
                        string path = string.Join(".", pathList);
                        if (q.Item2 == itemName && q.Item3 == path)
                        {
                            ListViewItem item = new ListViewItem(q.Item1.Split('|')[0].Trim());
                            string staging = q.Item1.Split('|')[1].Trim().Replace("ApplyModel.", "");
                            staging = staging.Replace(".", "");
                            item.SubItems.Add(staging);
                            item.SubItems.Add(q.Item3);
                            item.SubItems.Add(q.Item4);
                            lvStaging.Items.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {element.Path}: {ex.Message}");
                }
            }
            */

            // read json form 
            string fileName = txtDataDirectory.Text + stagingDirectory + cmbIG.Text + ".json";
            // Check if the file exists
            if (!System.IO.File.Exists(fileName))
            {
                MessageBox.Show("File not found: " + fileName);
                return;
            }
            // read the file
            string fileContent = System.IO.File.ReadAllText(fileName);
            //serialize the file content to a dictionary
            var fileData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent);
            // create a Dictionary to store the data
            
            Dictionary<string, string> data = new Dictionary<string, string>();
            
            foreach (var q in qList)
            {
                if (q.Item2.Contains(itemName))
                {
                    //txtApplyModel.Text += q.Item1 + " | " + q.Item2 + " | " + q.Item3 + " | " + q.Item4 + Environment.NewLine;
                    // add item to listview
                    ListViewItem item = new ListViewItem(q.Item1.Split('|')[0].Trim());
                    string staging = q.Item1.Split('|')[1].Trim().Replace("ApplyModel.", "");
                    staging = staging.Replace(".", "");
                    item.SubItems.Add(staging);
                    item.SubItems.Add(q.Item3);
                    item.SubItems.Add(q.Item4);
                    lvStaging.Items.Add(item);

                    if (fileData != null && fileData.ContainsKey(staging))
                    {
                        // add the value to the dictionary
                        string value = fileData[staging].Split("|")[0].Trim();
                        staging = staging.Replace(".", "");
                        data.Add(staging, value);
                    }
                }
            }
            
            // transform the dictionary to json
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            // display json in textBox
            txtStaging.Text = json;

            // txtFHIRData.Text = CreateFHIRData(json, itemName);
            // display json in textBox
            txtFume.Text = await CreateFUME2(itemName);

            //RefineProfile(await GetStructureDefinition(itemName));
        }
        else
        {
            MessageBox.Show("No item selected.");
        }
        lvStaging.Refresh();
    }

    private string CreateFHIRData(string json, string itemName)
    {

        // Deserialize the JSON string into a Dictionary
        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        // Check if the data is not null
        if (data != null)
        {
            // Iterate through the dictionary and set the properties of the Observation resource
            foreach (var item in data)
            {
                string key = item.Key;
                string value = item.Value;
                string path = GetProfilePath(key);
            }
        }

        StructureDefinition sd = new StructureDefinition();
        sd = GetStructureDefinition(itemName).Result;

        string type = sd.Type;

        Base? resource = null;
        if (type == "Observation")
        {
            Observation obs = new Observation();

            foreach (var e in sd.Differential.Element)
            {
                string path = e.Path.Replace("Observation.", "");

            }
            resource = obs;
        }
        else if (type == "Patient")
        {
            resource = new Patient();
        }
        else if (type == "Encounter")
        {
            resource = new Encounter();
        }
        else if (type == "Condition")
        {
            resource = new Condition();
        }
        else if (type == "MedicationRequest")
        {
            resource = new MedicationRequest();
        }
        else if (type == "Procedure")
        {
            resource = new Procedure();
        }
        else
        {
            MessageBox.Show("Unknown resource type: " + type);
            return string.Empty;
        }

        var fhirJson = new FhirJsonSerializer(new SerializerSettings()
        {
            Pretty = true,
        }).SerializeToString(resource);
        return fhirJson;
    }

    private string GetProfilePath(string applyModel)
    {
        string path = string.Empty;
        foreach (ListViewItem item in lvStaging.Items)
        {
            if (applyModel == item.SubItems[1].Text) path = item.SubItems[2].Text;
        }
        return path;
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {

    }

    private void btnFHIRData_Click(object sender, EventArgs e)
    {
        string fume = txtFume.Text + "*" + Environment.NewLine;
        var fumeList = fume.Split(Environment.NewLine).ToList();

        List<List<string>> fumeListGroup = new List<List<string>>();

        Dictionary<string, object> fumeDict = new Dictionary<string, object>();
        Dictionary<string, object> fumeDictTemp = new Dictionary<string, object>();
        Dictionary<string, object> fumeDictJson = new Dictionary<string, object>();
        List<Dictionary<string, object>> fumeDictList = new List<Dictionary<string, object>>();

        //int level = 0;
        List<string> fumeItem = new List<string>();
        foreach (var f in fumeList)
        {
            if (f.StartsWith("*"))
            {
                if (fumeList.Count > 0)
                {
                    fumeListGroup.Add(fumeItem);
                    fumeItem = new List<string>();
                }
                fumeItem.Add(f);
            }
            else
            {
                fumeItem.Add(f);
            }
        }

        // print out fumeListGroup
        fumeDict = new Dictionary<string, object>();
        int level = 0;
        int workLevel = 0;
        foreach (var f in fumeListGroup)
        {

            fumeDictTemp = new Dictionary<string, object>();
            fumeDict = new Dictionary<string, object>();
            fumeDictList = new List<Dictionary<string, object>>();
            if (f.Count == 0) continue;


            for (int i = f.Count - 1; i >= 0; i--)
            {

                level = ComputeLevel(f[i]);

                string s = f[i].ToString();
                if (s.Contains("*") == false) continue;
                s = s.Replace("*", "").Trim();
                //Console.WriteLine(s);
                if (s.Contains("="))
                {
                    if (workLevel != 0 && level > workLevel)
                    {
                        Dictionary<string, object> dic = fumeDictTemp;
                        fumeDictList.Add(dic);
                        fumeDictTemp = new Dictionary<string, object>();
                    }
                    var ss = s.Split("=");
                    var key = ss[0].Trim();
                    var value = ss[1].Trim();
                    value = value.Replace("\"", "").Trim();
                    fumeDictTemp.Add(key, value);
                }
                else
                {
                    if (fumeDict.Keys.Contains(s) && level == 0) continue;
                    Dictionary<string, object> dic = fumeDictTemp;
                    fumeDictTemp = new Dictionary<string, object>();
                    fumeDictTemp.Add(s, dic);
                    if (level == 0 && fumeDictList.Count > 0) fumeDictList.Add(dic);
                    if (level == 0)
                    {
                        if (fumeDictList.Count > 0)
                        {
                            fumeDictTemp = new Dictionary<string, object>();
                            foreach (var dicList in fumeDictList)
                            {
                                var key = dicList.Keys.FirstOrDefault();
                                if (key != null && dicList.ContainsKey(key))
                                {
                                    var valueFume = dicList[key];
                                    try
                                    {
                                        fumeDictTemp.Add(key, valueFume);
                                    }
                                    catch (Exception ex)
                                    {
                                        txtMsg.Text = txtMsg.Text + $"Error adding key {key}: {ex.Message}" + Environment.NewLine;
                                    }
                                }
                            }
                            fumeDict.Add(s, fumeDictTemp);
                            fumeDictList.Clear();
                        }
                        else
                        {
                            fumeDict.Add(s, dic);
                        }
                    }


                }
                workLevel = level;
            }
            var keyFume = fumeDict.Keys.FirstOrDefault();
            if (keyFume != null && fumeDict.ContainsKey(keyFume))
            {
                var valueFume = fumeDict[keyFume];
                try
                {
                    fumeDictJson.Add(keyFume, valueFume);
                }
                catch (Exception ex)
                {
                    txtMsg.Text = txtMsg.Text + $"Error adding key {keyFume}: {ex.Message}" + Environment.NewLine;
                }
            }
        }
        var jsonFume = JsonConvert.SerializeObject(fumeDictJson, Newtonsoft.Json.Formatting.Indented);
        txtFHIRData.Text = jsonFume;
    }

    private int ComputeLevel(string fume)
    {
        int level = 0;
        if (fume.StartsWith("*"))
        {
            level = 0;
        }
        else
        {
            int spaceCount = fume.TakeWhile(c => c == ' ').Count();
            level = spaceCount / 2;
        }
        return level;
    }

    private string GetStagingApplyModel(string path)
    {
        string applyModel = string.Empty;

        foreach (ListViewItem item in lvStaging.Items)
        {
            //if(path == item.SubItems[2].Text) applyModel = item.SubItems[1].Text;
            if (item.SubItems[2].Text.Contains(path)) applyModel = item.SubItems[1].Text;
        }
        applyModel = applyModel.Replace(".", "");
        return applyModel;
    }

    private async Task<StructureDefinition> GetStructureDefinition(string profileName)
    {
        StructureDefinition sd = new StructureDefinition();
        if (resolver != null)
        {
            var resolvedSd = await resolver.ResolveByUriAsync("StructureDefinition/" + profileName) as StructureDefinition;
            if (resolvedSd == null)
            {
                throw new InvalidOperationException($"Failed to resolve StructureDefinition for profile: {profileName}");
            }
            sd = resolvedSd;
        }
        return sd;
    }

    private async Task<string> CreateFUME2(string profileName)
    {
        string fume2 = string.Empty;
        string fumeDetail = string.Empty;
        string pathInfo = string.Empty;
        List<Tuple<string, string, string>> fumeListTuple = new List<Tuple<string, string, string>>();

        StructureDefinition sd = new StructureDefinition();
        sd = await GetStructureDefinition(profileName);
        foreach (var e in sd.Differential.Element)
        {
            var pathList = e.Path.Split(".").ToList();
            int cnt = pathList.Count;
            // remove the first element of the list
            pathList.RemoveAt(0);
            // join the list to a string
            string path = string.Join(".", pathList);

            string type = e.Type.FirstOrDefault()?.Code ?? string.Empty;
            if (type == string.Empty)
            {
                string rule = "StructureDefinition.snapshot.element.where(path = '" + e.Path + "')";
                var obj = sd.Select(rule).FirstOrDefault() as ElementDefinition;
                if (obj != null)
                {
                    type = obj.Type != null ? obj.Type.FirstOrDefault()?.Code ?? string.Empty : string.Empty;
                }
            }
            if (type == string.Empty)
            {
                foreach (ListViewItem item in lvStaging.Items)
                {
                    if (item.SubItems[2].Text == path)
                    {
                        type = item.SubItems[3].Text;
                        break;
                    }

                }
            }
            if (pathList.Count > 0) path = pathList.Last();
            // GetPostionStar前必須修正path名稱才能得到正確FHIR Data所需要的Element呈現方式
            if (path.EndsWith("[x]"))
            {
                if (type == "dateTime")
                {
                    path = path.Replace("[x]", "") + "DateTime";
                }
                else if (type == "CodeableConcept" || type == "Quantity")
                {
                    path = "valueCodeableConcept";
                    type = "CodeableConcept";
                }
                else if (type == "string") path = "valueString";
                else if (type == "integer") path = "valueInteger";
                else if (type == "decimal") path = "valueDecimal";
                else if (type == "boolean") path = "valueBoolean";
                else if (type == "uri") path = "valueUri";
                else if (type == "base64Binary") path = "valueBase64Binary";
                else if (type == "code") path = "valueCode";
                else if (type == "oid") path = "valueOid";
                else if (type == "uuid") path = "valueUuid";
                else if (type == "Attachment") path = "valueAttachment";
                else if (type == "Reference") path = "valueReference";
                else if (type == "date") path = "valueDate";
                else if (type == "time") path = "valueTime";
                else if (type == "Address") path = "valueAddress";
                else if (type == "HumanName") path = "valueHumanName";
                else if (type == "ContactPoint") path = "valueContactPoint";
                else if (type == "Identifier") path = "valueIdentifier";
                else if (type == "Period") path = "valuePeriod";
                else if (type == "Range") path = "valueRange";
                else if (type == "Ratio") path = "valueRatio";
                else continue;
            }
            pathInfo = GetPostionStar(cnt, path);
            fume2 += pathInfo + "[" + type + "]" + "[" + e.Path + "]" + Environment.NewLine;
            Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(pathInfo, type, e.Path);
            fumeListTuple.Add(fumeTuple);
        }
        fumeListTuple = CleanFumeTupleList(fumeListTuple);
        fumeListTuple = RefineFUME(fumeListTuple);
        fumeListTuple = GetFUMEValue(fumeListTuple);
        fumeListTuple = GetFUMEPattern(sd, fumeListTuple);
        fume2 = string.Empty;
        foreach (var fume in fumeListTuple)
        {
            fume2 += fume.Item1 + Environment.NewLine;
            fumeDetail += fume.Item1 + "[" + fume.Item2 + "]" + "[" + fume.Item3 + "]" + Environment.NewLine;
        }
        txtFHIRData.Text = fumeDetail;
        return fume2;
    }

    private List<Tuple<string, string, string>> GetFUMEPattern(StructureDefinition sd,List<Tuple<string, string, string>> fumeListTuple)
    {
        List<Tuple<string, string, string>> patternFUME = new List<Tuple<string, string, string>>();

        for (int i = 0; i < fumeListTuple.Count; i++)
        {
            string path = fumeListTuple[i].Item3;
            string rule = "StructureDefinition.differential.element.where(path = '" + path + "')";
            var result = sd.IsTrue(rule);
            bool isPattern = false;
            if (result)
            {
                var obj = sd.Select(rule).FirstOrDefault() as ElementDefinition;
                if (obj != null && obj.Pattern != null)
                {
                    var value = obj.Pattern;
                    string pattern = value != null ? value.ToString() ?? string.Empty : string.Empty;
                    if (pattern != string.Empty)
                    {
                        var fume = fumeListTuple[i];
                        string fumeInfo = fume.Item1;
                        string type = fume.Item2;
                        string pathFUME = fume.Item3;
                        patternFUME.Add(new Tuple<string, string, string>(fumeInfo + " = " + pattern, type, pathFUME));
                        isPattern = true;
                    }
                }
            }
            if (isPattern == false) patternFUME.Add(fumeListTuple[i]);
        }   
        return patternFUME;
    }

    private List<Tuple<string, string, string>> GetFUMEValue(List<Tuple<string, string, string>> fumeListTuple)
    {
        // Clean the FUME list
        // This is a placeholder implementation, replace with actual logic
        List<Tuple<string, string, string>> valuedFUME = new List<Tuple<string, string, string>>();

        for (int i = 0; i < fumeListTuple.Count; i++)
        {
            var fume = fumeListTuple[i];
            string fumeInfo = fume.Item1;
            string type = fume.Item2;
            string path = fume.Item3;

            var pathList = path.Split(".").ToList();
            //remove the first element of the list
            pathList.RemoveAt(0);
            //join the list to a string
            path = string.Join(".", pathList);
            if (type == "dateTime")
            {
                path = path.Replace("[x]", "") + "DateTime";
                var updatedFume = new Tuple<string, string, string>(fume.Item1 + " = " + GetStagingApplyModel(path), fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            else if (type == "Reference")
            {
                valuedFUME.Add(fume);
                string fumeName = fumeInfo.Replace("*", "").Trim();
                fumeInfo = "  " + fumeInfo.Replace(fumeName, "reference") + " = " + GetStagingApplyModel(path);
                var updatedFume = new Tuple<string, string, string>(fumeInfo, fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            else
            {
                valuedFUME.Add(fume);
            }
        }

        return valuedFUME;
    }
    private List<Tuple<string, string, string>> RefineFUME(List<Tuple<string, string, string>> fumeListTuple)
    {
        // Refine the FUME list
        // This is a placeholder implementation, replace with actual logic
        List<Tuple<string, string, string>> refinedFUME = new List<Tuple<string, string, string>>();
        Tuple<string, string, string> fumeAddTuple = new Tuple<string, string, string>("", "", "");
        int indexAdd = 0;
        for (int i = 0; i < fumeListTuple.Count; i++)
        {
            if (indexAdd > 0)
            {
                indexAdd--;
                if (indexAdd == 0)
                {
                    refinedFUME.Add(fumeAddTuple);
                    fumeAddTuple = new Tuple<string, string, string>("", "", "");
                }           
            }
            var fume = fumeListTuple[i];
            string fumeInfo = fume.Item1;
            string type = fume.Item2;
            string path = fume.Item3;
            refinedFUME.Add(fume);
            if (type == "CodeableConcept")
            {
                string type1 = string.Empty;
                string type2 = string.Empty;
                string type3 = string.Empty;

                if (i < fumeListTuple.Count - 1)
                {
                    var fume1 = fumeListTuple[i + 1];
                    type1 = fume1.Item2;
                }
                if (type1 != "Coding")
                {
                    string fumeName = fumeInfo.Replace("*", "").Trim();
                    Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>("  " + fumeInfo.Replace(fumeName, "coding"), "Coding", path + ".coding");
                    refinedFUME.Add(fumeTuple);
                    fumeTuple = new Tuple<string, string, string>("    " + fumeInfo.Replace(fumeName, "system"), "uri", path + ".coding.system");
                    refinedFUME.Add(fumeTuple);
                    fumeTuple = new Tuple<string, string, string>("    " + fumeInfo.Replace(fumeName, "code"), "code", path + ".coding.code");
                    refinedFUME.Add(fumeTuple);
                }
                else
                {
                    if (i < fumeListTuple.Count - 2)
                    {
                        type2 = fumeListTuple[i + 2].Item2;
                    }
                    if (i < fumeListTuple.Count - 3)
                    {
                        type3 = fumeListTuple[i + 3].Item2;
                    }
                    if (type2 == "uri" && type3 != "code")
                    {
                        fume = fumeListTuple[i + 2];
                        fumeInfo = fume.Item1;
                        fumeAddTuple = new Tuple<string, string, string>(fumeInfo.Replace("system", "code"), "code", path + ".coding.code");
                        indexAdd = 2;
                    }
                    if (type2 == "code" && type3 != "uri")
                    {
                        fume = fumeListTuple[i + 2];
                        fumeInfo = fume.Item1;
                        fumeAddTuple = new Tuple<string, string, string>(fumeInfo.Replace("code", "system"), "uri", path + ".coding.system");
                        indexAdd = 2;
                    }
                }
            }

        }

        for (int i = 0; i < refinedFUME.Count; i++)
        {
            var fume = refinedFUME[i];
            string fumeInfo = fume.Item1;
            string type = fume.Item2;
            string path = fume.Item3;

            if (type == "CodeableConcept")
            {
                Tuple<bool,List<int>> codeableTuple = CheckCodeableConcept(i, refinedFUME);
                bool isCodeableConcept = codeableTuple.Item1;
                List<int> codeableConceptList = codeableTuple.Item2;
                //if (!isCodeableConcept) continue;
                for (int j = 0; j < codeableConceptList.Count; j++)
                {
                    int index = codeableConceptList[j]-j;
                    refinedFUME.RemoveAt(index);
                }
            }
            
        }
        return refinedFUME;
    }

    private Tuple<bool,List<int>> CheckCodeableConcept(int index, List<Tuple<string, string, string>> fumeListTuple)
    {
        List<int> codeableConceptList = new List<int>();
        // Check if the type is CodeableConcept
        bool isCodeableConcept = true;
        int indexConcept = index;
        int indexCoding = index + 1;
        int indexSystm = index + 2;
        int indexCode = index + 3;
        string path = fumeListTuple[index+1].Item3;
        bool isCodeable = true;
        if (fumeListTuple[index].Item2 != "CodeableConcept") isCodeableConcept = false;
        if (fumeListTuple[indexCoding].Item2 != "coding")  isCodeableConcept = false;
        if (fumeListTuple[indexSystm].Item2 != "system")  isCodeableConcept = false;
        if (fumeListTuple[indexCode].Item2 != "code")  isCodeableConcept = false;

        int indexCheck = index + 4;
        if (indexCheck >= fumeListTuple.Count) isCodeable = false;

        while (isCodeable)
        {
            string pathCheck = fumeListTuple[indexCheck].Item3;
            if (pathCheck.StartsWith(path))
            {
                codeableConceptList.Add(indexCheck);
                indexCheck++;
                if (indexCheck >= fumeListTuple.Count) isCodeable = false;
            }
            else
            {
                isCodeable = false;
            }
        }
        Tuple<bool, List<int>> codeableConcept = new Tuple<bool, List<int>>(isCodeableConcept, codeableConceptList);
        return codeableConcept;
    }

    private async Task<string> CreateFUME(string profileName)
    {
        List<Tuple<string, string, string>> fumeTupleList = new List<Tuple<string, string, string>>();

        // Generate FUME from the profile
        // This is a placeholder implementation, replace with actual logic
        StructureDefinition sd = new StructureDefinition();
        sd = await GetStructureDefinition(profileName);

        //RefineProfile(sd);

        string resourceType = sd.Type;

        string fume = "Instance:" + Environment.NewLine;
        fume = string.Empty;
        fume += "InstanceOf: " + resourceType + Environment.NewLine;
        int ncnt = 0;
        int codeIndex = 0;
        for (int i = 0; i < sd.Differential.Element.Count; i++)
        {
            string applyModel = string.Empty;

            var e = sd.Differential.Element[i];
            string pathOri = e.Path;
            if (i < sd.Differential.Element.Count - 1)
            {
                var ne = sd.Differential.Element[i + 1];
                ncnt = ne.Path.Split('.').Length;
            }
            int cnt = e.Path.Split('.').Length;
            string path = e.Path.Replace(resourceType + ".", "");
            string pathInfo = string.Empty;
            path = e.Path.Split('.')[cnt - 1];

            string type = e.Type.FirstOrDefault()?.Code ?? string.Empty;
            if (type == string.Empty)
            {
                string rule = "StructureDefinition.snapshot.element.where(path = '" + path + "')";
                var obj = sd.Select(rule).FirstOrDefault() as ElementDefinition;
                if (obj != null)
                {
                    type = obj.Type != null ? obj.Type.FirstOrDefault()?.Code ?? string.Empty : string.Empty;
                }
            }
            if (type == string.Empty)
            {
                foreach (ListViewItem item in lvStaging.Items)
                {
                    if (item.SubItems[2].Text == path)
                    {
                        type = item.SubItems[3].Text;
                        break;
                    }

                }
            }

            if (type == "Quantity") type = "CodeableConcept"; // 2023-10-03 hard code for now, slicing is not supported yet
            if (path.EndsWith("[x]"))
            {
                if (type == "dateTime")
                {
                    path = path.Replace("[x]", "") + "DateTime";
                }
                else if (type == "CodeableConcept") path = "valueCodeableConcept";
                else if (type == "string") path = "valueString";
                //else if (type == "boolean") path = "valueBoolean";
                //else if (type == "integer") path = "valueInteger";
                //else if (type == "decimal") path = "valueDecimal";
                //else if (type == "uri") path = "valueUri";
                //else if (type == "base64Binary") path = "valueBase64Binary";
                //else if (type == "code") path = "valueCode";
                //else if (type == "oid") path = "valueOid";
                //else if (type == "uuid") path = "valueUuid";
                //else if (type == "Attachment") path = "valueAttachment";
                //else if (type == "Reference") path = "valueReference";
                //else if (type == "date") path = "valueDate";
                //else if (type == "time") path = "valueTime";
                //else if (type == "Address") path = "valueAddress";
                //else if (type == "HumanName") path = "valueHumanName";
                //else if (type == "ContactPoint") path = "valueContactPoint";
                //else if (type == "Identifier") path = "valueIdentifier";
                //else if (type == "Period") path = "valuePeriod";
                //else if (type == "Range") path = "valueRange";
                //else if (type == "Ratio") path = "valueRatio";
                else continue;
            }

            // GetPostionStar前必須修正path名稱才能得到正確FHIR Data所需要的Element呈現方式
            pathInfo = GetPostionStar(cnt, path);


            if (type == "Reference")
            {
                pathInfo += Environment.NewLine;
                pathInfo += GetPostionStar(cnt + 1, "reference") + " = ";
                if (path == "subject")
                {
                    pathInfo += "patient";
                }
                else
                {
                    //pathInfo += path;
                }
                pathInfo += GetStagingApplyModel(path) + Environment.NewLine;
            }
            else if (type == "dateTime")
            {
                pathInfo += " = ";
                pathInfo += GetStagingApplyModel(path) + Environment.NewLine;
            }
            else if (e.Binding != null)
            {
                //if(i-codeIndex < 2) continue;
                pathInfo += Environment.NewLine;
                pathInfo += GetPostionStar(cnt + 1, "coding") + Environment.NewLine;
                pathInfo += GetPostionStar(cnt + 2, "system") + " = " + "\"" + e.Binding.ValueSet + "\"" + Environment.NewLine;
                pathInfo += GetPostionStar(cnt + 2, "code") + " = " + GetStagingApplyModel(path) + Environment.NewLine;
                //pathInfo += GetPostionStar(cnt + 2, "display") + " = " + Environment.NewLine;
                codeIndex = i;
            }
            else if (type == "CodeableConcept")
            {
                //if(i-codeIndex < 2) continue;
                pathInfo += Environment.NewLine;
                pathInfo += GetPostionStar(cnt + 1, "coding") + Environment.NewLine;
                string valueSet = e.Binding?.ValueSet ?? "url";
                pathInfo += GetPostionStar(cnt + 2, "system") + " = " + "\"" + valueSet + "\"" + Environment.NewLine;
                pathInfo += GetPostionStar(cnt + 2, "code") + " = " + GetStagingApplyModel(path) + Environment.NewLine;
                //pathInfo += GetPostionStar(cnt + 2, "display") + " = " + Environment.NewLine;
                codeIndex = i;
            }
            else if (type == "string" && e.Binding == null)
            {
                pathInfo += " = ";
                pathInfo += GetStagingApplyModel(path) + Environment.NewLine;
            }
            /*
            else if (type == "boolean" && e.Binding == null)
            {
                pathInfo += " = ";
                pathInfo += GetStagingApplyModel(path) + Environment.NewLine;
            }
            else if (type == "integer" && e.Binding == null)
            {
                pathInfo += " = ";
                pathInfo += GetStagingApplyModel(path) + Environment.NewLine;
            }
            else if (type == "decimal" && e.Binding == null)
            {
                pathInfo += " = ";
                pathInfo += GetStagingApplyModel(path) + Environment.NewLine;
            }
            */

            //else{
            //    if(ncnt <= cnt) pathInfo += " = ";
            //}

            if (e.Pattern != null)
            {
                string typePattern = e.Pattern.TypeName ?? string.Empty;
                if (typePattern == "CodeableConcept")
                {
                    var pattern = e.Pattern as CodeableConcept;
                    if (pattern != null)
                    {
                        pathInfo += Environment.NewLine;
                        foreach (var coding in pattern.Coding)
                        {
                            pathInfo += GetPostionStar(cnt + 1, "coding") + Environment.NewLine;
                            pathInfo += GetPostionStar(cnt + 2, "system") + " = " + "\"" + coding.System + "\"" + Environment.NewLine;
                            pathInfo += GetPostionStar(cnt + 2, "code") + " = " + "\"" + coding.Code + "\"" + Environment.NewLine;
                            //pathInfo += GetPostionStar(cnt + 2, "display") + " = " + "\"" + coding.Display + "\"" + Environment.NewLine;
                        }
                    }
                    else
                    {
                        MessageBox.Show("CodeableConcept is not supported yet.");
                    }
                }
                else
                {
                    string pattern = e.Pattern != null ? e.Pattern.ToString() ?? string.Empty : string.Empty;
                    pathInfo += " = \"" + pattern + "\"";
                }
            }

            Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(pathInfo, type, pathOri);
            fumeTupleList.Add(fumeTuple);
        }
        foreach (var fumeTuple in fumeTupleList)
        {
            txtMsg.Text += fumeTuple.Item2 + " ### " + fumeTuple.Item3 + Environment.NewLine;
        }
        fumeTupleList = CleanFumeTupleList(fumeTupleList);
        foreach (var fumeTuple in fumeTupleList)
        {
            fume += fumeTuple.Item1;

            if (fumeTuple.Item1.EndsWith(Environment.NewLine) == false) fume += Environment.NewLine;
            txtMsg.Text += fumeTuple.Item2 + " ### " + fumeTuple.Item3 + Environment.NewLine;
        }
        return fume;
    }

    private List<Tuple<string, string, string>> CleanFumeTupleList(List<Tuple<string, string, string>> fumeTupleList)
    {
        // Clean up the fumeTupleList
        List<Tuple<string, string, string>> targetFumeTupleList = new List<Tuple<string, string, string>>();

        targetFumeTupleList = fumeTupleList;
        if(targetFumeTupleList[0].Item1.Length == 0)
        {
            targetFumeTupleList.RemoveAt(0);
        }
        for (int i = 1; i < targetFumeTupleList.Count; i++)
        {
            string fume1 = targetFumeTupleList[i-1].Item1;
            string fume2 = targetFumeTupleList[i].Item1;
            string path1 = targetFumeTupleList[i - 1].Item3;
            string path2 = targetFumeTupleList[i].Item3;
            if (fume1 == fume2)
            {
                targetFumeTupleList.RemoveAt(i);
                i--;
            }
            if(path2.EndsWith("coding") == true && path2.Contains(path1) == false)
            {
                string fume = fume2.Replace("coding", "code");
                fume = fume.Substring(2);
                string path = path2.Replace("coding", "");
                Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fume, "CodeableConcept", path);
                targetFumeTupleList.Insert(i, fumeTuple);
                i++;
            }
            if(fume2.Trim().EndsWith("=")){
                targetFumeTupleList.RemoveAt(i);
                i--;
            }
            if (fume2 == Environment.NewLine)
            {
                targetFumeTupleList.RemoveAt(i);
                i--;
            }
        }
       
        // Remove duplicates
        

        return targetFumeTupleList;
    }
    private string GetPostionStar(int cnt, string path)
    {
        string starPos = string.Empty;

        if (cnt == 2) starPos = "* " + path;
        else if (cnt == 3) starPos = "  * " + path;
        else if (cnt == 4) starPos = "    * " + path;
        else if (cnt == 5) starPos = "      * " + path;
        else if (cnt == 6) starPos = "        * " + path;
        return starPos;
    }

    private void RefineProfile(StructureDefinition sd)
    {

        foreach (var element in sd.Differential.Element)
        {
            var path = element.Path;
            var type = element.Type.FirstOrDefault()?.Code;
            if (type == null)
            {
                string rule = "StructureDefinition.snapshot.element.where(path = '" + path + "')";
                var obj = sd.Select(rule).FirstOrDefault() as ElementDefinition;
                if (obj != null)
                {
                    type = obj.Type.FirstOrDefault()?.Code;
                }
            }
            txtMsg.Text = txtMsg.Text + $"{"  "} {path} {type}" + Environment.NewLine;
            //map.Add(path, type); 
        }
    }

    private void lbBundleList_SelectedIndexChanged(object sender, EventArgs e)
    {
        lvBundleProfile.Items.Clear();
        // show the selected item in the text box
        string bundleName = lbBundleList.SelectedItem?.ToString() ?? string.Empty;

        StructureDefinition sd = new StructureDefinition();
        sd = GetStructureDefinition(bundleName).Result;
        bool isRequired = false;
        string element = string.Empty;

        lvBundleProfile.Columns.Clear();
        lvBundleProfile.Columns.Add("Element", 400);

        foreach (var n in sd.Differential.Element)
        {
            var n_list = n.ElementId.Split(".");
            if (n_list.Count() == 2 && n.ElementId.Contains(":") == false)
            {
                if (n.Min > 0)
                {
                    Console.WriteLine(n.ElementId + "   " + n.Min + "." + n.Max + "  - " + n.Short);
                }
            }
            else if (n.ElementId.Contains(":"))
            {
                var n_list2 = n.ElementId.Split(":");

                if (n_list2[0] == "Bundle.entry")
                {
                    int level = n_list2[1].Count(x => x == '.') - 1;
                    if (level < 0)
                    {
                        isRequired = n.Min > 0;
                        element = n.ElementId;
                        lvBundleProfile.Items.Add(element);
                    }
                    else
                    {
                        string profile = string.Empty;
                        if (n.Type.Count > 0 && n.Type[0].Profile != null && n.Type[0].Profile.Any())
                        {
                            profile = n.Type[0].Profile.First().Split("/").Last();
                        }
                        // new profile Tuple

                        element = "";
                    }
                }
            }
        }
    }

    private void splitContainer7_Panel2_Paint(object sender, PaintEventArgs e)
    {

    }

    private void splitProfile_Panel1_Paint(object sender, PaintEventArgs e)
    {

    }

    private void splitQuestionnaire_SplitterMoved(object sender, SplitterEventArgs e)
    {

    }

    private void txtFHIRData_TextChanged(object sender, EventArgs e)
    {

    }

    private void btnFUME_Click(object sender, EventArgs e)
    {
        StructureMap sm = new StructureMap();
        //add id
        sm.Id = lbStaging.SelectedItem?.ToString() ?? string.Empty;
        sm.Meta = new Meta();

        sm.Meta.VersionId = "1.0";
        sm.Meta.LastUpdated = DateTimeOffset.Now;
        
        sm.Url = "http://tmhtc.net/StructureMap/" + sm.Id;
        sm.Name = lbStaging.SelectedItem?.ToString() ?? string.Empty;
        sm.Title = lbStaging.SelectedItem?.ToString() ?? string.Empty;
        sm.Status = PublicationStatus.Active;
        sm.Description = "HIS資料轉換，來源為Staging資料";
        sm.Purpose = "簡易型FHIR Gateway";

        sm.Date = DateTimeOffset.Now.ToString("o");

        sm.UseContext = new List<UsageContext>();
        UsageContext uc = new UsageContext();
        CodeableConcept cc = new CodeableConcept();
      
        Coding coding = new Coding();
        coding.System = "http://snomed.info/sct";
        coding.Code = "706594005";
        coding.Display = "Information system software";
        cc.Coding.Add(coding);

        uc.Code = cc.Coding.FirstOrDefault();
        

       
        sm.Group = new List<StructureMap.GroupComponent>();

        StructureMap.GroupComponent group = new StructureMap.GroupComponent();
        group.Name = "test";
        group.Rule = new List<StructureMap.RuleComponent>();
        StructureMap.RuleComponent rule = new StructureMap.RuleComponent();

        rule.Name = "test";
        rule.Source = new List<StructureMap.SourceComponent>();
        StructureMap.SourceComponent source = new StructureMap.SourceComponent();
        source.Context = "input";

        rule.Source.Add(source);

        rule.Extension = new List<Extension>();
        Extension ext = new Extension();

        ext.Url = "http://hl7.org/fhir/StructureDefinition/StructureMap-structure";
        
  

        var expression = new Expression();
        expression.Language = "application/vnd.outburn.fume";
        expression.Expression_ = txtFume.Text;
        ext.Value = expression;

        rule.Extension.Add(ext);

        group.Rule.Add(rule);
        sm.Group.Add(group);

        // Serialize the StructureMap to JSON
        var json = new FhirJsonSerializer(new SerializerSettings() { Pretty = true }).SerializeToString(sm);
        txtFHIRData.Text = json;

    }


    private void btnSaveFHIR_Click(object sender, EventArgs e)
    {
        

    }        // Serialize the questionnaire data to JSON可顯示中文
}
