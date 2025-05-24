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
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


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


    //sliceList == <profileName, path, slice>   
    private List<Tuple<string, string, string>> slicingList = new List<Tuple<string, string, string>>();

    private string profileDirectory = "profiles\\";

    private string stagingDirectory = "staging\\";

    //private string applyModelDirectory = "applymodel\\";

    //private string questionnaireDirectory = "questionnaire\\";

    //private string fumeDirectory = "fume\\";


    private string dataDirectory = string.Empty;
    private string fhirServer = string.Empty;

    private FhirClient? client;
    private string applyModel = "";
    public FormIGAnalyzer()
    {
        InitializeComponent();
        ReadDefaultProfile();

        profilePath = txtDataDirectory.Text + profileDirectory + igName;
        profileName = "https://twcore.mohw.gov.tw/ig/" + igName + "/StructureDefinition";
        FormListView form = new FormListView();
        //form.Show(); // Use the private member by showing the form
    }

    private void ReadDefaultProfile()
    {
        // Read the default profile from the Application.json file

        //string defaultProfilePath = Path.Combine(txtDataDirectory.Text, profileDirectory, "default-profile.json");
        // Assuming Application.json is in the same directory as the executable
        string defaultProfilePath = Path.Combine(Application.StartupPath, "Application.json");


        if (!File.Exists(defaultProfilePath))
        {
            MessageBox.Show("Default profile file not found: " + defaultProfilePath);
            return;
        }
        try
        {
            string jsonContent = File.ReadAllText(defaultProfilePath);
            var defaultProfile = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
            // Get  IG name from the Json file
            if (defaultProfile != null && defaultProfile.ContainsKey("IGName"))
            {
                igName = defaultProfile["IGName"];
                cmbIG.Text = igName;
            }
            else
            {
                MessageBox.Show("IGName not found in the default profile.");
            }
            // Get  DataDirectory from the Json file
            if (defaultProfile != null && defaultProfile.ContainsKey("DataDirectory"))
            {
                dataDirectory = defaultProfile["DataDirectory"];
                txtDataDirectory.Text = dataDirectory;
                profilePath = dataDirectory + profileDirectory + igName;
            }
            else
            {
                MessageBox.Show("DataDirectory not found in the default profile.");
            }
            // Get  FHIRServer from the Json file
            if (defaultProfile != null && defaultProfile.ContainsKey("FHIRServer"))
            {
                fhirServer = defaultProfile["FHIRServer"];
                txtFHIRServer.Text = fhirServer;
            }
            else
            {
                MessageBox.Show("FHIRServer not found in the default profile.");
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show("Error reading default profile: " + ex.Message);
        }

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

    /// <summary>
    /// 處理「選擇」按鈕的點擊事件，允許使用者選擇 IG 套件檔案，並處理所選套件以擷取並顯示相關的 FHIR profiles、bundles 和 models，
    /// 並將擷取到的資訊填入各個 UI 元件。
    ///
    /// 此方法執行以下步驟：
    /// 1. 開啟檔案對話框，讓使用者選擇 .tgz 格式的 IG 套件檔案。
    /// 2. 初始化 resolver，並從所選套件取得 canonical URI 清單。
    /// 3. 根據命名規則與邏輯，過濾並分類 profiles 與 bundles。
    /// 4. 將 profiles 與 bundles 顯示於 UI 清單中。
    /// 5. 初始化一組預設的 ValueSet URL，供後續使用。
    /// 6. 解析每個 profile 的 StructureDefinition，並擷取 value set 綁定資訊。
    /// 7. 解析 apply model 的 StructureDefinition，並將 apply model 元素及其對應 mapping 資訊顯示於 UI。
    /// 8. 處理並顯示處理過程中遇到的錯誤訊息。
    /// </summary>
    /// <param name="sender">事件來源。</param>
    /// <param name="e">包含事件資料的 <see cref="EventArgs"/> 物件。</param>
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
            //lbStaging.Items.Add(profile);
        }

        foreach (var bundle in bundles)
        {
            lbBundleList.Items.Add(bundle);
        }


        urlList.Add("Observation-diagnostic-twpas.valueCodeableConcept", "http://loinc.org/vs/LL1971-2");
        //urlList.Add("Observation-diagnostic-twpas.interpretation", "http://terminology.hl7.org/ValueSet/v3-ObservationInterpretation");
        urlList.Add("Observation-cancer-stage-twpas.valueCodeableConcept", "https://twcore.mohw.gov.tw/ig/pas/ValueSet/cancer-stage-score");
        urlList.Add("MedicationRequest-treat-twpas.dosageInstruction.doseAndRate.doseQuantity.code", "http://hl7.org/fhir/ValueSet/ucum-common");
        urlList.Add("MedicationRequest-apply-twpas.dosageInstruction.doseAndRate.doseQuantity.code", "http://hl7.org/fhir/ValueSet/ucum-common");
        //urlList.Add("Claim-twpas.item.quantity.unit", "http://hl7.org/fhir/ValueSet/ucum-common");
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
            foreach (var element in sd.Differential.Element)
            {
                string slice = string.Empty;
                slice = GetElementSlicing(element);
                if (slice != string.Empty)
                {
                    string path = element.Path;
                    path = path.Replace(sd.Type + ".", "");
                    path = path.Replace("[x]", "");
                    var q = new Tuple<string, string, string>(sd.Id, path, slice);
                    slicingList.Add(q);
                }
            }
        }

        foreach (var s in slicingList)
        {
            txtMsg.Text = txtMsg.Text + s.Item1 + " % " + s.Item2 + " % " + s.Item3 + Environment.NewLine;
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
        if (q3.Contains("基因突變類型")) q3 = "component.interpretation";
        if (q3 == "item.quantity.unit") q3 = "item.quantity.code";
        if (q3 == "dosageInstruction.doseAndRate.doseQuantity.unit") q3 = "dosageInstruction.doseAndRate.doseQuantity.code";
        if (q3 == "ingredient.quantity.numerator.unit") q3 = "ingredient.quantity.numerator.code";
        if (q3 == "contnet.url") q3 = "content.url";

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

        lvConstraint.Items.Clear();

        lvConstraint.Columns.Clear();
        lvConstraint.Columns.Add("Key", 400);
        lvConstraint.Columns.Add("Human", 400);
        lvConstraint.Columns.Add("Expression", 400);


        if (rbApplyModel.Checked)
        {
            lvElement.Columns.Add("ApplyModel", 500);
        }
        else
        {
            lvElement.Columns.Add("Slicing", 500);
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
                    else
                    {
                        string slice = string.Empty;
                        slice = GetElementSlicing(element);
                        itemElement.SubItems.Add(slice);
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
                if (element.Constraint != null)
                {
                    foreach (var constraint in element.Constraint)
                    {
                        // add item to listview
                        ListViewItem item = new ListViewItem(constraint.Key);
                        item.SubItems.Add(constraint.Human);
                        item.SubItems.Add(constraint.Expression);
                        lvConstraint.Items.Add(item);
                    }
                }

            }

        }
        else
        {
            MessageBox.Show("Failed to resolve the StructureDefinition.");
        }
    }

    private string GetElementSlicing(ElementDefinition element)
    {
        string slice = string.Empty;
        if (element.Type.Count > 1)
        {
            slice += "Type : ";
            foreach (var sliceType in element.Type)
            {
                slice += sliceType.Code + " | ";
            }

        }

        if (element.Slicing != null)
        {
            slice += "Slicing : ";
            foreach (var s in element.Slicing.Discriminator)
            {
                slice += s.Path + " , " + (s.Type.HasValue ? s.Type.Value.ToString() : string.Empty) + " | ";
            }
        }

        if (slice.EndsWith(" | "))
        {
            slice = slice.Substring(0, slice.Length - 3);
        }


        return slice;
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
        lvApplyModel.Columns.Add("Rule", 600);

        if (lbApplyModel.SelectedItem != null)
        {
            string itemName = lbApplyModel.SelectedItem?.ToString()?.Split('|')[0].Trim() ?? string.Empty;
            // Display the results in the textBox

            foreach (var q in qList)
            {
                if (q.Item1.Contains(itemName))
                {
                    // add item to listview
                    ListViewItem item = new ListViewItem(q.Item1.Split('|')[0].Trim());

                    item.SubItems.Add(q.Item1.Split('|')[1].Trim());
                    item.SubItems.Add(q.Item2);
                    string path = q.Item3;
                    string rule = GetRuleByPath(path);
                    if (path.Contains("where"))
                    {
                        path = path.Replace(".where" + "(" + rule + ")", "");
                    }
                    else if (path.Contains("(") && path.Contains(")"))
                    {
                        path = path.Replace("(" + rule + ")", "");
                    }
                    item.SubItems.Add(path);

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
                    if (rule != string.Empty)
                    {
                        if (url == string.Empty) item.SubItems.Add(string.Empty);
                        item.SubItems.Add(rule.Replace(")", ""));
                    }

                    string slice = GetSlicingByProfilePath(q.Item2, q.Item3);
                    // if slice is not empty set color to red
                    if (slice != string.Empty && slice.StartsWith("Type"))
                    {
                        item.ForeColor = Color.Red;
                    }

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

    private string GetRuleByPath(string path)
    {
        string rule = string.Empty;

        if (path.Contains("where"))
        {
            var splitList = path.Split("where").ToList();
            rule = splitList[1];
            //get position of "(" and ")" 
            int start = rule.IndexOf("(");
            int end = rule.IndexOf(")");
            if (start != -1 && end != -1)
            {
                rule = rule.Substring(start + 1, end - start - 1);
            }
        }
        /*
        if (path.Contains("(") && path.Contains(")"))
        {
            var splitList = path.Split("(").ToList();
            path = splitList[0];
            rule = splitList[1];
            //get position of "(" and ")" 
            int start = rule.IndexOf("(");
            int end = rule.IndexOf(")");
            if (start != -1 && end != -1)
            {
                //rule += rule.Substring(start + 1, end - start - 1);
            }
        }
        */
        if (rule.EndsWith(")"))
        {
            rule = rule.Substring(0, rule.Length - 1);
        }
        return rule;
    }

    private string GetSlicingByProfilePath(string profile, string path)
    {
        string slice = string.Empty;

        foreach (var s in slicingList)
        {
            if (s.Item1 == profile && path.Contains(s.Item2))
            {
                slice = s.Item3;
                break;
            }

        }
        return slice;
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
        List<Object> textBoxList = new List<Object>();
        for (int i = 0; i < total; i++)
        {

            TextBox textBox = new TextBox();
            ComboBox comboBox = new ComboBox();

            if (lvApplyModel.SelectedItems[0].ForeColor == Color.Red && i == 4) // i = 4 is type
            {
                if (lvApplyModel.SelectedItems[0].SubItems.Count > i)
                {
                    //var item = lvApplyModel.SelectedItems[0].SubItems[i].Text;
                    comboBox.Text = lvApplyModel.SelectedItems[0].SubItems[cnt].Text; // Set the name of the TextBox
                }
                string profile = lvApplyModel.SelectedItems[0].SubItems[2].Text;
                string path = lvApplyModel.SelectedItems[0].SubItems[3].Text;
                string slice = GetSlicingByProfilePath(profile, path);

                if (slice != string.Empty && slice.StartsWith("Type"))
                {
                    slice = slice.Replace("Type : ", "").Trim();
                    List<string> sliceList = slice.Split('|').ToList();
                    foreach (var s in sliceList)
                    {
                        // Add the display name to the ComboBox
                        comboBox.Items.Add(s.Trim());
                    }
                }
                comboBox.Width = 600; // Set the width of the ComboBox
                comboBox.Location = new Point(xPos, yPos); // Set the location of the ComboBox
                comboBox.TextChanged += (s, e) =>
                {
                    // Handle the text changed event if needed
                    // For example, you can update the TextBox with the selected value
                    var txtPathControl = form.Controls.Find("txtPath", true).FirstOrDefault() as TextBox;
                    if (txtPathControl != null)
                    {
                        string combo = comboBox.Text;
                        // change the fisrt character to upper case
                        combo = char.ToUpper(combo[0]) + combo.Substring(1);
                        txtPathControl.Text = "value" + combo;
                    }
                };
                textBoxList.Add(comboBox);
                form.Controls.Add(comboBox);
            }
            else
            {
                if (lvApplyModel.SelectedItems[0].SubItems.Count > i)
                {
                    //var item = lvApplyModel.SelectedItems[0].SubItems[i].Text;
                    textBox.Text = lvApplyModel.SelectedItems[0].SubItems[cnt].Text; // Set the name of the TextBox
                }
                if (i == 3) textBox.Name = "txtPath";
                textBox.Width = 600; // Set the width of the TextBox
                textBox.AutoSize = true; // Set the TextBox to auto-size based on its content
                textBox.Location = new Point(xPos, yPos); // Set the location of the TextBox
                textBoxList.Add(textBox);
                form.Controls.Add(textBox);
            }
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
        if (sender is Button button && button.Tag is List<Object> textBoxList)
        {
            // Get the selected item from the ListView
            var item = lvApplyModel.SelectedItems[0];
            if (textBoxList[0] is TextBox tb0)
            {
                item.Text = tb0.Text;
            }
            else if (textBoxList[0] is ComboBox cb)
            {
                item.Text = cb.Text;
            }
            int total = item.SubItems.Count;

            for (int i = 1; i < total; i++)
            {
                if (textBoxList[i] is TextBox tb)
                {
                    item.SubItems[i].Text = tb.Text;
                }
                else if (textBoxList[i] is ComboBox cb)
                {
                    item.SubItems[i].Text = cb.Text;
                }
            }

            for (int i = total; i < textBoxList.Count; i++)
            {
                if (textBoxList[i] is TextBox tb)
                {
                    item.SubItems.Add(tb.Text);
                }
                else if (textBoxList[i] is ComboBox cb)
                {
                    item.SubItems.Add(cb.Text);
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

    private async void btnLoad_Click(object sender, EventArgs e)
    {
        foreach (ListViewItem item in lvApplyModel.Items)
        {
            string applyModel = item.SubItems[1].Text;
            string profile = item.SubItems[2].Text;
            string path = item.SubItems[3].Text;
            string type = item.SubItems[4].Text;
            if (path.Contains(")")) continue;
            string typePlus = await GetSnapshotType(profile, path);
            if (typePlus != string.Empty && type != typePlus)
            {
                item.SubItems[4].Text = typePlus;
                item.ForeColor = Color.Blue;
                txtMsg.Text = txtMsg.Text + "ApplyModel: " + applyModel + "  Type: (" + type + " , " + typePlus + ")" + Environment.NewLine;
            }
        }
        lvApplyModel.Refresh();
    }

    private async Task<string> GetSnapshotType(string profile, string path)
    {
        StructureDefinition sd = new StructureDefinition();
        sd = await GetStructureDefinition(profile);
        string typePlus = string.Empty;
        if (sd != null)
        {
            //path = profile.Split("-")[0] + "." + path;
            path = sd.Type + "." + path;
            string rule = "StructureDefinition.snapshot.element.where(path = '" + path + "')";
            var obj = sd.Select(rule).FirstOrDefault() as ElementDefinition;
            if (obj != null)
            {
                typePlus = obj.Type != null && obj.Type.Any() ? obj.Type.FirstOrDefault()?.Code ?? string.Empty : string.Empty;
            }
        }
        return typePlus;
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
        lvStaging.Columns.Add("Rule", 400);
        if (lbStaging.SelectedItem != null)
        {
            string itemName = lbStaging.SelectedItem?.ToString()?.Split('|')[0].Trim() ?? string.Empty;


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
                    string path = q.Item3;
                    string rule = GetRuleByPath(path);
                    if (path.Contains("where"))
                    {
                        path = path.Replace(".where" + "(" + rule + ")", "");
                    }
                    // remove content after the first "("  
                    if (path.Contains("("))
                    {
                        path = path.Substring(0, path.IndexOf("("));
                    }

                    item.SubItems.Add(path);
                    item.SubItems.Add(q.Item4);
                    item.SubItems.Add(rule);

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
    private async void btnFHIRData_Click(object sender, EventArgs e)
    {
        string jsonFHIR = string.Empty;
        // call FUME Server http://127.0.0.1:42420/Mapping/<Map ID>

        HttpClient fumeClient = new HttpClient();

        string url = "http://127.0.0.1:42420/Mapping/";
        string mapID = lbStaging.SelectedItem?.ToString() ?? string.Empty;
        mapID = mapID.Replace("-", "");
        url += mapID;

        fumeClient.BaseAddress = new Uri(url);

        string staging = txtStaging.Text;

        //using POST command to send the data
        var content = new StringContent(staging, System.Text.Encoding.UTF8, "application/json");

        var response = await fumeClient.PostAsync(url, content);
        if (response.IsSuccessStatusCode)
        {
            // Read the response content
            var responseContent = await response.Content.ReadAsStringAsync();
            // Display the response content in the TextBox
            jsonFHIR = responseContent;
        }
        else
        {
            // Handle error response
            MessageBox.Show("Error: " + response.StatusCode);
        }

        if (jsonFHIR != string.Empty)
        {
            // Deserialize the JSON string into FHIR resource
            var fhirResource = new FhirJsonParser().Parse<Resource>(jsonFHIR);

            // Serialize the FHIR resource to JSON
            var json = new FhirJsonSerializer(new SerializerSettings()
            {
                Pretty = true,
            }).SerializeToString(fhirResource);
            // Display the JSON string in the TextBox
            txtFHIRData.Text = json;
        }
        else
        {
            MessageBox.Show("No data returned from FUME server.");
        }
    }

    private void btnFHIRData_Click2(object sender, EventArgs e)
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

    private string GetStagingApplyModelWithRule(string path, string rule)
    {
        string applyModel = string.Empty;


        string itemRule = string.Empty;
        foreach (ListViewItem item in lvStaging.Items)
        {
            if (path == item.SubItems[2].Text.Trim())
            {
                itemRule = item.SubItems[4].Text;
                if (itemRule.Contains(rule))
                {
                    applyModel = item.SubItems[1].Text;
                    break;
                }
            }
        }
        applyModel = applyModel.Replace(".", "");

        return applyModel;
    }

    private string GetStagingApplyModel(string path)
    {
        string applyModel = string.Empty;
        // remove the last [x] from the path, 未來可能修正
        if (path.EndsWith("[x]"))
        {
            path = path.Replace("[x]", "");
        }

        foreach (ListViewItem item in lvStaging.Items)
        {
            if (path == item.SubItems[2].Text) applyModel = item.SubItems[1].Text;

            //if (item.SubItems[2].Text.Contains(path)) applyModel = item.SubItems[1].Text;
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

        string fumeDetail = string.Empty;
        string pathInfo = string.Empty;
        List<Tuple<string, string, string>> fumeListTuple = new List<Tuple<string, string, string>>();

        StructureDefinition sd = new StructureDefinition();
        sd = await GetStructureDefinition(profileName);
        if (sd == null)
        {
            MessageBox.Show("Failed to resolve the StructureDefinition.");
            return string.Empty;
        }
        string fume2 = "Instance:" + Environment.NewLine;
        fume2 = string.Empty;
        fume2 += "InstanceOf: " + sd.Type + Environment.NewLine;

        foreach (var e in sd.Differential.Element)
        {
            //刪除欄位
            if (e.Max == "0") continue;

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
                if (type == "dateTime") path = path.Replace("[x]", "") + "DateTime";
                else if (type == "Period") path = path.Replace("[x]", "") + "Period";
                else if (type == "Quantity") path = path.Replace("[x]", "") + "Quantity";
                else if (type == "CodeableConcept") path = path.Replace("[x]", "") + "CodeableConcept";
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
                else if (type == "Range") path = "valueRange";
                else if (type == "Ratio") path = "valueRatio";
                else continue;
            }
            pathInfo = GetPostionStar(cnt, path);
            Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(pathInfo, type, e.Path);
            fumeListTuple.Add(fumeTuple);
        }
        fumeListTuple = CleanFumeTupleList(fumeListTuple); // ==> 問題可能是Slicing所造成，先取消操作
        fumeListTuple = RefineFUME(fumeListTuple);
        fumeListTuple = GetFUMEPattern(sd, fumeListTuple);
        fumeListTuple = GetFUMECodeable(profileName, fumeListTuple);
        fumeListTuple = GetFUMEQuantity(fumeListTuple);
        fumeListTuple = GetFUMEPeriod(fumeListTuple);
        fumeListTuple = GetFUMECoding(fumeListTuple);
        fumeListTuple = GetFUMEValue(fumeListTuple);
        fumeListTuple = GetFUMESlice(fumeListTuple);
        //fumeListTuple = GetFUMEValueWithRule(fumeListTuple);
        lbFUME.Items.Clear();
        foreach (var fume in fumeListTuple)
        {
            fume2 += fume.Item1 + Environment.NewLine;
            lbFUME.Items.Add(fume.Item1);
            fumeDetail += fume.Item1 + "[" + fume.Item2 + "]" + "[" + fume.Item3 + "]" + Environment.NewLine;
        }
        txtFHIRData.Text = fumeDetail;
        return fume2;
    }
    /*
        private List<string> CtreateFumeSlice(string profileName, string path)
        {
            List<string> sliceList = new List<string>();
            StructureDefinition sd = new StructureDefinition();
            sd = GetStructureDefinition(profileName).Result;


            return sliceList;
        }
        */
    private List<Tuple<string, string, string>> GetFUMESlice(List<Tuple<string, string, string>> fumeListTuple)
    {
        List<Tuple<string, string, string>> sliceFUME = new List<Tuple<string, string, string>>();
        string profileName = lbStaging.SelectedItem?.ToString()?.Split('|')[0].Trim() ?? string.Empty;
        List<Tuple<string, string>> profileSliceList = new List<Tuple<string, string>>();
        //txtMsg.Text = string.Empty;
        foreach (var s in slicingList)
        {
            string profile = s.Item1;
            string path = s.Item2;
            string slice = s.Item3;
            string sliceHead = string.Empty;
            string sliceTail = string.Empty;
            string slicePath = string.Empty;
            string sliceType = string.Empty;
            if (!string.IsNullOrEmpty(slice))
            {
                sliceHead = slice.Split(":").FirstOrDefault()?.Trim() ?? string.Empty;
                sliceTail = slice.Split(":").LastOrDefault()?.Trim() ?? string.Empty;
                slicePath = sliceTail.Split(",").FirstOrDefault()?.Trim() ?? string.Empty;
                sliceType = sliceTail.Split(",").LastOrDefault()?.Trim() ?? string.Empty;
            }

            if (profile == profileName)
            {
                if (sliceType == "Value") profileSliceList.Add(new Tuple<string, string>(path, slice));
            }
        }

        bool isAdded = false;
        for (int i = 0; i < fumeListTuple.Count; i++)
        {
            var fume = fumeListTuple[i];
            var fumeInfo = fume.Item1;
            var type = fume.Item2;
            var path = fume.Item3;
            List<string> pathList = path.Split(".").ToList();
            pathList.RemoveAt(0);
            path = string.Join(".", pathList);
            path = path.Replace("[x]", "");
            bool isSlice = false;

            if (profileSliceList.Count > 0)
            {
                foreach (var s in profileSliceList)
                {
                    if (path == s.Item1 && isAdded == false)
                    {
                        List<Tuple<string, string, string>> sliceList = CreateFUMESlice(profileName, path);
                        sliceList = GetFUMESliceValue(sliceList, path);
                        sliceFUME.AddRange(sliceList);
                        isAdded = true;
                    }
                    if (path.Contains(s.Item1))
                    {
                        isSlice = true;
                    }
                }
            }
            if (isSlice == true)
            {
                //string fumeSInfo = "S" + fume.Item1;
                //Tuple<string, string, string> sliceTuple = new Tuple<string, string, string>(fumeSInfo, type, fume.Item3);
                //sliceFUME.Add(sliceTuple);
            }
            else
            {
                isAdded = false;
                sliceFUME.Add(fume);
            }
        }
        return sliceFUME;
    }

    private List<Tuple<string, string, string>> GetFUMESliceValue(List<Tuple<string, string, string>> fumeListTuple, string path)
    {
        List<Tuple<string, string, string>> sliceFUME = new List<Tuple<string, string, string>>();
        for (int i = 0; i < fumeListTuple.Count; i++)
        {
            var fume = fumeListTuple[i];
            sliceFUME.Add(fume);
        }
        for (int i = 0; i < lvStaging.Items.Count; i++)
        {
            string pathItem = lvStaging.Items[i].SubItems[2].Text.Trim();
            if (pathItem.Contains(path) == false) continue;

            string rule = lvStaging.Items[i].SubItems[4].Text.Trim();
            string applyModel = lvStaging.Items[i].SubItems[1].Text;
            if (rule == string.Empty) continue;
            string rulePath = string.Empty;
            string ruleValue = string.Empty;
            var ruleParts = rule?.Split("=");
            if (ruleParts != null && ruleParts.Length > 0 && ruleParts[0] != null)
            {
                rulePath = ruleParts[0].Trim();
                ruleValue = ruleParts[1].Trim();
                ruleValue = ruleValue.Replace("'", "").Trim();
            }
            bool isFind = false;
            for (int j = 0; j < sliceFUME.Count; j++)
            {
                var fume = sliceFUME[j];
                var fumeInfo = fume.Item1;
                if (isFind == false)
                {

                    var pathFUME = fume.Item3;
                    List<string> pathList = pathFUME.Split(".").ToList();
                    //pathList.RemoveAt(0);
                    //pathList.RemoveAt(0);
                    pathFUME = string.Join(".", pathList);

                    string valueFUME = string.Empty;
                    var valueParts = fumeInfo.Split("=");
                    var lastValuePart = valueParts != null && valueParts.Length > 0 ? valueParts.LastOrDefault() : null;
                    if (lastValuePart != null)
                    {
                        valueFUME = lastValuePart.Trim();
                        valueFUME = valueFUME.Replace("\"", "").Trim();
                    }

                    if (pathFUME.Contains(rulePath) && valueFUME == ruleValue)
                    {
                        isFind = true;
                        continue;
                    }
                }
                if (isFind == true)
                {
                    string fumeInfoPath = fume.Item3;
                    List<string> pathListFUME = fumeInfoPath.Split(".").ToList();
                    pathListFUME.RemoveAt(0);
                    fumeInfoPath = string.Join(".", pathListFUME);

                    if (pathItem == fumeInfoPath)
                    {
                        fumeInfo = fumeInfo + " = " + applyModel;
                        sliceFUME[j] = new Tuple<string, string, string>(fumeInfo, fume.Item2, fume.Item3);
                        isFind = false;
                    }
                }
            }
        }

        return sliceFUME;
    }
    private List<Tuple<string, string, string>> GetFUMESliceValue2(List<Tuple<string, string, string>> fumeListTuple)
    {
        List<Tuple<string, string, string>> sliceFUME = new List<Tuple<string, string, string>>();

        for (int i = 0; i < fumeListTuple.Count; i++)
        {
            var fume = fumeListTuple[i];
            var fumeInfo = fume.Item1;
            var rulePart = fumeInfo.Split(":").LastOrDefault();
            var rule = rulePart != null ? rulePart.Trim() : string.Empty;
            var type = fume.Item2;
            var path = fume.Item3;
            List<string> pathList = path.Split(".").ToList();
            pathList.RemoveAt(0);
            path = string.Join(".", pathList);

            for (int j = 0; j < lvStaging.Items.Count; j++)
            {
                if (lvStaging.Items[j].SubItems[2].Text.Contains(path))
                {
                    if (lvStaging.Items[j].SubItems[4].Text.Contains(rule))
                    {
                        fumeInfo = fumeInfo.Replace(rule, lvStaging.Items[j].SubItems[1].Text);
                        fumeInfo = fumeInfo.Replace(":", "=");
                    }
                }
            }
            Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fumeInfo, type, path);
            sliceFUME.Add(fumeTuple);
        }

        return sliceFUME;
    }
    private List<Tuple<string, string, string>> GetFUMECoding(List<Tuple<string, string, string>> fumeListTuple)
    {
        List<Tuple<string, string, string>> codingFUME = new List<Tuple<string, string, string>>();
        for (int i = 0; i < fumeListTuple.Count; i++)
        {
            var fume = fumeListTuple[i];
            var fumeInfo = fume.Item1;
            var type = fume.Item2;
            var path = fume.Item3;

            if (type == "Coding")
            {
                codingFUME.Add(fume);
                int cnt = 0;
                string pathNext = string.Empty;
                if (i + 1 < fumeListTuple.Count)
                {
                    pathNext = fumeListTuple[i + 1].Item3;
                }
                while (!string.IsNullOrEmpty(pathNext) && pathNext.Contains(path) == true)
                {
                    cnt++;
                    if (i + cnt >= fumeListTuple.Count) break;
                    pathNext = fumeListTuple[i + cnt].Item3;
                }
                if (cnt == 0)
                {
                    string fumeName = fumeInfo.Replace("*", "").Trim();
                    fume = new Tuple<string, string, string>("    " + fumeInfo.Replace(fumeName, "system"), "uri", path + ".coding.system");
                    codingFUME.Add(fume);
                    fume = new Tuple<string, string, string>("    " + fumeInfo.Replace(fumeName, "code"), "code", path + ".coding.code");
                    codingFUME.Add(fume);
                }
                else
                {

                }
            }
            else
            {
                codingFUME.Add(fume);
            }
        }
        return codingFUME;
    }

    private List<Tuple<string, string, string>> GetFUMEPeriod(List<Tuple<string, string, string>> fumeListTuple)
    {
        List<Tuple<string, string, string>> periodFUME = new List<Tuple<string, string, string>>();
        for (int i = 0; i < fumeListTuple.Count; i++)
        {
            var fume = fumeListTuple[i];
            string fumeInfo = fume.Item1;
            string type = fume.Item2;
            string path = fume.Item3;
            if (type == "Period")
            {
                periodFUME.Add(fume);
                int cnt = 0;
                string pathNext = string.Empty;
                if (i + 1 < fumeListTuple.Count)
                {
                    pathNext = fumeListTuple[i + 1].Item3;
                }
                while (!string.IsNullOrEmpty(pathNext) && pathNext.Contains(path) == true)
                {
                    cnt++;
                    if (i + cnt >= fumeListTuple.Count) break;
                    pathNext = fumeListTuple[i + cnt].Item3;
                }
                if (cnt == 0)
                {
                    // add valuePeriod 當start/ end都沒有時
                }
                else
                {
                    for (int j = 1; j < cnt; j++)
                    {
                        path = fumeListTuple[i + j].Item3;
                        path = path.Replace("[x]", "Period");
                        type = fumeListTuple[i + j].Item2;
                        fumeInfo = fumeListTuple[i + j].Item1;
                        Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fumeInfo, type, path);
                        periodFUME.Add(fumeTuple);
                    }
                    i = i + cnt - 1;
                }
            }
            else
            {
                periodFUME.Add(fume);
            }
        }

        return periodFUME;
    }
    private List<Tuple<string, string, string>> GetFUMEQuantity(List<Tuple<string, string, string>> fumeListTuple)
    {
        List<Tuple<string, string, string>> quantityFUME = new List<Tuple<string, string, string>>();
        for (int i = 0; i < fumeListTuple.Count; i++)
        {
            var fume = fumeListTuple[i];
            string fumeInfo = fume.Item1;
            string type = fume.Item2;
            string path = fume.Item3;
            if (type == "Quantity")
            {
                quantityFUME.Add(fume);
                int cnt = 0;
                string pathNext = string.Empty;
                if (i + 1 < fumeListTuple.Count)
                {
                    pathNext = fumeListTuple[i + 1].Item3;
                }
                while (!string.IsNullOrEmpty(pathNext) && pathNext.Contains(path) == true)
                {
                    cnt++;
                    if (i + cnt >= fumeListTuple.Count) break;
                    pathNext = fumeListTuple[i + cnt].Item3;
                }
                if (cnt == 0)
                {
                    string fumeName = fumeInfo.Replace("*", "").Trim();
                    fumeInfo = "  " + fumeInfo.Replace(fumeName, "value");
                    path = path + ".value";
                    type = "decimal";
                    Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fumeInfo, type, path);
                    quantityFUME.Add(fumeTuple);
                }
                else
                {

                    for (int j = 1; j < cnt; j++)
                    {
                        path = fumeListTuple[i + j].Item3;
                        path = path.Replace("[x]", "Quantity");
                        type = fumeListTuple[i + j].Item2;
                        fumeInfo = fumeListTuple[i + j].Item1;
                        Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fumeInfo, type, path);
                        quantityFUME.Add(fumeTuple);
                    }
                    i = i + cnt - 1;

                }
            }
            else
            {
                quantityFUME.Add(fume);
            }
        }

        return quantityFUME;
    }
    private List<Tuple<string, string, string>> GetFUMECodeable(string profileName, List<Tuple<string, string, string>> fumeListTuple)
    {
        // Clean the FUME list
        // This is a placeholder implementation, replace with actual logic
        List<Tuple<string, string, string>> codeableFUME = new List<Tuple<string, string, string>>();

        for (int i = 0; i < fumeListTuple.Count; i++)
        {
            var fume = fumeListTuple[i];
            string fumeInfo = fume.Item1;
            string type = fume.Item2;
            string path = fume.Item3;

            string system = string.Empty;
            string code = string.Empty;
            bool isCodeable = false;

            if (type == "CodeableConcept")
            {
                List<string> pathList = path.Split(".").ToList();
                pathList.RemoveAt(0);
                path = string.Join(".", pathList);
                if (path.EndsWith("[x]"))
                {
                    path = path.Replace("[x]", "") + "CodeableConcept";
                }
                foreach (ListViewItem item in lvStaging.Items)
                {
                    string applyModelPath = item.SubItems[2].Text;
                    if (path == applyModelPath)
                    {
                        path = profileName + "." + path;
                        if (urlList.ContainsKey(path))
                        {
                            system = urlList[path];
                        }
                        else
                        {
                            system = string.Empty;
                        }
                        code = item.SubItems[1].Text;
                        isCodeable = true;
                        break;
                    }
                }
                if (isCodeable == true)
                {
                    /*
                    if (i + 3 >= fumeListTuple.Count)
                    {
                        codeableFUME.Add(fume);
                        break;
                    }
                    */
                    for (int j = i; j < i + 4; j++)
                    {
                        path = fumeListTuple[j].Item3;
                        type = fumeListTuple[j].Item2;
                        if (type == "code")
                        {
                            fumeInfo = fumeListTuple[j].Item1;
                            Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fumeInfo + " = " + code, type, path);
                            codeableFUME.Add(fumeTuple);
                        }
                        else if (type == "uri")
                        {
                            fumeInfo = fumeListTuple[j].Item1;
                            Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fumeInfo, type, path);
                            if (fumeInfo.Contains("=") == false)
                            {
                                fumeTuple = new Tuple<string, string, string>(fumeInfo + " = " + "\"" + system + "\"", type, path);
                            }
                            codeableFUME.Add(fumeTuple);
                        }
                        else
                        {
                            codeableFUME.Add(fumeListTuple[j]);
                        }
                    }
                    i = i + 3;
                }
                else
                {
                    codeableFUME.Add(fume);
                }
            }
            else
            {
                codeableFUME.Add(fume);
            }

        }

        return codeableFUME;
    }
    private List<Tuple<string, string, string>> GetFUMEPattern(StructureDefinition sd, List<Tuple<string, string, string>> fumeListTuple)
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
                    string patternType = value.TypeName ?? string.Empty;
                    if (patternType == "CodeableConcept")
                    {
                        var patternCodeableConcept = obj.Pattern as CodeableConcept;
                        if (patternCodeableConcept != null)
                        {
                            var coding = patternCodeableConcept.Coding.FirstOrDefault() as Coding;
                            if (coding != null)
                            {
                                string system = coding.System;
                                string code = coding.Code;
                                for (int j = i; j < i + 3; j++)
                                {
                                    isPattern = true;
                                    Tuple<string, string, string> fumeTuple = fumeListTuple[j];
                                    string pathFUME = fumeListTuple[j].Item3;
                                    string type = fumeListTuple[j].Item2;
                                    if (type == "code")
                                    {
                                        string fumeInfo = fumeListTuple[j].Item1;
                                        fumeTuple = new Tuple<string, string, string>(fumeInfo + " = " + "\"" + code + "\"", type, pathFUME);
                                        patternFUME.Add(fumeTuple);
                                    }
                                    else if (type == "uri")
                                    {
                                        string fumeInfo = fumeListTuple[j].Item1;
                                        fumeTuple = new Tuple<string, string, string>(fumeInfo + " = " + "\"" + system + "\"", type, pathFUME);
                                        patternFUME.Add(fumeTuple);
                                    }
                                    else
                                    {
                                        patternFUME.Add(fumeTuple);
                                    }
                                }
                                i = i + 3;
                            }
                        }
                    }
                    string pattern = value != null ? value.ToString() ?? string.Empty : string.Empty;
                    if (pattern != string.Empty)
                    {
                        var fume = fumeListTuple[i];
                        string fumeInfo = fume.Item1;
                        string type = fume.Item2;
                        string pathFUME = fume.Item3;
                        patternFUME.Add(new Tuple<string, string, string>(fumeInfo + " = " + "\"" + pattern + "\"", type, pathFUME));
                        isPattern = true;
                    }
                }
            }
            if (isPattern == false) patternFUME.Add(fumeListTuple[i]);
        }
        return patternFUME;
    }

    private string GetPathForX(string path, string type)
    {
        string pathX = string.Empty;
        // Check if the path contains "[x]" 
        if (path.EndsWith("[x]"))
        {
            if (type == "dateTime") path = path.Replace("[x]", "") + "DateTime";
            else if (type == "string") path = path.Replace("[x]", "") + "String";
            else if (type == "integer") path = path.Replace("[x]", "") + "Integer";
            else if (type == "decimal") path = path.Replace("[x]", "") + "Decimal";
            else if (type == "boolean") path = path.Replace("[x]", "") + "Boolean";
            else if (type == "uri") path = path.Replace("[x]", "") + "Uri";
            else if (type == "base64Binary") path = path.Replace("[x]", "") + "Base64Binary";
            else if (type == "code") path = path.Replace("[x]", "") + "Code";
            else if (type == "oid") path = path.Replace("[x]", "") + "Oid";
            else if (type == "uuid") path = path.Replace("[x]", "") + "Uuid";
            else if (type == "string") path = path.Replace("[x]", "") + "String";
            else if (type == "integer") path = path.Replace("[x]", "") + "Integer";
            else if (type == "decimal") path = path.Replace("[x]", "") + "Decimal";
            else if (type == "boolean") path = path.Replace("[x]", "") + "Boolean";
            else if (type == "uri") path = path.Replace("[x]", "") + "Uri";
            else if (type == "base64Binary") path = path.Replace("[x]", "") + "Base64Binary";
            else if (type == "code") path = path.Replace("[x]", "") + "Code";
            else if (type == "oid") path = path.Replace("[x]", "") + "Oid";
            else if (type == "uuid") path = path.Replace("[x]", "") + "Uuid";
        }
        else
        {
            // If the path does not contain "[x]", just return the original path
            pathX = path;
        }
        // This is a placeholder implementation, replace with actual logic
        return path;
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
            path = GetPathForX(path, type);
            if (type == "dateTime")
            {
                var updatedFume = new Tuple<string, string, string>(fume.Item1 + " = " + GetStagingApplyModel(path), fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            else if (type == "Reference")
            {
                valuedFUME.Add(fume);
                string fumeName = fumeInfo.Replace("*", "").Trim();
                path = path + ".reference";
                fumeInfo = "  " + fumeInfo.Replace(fumeName, "reference") + " = " + GetStagingApplyModel(path);
                var updatedFume = new Tuple<string, string, string>(fumeInfo, fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            else if (type == "boolean" || type == "decimal")
            {
                var updatedFume = new Tuple<string, string, string>(fume.Item1 + " = " + GetStagingApplyModel(path), fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            else if (type == "date" || type == "base64Binary")
            {
                var updatedFume = new Tuple<string, string, string>(fume.Item1 + " = " + GetStagingApplyModel(path), fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            else if (type == "integer" || type == "string")
            {
                string applyModel = GetStagingApplyModel(path);
                if (applyModel == string.Empty)
                {
                    // applyModel type錯誤，未來可能修改
                    applyModel = GetStagingApplyModel("value");
                }
                var updatedFume = new Tuple<string, string, string>(fume.Item1 + " = " + applyModel, fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            else if (type == "code" || type == "url")
            {
                if (fume.Item1.Contains("="))
                {
                    valuedFUME.Add(fume);
                    continue;
                }
                var updatedFume = new Tuple<string, string, string>(fume.Item1 + " = " + GetStagingApplyModel(path), fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            else
            {
                valuedFUME.Add(fume);
            }
        }

        return valuedFUME;
    }

    private List<Tuple<string, string, string>> GetFUMEValueWithRule(List<Tuple<string, string, string>> fumeListTuple)
    {
        // Clean the FUME list
        // This is a placeholder implementation, replace with actual logic
        List<Tuple<string, string, string>> ruleFUME = new List<Tuple<string, string, string>>();

        StructureDefinition sd = new StructureDefinition();
        string profileName = lbStaging.SelectedItem != null ? lbStaging.SelectedItem.ToString() ?? string.Empty : string.Empty;

        sd = GetStructureDefinition(profileName).Result;
        if (sd == null)
        {
            MessageBox.Show("Failed to resolve the StructureDefinition.");
            return ruleFUME;
        }

        for (int i = 0; i < fumeListTuple.Count; i++)
        {
            var fume = fumeListTuple[i];
            string rule = "StructureDefinition.differential.element.where(path = '" + fume.Item3 + "')";
            var result = sd.IsTrue(rule);
            bool isRule = false;
            if (result)
            {
                var obj = sd.Select(rule).FirstOrDefault() as ElementDefinition;
                if (obj != null)
                {
                    string elementRule = obj.ElementId;
                    if (elementRule != null && elementRule.Contains(":"))
                    {
                        elementRule = elementRule.Split(":")[1];
                        elementRule = elementRule.Split(".")[0];
                        // Claim特殊作法，未來可能修改
                        string path = fume.Item3;
                        //path = path.ToLower();
                        path = path.Replace("[x]", "Quantity");
                        List<string> pathList = path.Split(".").ToList();
                        //remove the first element of the list
                        pathList.RemoveAt(0);
                        pathList.Add("value");
                        //join the list to a string
                        path = string.Join(".", pathList);
                        string applyModel = GetStagingApplyModelWithRule(path, elementRule);
                        if (applyModel != string.Empty)
                        {
                            var updatedFume = new Tuple<string, string, string>(fume.Item1 + " = " + applyModel, fume.Item2, fume.Item3);
                            isRule = true;
                            ruleFUME.Add(updatedFume);
                        }

                    }
                }
            }
            if (isRule == false)
            {
                ruleFUME.Add(fume);
            }

        }
        return ruleFUME;
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
                else
                {
                    type1 = string.Empty; //最後一個元素，type = CodeableConcept
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
                    if (i < fumeListTuple.Count - 2) type2 = fumeListTuple[i + 2].Item2;
                    if (i < fumeListTuple.Count - 3) type3 = fumeListTuple[i + 3].Item2;
                    if (type2 == "uri" && type3 != "code")
                    {
                        fume = fumeListTuple[i + 2];
                        fumeInfo = fume.Item1;
                        fumeAddTuple = new Tuple<string, string, string>(fumeInfo.Replace("system", "code"), "code", path + ".coding.code");
                        indexAdd = 2;
                    }
                    else if (type2 == "code" && type3 != "uri")
                    {
                        fume = fumeListTuple[i + 2];
                        fumeInfo = fume.Item1;
                        fumeAddTuple = new Tuple<string, string, string>(fumeInfo.Replace("code", "system"), "uri", path + ".coding.system");
                        indexAdd = 2;
                    }
                    else
                    {
                        string fumeName = fumeInfo.Replace("*", "").Trim();
                        Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>("  " + fumeInfo.Replace(fumeName, "coding"), "Coding", path + ".coding");
                        refinedFUME.Add(fumeTuple);
                        fumeTuple = new Tuple<string, string, string>("    " + fumeInfo.Replace(fumeName, "system"), "uri", path + ".coding.system");
                        refinedFUME.Add(fumeTuple);
                        fumeTuple = new Tuple<string, string, string>("    " + fumeInfo.Replace(fumeName, "code"), "code", path + ".coding.code");
                        refinedFUME.Add(fumeTuple);
                        i = i + 1;
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
                Tuple<bool, List<int>> codeableTuple = CheckCodeableConcept(i, refinedFUME);
                bool isCodeableConcept = codeableTuple.Item1;
                List<int> codeableConceptList = codeableTuple.Item2;
                //if (!isCodeableConcept) continue;
                for (int j = 0; j < codeableConceptList.Count; j++)
                {
                    int index = codeableConceptList[j] - j;
                    refinedFUME.RemoveAt(index);
                }
            }

        }
        return refinedFUME;
    }

    private Tuple<bool, List<int>> CheckCodeableConcept(int index, List<Tuple<string, string, string>> fumeListTuple)
    {
        List<int> codeableConceptList = new List<int>();
        // Check if the type is CodeableConcept
        bool isCodeableConcept = true;
        int indexConcept = index;
        int indexCoding = index + 1;
        int indexSystm = index + 2;
        int indexCode = index + 3;
        string path = fumeListTuple[index + 1].Item3;
        bool isCodeable = true;
        if (fumeListTuple[index].Item2 != "CodeableConcept") isCodeableConcept = false;
        if (fumeListTuple[indexCoding].Item2 != "coding") isCodeableConcept = false;
        if (fumeListTuple[indexSystm].Item2 != "system") isCodeableConcept = false;
        if (fumeListTuple[indexCode].Item2 != "code") isCodeableConcept = false;

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


    private List<Tuple<string, string, string>> CleanFumeTupleList(List<Tuple<string, string, string>> fumeTupleList)
    {
        // Clean up the fumeTupleList
        List<Tuple<string, string, string>> targetFumeTupleList = new List<Tuple<string, string, string>>();

        targetFumeTupleList = fumeTupleList;
        if (targetFumeTupleList[0].Item1.Length == 0)
        {
            targetFumeTupleList.RemoveAt(0);
        }
        for (int i = 1; i < targetFumeTupleList.Count; i++)
        {
            string fume1 = targetFumeTupleList[i - 1].Item1;
            string fume2 = targetFumeTupleList[i].Item1;
            string path1 = targetFumeTupleList[i - 1].Item3;
            string path2 = targetFumeTupleList[i].Item3;
            if (fume1 == fume2)
            {
                targetFumeTupleList.RemoveAt(i);
                i--;
            }
            if (path2.EndsWith("coding") == true && path2.Contains(path1) == false)
            {
                string fume = fume2.Replace("coding", "code");
                fume = fume.Substring(2);
                string path = path2.Replace(".coding", "");
                Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fume, "CodeableConcept", path);
                targetFumeTupleList.Insert(i, fumeTuple);
                i++;
            }
            if (fume2.Trim().EndsWith("="))
            {
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
        // read FUME template from  D:\Hongyu\Project\data\IGAnalyzer\fume\template.json
        string fumeTemplate = File.ReadAllText(@"D:\Hongyu\Project\data\IGAnalyzer\fume\template.json");

        // Deserialize the JSON string to a FHIR resource
        StructureMap map = new FhirJsonParser().Parse<StructureMap>(fumeTemplate);

        map.Id = lbStaging.SelectedItem?.ToString() ?? string.Empty;
        map.Id = map.Id.Replace("-", "");

        var expression = new Expression();
        expression.Language = "application/vnd.outburn.fume";
        expression.Expression_ = txtFume.Text;

        var meta = new Meta();
        meta.VersionId = "1.0";
        meta.LastUpdated = DateTimeOffset.Now;
        meta.Source = "http://tmhtc.net/StructureMap/" + map.Id;
        map.Meta = meta;

        // Add the expression to the StructureMap
        map.Group[0].Rule[0].Extension[0].Value = expression;
        // Serialize the StructureMap to JSON
        var json = new FhirJsonSerializer(new SerializerSettings() { Pretty = true }).SerializeToString(map);
        txtFHIRData.Text = json;
    }

    private async void btnSaveFHIR_Click(object sender, EventArgs e)
    {
        string jsonFHIR = txtFHIRData.Text;

        if (string.IsNullOrEmpty(jsonFHIR))
        {
            MessageBox.Show("Please generate FHIR data first.");
            return;
        }
        // Upload the FHIR data to the server using existed client
        FhirClient client = new FhirClient(txtFHIRServer.Text);

        //Serialize the JSON string to a FHIR resource
        var resource = new FhirJsonParser().Parse<Resource>(jsonFHIR);
        // Save the resource to the server using "put" method

        var result = await client.UpdateAsync(resource);
        if (result != null)
        {
            MessageBox.Show("FHIR data saved successfully. Resource id :" + result.Id);
            txtFHIRData.Text = result.ToJson();
        }
        else
        {
            MessageBox.Show("Failed to save FHIR data.");
        }
    }

    private void tabStaging_Enter(object sender, EventArgs e)
    {
        lbStaging.Items.Clear();
        foreach (var profile in profiles)
        {
            // check if the profile is contained in the qList
            foreach (var q in qList)
            {
                if (q.Item2.Contains(profile))
                {
                    lbStaging.Items.Add(profile);
                    break;
                }
            }

        }
    }

    private async void btnStagingLoad_Click(object sender, EventArgs e)
    {
        foreach (ListViewItem item in lvStaging.Items)
        {

            string profile = lbStaging.SelectedItem?.ToString() ?? string.Empty;
            string path = item.SubItems[2].Text;
            string type = item.SubItems[3].Text;
            if (path.Contains(")")) continue;
            string typePlus = await GetSnapshotType(profile, path);
            if (typePlus != string.Empty && type != typePlus)
            {
                item.SubItems[3].Text = typePlus;
                item.ForeColor = Color.Blue;
                txtMsg.Text = txtMsg.Text + "Path: " + path + "  Type: (" + type + " , " + typePlus + ")" + Environment.NewLine;
            }
        }
        lvStaging.Refresh();
    }

    private List<Tuple<string, string, string>> GetFUMEList(string fume)
    {
        List<Tuple<string, string, string>> fumeList = new List<Tuple<string, string, string>>();
        // Split the FUME string into lines
        List<string> lines = fume.Split(Environment.NewLine).ToList();
        foreach (string line in lines)
        {
            List<string> parts = line.Split("__", StringSplitOptions.RemoveEmptyEntries).ToList();
            if (parts.Count == 3)
            {
                // remove [] from the path
                string path = parts[2].Trim();
                path = path.Substring(1, path.Length - 2);
                string type = parts[1].Trim();
                type = type.Substring(1, type.Length - 2);
                fumeList.Add(new Tuple<string, string, string>(parts[0], type, path));
            }
        }
        return fumeList;
    }

    private string RefinePath(string path, string type)
    {
        string pathX = string.Empty;
        if (path.Contains("[x]"))
        {
            if (type == "dateTime") pathX = path.Replace("[x]", "") + "DateTime";
            else if (type == "Reference") pathX = path + ".reference";
            else if (type == "string") pathX = path.Replace("[x]", "") + "String";
            else if (type == "integer") pathX = path.Replace("[x]", "") + "Integer";
            else if (type == "decimal") pathX = path.Replace("[x]", "") + "Decimal";
            else if (type == "boolean") pathX = path.Replace("[x]", "") + "Boolean";
            else if (type == "uri") pathX = path.Replace("[x]", "") + "Uri";
            else if (type == "base64Binary") pathX = path.Replace("[x]", "") + "Base64Binary";
            else if (type == "code") pathX = path.Replace("[x]", "") + "Code";
            else if (type == "oid") pathX = path.Replace("[x]", "") + "Oid";
            else if (type == "uuid") pathX = path.Replace("[x]", "") + "Uuid";

        }
        else
        {
            pathX = path;
        }
        return pathX;
    }
    private void btnConfirm_Click(object sender, EventArgs e)
    {
        List<Tuple<string, string, string>> checkFUMEList = new List<Tuple<string, string, string>>();

        string txtFumeValue = txtFHIRData.Text;
        if (txtFumeValue == string.Empty)
        {
            MessageBox.Show("Please input FUME first.");
            return;
        }
        // transform the FUME to checkFUMEList
        checkFUMEList = GetFUMEList(txtFumeValue);
        int error = 0;
        string fumeString = txtFume.Text;

        for (int i = 0; i < lvStaging.Items.Count; i++)
        {
            ListViewItem item = lvStaging.Items[i];
            string path = item.SubItems[2].Text;
            string type = item.SubItems[3].Text;
            string applyModel = item.SubItems[1].Text;
            bool isExist = false;
            bool isMatch = false;

            if (type == "BackboneElement") continue;
            for (int j = 0; j < checkFUMEList.Count; j++)
            {
                var fume = checkFUMEList[j];
                List<string> pathList = fume.Item3.Split(".").ToList();
                pathList.RemoveAt(0);
                string itemPath = string.Join(".", pathList);
                string itemType = fume.Item2;
                //itemPath = RefinePath(itemPath, itemType);
                if (itemPath == path)
                {
                    isExist = true;
                    break;
                }
            }
            isMatch = fumeString.Contains(applyModel);

            if (isExist == false && isMatch == false)
            {
                item.BackColor = Color.Red;
                item.ForeColor = Color.White;
                error++;
            }
            else if (isExist == true && isMatch == false)
            {
                item.BackColor = Color.Blue;
                item.ForeColor = Color.White;
                error++;
            }
            else if (isExist == false && isMatch == true)
            {
                item.BackColor = Color.Yellow;
                item.ForeColor = Color.Black;
                error++;
            }
        }
        if (error == 0)
        {
            MessageBox.Show("All items are correct.");
        }
    }
    private void btnConfirm_Click2(object sender, EventArgs e)
    {
        List<Tuple<string, string, string>> checkFUMEList = new List<Tuple<string, string, string>>();

        string txtFumeValue = txtFHIRData.Text;
        if (txtFumeValue == string.Empty)
        {
            MessageBox.Show("Please input FUME first.");
            return;
        }

        // transform the FUME to checkFUMEList
        checkFUMEList = GetFUMEList(txtFumeValue);

        // check the lvStaging items exist in the checkFUMEList
        for (int i = 0; i < checkFUMEList.Count; i++)
        {
            var fume = checkFUMEList[i];
            List<string> pathList = fume.Item3.Split(".").ToList();
            pathList.RemoveAt(0);
            string path = string.Join(".", pathList);
            string type = fume.Item2;

            bool isExist = false;
            ListViewItem checkItme = new ListViewItem();
            string itemPath = string.Empty;
            string itemType = string.Empty;
            string applyModel = string.Empty;
            int index = 0;
            string fumeString = txtFume.Text;
            for (int j = 0; j < lvStaging.Items.Count; j++)
            {
                ListViewItem item = lvStaging.Items[j];
                itemPath = item.SubItems[2].Text;
                itemType = item.SubItems[3].Text;
                applyModel = item.SubItems[1].Text;
                if (itemPath == path)
                {
                    isExist = true;
                    checkItme = item;
                    index = j;
                    break;
                }
            }

            if (isExist == true)
            {
                if (type == itemType && fumeString.Contains(applyModel) == true)
                {
                    checkItme.BackColor = Color.LightGreen;
                    checkItme.ForeColor = Color.Black;
                }
                else if (type != itemType)
                {
                    checkItme.BackColor = Color.Yellow;
                    checkItme.ForeColor = Color.Black;
                }
                else if (fumeString.Contains(applyModel) == false)
                {
                    checkItme.BackColor = Color.Blue;
                    checkItme.ForeColor = Color.White;
                }
            }
            else
            {
                checkItme.BackColor = Color.Red;
                checkItme.ForeColor = Color.White;
            }
        }
    }

    private string RemoveFistPart(string path)
    {
        // Remove the first part of the path
        if (path.Contains("."))
        {
            List<string> pathList = path.Split('.').ToList();
            pathList.RemoveAt(0);
            path = string.Join(".", pathList);
        }
        return path;
    }
    List<Tuple<string, string, string>> CreateFUMESlice(string profileName, string pathSlice)
    {

        StructureDefinition sd = GetStructureDefinition(profileName).Result;

        pathSlice = sd.Type + "." + pathSlice;
        //string path = string.Empty;
        int cnt = 0;
        bool isSliceHeader = false;
        string dpath = string.Empty;
        string dtype = string.Empty;
        string typeValue = string.Empty;
        string pathX = string.Empty;
        string nameX = string.Empty;
        List<Tuple<string, string>> sliceTemplates = new List<Tuple<string, string>>();
        List<Tuple<string, string, string>> sliceFume = new List<Tuple<string, string, string>>();

        for (int i = 0; i < sd.Differential.Element.Count; i++)
        {
            var e = sd.Differential.Element[i];

            string path = e.Path;
            string type = e.Type.FirstOrDefault()?.Code ?? string.Empty;
            string name = e.SliceName;
            string pattern = e.Pattern?.ToString() ?? string.Empty;
            string max = e.Max ?? string.Empty;

            if (max == "0") continue;

            if (path.Contains(pathSlice))
            {
                if (e.Slicing != null)
                {
                    foreach (var d in e.Slicing.Discriminator)
                    {
                        dpath = d.Path;
                        dtype = d.Type?.ToString() ?? string.Empty;
                        isSliceHeader = true;
                        continue;
                    }
                }

                if (name != null && name != string.Empty)
                {
                    isSliceHeader = false;
                    cnt++;
                    //string fume = sliceList[0] + " = " + cnt.ToString();
                }

                List<string> pathList = new List<string>();
                pathList = path.Split('.').ToList();
                int level = pathList.Count - 2;
                pathList.RemoveAt(0);
                path = string.Join(".", pathList);
                string sapce = new string(' ', level * 2);
                path = sapce + "* " + path.Split(".").Last();

                if (isSliceHeader)
                {
                    if (type == string.Empty)
                    {
                        string rule = "StructureDefinition.snapshot.element.where(path = '" + e.Path + "')";
                        var obj = sd.Select(rule).FirstOrDefault() as ElementDefinition;
                        if (obj != null && obj.Type != null && obj.Type.Any())
                        {
                            type = obj.Type.FirstOrDefault()?.Code ?? string.Empty;
                        }
                        else
                        {
                            type = string.Empty;
                        }
                    }
                    Tuple<string, string> template = new Tuple<string, string>(path, type);
                    sliceTemplates.Add(template);
                }
                else
                {
                    /*
                    * supportingInfo[BackboneElement][Claim.supportingInfo]  ==> name != null && name != string.Empty
                        * sequence = 1[positiveInt][Claim.supportingInfo]    ==> name != null && name != string.Empty
                        * category[CodeableConcept][Claim.supportingInfo.category] ==> pattern != string.Empty
                            * coding[Coding][Claim.supportingInfo.category.coding]
                            * system = "https://twcore.mohw.gov.tw/ig/pas/CodeSystem/nhi-supporting-info-type"[uri][Claim.supportingInfo.category.coding.system]
                            * code = "weight"[code][Claim.supportingInfo.category.coding.code]
                        * valueQuantity[Quantity][Claim.supportingInfo.valueQuantity] ==> typeValue != string.Empty
                            * value = patientweight[Quantity][Claim.supportingInfo.valueQuantity.value]  typeValue != string.Empty
                            * system = "http://unitsofmeasure.org"[][Claim.supportingInfo.value[x].system] typeValue == string.Empty
                            * code = "kg"[][Claim.supportingInfo.value[x].code] typeValue == string.Empty
                    */
                    if (name != null && name != string.Empty)
                    {
                        string fume = path;
                        Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fume, "BackboneElement", e.Path); // hard code
                        sliceFume.Add(fumeTuple);
                        if (sliceTemplates.Count > 2)
                        {
                            string pathTemplate = sliceTemplates[1].Item1; // hard code
                            List<string> pathListTemplate = new List<string>();
                            pathListTemplate = path.Split('.').ToList();
                            pathListTemplate.RemoveAt(0);
                            int levelTemplate = pathListTemplate.Count - 2;
                            string sapceTemplate = new string(' ', level * 2);
                            fume = pathTemplate + " = " + cnt.ToString();
                            fume = sapceTemplate + fume;
                            Tuple<string, string, string> fumeTupleTemplate = new Tuple<string, string, string>(fume, "positiveInt", e.Path);  // hard code
                            sliceFume.Add(fumeTupleTemplate);
                        }
                        typeValue = string.Empty;
                        nameX = name;
                    }
                    else if (pattern != string.Empty)
                    {
                        string patternType = e.Pattern?.TypeName ?? string.Empty;
                        if (patternType == "CodeableConcept")
                        {
                            var patternCodeableConcept = e.Pattern as CodeableConcept;
                            if (patternCodeableConcept != null)
                            {
                                var codingObj = patternCodeableConcept.Coding.FirstOrDefault();
                                string system = codingObj?.System ?? string.Empty;
                                string code = codingObj?.Code ?? string.Empty;

                                string fume = path;
                                Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fume, patternType, e.Path);
                                sliceFume.Add(fumeTuple);

                                string pathname = path.Replace("*", "").Trim();
                                string coding = "  " + path.Replace(pathname, "coding");
                                fumeTuple = new Tuple<string, string, string>(coding, "Coding", e.Path + ".coding");
                                sliceFume.Add(fumeTuple);

                                string systemPath = "    " + path.Replace(pathname, "system") + " = " + "\"" + system + "\"";
                                fumeTuple = new Tuple<string, string, string>(systemPath, "uri", e.Path + ".coding.system");
                                sliceFume.Add(fumeTuple);

                                string codePath = "    " + path.Replace(pathname, "code") + " = " + "\"" + code + "\"";

                                fumeTuple = new Tuple<string, string, string>(codePath, "code", e.Path + ".coding.code");
                                sliceFume.Add(fumeTuple);

                                pathX = pathname + ".coding.code";

                            }
                        }
                        else if (patternType == "Quantity")
                        {

                        }
                        else
                        {
                            //pattern = pattern.Replace("\"", "");
                            path = path.Replace("[x]", typeValue);
                            string fume = path + " = " + "\"" + pattern + "\"";
                            Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fume, type, e.Path);
                            sliceFume.Add(fumeTuple);
                        }
                    }
                    else if (type != string.Empty)
                    {
                        typeValue = type;
                        // make first caracter of type to upper case
                        type = char.ToUpper(type[0]) + type.Substring(1);
                        path = path.Replace("[x]", type);
                        //string fume = path + " = " + type;
                        //string fume = path + " = " + pathX;
                        string fume = path;
                        string fumePath = e.Path.Replace("[x]", type);
                        Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fume, type, fumePath);
                        sliceFume.Add(fumeTuple);

                        if (type == "Reference")
                        {
                            string pathname = path.Replace("*", "").Trim();
                            string fumeReference = "  " + path.Replace(pathname, "reference");
                            //fumeReference = fumeReference + " : " + pathX + " = " + "'" + nameX + "'";

                            fumePath = e.Path.Replace("[x]", type);
                            fumeTuple = new Tuple<string, string, string>(fumeReference, "Reference", fumePath + ".reference");
                            sliceFume.Add(fumeTuple);
                        }
                    }
                    else
                    {
                        if (typeValue != string.Empty)
                        {
                            path = path.Replace("[x]", typeValue);
                            //string fume = path + " : " + pathX +" = " + "'" + nameX + "'";
                            string fume = path;
                            string fumePath = e.Path.Replace("[x]", typeValue);
                            Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fume, typeValue, fumePath);
                            sliceFume.Add(fumeTuple);
                        }
                        else
                        {
                            string fume = path;
                            Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fume, type, e.Path);
                            sliceFume.Add(fumeTuple);
                        }
                    }
                }
            }
            string slice = string.Empty;
            foreach (var s in sliceFume)
            {
                slice += s.Item1 + "[" + s.Item2 + "]" + "[" + s.Item3 + "]" + Environment.NewLine;
            }
            txtFHIRData.Text = slice;
        }
        return sliceFume;
    }

    private void lbFUME_DoubleClick(object sender, EventArgs e)
    {
        // Get the selected item
        string selectedItem = lbFUME.SelectedItem?.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(selectedItem))
        {
            MessageBox.Show("Please select a FUME item.");
            return;
        }
        else
        {
            MessageBox.Show("Selected FUME item: " + selectedItem);
        }
    }
    private void lvStaging_DoubleClick(object sender, EventArgs e)
    {


    }
    
    private void lvStaging_SelectedIndexChanged(object sender, EventArgs e)
    {
        // show the selected item in the text box
        if (lvStaging.SelectedItems.Count > 0)
        {
            ListViewItem item = lvStaging.SelectedItems[0];
            string applyModel = item.SubItems[1].Text;
            for (int i = 0; i < lbFUME.Items.Count; i++)
            {
                var itemFUME = lbFUME.Items[i];
                string? fume = itemFUME?.ToString();
                if (!string.IsNullOrEmpty(fume))
                {
                    if (fume.Contains(applyModel))
                    {
                        lbFUME.SelectedItem = itemFUME;
                        string staging = txtStaging.Text;
                        int index = staging.IndexOf(applyModel);
                        if (index >= 0)
                        {
                            txtStaging.SelectionStart = index-1;
                            txtStaging.SelectionLength = applyModel.Length;
                            txtStaging.ScrollToCaret();
                            // get the following text after the applyModel
                            string followingText = txtStaging.Text.Substring(index + applyModel.Length);
                            // Find the next line break after the applyModel
                            int nextLineBreak = followingText.IndexOf(Environment.NewLine);
                            if (nextLineBreak >= 0)
                            {
                                // Select the text until the next line break
                                txtStaging.SelectionLength += nextLineBreak + Environment.NewLine.Length;
                            }
                            else
                            {
                                // Select until the end of the text
                                txtStaging.SelectionLength = txtStaging.Text.Length - index;
                            }
                            // Set focus to the txtStaging
                            txtStaging.Focus();
                        }
                        else
                        {
                            txtStaging.SelectionStart = 0;
                            txtStaging.SelectionLength = 0;
                        }
                        break;
                    }
                    else
                    {
                        lbFUME.SelectedItem = null;
                    }
                }
            }
        }
        else
        {
            lbFUME.SelectedItem = null;
        }
    }
}