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
using System.Windows.Forms.VisualStyles;
using System.Reflection;
using Hl7.Fhir.Utility;
using System.Net.Sockets;
using Hl7.Fhir.ElementModel;
using System.DirectoryServices.ActiveDirectory;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics;
using System.Drawing.Imaging.Effects;


namespace IGAnalyzer;

using Firely.Fhir.Validation;
using Hl7.Fhir.Validation;
using System.Security.Cryptography.Xml;
using System.Text;

using Hl7.Fhir.Specification.Source;
using System.Collections;
using System.Drawing.Text;

public partial class FormIGAnalyzer : Form
{
    private IGClass ig = new IGClass();
    private AppSettings appSettings = new AppSettings();

    private FHIRData fhirData = new FHIRData();
    private string profilePath = string.Empty;
    private string profileName = string.Empty;

    private string igName = string.Empty;

    private string igSubName = string.Empty;

    private ModuleFHIR.FHIROperator fhirOperator = new ModuleFHIR.FHIROperator();
    FhirPackageSource? resolver;

    Validator? validator;

    private string profileDirectory = "profiles\\";

    private string stagingDirectory = "staging\\";

    private FhirClient? client;

    public FormIGAnalyzer()
    {
        InitializeComponent();
        ReadDefaultProfile();

        profilePath = txtDataDirectory.Text + profileDirectory + igName;
        profileName = "https://nhicore.nhi.gov.tw/pas/" + igName + "/StructureDefinition";
        FormListView form = new FormListView();
        //form.Show(); // Use the private member by showing the form
    }

    private void ReadDefaultProfile()
    {
        // Read the default profile from the Application.json file
        appSettings.Load();
        igName = appSettings.IGName ?? string.Empty;

        if (string.IsNullOrEmpty(igName) || string.IsNullOrEmpty(appSettings.DataDirectory) || string.IsNullOrEmpty(appSettings.FHIRServer))
        {
            MessageBox.Show("Default profile settings are not properly configured.");
            return;
        }
        cmbIG.Text = igName;
        txtDataDirectory.Text = appSettings.DataDirectory ?? string.Empty;
        txtFHIRServer.Text = appSettings.FHIRServer ?? string.Empty;
    }

    private void Initial()
    {
        ig.Clear();
        txtMsg.Text = string.Empty;
        txtPackage.Text = string.Empty;
        txtQuestionnaire.Text = string.Empty;
        lbProfile.Items.Clear();
        lbApplyModel.Items.Clear();
        lvElement.Items.Clear();
        lbProfile.Items.Clear();
        lbStaging.Items.Clear();
        lbBundleList.Items.Clear();
        lvBinding.Items.Clear();
        lvApplyModel.Items.Clear();
        lvElement.Items.Clear();
        client = new FhirClient(appSettings.FHIRServer);
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
            profileName = "https://nhicore.nhi.gov.tw/" + igName + "/StructureDefinition";
        }
        //ShowIGExample();

    }

    private void btnClose_Click(object? sender, EventArgs e)
    {
        this.Close();
    }

    private void GenerateBinding(Hl7.Fhir.Model.ElementDefinition element, string profileName, Dictionary<string, string> binding)
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
                if (binding.ContainsKey(code))
                {
                    return; // Or continue if called from a loop, but return is fine for a helper
                }
                binding.Add(code, valueSetUri);
            }
            catch (Exception err)
            {
                txtMsg.Text = txtMsg.Text + $"Error processing {profileName} {element.Path}: {err.Message}" + Environment.NewLine;
            }
        }
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
    /*
    功能轉移至IGClass，先保留
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
    */
    private async void btnSelect_ClickAsync(object? sender, EventArgs e)
    {

        Initial();
        //string tw_ig = ofdPackage.FileName;
        string tw_ig = profilePath + "\\package.tgz";
        ig.Package = tw_ig;
        ig.Name = igName;
        ig.canonical = profileName;
        ig.SubName = igSubName;
        ig.LoadCanonical();
        ig.ICD10PCSVersion = appSettings.ICD10PCSVersion ?? string.Empty;

        resolver = new(ModelInfo.ModelInspector, new string[] { ig.Package });
        if (resolver == null)
        {
            txtMsg.Text = txtMsg.Text + "Failed to initialize the resolver." + Environment.NewLine;
            tabIG.SelectedTab = tabMsg;
            return;
        }


        txtPackage.Text = $"{ig.IGName} {ig.Title} {ig.Version}";
        // Display the results in the ListBox
        lbProfile.Items.Clear();
        foreach (var profile in ig.Profiles)
        {
            lbProfile.Items.Add(profile);
        }

        foreach (var bundle in ig.Bundles)
        {
            lbBundleList.Items.Add(bundle);
        }

        foreach (var binding in appSettings.BindingsAdd)
        {
            if (!string.IsNullOrEmpty(binding.Path) && !string.IsNullOrEmpty(binding.ValueSet))
            {
                ig.Binding.Add(binding.Path, binding.ValueSet);
            }
        }
        /*
                foreach (string profileName in ig.Profiles)
                {

                    var sd = await resolver.ResolveByUriAsync("StructureDefinition/" + profileName) as StructureDefinition;
                    if (sd is not StructureDefinition resolvedProfile)
                    {
                        txtMsg.Text = txtMsg.Text + "Failed to resolve the StructureDefinition." + Environment.NewLine;
                        return;
                    }

                    foreach (var element in sd.Differential.Element)
                    {
                        GenerateBinding(element, profileName, ig.Binding);
                    }

                    foreach (var element in sd.Snapshot.Element)
                    {
                        GenerateBinding(element, profileName, ig.Binding);
                    }
                    foreach (var element in sd.Differential.Element)
                    {
                        string slice = string.Empty;
                        slice = GetElementSlicing(element);
                        if (slice != string.Empty)
                        {
                            string path = element.Path;
                            path = path.Replace(sd.Type + ".", "");
                            ig.AddSliceItem(sd.Id, path, slice);
                        }
                    }
                }
        */
        foreach (var s in ig.SliceList)
        {
            txtMsg.Text = txtMsg.Text + s.Item1 + " % " + s.Item2 + " % " + s.Item3 + Environment.NewLine;
        }

        var resolvedDefinition = await resolver.ResolveByUriAsync("StructureDefinition/" + ig.LogicModel);
        if (resolvedDefinition is not StructureDefinition applyModelDef)
        {
            txtMsg.Text = txtMsg.Text + "Failed to resolve the ApplyModel definition." + Environment.NewLine;
            return;
        }

        lbApplyModel.Items.Clear();
        /*
                foreach (var ele in applyModelDef.Differential.Element)
                {
                    var elementList = ele.Path.Split('.').ToList();
                    if (elementList.Count > 1)
                    {
                        //if (elementList.Count == 2)
                        //{
                        //    lbApplyModel.Items.Add(ele.Path + " | " + ele.Short);
                        //}

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
                                //q3 = ModifyByIGPackage(q3);
                                string q1 = ele.Short;
                                if (ele.Short.Contains("，"))
                                {
                                    q1 = ele.Short.Split("，")[0];
                                }
                                //  IG Package問題，未來修改
                                ig.AddQItem(q1 + " | " + ele.Path, GetProfileName(m.Identity, ig.Profiles), q3, ele.Type.FirstOrDefault()?.Code ?? string.Empty);
                                //var q = new Tuple<string, string, string, string>(q1 + " | " + ele.Path, GetProfileName(m.Identity, ig.Profiles), q3, ele.Type.FirstOrDefault()?.Code ?? string.Empty);
                                //qList.Add(q);
                            }
                        }
                        // ApplyModel 外掛，for specimen

                        // 依據第一個欄位排序
                        //g.QList.Sort((x, y) => string.Compare(x.Item1, y.Item1, StringComparison.Ordinal));
                    }
                }
        */
        foreach (var item in ig.LogicModelList)
        {
            lbApplyModel.Items.Add(item);
        }

        for (int i = 0; i < ig.QList.Count; i++)
        {
            var q = ig.QList[i];
            string q3 = q.Item3;
            q3 = ModifyByIGPackage(q3);
            ig.QList[i] = new Tuple<string, string, string, string>(q.Item1, q.Item2, q3, q.Item4);
        }

        foreach (var lm in appSettings.LogicModelAdd)
        {
            string name = lm.Name ?? string.Empty;
            string subname = name.Split("|")[1].Trim();
            bool found = false;
            for (int i = 0; i < ig.QList.Count; i++)
            {

                if (ig.QList[i].Item1.Contains(subname))
                {
                    ig.InsertQItem(i, name, lm.Profile ?? string.Empty, lm.Path ?? string.Empty, lm.Type ?? string.Empty);
                    found = true;
                    break;
                }
            }
            if (found == false) ig.AddQItem(name, lm.Profile ?? string.Empty, lm.Path ?? string.Empty, lm.Type ?? string.Empty);
        }

        ShowIGExample();
    }

    private string ModifyByIGPackage(string igPath)
    {
        string q3 = igPath;

        foreach (var upath in appSettings.PathUpdate)
        {
            if (upath.Before != null && upath.After != null)
            {
                q3 = q3.Replace(upath.Before, upath.After);
                continue;
            }
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

        lvBinding.Items.Clear();
        // add header to listview
        lvBinding.Columns.Clear();
        lvBinding.Columns.Add("Path", 400);
        lvBinding.Columns.Add("Strength", 100);
        lvBinding.Columns.Add("ValueSet", 800);

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

        var applyModelDefResult = await resolver.ResolveByUriAsync("StructureDefinition/" + ig.LogicModel);
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
                                var p1 = GetProfileName(m.Identity, ig.Profiles);
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
                    lvBinding.Items.Add(item);
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
    private async void lbApplyModel_SelectedIndexChanged(object sender, EventArgs e)
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
        lvApplyModel.Columns.Add("ValueSet", 400);
        lvApplyModel.Columns.Add("Rule", 400);

        if (lbApplyModel.SelectedItem != null)
        {
            string itemName = lbApplyModel.SelectedItem?.ToString()?.Split('|')[0].Trim() ?? string.Empty;
            // Display the results in the textBox

            foreach (var q in ig.QList)
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
                    if (path.Contains("(") && path.Contains(")"))
                    {
                        //path = path.Replace("(" + rule + ")", "");
                        string q3 = path;
                        int startIndex = q3.IndexOf('(');
                        int lastIndex = q3.LastIndexOf(')');
                        if (startIndex >= 0 && lastIndex > startIndex)
                        {
                            string description = q3.Substring(startIndex + 1, lastIndex - startIndex - 1);
                            //Console.WriteLine($"Description: {description}");
                            q3 = q3.Remove(startIndex, lastIndex - startIndex + 1);
                        }
                        path = q3;
                    }
                    item.SubItems.Add(path);

                    //string code = q.Item2 + "." + q.Item3;
                    string code = q.Item2 + "." + path;
                    string type = q.Item4;
                    string url = string.Empty;
                    if (ig.Binding.ContainsKey(code))
                    {
                        url = ig.Binding[code];
                    }
                    //Get type and binding from StructureDefinition
                    StructureDefinition? sd = null;
                    if (resolver != null)
                    {
                        sd = await GetStructureDefinition(q.Item2);
                        path = ConvertToX(path, type);
                        string ruleBase = "StructureDefinition.snapshot.element.where(path = '" + sd.Type + "." + path + "')";
                        //string ruleBase = "StructureDefinition.differential.element.where(path = '" + sd.Type + "." + path + "')";
                        ElementDefinition? obj = sd.Select(ruleBase).FirstOrDefault() as ElementDefinition;
                        if (obj != null)
                        {
                            if (obj.Type != null && obj.Type.Count > 0)
                            {
                                // if CodeableConcept is in Type, use CodeableConcept else use the first type
                                if (obj.Type.Any(t => t.Code == "CodeableConcept"))
                                {
                                    type = "CodeableConcept";
                                }
                                else
                                {
                                    type = obj.Type.FirstOrDefault()?.Code ?? string.Empty;
                                }
                            }
                            else
                            {
                                // If no type is defined, use the default type
                                type = string.Empty; // Default to string if no type is defined
                            }
                            //string typeProfile = obj.Type.FirstOrDefault()?.Code ?? string.Empty;
                            string urlProfile = obj.Binding?.ValueSet ?? string.Empty;
                            //type = typeProfile;
                            if (url == string.Empty) url = urlProfile;
                        }
                    }
                    if (type == "BackboneElement") continue; // Skip BackboneElement types

                    item.SubItems.Add(type);
                    // Hard coded Claim-twpas.item.modifier.coding.system to get value set URL
                    if (rule.Contains("coding.system"))
                    {
                        url = rule.Split("=")[1].Replace("'", "").Replace(")", "").Trim();
                        url = url.Replace("CodeSystem", "ValueSet");
                    }
                    // Hard coded Claim-twpas.item.modifier.coding.code to get value set URL
                    item.SubItems.Add(url);
                    //check if urlMap contains code
                    if (rule != string.Empty)
                    {
                        //if (url == string.Empty) item.SubItems.Add(string.Empty);
                        //item.SubItems.Add(rule.Replace(")", ""));
                        item.SubItems.Add(rule);
                    }

                    string pathProfile = ConvertToX(q.Item3, q.Item4);
                    string slice = GetSlicingByProfilePath(q.Item2, pathProfile);
                    // if slice is not empty set color to red
                    if (slice != string.Empty && slice.StartsWith("Type"))
                    {
                        item.ForeColor = Color.Red;
                    }

                    lvApplyModel.Items.Add(item);

                    //var qItem = new Tuple<string, string, string, string, string>(q.Item1, q.Item2, path, q.Item4, url);
                    var qItem = new Tuple<string, string, string, string, string>(q.Item1, q.Item2, path, type, url);
                    qItemList.Add(qItem);
                }

            }
            lvApplyModel.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            string title = lbApplyModel.SelectedItem?.ToString()?.Split('|')[1].Trim() ?? string.Empty;
            string json = fhirOperator.GenerateQuestionnaire(title, qItemList);
            txtQuestionnaire.Text = json;
        }
        else
        {
            MessageBox.Show("No item selected.");
        }
    }
    private void lbApplyModel_SelectedIndexChanged2(object sender, EventArgs e)
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

            foreach (var q in ig.QList)
            {
                if (q.Item1.Contains(itemName))
                {
                    // Skip BackboneElement types 

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
                    if (ig.Binding.ContainsKey(code))
                    {
                        url = ig.Binding[code];
                        item.SubItems.Add(url);
                    }
                    //  URL問題，未來修改 ==> Claim-twpasitem.modifier.coding.code 
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
                    //
                    string pathProfile = ConvertToX(q.Item3, q.Item4);

                    string slice = GetSlicingByProfilePath(q.Item2, pathProfile);
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

    private string ConvertToX(string path, string type)
    {
        string pathProfile = path;
        string typeProfile = type;
        // make first letter of typeProfile uppercase
        if (typeProfile.Length > 0)
        {
            typeProfile = char.ToUpper(typeProfile[0]) + typeProfile.Substring(1);
        }
        // replace "q.Item4" with "[x]" in pathProfile
        if (pathProfile.Contains(typeProfile))
        {
            pathProfile = pathProfile.Replace(typeProfile, "[x]");
        }
        return pathProfile;
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

        if (rule.EndsWith(")"))
        {
            rule = rule.Substring(0, rule.Length - 1);
        }
        return rule;
    }

    private string GetSlicingByProfilePath(string profile, string path)
    {
        string slice = string.Empty;

        foreach (var s in ig.SliceList)
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

    private AppSettings.Constraint? GetConstraint(string id)
    {
        string profileName = id.Split('.').FirstOrDefault()?.Trim() ?? string.Empty;
        string path = id.Replace(profileName + ".", "");
        AppSettings.Constraint? constraint = null;

        foreach (var c in appSettings.Constraints)
        {
            if (c.ProfileName == profileName && c.ImplySource == path)
            {
                constraint = c;
                break;
            }
        }
        return constraint;
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

        foreach (var item in questionnaireItem)
        {
            // Display the LinkId and Text of each item
            // Create a new Label for each item in the questionnaire
            Label label = new Label();
            string display = item.Text.Split('|').FirstOrDefault()?.Trim() ?? string.Empty;
            label.Text = $"{display}: ";
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

            AppSettings.Constraint? constraint = null;
            if (item.Type == Questionnaire.QuestionnaireItemType.Choice)
            {

                // Create a ComboBox for choice items
                ComboBox comboBox = new ComboBox();
                comboBox.Name = name; // Set the name of the ComboBox
                comboBox.Width = 400; // Set the width of the ComboBox
                comboBox.Location = new Point(xPos, yPos); // Set the location of the ComboBox
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList; // Set the ComboBox to be a drop-down list
                comboBox.Tag = item.LinkId; // Set the Tag property to the LinkId
                constraint = GetConstraint(item.LinkId);
                if (constraint != null)
                {

                    comboBox.Tag = constraint; // Set the Tag property to the constraint
                    comboBox.SelectedValueChanged += (s, ev) =>
                    {
                        // When the ComboBox text changes, update the constraint value
                        if (comboBox.Tag is AppSettings.Constraint c)
                        {
                            string value = comboBox.SelectedItem?.ToString() ?? string.Empty;
                            // Update the constraint in the appSettings
                            value = value.Split('|').FirstOrDefault()?.Trim() ?? string.Empty;
                            string target = appSettings.GetTargetString(
                                c.ProfileName ?? string.Empty,
                                c.ImplySource ?? string.Empty,
                                value);
                            // get target combox with name
                            var targetComboBox = this.splitQuestionnaire.Panel2.Controls
                                .OfType<ComboBox>()
                                .FirstOrDefault(cb => (cb.Tag as string) == c.ProfileName + "." + c.ImplyTarget);
                            if (targetComboBox != null)
                            {
                                List<string> targets = target.Split(",").ToList();
                                targetComboBox.Items.Clear();
                                foreach (var t in targets)
                                {
                                    targetComboBox.Items.Add(t.Trim());
                                }
                            }
                            else
                            {
                                MessageBox.Show("Target ComboBox not found. Target value: " + target);
                            }
                        }
                    };
                }
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
                if (textBox.Text == string.Empty) continue; // Skip empty text boxes
                questionnaireData.Add(textBox.Name.Replace(".", ""), textBox.Text);
            }
            else if (item is ComboBox comboBox)
            {
                if (comboBox.SelectedItem == null || comboBox.SelectedItem.ToString() == string.Empty) continue; // Skip empty combo boxes
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

    private void ShowLVDialog(ListView lv, string title)
    {
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
        int total = lv.Columns.Count;
        foreach (var col in lv.Columns)
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
            textBox.Text = lv.SelectedItems[0].SubItems[cnt].Text;
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

        form.Text = title;
        form.Size = new Size(xPos + 640, total * (height + 10) + 120); // Set the size of the form
        form.StartPosition = FormStartPosition.CenterParent;
        form.ShowDialog();
    }

    private void lvBinding_DoubleClick(object sender, EventArgs e)
    {
        ShowLVDialog(lvBinding, "Binding Information");
    }

    private void lvConstraint_DoubleClick(object sender, EventArgs e)
    {
        ShowLVDialog(lvConstraint, "Constraint Information");
    }

    private void lvBase_DoubleClick(object sender, EventArgs e)
    {
        ShowLVDialog(lvBase, "Base Information");
    }

    private void lvElement_DoubleClick(object sender, EventArgs e)
    {
        ShowLVDialog(lvElement, "Element Information");
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
                path = ConvertToX(path, lvApplyModel.SelectedItems[0].SubItems[4].Text);
                string slice = GetSlicingByProfilePath(profile, path);

                if (slice != string.Empty && slice.StartsWith("Type"))
                {
                    slice = slice.Replace("Type : ", "").Trim();
                    List<string> sliceList = slice.Split('|').ToList();
                    foreach (var s in sliceList)
                    {
                        // Add the display name to the ComboBox
                        if (s.Contains("Slicing") != true) comboBox.Items.Add(s.Trim());
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
        List<Tuple<string, string, string, string, string>> qItemList = new List<Tuple<string, string, string, string, string>>();

        // copy lvApplyModel to qList
        foreach (ListViewItem item in lvApplyModel.Items)
        {
            if (item.SubItems.Count < 6)
            {
                item.SubItems.Add(string.Empty);
            }
            var q = new Tuple<string, string, string, string, string>(item.SubItems[0].Text + "|" + item.SubItems[1].Text, item.SubItems[2].Text, item.SubItems[3].Text, item.SubItems[4].Text, item.SubItems[5].Text);
            qItemList.Add(q);
        }
        // clear txtQuestionnaire
        txtQuestionnaire.Text = string.Empty;

        // generate questionnaire
        if (lbApplyModel.SelectedItem != null)
        {
            string title = lbApplyModel.SelectedItem?.ToString()?.Split('|')[1].Trim() ?? string.Empty;
            string json = fhirOperator.GenerateQuestionnaire(title, qItemList);
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

    private async void lbSupplemental_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Clear the ListView and TextBox when selection changes
        lvMaster.Items.Clear();
        lvMaster.Columns.Clear();
        txtFHIRData.Text = string.Empty;
        lbReference.SelectedItem = null; // Clear master selection to avoid confusion
        lvReference.SelectedItems.Clear(); // Clear any selected references

        if (lbSupplemental.SelectedItem == null)
        {
            return; // Nothing selected
        }

        try
        {
            client = new FhirClient(txtFHIRServer.Text); // Ensure client is initialized with the latest server URL
            string itemName = lbSupplemental.SelectedItem?.ToString()?.Split('-')[0].Trim() ?? string.Empty;

            switch (itemName)
            {
                case "Encounter":
                    var encounters = await LoadFHIRDataAsync<Encounter>("Encounter");
                    DisplayFHIRData("Encounter", encounters, lvMaster,
                        e => e.Class?.Code ?? "No Code");
                    break;
                case "Observation":
                    var observations = await LoadFHIRDataAsync<Observation>("Observation");
                    DisplayFHIRData("Observation", observations, lvMaster,
                        o => o.Code?.Text ?? "No Code");
                    break;
                case "MedicationRequest":
                    var medicationRequests = await LoadFHIRDataAsync<MedicationRequest>("MedicationRequest");
                    DisplayFHIRData("MedicationRequest", medicationRequests, lvMaster,
                        m => m.Medication?.ToString() ?? "No Medication");
                    break;
                case "Coverage":
                    var coverages = await LoadFHIRDataAsync<Coverage>("Coverage");
                    DisplayFHIRData("Coverage", coverages, lvMaster,
                        c => c.Id ?? "No ID");
                    break;
                case "Procedure":
                    var procedures = await LoadFHIRDataAsync<Procedure>("Procedure");
                    DisplayFHIRData("Procedure", procedures, lvMaster,
                        p => p.Code?.Text ?? "No Code");
                    break;
                case "DiagnosticReport":
                    var diagnosticReports = await LoadFHIRDataAsync<DiagnosticReport>("DiagnosticReport");
                    DisplayFHIRData("DiagnosticReport", diagnosticReports, lvMaster,
                        d => d.Code?.Text ?? "No Code");
                    break;
                default:
                    MessageBox.Show($"非輔助數據範圍，請重新選擇: {itemName}", "重新選擇", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMsg.Text += $"Unsupported supplemental data type selected: {itemName}" + Environment.NewLine;
                    break;
            }
        }
        catch (Exception ex) // Catch issues like invalid FHIR server URL during client creation or other general errors
        {
            MessageBox.Show($"Error processing selection: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtMsg.Text += $"Error in lbSupplemental_SelectedIndexChanged: {ex.Message}" + Environment.NewLine;
        }
    }


    private async void lbMaster_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Clear the ListView and TextBox when selection changes
        lvMaster.Items.Clear();
        lvMaster.Columns.Clear();
        txtFHIRData.Text = string.Empty;
        lbSupplemental.SelectedItem = null; // Clear supplemental selection to avoid confusion
        lvSupplemental.SelectedItems.Clear(); // Clear any selected references

        if (lbReference.SelectedItem == null)
        {
            return; // Nothing selected
        }

        try
        {
            client = new FhirClient(txtFHIRServer.Text); // Ensure client is initialized with the latest server URL
            string itemName = lbReference.SelectedItem?.ToString()?.Split('-')[0].Trim() ?? string.Empty;

            switch (itemName)
            {
                case "Patient":
                    var patients = await LoadFHIRDataAsync<Patient>("Patient");
                    DisplayFHIRData("Patient", patients, lvMaster,
                        p => (p.Name != null && p.Name.Any()) ? p.Name[0].ToString() : "No Name");
                    break;
                case "Organization":
                    var organizations = await LoadFHIRDataAsync<Organization>("Organization");
                    DisplayFHIRData("Organization", organizations, lvMaster,
                        o => o.Name ?? "No Name");
                    break;
                case "Practitioner":
                    var practitioners = await LoadFHIRDataAsync<Practitioner>("Practitioner");
                    DisplayFHIRData("Practitioner", practitioners, lvMaster,
                        p => (p.Name != null && p.Name.Any()) ? p.Name[0].ToString() : "No Name");
                    break;
                case "Substance":
                    var substances = await LoadFHIRDataAsync<Substance>("Substance");
                    DisplayFHIRData("Substance", substances, lvMaster,
                        s => s.Code?.Text ?? "No Code");
                    break;
                case "Media":
                    var media = await LoadFHIRDataAsync<Media>("Media");
                    DisplayFHIRData("Media", media, lvMaster,
                        m => m.Type?.Text ?? "No Type");
                    break;
                case "ImagingStudy":
                    var imagingStudies = await LoadFHIRDataAsync<ImagingStudy>("ImagingStudy");
                    DisplayFHIRData("ImagingStudy", imagingStudies, lvMaster,
                        i => i.Description ?? "No Description");
                    break;
                case "Specimen":
                    var specimens = await LoadFHIRDataAsync<Specimen>("Specimen");
                    DisplayFHIRData("Specimen", specimens, lvMaster,
                        s => s.Type?.Text ?? "No Type");
                    break;
                case "DocumentReference":
                    var documentReferences = await LoadFHIRDataAsync<DocumentReference>("DocumentReference");
                    DisplayFHIRData("DocumentReference", documentReferences, lvMaster,
                        d => d.Description ?? "No Description");
                    break;
                default:
                    MessageBox.Show($"非主數據範圍，請重新選擇: {itemName}", "重新選擇", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMsg.Text += $"Unsupported master data type selected: {itemName}" + Environment.NewLine;
                    break;
            }
        }
        catch (Exception ex) // Catch issues like invalid FHIR server URL during client creation or other general errors
        {
            MessageBox.Show($"Error processing selection: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtMsg.Text += $"Error in lbMaster_SelectedIndexChanged: {ex.Message}" + Environment.NewLine;
        }

    }

    private async Task<IEnumerable<TResource>> LoadFHIRDataAsync<TResource>(string resourceTypeDisplayName) where TResource : Resource, new()
    {
        List<TResource> resources = new List<TResource>();

        if (client == null)
        {
            MessageBox.Show("FHIR client is not initialized.", "Client Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtMsg.Text += "FHIR client not initialized in LoadAndDisplayMasterDataAsync." + Environment.NewLine;
            return resources; // Return empty list
        }

        try
        {
            var bundleResult = await client.SearchAsync<TResource>();
            if (bundleResult == null || bundleResult.Entry.Count == 0)
            {
                txtMsg.Text += $"No {resourceTypeDisplayName} bundle returned from FHIR server or bundle is empty." + Environment.NewLine;
                // Optionally, show a MessageBox to the user
                // MessageBox.Show($"No {resourceTypeDisplayName}s found on the server.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return resources; // Return empty list
            }

            foreach (var entry in bundleResult.Entry)
            {
                if (entry.Resource is TResource resourceInstance)
                {
                    resources.Add(resourceInstance);
                }
            }
        }
        catch (FhirOperationException ex)
        {
            MessageBox.Show($"Error searching for {resourceTypeDisplayName}s: {ex.Message}\nDetails: {ex.Outcome?.ToString()}",
                            "FHIR Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtMsg.Text += $"Error searching for {resourceTypeDisplayName}s: {ex.Message}" + Environment.NewLine;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An unexpected error occurred while searching for {resourceTypeDisplayName}s: {ex.Message}",
                            "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtMsg.Text += $"An unexpected error occurred while searching for {resourceTypeDisplayName}s: {ex.Message}" + Environment.NewLine;
        }
        return resources;
    }

    private void DisplayFHIRData<TResource>(
        string resourceTypeDisplayName,
        IEnumerable<TResource> resources,
        ListView lvMaster,
        Func<TResource, string> getNameSelector) where TResource : Resource, new()
    {
        lvMaster.Items.Clear(); // Clear existing items before adding new ones
        lvMaster.Columns.Clear(); // Clear existing columns

        if (resources == null || !resources.Any())
        {
            // txtMsg.Text += $"No {resourceTypeDisplayName} data to display." + Environment.NewLine; // Optional: Log if no data
            return;
        }

        try
        {
            lvMaster.Columns.Add($"{resourceTypeDisplayName} ID", 200);
            lvMaster.Columns.Add("Type", 200);
            lvMaster.Columns.Add("Name", 200);
            lvMaster.Columns.Add("Profile", 400); // Optional: Add more columns as needed

            foreach (var resourceInstance in resources)
            {
                ListViewItem item = new ListViewItem(resourceInstance.Id);
                item.SubItems.Add(resourceInstance.TypeName); // Use TypeName for the canonical FHIR resource type
                item.SubItems.Add(getNameSelector(resourceInstance));
                string profileValue = "No Profile";
                if (resourceInstance.Meta != null && resourceInstance.Meta.Profile != null)
                {
                    var firstProfile = resourceInstance.Meta.Profile.FirstOrDefault();
                    if (!string.IsNullOrEmpty(firstProfile))
                    {
                        profileValue = firstProfile.Split('/').LastOrDefault() ?? "No Profile";
                    }
                }
                item.SubItems.Add(profileValue); // Display profile if available
                lvMaster.Items.Add(item);
            }
        }
        catch (Exception ex) // Catch any unexpected errors during display
        {
            MessageBox.Show($"An error occurred while displaying {resourceTypeDisplayName}s: {ex.Message}",
                            "Display Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtMsg.Text += $"Error in DisplayMasterData for {resourceTypeDisplayName}s: {ex.Message}" + Environment.NewLine;
        }
    }

    private void cmbDocument_SelectedIndexChanged(object sender, EventArgs e)
    {
        string docType = cmbDocument.SelectedItem?.ToString()?.Split('|')[0].Trim() ?? string.Empty;
        Dictionary<string, string> jsonData = new Dictionary<string, string>();

        //MessageBox.Show("Selected Document Type: " + docType, "Document Type Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);


        if (string.IsNullOrEmpty(txtStaging.Text))
        {
            //MessageBox.Show("No staging data available to process.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        if (cmbDocument.Tag == null)
        {
            cmbDocument.Tag = txtStaging.Text;
        }
        string jsonStaging = cmbDocument.Tag.ToString() ?? string.Empty;
        // Deserialize the JSON staging data to a dictionary
        var stagingData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStaging);

        jsonData.Add("applyreportType", docType);

        for (int i = 0; i < lvStaging.Items.Count; i++)
        {
            string itemType = lvStaging.Items[i].SubItems[3].Text;
            string itemApplyModel = lvStaging.Items[i].SubItems[1].Text;

            if (itemType == "url" && itemApplyModel.Contains(docType))
            {
                if (stagingData != null && stagingData.ContainsKey(itemApplyModel))
                {
                    jsonData.Add("treatcarePlanDocument", stagingData[itemApplyModel]);
                }
                continue;
            }

            if (itemType == "string" && itemApplyModel.Contains(docType))
            {
                if (stagingData != null && stagingData.ContainsKey(itemApplyModel))
                {
                    jsonData.Add("treatcarePlanDocumentTitle", stagingData[itemApplyModel]);
                }
                continue;
            }
        }
        AddFHIRData(jsonData);
        // serialize the jsonData to json
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData, Newtonsoft.Json.Formatting.Indented);
        // Display the JSON in the text box
        txtStaging.Text = json;
    }

    private async void lbStaging_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Clear the Panel2 as selected item changes
        //this.splitQuestionnaire.Panel2.Controls.Clear();
        //this.splitQuestionnaire.Panel2.AutoScroll = true;

        // Clear ListView
        lblDocument.Visible = false;
        cmbDocument.Visible = false;
        cmbDocument.Items.Clear();

        lvStaging.Items.Clear();
        lvStaging.Columns.Clear();
        txtFHIRData.Text = string.Empty;

        lvStaging.Columns.Add("Name", 400);
        lvStaging.Columns.Add("ApplyModel", 500);
        lvStaging.Columns.Add("Path", 400);
        lvStaging.Columns.Add("Type", 200);
        lvStaging.Columns.Add("Rule", 400);

        lvFUME.Columns.Clear();
        lvFUME.Columns.Add("FUME", 1000, HorizontalAlignment.Center);


        if (lbStaging.SelectedItem != null)
        {
            string itemName = lbStaging.SelectedItem?.ToString()?.Split('|')[0].Trim() ?? string.Empty;

            if (itemName == "DocumentReference-twpas")
            {
                if (client == null)
                {
                    client = new FhirClient(txtFHIRServer.Text);
                }
                string valueSetUrl = "https://nhicore.nhi.gov.tw/pas/ValueSet/nhi-pdf-type";
                string type = "expand";
                var valueSet = await EpandValueSetByUrlAsync(client, valueSetUrl);
                if (valueSet == null)
                {
                    valueSet = await GetValueSetByUrlAsync(client, valueSetUrl);
                    type = "compose";
                }

                if (valueSet != null)
                {
                    var jsonValueSet = new FhirJsonSerializer(new SerializerSettings()
                    {
                        Pretty = true,
                    }).SerializeToString(valueSet);
                    var vsMap = fhirOperator.ParsingValueSet(valueSet, type);
                    foreach (var m in vsMap)
                    {
                        // Add the display name to the ComboBox
                        cmbDocument.Items.Add(m.Item2 + " | " + m.Item3);
                    }
                    cmbDocument.SelectedIndex = 0; // Select the first item by default
                    cmbDocument.Visible = true;
                    lblDocument.Visible = true;
                }
            }


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

            foreach (var q in ig.QList)
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
                    string type = q.Item4;
                    //Get type and binding from StructureDefinition
                    StructureDefinition? sd = null;
                    if (resolver != null)
                    {
                        sd = await GetStructureDefinition(q.Item2);
                        path = ConvertToX(path, type);
                        string ruleBase = "StructureDefinition.snapshot.element.where(path = '" + sd.Type + "." + path + "')";
                        //string ruleBase = "StructureDefinition.differential.element.where(path = '" + sd.Type + "." + path + "')";
                        ElementDefinition? obj = sd.Select(ruleBase).FirstOrDefault() as ElementDefinition;
                        if (obj != null)
                        {
                            string typeProfile = obj.Type.FirstOrDefault()?.Code ?? string.Empty;
                            type = typeProfile;
                        }
                    }
                    item.SubItems.Add(type);
                    item.SubItems.Add(rule);

                    lvStaging.Items.Add(item);

                    if (fileData != null && fileData.ContainsKey(staging))
                    {
                        // add the value to the dictionary
                        string value = fileData[staging].Split("|")[0].Trim();
                        staging = staging.Replace(".", "");
                        if (data.ContainsKey(staging))
                        {
                            data[staging] = value; // update the value if key already exists
                        }
                        else
                        {
                            data.Add(staging, value);
                        }
                    }
                }
            }

            // add the FHIR Server data to the dictionary
            AddFHIRData(data);

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

    private void AddFHIRData(Dictionary<string, string> data)
    {
        if (fhirData.Patient != null)
        {
            data.Add("fhirdatapatient", "Patient/" + fhirData.Patient);
        }

        foreach (var fhir in fhirData.IGData)
        {
            string role = fhir.Item1;
            string profile = fhir.Item2;
            string reference = fhir.Item3;

            if (data.ContainsKey(role))
            {
                switch (profile)
                {
                    case "Patient":
                        data[role] = "Patient/" + reference;
                        fhirData.Patient = reference; // Update the patient reference
                        break;
                    case "Organization":
                        data[role] = "Organization/" + reference;
                        break;
                    case "Practitioner":
                        data[role] = "Practitioner/" + reference;
                        break;
                    case "Substance":
                        data[role] = "Substance/" + reference;
                        break;
                    case "Media":
                        data[role] = "Media/" + reference;
                        break;
                    case "ImagingStudy":
                        data[role] = "ImagingStudy/" + reference;
                        break;
                    case "Specimen":
                        data[role] = "Specimen/" + reference;
                        break;
                    case "DocumentReference":
                        data[role] = "DocumentReference/" + reference;
                        break;
                    case "Observation":
                        data[role] = "Observation/" + reference;
                        break;
                    case "MedicationRequest":
                        data[role] = "MedicationRequest/" + reference;
                        break;
                    case "Coverage":
                        data[role] = "Coverage/" + reference;
                        break;
                    case "Procedure":
                        data[role] = "Procedure/" + reference;
                        break;
                    case "DiagnosticReport":
                        data[role] = "DiagnosticReport/" + reference;
                        break;
                    case "Encounter":
                        data[role] = "Encounter/" + reference;
                        break;
                    default:
                        MessageBox.Show("Unknown profile type: " + profile, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtMsg.Text += $"Unknown profile type: {profile}" + Environment.NewLine;
                        break;
                }
            }
        }

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

        try
        {
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
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show($"Error connecting to FUME server: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtMsg.Text += $"Error connecting to FUME server: {ex.Message}" + Environment.NewLine;
            return; // Exit if the request fails
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
            string itemPath = item.SubItems[2].Text.Trim();
            itemPath = itemPath.Replace("CodeableConcept", ""); // remove [x] from the path
            // 20250623
            if (path == itemPath) applyModel = item.SubItems[1].Text;

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
        fume2 += "* meta" + Environment.NewLine;
        fume2 += "  * profile = \"" + sd.Url + "\"" + Environment.NewLine;

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
        fumeListTuple = GetFUMECoding(fumeListTuple);
        fumeListTuple = GetFUMEPattern(sd, fumeListTuple);
        fumeListTuple = GetFUMECodeable(profileName, fumeListTuple);
        //fumeListTuple = GetFUMEQuantity(fumeListTuple);
        fumeListTuple = GetFUMEPeriod(fumeListTuple);
        //fumeListTuple = GetFUMECoding(fumeListTuple);
        fumeListTuple = await GetFUMESlice(fumeListTuple);
        fumeListTuple = GetFUMEQuantity(fumeListTuple);
        fumeListTuple = GetFUMEValue(fumeListTuple);
        fumeListTuple = GetFUMELost(fumeListTuple);
        fumeListTuple = GetFUMEDefault(fumeListTuple);

        //fumeListTuple = GetFUMEValueWithRule(fumeListTuple);
        lvFUME.Items.Clear();
        foreach (var fume in fumeListTuple)
        {
            fume2 += fume.Item1 + Environment.NewLine;
            lvFUME.Items.Add(fume.Item1);
            fumeDetail += fume.Item1 + "[" + fume.Item2 + "]" + "[" + fume.Item3 + "]" + Environment.NewLine;
        }
        txtFHIRData.Text = fumeDetail;
        return fume2;
    }
    private List<Tuple<string, string, string>> GetFUMEDefault(List<Tuple<string, string, string>> fumeListTuple)
    {
        string profileName = lbStaging.SelectedItem?.ToString()?.Split('|')[0].Trim() ?? string.Empty;

        foreach (var item in appSettings.StagingDefault)
        {
            if (item.Profile == profileName)
            {
                string path = profileName.Split("-")[0] + "." + item.Element ?? string.Empty;
                string type = item.Type ?? string.Empty;
                string anchor = item.Anchor ?? string.Empty;

                foreach (var tuple in fumeListTuple)
                {
                    int level = ComputeLevel(tuple.Item1);
                    string fumeAnchor = tuple.Item1.Split('=')[0].Replace("*", "").Trim();

                    if (fumeAnchor == anchor)
                    {
                        string fumeInfo = GetPostionStar(level + 2, item.Element + " = " + "\"" + item.Value + "\"");
                        Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fumeInfo, type, path);
                        //insert the fumeTuple into the fumeListTuple in front of the tuple with the same anchor
                        int index = fumeListTuple.IndexOf(tuple);
                        if (index >= 0)
                        {
                            fumeListTuple.Insert(index, fumeTuple);
                        }
                        break; // Exit the loop once the anchor is found
                    }
                    else
                    {
                        continue;
                    }
                }

            }
        }

        return fumeListTuple;
    }
    private async Task<List<Tuple<string, string, string>>> GetFUMESlice(List<Tuple<string, string, string>> fumeListTuple)
    {
        List<Tuple<string, string, string>> sliceFUME = new List<Tuple<string, string, string>>();
        string profileName = lbStaging.SelectedItem?.ToString()?.Split('|')[0].Trim() ?? string.Empty;
        List<Tuple<string, string>> profileSliceList = new List<Tuple<string, string>>();
        //txtMsg.Text = string.Empty;
        foreach (var s in ig.SliceList)
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
            bool isSlice = false;

            if (profileSliceList.Count > 0)
            {
                for (int j = 0; j < profileSliceList.Count; j++)
                {
                    var s = profileSliceList[j];
                    if (path == s.Item1 && isAdded == false)
                    {
                        List<Tuple<string, string, string>> sliceList = await CreateFUMESlice(profileName, path);
                        sliceList = GetFUMESliceValue(sliceList, path);
                        sliceList = CheckFUMESlice(sliceList);
                        sliceFUME.AddRange(sliceList);
                        profileSliceList[j] = new Tuple<string, string>(s.Item1, s.Item2);
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

            }
            else
            {
                isAdded = false;
                sliceFUME.Add(fume);
            }
        }
        return sliceFUME;
    }

    private List<Tuple<string, string, string>> GetFUMELost(List<Tuple<string, string, string>> fumeListTuple)
    {
        List<Tuple<string, string, string>> lostFUME = new List<Tuple<string, string, string>>();
        List<int> levels = GetLostLevels(fumeListTuple);
        List<Tuple<int, string>> fumeListTupleWithIndex = new List<Tuple<int, string>>();
        for (int i = 0; i < levels.Count; i++)
        {
            string fumeInfo = fumeListTuple[levels[i]].Item1;
            string path = fumeListTuple[levels[i]].Item3;
            //if (path == "Claim.diagnosis.diagnosiscode.coding.code") continue; // hard code
            string type = fumeListTuple[levels[i]].Item2;

            string headFume = "* " + path.Split('.')[1];
            if (fumeListTupleWithIndex.Any(t => t.Item2 == headFume))
            {
                continue; // Skip if the headFume already exists
            }
            string headType = "BackboneElement";
            string headPath = path.Split('.')[0] + "." + path.Split('.')[1];
            Tuple<string, string, string> headTuple = new Tuple<string, string, string>(headFume, headType, headPath);
            fumeListTupleWithIndex.Add(new Tuple<int, string>(levels[i], headFume));
            lostFUME.Add(headTuple);
        }

        for (int i = fumeListTupleWithIndex.Count - 1; i >= 0; i--)
        {
            fumeListTuple.Insert(fumeListTupleWithIndex[i].Item1, lostFUME[i]);
        }
        return fumeListTuple;
    }
    private List<Tuple<string, string, string>> CheckFUMESlice(List<Tuple<string, string, string>> sliceList)
    {
        //List<Tuple<string, string, string>> sliceFUME = new List<Tuple<string, string, string>>();
        string ignorePath = string.Empty;
        int ignoreIndex = -1;
        for (int i = 0; i < sliceList.Count; i++)
        {
            string fumeInfo = sliceList[i].Item1;
            if (fumeInfo.Contains("=") == false) continue; // Skip if no rule is defined
            string url = fumeInfo.Split('=').LastOrDefault()?.Trim().Trim('\"') ?? string.Empty;
            if (appSettings.BindingIgnore.Contains(url))
            {
                string innerPath = sliceList[i].Item3;
                List<string> pathList = innerPath.Split('.').ToList();
                pathList.RemoveAt(pathList.Count - 1);
                ignorePath = string.Join(".", pathList);
                ignoreIndex = i;
                continue;
            }
        }

        if (ignoreIndex == -1) return sliceList; // No ignore path found, return original list
        string outerPath = sliceList[ignoreIndex].Item3;
        while (outerPath != ignorePath)
        {
            ignoreIndex--;
            outerPath = sliceList[ignoreIndex].Item3;
        }
        for (int i = ignoreIndex; i < sliceList.Count; i++)
        {
            sliceList.RemoveAt(i);
            i--;
        }

        return sliceList;
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
                    fume = new Tuple<string, string, string>("  " + fumeInfo.Replace(fumeName, "system"), "uri", path + ".coding.system");
                    codingFUME.Add(fume);
                    fume = new Tuple<string, string, string>("  " + fumeInfo.Replace(fumeName, "code"), "code", path + ".coding.code");
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
                path = path.Replace("[x]", "Quantity");

                quantityFUME.Add(new Tuple<string, string, string>(fumeInfo, type, path));
                int cnt = 0;
                string pathNext = string.Empty;
                if (i + 1 < fumeListTuple.Count)
                {
                    pathNext = fumeListTuple[i + 1].Item3;
                    pathNext = pathNext.Replace("[x]", "Quantity");
                }
                while (!string.IsNullOrEmpty(pathNext) && pathNext.Contains(path) == true)
                {
                    cnt++;
                    if (i + cnt >= fumeListTuple.Count) break;
                    pathNext = fumeListTuple[i + cnt].Item3;
                    pathNext = pathNext.Replace("[x]", "Quantity");
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
                        path = path.Replace("CodeableConcept", "[x]"); // 修正path名稱 20250623
                        if (ig.Binding.ContainsKey(path))
                        {
                            system = ig.Binding[path];
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
                            if (fumeInfo.Contains("=") == false && (system == string.Empty) == false)
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
                                for (int j = i; j < i + 4; j++)
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
                                i = i + 4;
                            }
                        }
                    }
                    else if (patternType == "Coding")
                    {
                        var coding = obj.Pattern as Coding;
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

                                }
                            }
                            i = i + 3;
                        }
                    }
                    string pattern = value != null ? value.ToString() ?? string.Empty : string.Empty;
                    if (pattern != string.Empty)
                    {
                        var fume = fumeListTuple[i];
                        string fumeInfo = fume.Item1;
                        string type = fume.Item2;
                        string pathFUME = fume.Item3;
                        if (patternType == "Coding" || patternType == "CodeableConcept") // Hard code for Encounter ???
                        {
                            patternFUME.Add(fume);
                        }
                        else
                        {
                            patternFUME.Add(new Tuple<string, string, string>(fumeInfo + " = " + "\"" + pattern + "\"", type, pathFUME));
                        }
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
            else if (type == "Identifier")
            {
                valuedFUME.Add(fume);
                string fumeName = fumeInfo.Replace("*", "").Trim();
                path = path + ".value";
                fumeInfo = "  " + fumeInfo.Replace(fumeName, "value") + " = " + GetStagingApplyModel(path);
                var updatedFume = new Tuple<string, string, string>(fumeInfo, fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            else if (type == "Attachment")
            {
                if (fumeInfo.Contains("=")) continue;
                valuedFUME.Add(fume);
            }
            else if (type == "Reference")
            {
                // 20250623 for claim 
                if (fumeInfo.Contains("="))
                {
                    valuedFUME.Add(fume);
                    continue;
                }
                valuedFUME.Add(fume);
                if (i + 1 < fumeListTuple.Count && fumeListTuple[i + 1].Item2 == "Reference" && fumeListTuple[i + 1].Item1.Contains("="))
                {
                    continue;
                }
                // 20250623 for claim 
                string fumeName = fumeInfo.Replace("*", "").Trim();
                path = path + ".reference";
                if (fumeName == "subject")
                {
                    if (fhirData.Patient == string.Empty)
                    {
                        continue;
                    }
                    string patient = fhirData.Patient ?? string.Empty;
                    //fumeInfo = "  " + fumeInfo.Replace(fumeName, "reference") + " = " + "\"Patient/" + patient + "\"";
                    fumeInfo = "  " + fumeInfo.Replace(fumeName, "reference") + " = " + "fhirdatapatient";
                }
                else
                {
                    fumeInfo = "  " + fumeInfo.Replace(fumeName, "reference") + " = " + GetStagingApplyModel(path);
                }

                var updatedFume = new Tuple<string, string, string>(fumeInfo, fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            else if (type == "boolean" || type == "decimal" || type == "positiveInt")
            {
                if (fume.Item1.Trim().EndsWith("sequence"))// hard code for Claim.diagnosis.sequence
                {
                    valuedFUME.Add(new Tuple<string, string, string>(fume.Item1 + " = 1", fume.Item2, fume.Item3));
                    continue;
                }
                if (fume.Item1.Contains("="))
                {
                    valuedFUME.Add(fume);
                    continue;
                }
                var updatedFume = new Tuple<string, string, string>(fume.Item1 + " = " + GetStagingApplyModel(path), fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            else if (type == "date" || type == "base64Binary")
            {
                if (fume.Item1.Contains("="))
                {
                    valuedFUME.Add(fume);
                    continue;
                }
                var updatedFume = new Tuple<string, string, string>(fume.Item1 + " = " + GetStagingApplyModel(path), fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            else if (type == "integer" || type == "string")
            {
                if (fume.Item1.Contains("="))
                {
                    valuedFUME.Add(fume);
                    continue;
                }
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
                //20250605 怪怪的
                var updatedFume = new Tuple<string, string, string>(fume.Item1 + " = " + GetStagingApplyModel(path.Replace(".coding.code", "")), fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            else if (type == "uri")
            {
                if (fume.Item1.Contains("="))
                {
                    valuedFUME.Add(fume);
                    continue;
                }
                string system = string.Empty;
                string profileName = lbStaging.SelectedItem != null ? lbStaging.SelectedItem.ToString() ?? string.Empty : string.Empty;
                path = profileName + "." + path;
                path = path.Replace(".system", ""); // 20250605 修正 MedicationRequest.medication[x].coding.system --> MedicationRequest[x].medication.coding
                if (ig.Binding.ContainsKey(path))
                {
                    system = ig.Binding[path];
                }
                else
                {
                    system = string.Empty;
                }
                var updatedFume = new Tuple<string, string, string>(fume.Item1 + " = " + "\"" + system + "\"", fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            // CodeableConcept 可能有多個值，未來可能修改
            /*
            else if (type == "CodeableConcept")
            {
                // CodeableConcept 可能有多個值，未來可能修改
                if (fume.Item1.Contains("="))
                {
                    valuedFUME.Add(fume);
                    continue;
                }
                var updatedFume = new Tuple<string, string, string>(fume.Item1 + " = " + GetStagingApplyModel(path), fume.Item2, fume.Item3);
                valuedFUME.Add(updatedFume);
            }
            */
            //
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

            if (type == "Quantity") // special for specimen 未來可能修改
            {
                if (i == 0) continue;
                var fume0 = refinedFUME[i - 1];
                string fumeInfo0 = fume0.Item1;
                string type0 = fume0.Item2;
                string path0 = fume0.Item3;
                int level0 = ComputeLevel(fumeInfo0);
                int level = ComputeLevel(fumeInfo);
                if (level - level0 > 1)
                {
                    // 如果上一個元素的層級比當前元素的層級小2，補齊Quantity的層級

                    string fumeName = fumeInfo0.Replace("*", "").Trim();
                    fumeInfo0 = "  " + fumeInfo0.Replace(fumeName, "quantity");
                    type0 = "Quantity";
                    path0 = path0 + ".quantity";
                    refinedFUME.Insert(i, new Tuple<string, string, string>(fumeInfo0, type0, path0));
                    i++;
                    continue;
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
                int sliceStart = 0;
                int sliceEnd = 0;

                for (int j = 0; j < ig.SliceList.Count; j++)
                {
                    Tuple<string, string, string> slice = ig.SliceList[j];
                    string sliceProfile = slice.Item1;
                    string slicePath = slice.Item2;

                    if (path2.Contains(slicePath) == false) continue;
                    string sliceRule = slice.Item3;
                    if (sliceRule.Contains("Pattern") == true)
                    {
                        sliceStart = 0;
                        sliceEnd = 0;
                        continue;
                    }

                    sliceStart = i;
                    int k = 1;

                    string path = targetFumeTupleList[i + k].Item3;
                    while (path.Contains(slicePath) == true)
                    {
                        k++;
                        if (i + k < targetFumeTupleList.Count)
                        {
                            path = targetFumeTupleList[i + k].Item3;
                        }
                        else
                        {
                            break;
                        }
                    }
                    sliceEnd = i + k - 1;
                }
                targetFumeTupleList.RemoveAt(i);
                i--;

                //sliceStart = sliceStart + 3;
                for (int k = sliceStart; k < sliceEnd; k++)
                {
                    targetFumeTupleList.RemoveAt(sliceStart);
                }
                //i = i - (sliceEnd - sliceStart - 1);

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
        return targetFumeTupleList;
    }

    private List<Tuple<string, string, string>> CleanFumeTupleList2(List<Tuple<string, string, string>> fumeTupleList)
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

    private async void lbBundleList_SelectedIndexChanged(object sender, EventArgs e)
    {
        lvBundleProfile.Items.Clear();
        // show the selected item in the text box
        string bundleName = lbBundleList.SelectedItem?.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(bundleName))
        {
            MessageBox.Show("Please select a bundle.");
            return;
        }

        StructureDefinition sd = new StructureDefinition();
        sd = await GetStructureDefinition(bundleName);
        //bool isRequired = false;
        string element = string.Empty;

        lvBundleProfile.Items.Clear();
        lvBundleProfile.Columns.Clear();
        lvBundleProfile.Columns.Add("Element", 280);
        lvBundleProfile.Columns.Add("Min", 50);
        lvBundleProfile.Columns.Add("Max", 50);
        lvBundleProfile.Columns.Add("Short", 200);
        lvBundleProfile.Columns.Add("Profile", 200);

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
                        //isRequired = n.Min > 0;
                        element = n.ElementId.Replace("Bundle.entry:", "");
                        lvBundleProfile.Items.Add(element);
                        lvBundleProfile.Items[lvBundleProfile.Items.Count - 1].SubItems.Add(n.Min.ToString());
                        lvBundleProfile.Items[lvBundleProfile.Items.Count - 1].SubItems.Add(n.Max.ToString());
                        lvBundleProfile.Items[lvBundleProfile.Items.Count - 1].SubItems.Add(n.Short ?? string.Empty);
                    }
                    else
                    {
                        string profile = string.Empty;
                        if (n.Type.Count > 0 && n.Type[0].Profile != null && n.Type[0].Profile.Any())
                        {
                            profile = n.Type[0].Profile.First().Split("/").Last();
                        }
                        // new profile Tuple
                        lvBundleProfile.Items[lvBundleProfile.Items.Count - 1].SubItems.Add(profile ?? string.Empty);
                    }
                }
            }
        }
        lvBundleInfo.Clear();
        lvBundleInfo.Columns.Clear();
        lvBundleInfo.Columns.Add("Id", 100);
        lvBundleInfo.Columns.Add("Human", 500);
        lvBundleInfo.Columns.Add("Expression", 500);
        foreach (var constraint in sd.Differential.Element.Where(e => e.Constraint != null && e.Constraint.Any()))
        {
            foreach (var c in constraint.Constraint)
            {
                ListViewItem item = new ListViewItem(c.Key);
                item.SubItems.Add(c.Human ?? string.Empty);
                item.SubItems.Add(c.Expression ?? string.Empty);
                lvBundleInfo.Items.Add(item);
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

    private async void UploadFHIRData(string jsonFHIR)
    {
        // Upload the FHIR data to the server using existed client
        FhirClient client = new FhirClient(txtFHIRServer.Text);

        //Serialize the JSON string to a FHIR resource
        var resource = new FhirJsonParser().Parse<Resource>(jsonFHIR);
        // Save the resource to the server using "put" method



        Resource? result = null;

        try
        {
            if (resource.TypeName == "StructureMap" || resource.Id != null)
            {
                result = await client.UpdateAsync(resource);
            }
            else
            {
                result = await client.CreateAsync(resource);
            }
        }
        catch (FhirOperationException ex)
        {
            MessageBox.Show("Error saving FHIR data: " + ex.Message);
            return;
        }
        catch (Exception ex)
        {
            MessageBox.Show("An unexpected error occurred: " + ex.Message);
            return;
        }


        if (result != null)
        {
            MessageBox.Show("FHIR data saved successfully. Resource id :" + result.Id);
        }
        else
        {
            MessageBox.Show("Failed to save FHIR data.");
        }
    }
    private void btnSaveFHIR_Click(object sender, EventArgs e)
    {
        string jsonFHIR = txtFHIRData.Text;

        if (string.IsNullOrEmpty(jsonFHIR))
        {
            MessageBox.Show("Please generate FHIR data first.");
            return;
        }
        UploadFHIRData(jsonFHIR);
    }

    private void tabStaging_Enter(object sender, EventArgs e)
    {
        lbStaging.Items.Clear();
        lbReference.Items.Clear();
        bool isMaster = false;
        bool isLogic = false;
        bool isBase = false;
        List<string> baseList = new List<string> { "Patient", "Practitioner", "Organization" };
        foreach (var profile in ig.Profiles)
        {
            // check if the profile is contained in the qList
            isLogic = false;
            foreach (var q in ig.QList)
            {
                if (q.Item2.Contains(profile))
                {
                    isLogic = true;
                    foreach (var master in appSettings.MasterData)
                    {
                        if (master.Name == profile)
                        {
                            isMaster = true;
                            if (!string.IsNullOrEmpty(master.ResourceType) && baseList.Contains(master.ResourceType))
                            {
                                isBase = true;
                            }
                            //txtMsg.Text = txtMsg.Text + "Profile: " + profile + " is a master profile." + Environment.NewLine;
                        }
                    }
                    if (isMaster == false || isBase == false)
                    {
                        lbStaging.Items.Add(profile);
                    }
                    if (isMaster == true)
                    {
                        lbReference.Items.Add(profile);
                    }
                    isMaster = false;
                    isBase = false;
                    break;
                }
            }
            if (isLogic == false)
            {
                //lbReference.Items.Add(profile);
            }
        }

        lvReference.Items.Clear();
        lvReference.Columns.Clear();
        lvReference.Columns.Add("Display", 200);
        lvReference.Columns.Add("Reference", 200);
        lvReference.Columns.Add("Resource Type", 200);
        lvReference.Columns.Add("Role", 200);
        foreach (var master in appSettings.MasterData)
        {
            if (master.Display != null)
            {
                string display = master.Display;
                string reference = string.Empty;
                string resourceType = master.ResourceType ?? string.Empty;
                string role = master.Role ?? string.Empty;
                // add role, reference and resourceType to the ListView
                lvReference.Items.Add(new ListViewItem(new string[] { display, reference, resourceType, role }));
            }
        }


        lvSupplemental.Items.Clear();
        lvSupplemental.Columns.Clear();
        lvSupplemental.Columns.Add("Display", 200);
        lvSupplemental.Columns.Add("Reference", 200);
        lvSupplemental.Columns.Add("Resource Type", 200);
        lvSupplemental.Columns.Add("Role", 200);
        List<string> sList = new List<string>();
        foreach (var supplemental in appSettings.SupplementalData)
        {
            if (supplemental.Display != null)
            {
                string display = supplemental.Display;
                string reference = string.Empty;
                string resourceType = supplemental.ResourceType ?? string.Empty;
                if (sList.Contains(resourceType) != true) sList.Add(resourceType);
                string role = supplemental.Role ?? string.Empty;
                // add role, reference and resourceType to the ListView
                lvSupplemental.Items.Add(new ListViewItem(new string[] { display, reference, resourceType, role }));
            }
        }

        //add sList tp lbSupplemental
        foreach (var s in sList)
        {
            lbSupplemental.Items.Add(s);
        }
    }

    private void btnStagingLoad_Click(object sender, EventArgs e)
    {

    }

    private async void btnStagingLoad_Click2(object sender, EventArgs e)
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
    private void CheckFUME()
    {
        int cnt = 0;
        int first = 0;
        for (int i = 0; i < lvFUME.Items.Count; i++)
        {
            if (i == lvFUME.Items.Count - 1) continue;
            var item1 = lvFUME.Items[i];
            string fume1 = item1.Text;
            if (fume1.Contains('=')) continue;

            var item2 = lvFUME.Items[i + 1];
            string fume2 = item2.Text;
            int level1 = ComputeLevel(fume1);
            int level2 = ComputeLevel(fume2);
            if (level1 < level2)
            {
                continue;
            }
            else
            {
                // set item color to be red
                if (item1 != null && item2 != null)
                {
                    item1.BackColor = Color.Red;
                    item1.ForeColor = Color.White;
                    if (first == 0)
                    {
                        first = i;
                    }
                    cnt++;
                }
            }
        }
        txtMsg.Text = txtMsg.Text + "Total " + cnt + " items are not matched in FUME.";
        lvFUME.EnsureVisible(first);
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
        CheckFUME();
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

    private async Task<List<Tuple<string, string, string>>> CreateFUMESliceExtension(string root, string profileFullName, int level)
    {
        // Create FUME for Extension slice
        string profileName = profileFullName.Split("/").Last();
        StructureDefinition sd = await GetStructureDefinition(profileName);
        List<Tuple<string, string, string>> extensionFume = new List<Tuple<string, string, string>>();
        string space = new string(' ', level * 2);

        for (int i = 1; i < sd.Differential.Element.Count; i++) // Start from 1 to skip the root element
        {
            var e = sd.Differential.Element[i];
            if (e.Max == "0") continue;
            string path = RemoveFistPart(e.Path);
            string fumeInfo = "* " + path;
            var fixedObj = e.Fixed;
            if (fixedObj != null)
            {
                fumeInfo = space + fumeInfo + " = " + "\"" + fixedObj.ToString() + "\"";
                string type = fixedObj.TypeName;
                Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fumeInfo, type, root + "." + path);
                extensionFume.Add(fumeTuple);
            }
            else
            {
                string type = e.Type.FirstOrDefault()?.Code ?? string.Empty;
                if (type == "Reference")
                {
                    fumeInfo = space + fumeInfo.Replace("[x]", type);
                    path = path.Replace("[x]", type);
                    Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fumeInfo, type, root + "." + path);
                    extensionFume.Add(fumeTuple);
                    string fumeName = fumeInfo.Replace("*", "").Trim();
                    fumeInfo = "  " + fumeInfo.Replace(fumeName, "reference");
                    fumeTuple = new Tuple<string, string, string>(fumeInfo, "Reference", root + "." + path + ".reference");
                    extensionFume.Add(fumeTuple);
                }
            }
        }
        return extensionFume;
    }

    async Task<List<Tuple<string, string, string>>> CreateFUMESlice(string profileName, string pathSlice)
    {

        StructureDefinition sd = await GetStructureDefinition(profileName);

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
            //
            if (path.EndsWith("coding.system")) type = "uri";
            if (path.EndsWith("coding.code")) type = "code";
            //

            string sliceName = e.SliceName;
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

                if (sliceName != null && sliceName != string.Empty)
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
                    if (sliceName != null && sliceName != string.Empty)
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
                        //nameX = name;
                        // type == "Extension"
                        if (type == "Extension")
                        {

                            string profileExtension = e.Type.FirstOrDefault()?.Profile?.FirstOrDefault() ?? string.Empty;
                            if (profileExtension.Contains("us"))
                            {
                                //Hard code for US Da Vinci Extension
                                /*
                                * diagnosis[BackboneElement][Claim.diagnosis]
                                    * extension[BackboneElement][Claim.diagnosis.extension]
                                        * url = "http://hl7.org/fhir/us/davinci-pas/StructureDefinition/extension-diagnosisRecordedDate"[uri][Claim.diagnosis.extension.url]
                                        * valueDate = diagnosisdiagDate[date][Claim.diagnosis.extension.valueDate]
                                */
                                string daFume = "    * " + "url" + " = " + "\"" + profileExtension + "\"";
                                Tuple<string, string, string> daTuple = new Tuple<string, string, string>(daFume, "uri", e.Path + ".url");
                                sliceFume.Add(daTuple);
                                daFume = "    * " + "valueDate";
                                daTuple = new Tuple<string, string, string>(daFume, "date", e.Path + ".valueDate");
                                sliceFume.Add(daTuple);
                            }
                            else
                            {
                                List<Tuple<string, string, string>> extensionFume = new List<Tuple<string, string, string>>();
                                extensionFume = await CreateFUMESliceExtension(e.Path, profileExtension, level + 1);
                                sliceFume.AddRange(extensionFume);
                            }
                        }
                    }
                    else if (pattern != string.Empty)
                    {
                        // Check if the system is in the BindingIgnore list - ICD 2014
                        /*
                        bool isBindingIgnore = false;
                        if (appSettings.BindingIgnore.Contains(pattern) == true)
                        {
                            isBindingIgnore = false;
                        }
                        */
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
                            string fume = string.Empty;
                            /*
                            if (isBindingIgnore == true)
                            {
                                fume = path;
                            }
                            else
                            {
                                fume = path + " = " + "\"" + pattern + "\"";
                            }
                            */
                            fume = path + " = " + "\"" + pattern + "\"";
                            Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fume, type, e.Path);
                            sliceFume.Add(fumeTuple);
                        }
                    }
                    else if (type != string.Empty)
                    {
                        typeValue = type;
                        string fumePath = e.Path;
                        string fume = string.Empty;
                        // make first caracter of type to upper case, not for code // 20250612
                        if (type != "code")
                        {
                            type = char.ToUpper(type[0]) + type.Substring(1);
                            path = path.Replace("[x]", type);
                            //string fume = path + " = " + type;
                            //string fume = path + " = " + pathX;
                            fume = path;
                            fumePath = e.Path.Replace("[x]", type);
                        }
                        else
                        {
                            fume = path;
                        }
                        Tuple<string, string, string> fumeTuple = new Tuple<string, string, string>(fume, type, fumePath);
                        sliceFume.Add(fumeTuple);

                        if (type == "Reference")
                        {
                            string pathname = path.Replace("*", "").Trim();
                            string fumeReference = "  " + path.Replace(pathname, "reference");
                            //string fumeReference = path.Replace(pathname, "reference"); // 20250623
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

    private void lvFUME_DoubleClick(object sender, EventArgs e)
    {
        // Get the selected item
        /*
        string selectedItem = lvFUME.SelectedItem?.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(selectedItem))
        {
            MessageBox.Show("Please select an item from the list.");
            return;
        }
        //Label label = new Label();
        int yPos = 10; // Starting Y position for the labels
        int xPos = 10; // Starting X position for the labels
        int height = 0; // Height of the labels
        FormListView form = new FormListView();
        form.FormBorderStyle = FormBorderStyle.FixedDialog; // Set the form border style to fixed dialog
        form.MaximizeBox = false; // Disable the maximize button
        form.MinimizeBox = false; // Disable the minimize button
        form.Controls.Clear(); // Clear the form controls before adding new ones
                               // Create a new label for the selected item
        Label label = new Label
        {
            Text = selectedItem,
            Location = new Point(xPos, yPos),
            AutoSize = true, // Automatically adjust the label size based on the text
            Font = new Font("Arial", 10, FontStyle.Regular),
            ForeColor = Color.Black,
            BackColor = Color.Transparent // Set the background color to transparent
        };
        form.Controls.Add(label); // Add the label to the form
        xPos += label.Width + 10; // Update the X position for the next control
        TextBox txtFume = new TextBox
        {
            //Text = txtFHIRData.Text,
            Location = new Point(xPos, yPos),
            Width = 300, // Set the width to fit the form
            Height = form.ClientSize.Height - yPos - 20, // Set the height to fit the form
            //Multiline = true, // Allow multiline text
            ScrollBars = ScrollBars.Vertical, // Add vertical scroll bars
            Font = new Font("Arial", 10, FontStyle.Regular),
            ForeColor = Color.Black,
            BackColor = Color.White // Set the background color to white
        };
        form.Controls.Add(txtFume); // Add the text box to the form
        yPos += label.Height + 10; // Update the Y position for the next control
        Button btnSave = new Button
        {
            Text = "Save",
            Location = new Point(xPos , yPos + 10),
            Width = 80,
            Height = 30,
            Font = new Font("Arial", 10, FontStyle.Regular),
            ForeColor = Color.Black,
            BackColor = Color.LightGray
        };
        form.Controls.Add(btnSave); // Add the button to the form

        Button btnClose = new Button
        {
            Text = "Close",
            Location = new Point(xPos + btnSave.Width + 10, yPos+10),
            Width = 80,
            Height = 30,
            Font = new Font("Arial", 10, FontStyle.Regular),
            ForeColor = Color.Black,
            BackColor = Color.LightGray
        };
        btnClose.Click += (s, args) => form.Close(); // Add click event handler to close the form
        form.Controls.Add(btnClose); // Add the button to the form



        height = yPos + txtFume.Height + 20; // Add some padding
        Width = Math.Max(label.Width + txtFume.Width + 30, 500); // Set the width of the form to fit the controls
        //form.ClientSize = new Size(Width, height); // Set the client size of the form
        form.Text = "FUME Details"; // Set the form title
        form.StartPosition = FormStartPosition.CenterScreen; // Center the form on the screen
        form.Show(); // Show the form
        */
    }
    private void lvStaging_DoubleClick(object sender, EventArgs e)
    {
        ShowLVDialog(lvStaging, "Staging Details");
    }

    private void lvStaging_SelectedIndexChanged(object sender, EventArgs e)
    {
        // show the selected item in the text box
        if (lvStaging.SelectedItems.Count > 0)
        {
            ListViewItem item = lvStaging.SelectedItems[0];
            string applyModel = item.SubItems[1].Text;
            for (int i = 0; i < lvFUME.Items.Count; i++)
            {
                var itemFUME = lvFUME.Items[i];
                string? fume = itemFUME?.ToString();
                if (!string.IsNullOrEmpty(fume))
                {
                    if (fume.Contains(applyModel))
                    {
                        //lvFUME.SelectedItem = itemFUME;
                        string staging = txtStaging.Text;
                        int index = staging.IndexOf(applyModel);
                        if (index >= 0)
                        {
                            txtStaging.SelectionStart = index - 1;
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
                        //lvFUME.SelectedItem = null;
                    }
                }
            }
        }
        else
        {
            //lvFUME.SelectedItem = null;
        }
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void splitContainer13_Panel2_Paint(object sender, PaintEventArgs e)
    {

    }

    private void splitContainer13_SplitterMoved(object sender, SplitterEventArgs e)
    {

    }

    private void ShowIGExample()
    {

        string exampleDir = txtDataDirectory.Text + @"profiles\" + igName + @"\package\example\";

        // get files list from the example directory, if it does not exist, create it
        if (!System.IO.Directory.Exists(exampleDir))
        {
            MessageBox.Show("Example directory does not exist: " + exampleDir);
            return;
        }

        lbBase.Items.Clear(); // Clear the listbox before adding new items  
        string[] files = System.IO.Directory.GetFiles(exampleDir, "*.json");

        string msg = string.Empty;
        foreach (string file in files)
        {
            // get the file name without the directory and extension
            string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
            // add the file name to the listbox
            lbBase.Items.Add(fileName);
        }
    }
    private void tabConfiguration_Enter(object sender, EventArgs e)
    {
        //ShowIGExample();
    }

    private void lbBase_SelectedIndexChanged(object sender, EventArgs e)
    {
        // get the selected item from the listbox
        string selectedItem = lbBase.SelectedItem?.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(selectedItem))
        {
            MessageBox.Show("Please select an item from the list.");
            return;
        }
        // load the selected item to the text box
        string exampleDir = txtDataDirectory.Text + @"profiles\" + igName + @"\package\example\";
        string filePath = System.IO.Path.Combine(exampleDir, selectedItem + ".json");
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);

            // Deserialize the JSON to a FHIR resource
            Resource? resource = null;
            Narrative? narr = null;
            string resourceType = string.Empty;

            try
            {
                resource = new FhirJsonParser().Parse<Resource>(json);
                // Extract the narrative from the resource
                if (resource == null)
                {
                    MessageBox.Show("Failed to parse the resource from the JSON.");
                    return;
                }

                // get type of the resource
                resourceType = resource.TypeName;
                DomainResource? domainResource = null;
                if (resourceType == "Bundle")
                {
                    if (resource is Bundle bundle && bundle.Entry != null && bundle.Entry.Count > 0)
                    {
                        // Use the first entry's resource for narrative
                        domainResource = bundle.Entry[0].Resource as DomainResource;
                    }
                }
                else
                {
                    domainResource = resource as DomainResource;
                }
                narr = domainResource?.Text;
                if (domainResource != null)
                {
                    domainResource.Text = null; // Clear the text property to avoid serialization issues
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error parsing JSON: " + ex.Message);
                return;
            }

            if (narr != null)
            {
                WebBrowser webBrowser = new WebBrowser();
                splitBase3.Panel2.Controls.Clear(); // Clear the panel before adding new controls
                splitBase3.Panel2.BackColor = Color.LightGray; // Set the background color of the panel
                webBrowser.Dock = DockStyle.Fill; // Fill the panel with the web browser
                splitBase3.Panel2.Controls.Add(webBrowser); // Add the web browser to the panel

                //set font size to 16px
                string style = "<style>body { font-size: 18px; font-family: Arial, sans-serif; }</style>";
                // Create a new HTML document with the narrative content
                string htmlContent = "<html><head>" + style + "</head><body>" + narr.Div?.ToString() + "</body></html>";
                // Load the HTML content into the web browser
                webBrowser.DocumentText = htmlContent; // Set the HTML content of the web browser
                //webBrowser.DocumentText = narr.Div?.ToString() ?? string.Empty; // Set the HTML content of the web browser
                webBrowser.ScriptErrorsSuppressed = true; // Suppress script errors in the web browser
                webBrowser.ScrollBarsEnabled = true; // Enable scroll bars in the web browser
                webBrowser.AllowNavigation = true; // Allow navigation in the web browser
                webBrowser.AllowWebBrowserDrop = false; // Disable drag and drop in the web browser
                webBrowser.IsWebBrowserContextMenuEnabled = true; // Enable the context menu in the web browser
                webBrowser.WebBrowserShortcutsEnabled = true; // Enable web browser shortcuts
                webBrowser.ObjectForScripting = this; // Set the object for scripting to this form
                webBrowser.ScriptErrorsSuppressed = true; // Suppress script errors in the web browser
                webBrowser.Visible = true; // Make the web browser visible   
            }
            else
            {
                MessageBox.Show("No narrative found in the selected resource.");
            }
            FhirJsonSerializer serializer = new FhirJsonSerializer(new SerializerSettings()
            {
                Pretty = true,
            });
            string pat_Json = serializer.SerializeToString(resource);
            // Display the JSON in the text box
            txtBase.Text = pat_Json;

            if (resourceType == "Bundle")
            {
                lvBase.Items.Clear(); // Clear the listview before adding new items
                lvBase.Columns.Clear(); // Clear the columns before adding new ones
                                        //bundle as resource
                lvBase.Columns.Add("Resource Type", 150); // Add a column for the resource type
                lvBase.Columns.Add("Resource ID", 150); // Add a column for the resource ID
                lvBase.Columns.Add("Resource Path", 700); // Add a column for the resource path

                Bundle? bundle = resource as Bundle;
                if (bundle != null)
                {
                    foreach (var entry in bundle.Entry)
                    {
                        if (entry.Resource != null)
                        {

                            ListViewItem item = new ListViewItem(entry.Resource.TypeName); // Create a new listview item with the resource type
                            item.SubItems.Add(entry.Resource.Id); // Add the resource ID to the item
                            item.SubItems.Add(entry.FullUrl); // Add the resource path to the item
                            lvBase.Items.Add(item); // Add the item to the listview
                        }
                    }
                }
            }
            else
            {
                // Display the resource information in the listview
                lvBase.Items.Clear(); // Clear the listview before adding new items
                lvBase.Columns.Clear(); // Clear the columns before adding new ones
                lvBase.Columns.Add("Path", 250); // Add a column for the path
                lvBase.Columns.Add("Type", 100); // Add a column for the type
                lvBase.Columns.Add("Value", 150); // Add a column for the value
                lvBase.Columns.Add("Logic Model", 150); // Add a column for the apply model
                lvBase.Columns.Add("Rule", 150); // Add a column for the profile

                string profileName = resource.Meta?.Profile?.FirstOrDefault() ?? string.Empty;
                profileName = profileName.Split('/').LastOrDefault() ?? string.Empty;
                if (profileName == null || profileName.Length == 0) return; // If profileName is null, return
                foreach (var q in ig.QList)
                {
                    if (q.Item2.Contains(profileName))
                    {
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
                        ListViewItem item = new ListViewItem(path);
                        string type = q.Item4;
                        item.SubItems.Add(type); // Add the type to the item

                        path = GetExampleElementPath(path, type); // Get the path for the example element
                        string value = string.Empty;
                        try
                        {
                            if (resource.IsTrue(path))
                            {
                                if (resource.Select(path).Count() > 1)
                                {
                                    path = GetExampleElementPathByRule(path, type, rule); // Get the path for the example element by rule
                                    value = resource.Select(path).FirstOrDefault()?.ToString() ?? string.Empty; // Get the value from the resource
                                }
                                else
                                {
                                    value = resource.Select(path).FirstOrDefault()?.ToString() ?? string.Empty; // Get the value from the resource
                                }
                                if (value == "Hl7.Fhir.Model.CodeableConcept") // If the value starts with "Hl7.Fhir.Model.", remove it
                                {
                                    value = resource.Select(path + ".coding.code").FirstOrDefault()?.ToString() ?? string.Empty; // Get the value from the resource
                                }
                                if (value == "Hl7.Fhir.Model.Quantity") // If the value starts with "Hl7.Fhir.Model.", remove it
                                {
                                    value = resource.Select(path + ".value").FirstOrDefault()?.ToString() ?? string.Empty; // Get the value from the resource
                                }
                            }
                        }

                        catch (Exception ex)
                        {
                            txtMsg.Text = txtMsg.Text + "Error selecting path: " + path + " - " + ex.Message + Environment.NewLine;
                            continue; // Skip this item if there is an error
                        }

                        item.SubItems.Add(value); // Get the value from the tuple
                        item.SubItems.Add(q.Item1);
                        item.SubItems.Add(rule); // Get the apply model from the tuple
                        lvBase.Items.Add(item); // Add the item to the listview
                    }
                }
                lvBase.GridLines = true; // Enable grid lines in the listview
                lvBase.FullRowSelect = true; // Enable full row selection in the listview
                lvBase.Scrollable = true; // Enable scrolling in the listview
                lvBase.AutoArrange = true; // Enable auto-arranging of items in the listview
                lvBase.Refresh(); // Refresh the listview to display the items
            }
        }
        else
        {
            MessageBox.Show("File not found: " + filePath);
        }
    }

    private string GetExampleElementPathByRule(string pathQ, string type, string rule)
    {
        // Get the example element path by rule
        string path = pathQ;
        List<string> pathList = path.Split('.').ToList();
        string rulePath = "where(" + rule + ")";
        int index = 0;
        switch (type)
        {
            case "decimal":
                index = 2; // For Quantity, insert the rule after the second element
                break;
            case "Reference":
                index = 2; // For Reference, insert the rule after the second element
                break;
            case "CodeableConcept":
                index = 2; // For CodeableConcept, insert the rule after the second element
                break;
            default:
                index = 1;
                break;
        }
        pathList.Insert(pathList.Count - index, rulePath);
        path = string.Join(".", pathList).Trim();
        return path; // Return the path as is for now
    }
    private string GetExampleElementPath(string pathQ, string type)
    {
        string path = pathQ;
        if (path == "code") path = path + ".coding.code"; // Special case for code
        else if (path.Contains("Period")) path = path.Replace("Period", "");
        else if (path.Contains("Quantity")) path = path.Replace("Quantity", "");
        else if (path.EndsWith("DateTime")) path = path.Replace("DateTime", "");
        // slicing handling => if the path contains a slice, remove the slice name
        else if (path.Contains("value"))
        {
            if (path.Contains(".") == false)
            {
                path = "value"; // Add .value if it does not exist
            }
            else
            {
                List<string> pathList = path.Split('.').ToList();
                for (int i = 0; i < pathList.Count; i++)
                {
                    var p = pathList[i];
                    if (p.Contains("value") && p != "value")
                    {
                        pathList[i] = "value"; // Replace any part of the path that contains "value" with "value"
                    }
                }
                path = string.Join(".", pathList); // Join the path back together
            }
        }
        //else if (path.Contains("CodeableConcept") && path.StartsWith("value") == false) path = path + ".coding.code";
        else if (path.Contains("CodeableConcept")) path = path.Replace("CodeableConcept", "") + ".coding.code";

        // Hard code for CodeableConcept and Quantity
        if (type == "CodeableConcept")
        {
            bool isCoding = true;
            if (path.Contains("."))
            {
                if (path.EndsWith("reference") == false) isCoding = false;
                if (path.EndsWith("coding.code") == false) isCoding = false;
                if (path.EndsWith("text") == false) isCoding = false;
                if (path.EndsWith("count") == false) isCoding = false;
                if (path.EndsWith("value")) isCoding = false; // If the path is "value", do not append ".coding.code")
            }
            if (path == "value") isCoding = false; // If the path is "value", do not append ".coding.code"
            if (isCoding == true) path = path + ".coding.code";
        }
        return path;
    }

    private void btnUpload_Click(object sender, EventArgs e)
    {
        string jsonFHIR = txtBase.Text;

        if (string.IsNullOrEmpty(jsonFHIR))
        {
            MessageBox.Show("Please generate FHIR data first.");
            return;
        }
        UploadFHIRData(jsonFHIR);
    }

    private void lvMaster_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void lvMaster_DoubleClickAsync(object sender, EventArgs e)
    {
        if (lvMaster.SelectedItems.Count == 0 || (lvReference.SelectedItems.Count == 0 && lvSupplemental.SelectedItems.Count == 0))
        {
            MessageBox.Show("Please select an item from the list.");
            return;
        }
        // lvSelect
        ListView lvSelect = lvReference.SelectedItems.Count > 0 ? lvReference : lvSupplemental; // Check which listview is visible

        // get the resource from FHIR server by the selected item in the listview
        string id = lvMaster.SelectedItems[0].Text;
        string type = lvMaster.SelectedItems[0].SubItems[1].Text;
        string role = lvSelect.SelectedItems[0].SubItems[3].Text;

        if (type != lvSelect.SelectedItems[0].SubItems[2].Text)
        {
            MessageBox.Show("The selected item type does not match the reference item type.");
            return;
        }
        lvSelect.SelectedItems[0].SubItems[1].Text = id; // Set the id to the selected item in the reference listview
        switch (type)
        {
            case "Patient":
                fhirData.AddPatient(id);
                fhirData.AddIGData(role, type, id);
                break;
            default:
                fhirData.AddIGData(role, type, id);
                break;
        }
    }
    private async void btnMasterSelect_ClickAsync(object sender, EventArgs e)
    {
        // get the resource from FHIR server by the selected item in the listview
        string id = lvMaster.SelectedItems.Count > 0 ? lvMaster.SelectedItems[0].Text : string.Empty;
        string type = lvMaster.SelectedItems.Count > 0 ? lvMaster.SelectedItems[0].SubItems[1].Text : string.Empty;
        if (string.IsNullOrEmpty(id))
        {
            MessageBox.Show("Please select an item from the list.");

        }
        // get the resource from FHIR server by the id
        client = new FhirClient(txtFHIRServer.Text);

        DomainResource? resource = null;
        id = id.Trim();
        string resourcePath = $"{type}/{id}";
        try
        {
            Resource? fetchedResource = await client.ReadAsync<Resource>(resourcePath);
            DomainResource? domainResource = fetchedResource as DomainResource;
            resource = domainResource;
            if (resource != null)
            {
                // Serialize the resource to JSON
                FhirJsonSerializer serializer = new FhirJsonSerializer(new SerializerSettings()
                {
                    Pretty = true,
                });
                string json = serializer.SerializeToString(resource);
                // Display the JSON in the text box
                txtMasterFHIR.Text = json;
            }
            else
            {
                MessageBox.Show("Resource not found: " + id);
            }
        }
        catch (FhirOperationException ex)
        {
            MessageBox.Show("Error reading resource: " + ex.Message);

        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }

    private void lvBase_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void toolStripStatusLabel1_Click(object sender, EventArgs e)
    {

    }

    private async void btnIGCheck_Click(object sender, EventArgs e)
    {
        bool isNew = await ig.CheckIGVersion();
        if (isNew)
        {
            MessageBox.Show("The IG version is up to date.");
        }
        else
        {
            MessageBox.Show("The IG version is not up to date. Please update the IG.");
        }
    }

    private void btnFUMECheck_Click(object sender, EventArgs e)
    {
        lvFUME.Visible = !lvFUME.Visible; // Toggle the visibility of the FUME listview
        txtFume.Visible = !txtFume.Visible; // Toggle the visibility of the FUME text box

        string fumeText = txtFHIRData.Text;
        List<string> fumeLines = fumeText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
        List<string> fumePaths = new List<string>();
        foreach (string line in fumeLines)
        {
            string processedLine = line;
            if (processedLine.Contains("[x]")) processedLine = processedLine.Replace("[x]", ""); // Remove [x] from the line
            if (processedLine.Contains("[") && processedLine.Contains("]"))
            {
                // Extract the path from the line, path is inside the last square brackets
                int lastBracketIndex = processedLine.LastIndexOf('[') + 1;
                if (lastBracketIndex >= 0)
                {
                    string path = processedLine.Substring(lastBracketIndex, processedLine.Length - lastBracketIndex - 1).Trim();
                    fumePaths.Add(path);
                }
            }
        }
        if (lvFUME.Visible == true)
        {
            string root = string.Empty;
            string node = string.Empty;
            string leaf = string.Empty;
            int level0 = 0;
            int level1 = 0;
            List<string> rootList = new List<string>();

            //int level = ComputeLevel(lvFUME.Items[0].Text);
            //root = fumePaths[0]; // Set the root item
            for (int i = 0; i < lvFUME.Items.Count; i++)
            {
                int level = ComputeLevel(lvFUME.Items[i].Text);
                if (i == 0)
                {
                    level0 = 0;
                }
                else
                {
                    level0 = ComputeLevel(lvFUME.Items[i - 1].Text);
                }

                if (i == lvFUME.Items.Count - 1)
                {
                    level1 = level;
                }
                else
                {
                    level1 = ComputeLevel(lvFUME.Items[i + 1].Text);
                }

                string levelType = "N";
                if (level == 0) levelType = "R";
                else if (level >= level0 && level >= level1) levelType = "L";

                ListViewItem item = lvFUME.Items[i];


                switch (levelType)
                {
                    case "R":
                        root = fumePaths[i]; // Set the root item
                        node = string.Empty; // Reset the node item
                        leaf = string.Empty; // Reset the left item
                        if (rootList.Contains(root) == false)
                        {
                            rootList.Add(root); // Add the root item to the list if it does not exist
                        }
                        else
                        {
                            //root = "XXXXX";
                            //item.ForeColor = Color.Red; // Set the color of the root item to red if it already exists
                        }
                        break;
                    case "N":
                        node = fumePaths[i]; // Set the node item
                        if (node.Contains(root) == false) item.ForeColor = Color.Red; // Set the color of the node item to blue
                        break;
                    case "L":
                        leaf = fumePaths[i]; // Set the left item
                        //if(leafFume.EndsWith("=") == true)item.ForeColor = Color.Red;

                        if (leaf.Contains(root) == false) item.ForeColor = Color.Red; // Set the color of the left item to blue
                        if (leaf.Contains(node) == false) item.ForeColor = Color.Red; // Set the color of the left item to blue
                        break;
                    default:
                        break;
                }

            }
        }
    }
    private List<int> GetLostLevels(List<Tuple<string, string, string>> fumeListTuple)
    {
        List<int> levels = new List<int>();
        string root = string.Empty;
        string node = string.Empty;
        string leaf = string.Empty;
        int level0 = 0;
        int level1 = 0;
        List<string> rootList = new List<string>();

        //int level = ComputeLevel(lvFUME.Items[0].Text);
        //root = fumePaths[0]; // Set the root item
        for (int i = 0; i < fumeListTuple.Count; i++)
        {
            int level = ComputeLevel(fumeListTuple[i].Item1);
            if (i == 0)
            {
                level0 = 0;
            }
            else
            {
                level0 = ComputeLevel(fumeListTuple[i - 1].Item1);
            }

            if (i == fumeListTuple.Count - 1)
            {
                level1 = level;
            }
            else
            {
                level1 = ComputeLevel(fumeListTuple[i + 1].Item1);
            }

            string levelType = "N";
            if (level == 0) levelType = "R";
            else if (level >= level0 && level >= level1) levelType = "L";

            switch (levelType)
            {
                case "R":
                    root = fumeListTuple[i].Item3; // Set the root item
                    root = root.Replace("[x]", ""); // Remove [x] from the root item // hard code for [x] removal 20250621
                    node = string.Empty; // Reset the node item
                    leaf = string.Empty; // Reset the left item
                    if (rootList.Contains(root) == false)
                    {
                        rootList.Add(root); // Add the root item to the list if it does not exist
                    }
                    else
                    {
                        //root = "XXXXX";
                        //item.ForeColor = Color.Red; // Set the color of the root item to red if it already exists
                    }
                    break;
                case "N":
                    node = fumeListTuple[i].Item3; // Set the node item
                    node = node.Replace("[x]", ""); // Remove [x] from the node item // hard code for [x] removal 20250621
                    if (node.Contains(root) == false) levels.Add(i); // Set the color of the node item to blue
                    break;
                case "L":
                    leaf = fumeListTuple[i].Item3; // Set the left item
                    leaf = leaf.Replace("[x]", ""); // Remove [x] from the left item // hard code for [x] removal 20250621
                    //if(leafFume.EndsWith("=") == true)item.ForeColor = Color.Red;

                    if (leaf.Contains(root) == false) levels.Add(i); // Set the color of the left item to blue
                    if (leaf.Contains(node) == false) levels.Add(i); // Set the color of the left item to blue
                    break;
                default:
                    break;
            }

        }
        return levels;
    }

    private async void lvBundleProfile_SelectedIndexChanged(object sender, EventArgs e)
    {
        string profileName = lvBundleProfile.SelectedItems.Count > 0 ? lvBundleProfile.SelectedItems[0].SubItems[4].Text : string.Empty;
        profileName = profileName.Split("-")[0].Trim() ?? string.Empty;
        /*
        if (string.IsNullOrEmpty(profileName))
        {
            MessageBox.Show("Please select an item from the list.");
            return;
        }
        */
        switch (profileName)
        {
            case "Specimen":
                var specimens = await LoadFHIRDataAsync<Specimen>(profileName);
                DisplayFHIRData(
                    "Specimen", specimens, lvBundle,
                    s => s.Id ?? string.Empty
                );
                break;
            case "Observation":
                var observations = await LoadFHIRDataAsync<Observation>("Observation");
                DisplayFHIRData("Observation", observations, lvBundle,
                    o => o.Id ?? string.Empty);
                break;
            case "Procedure":
                var procedures = await LoadFHIRDataAsync<Procedure>("Procedure");
                DisplayFHIRData("Procedure", procedures, lvBundle,
                    p => p.Id ?? string.Empty);
                break;
            case "MedicationRequest":
                var medicationRequests = await LoadFHIRDataAsync<MedicationRequest>("MedicationRequest");
                DisplayFHIRData("MedicationRequest", medicationRequests, lvBundle,
                    m => m.Id ?? string.Empty);
                break;
            case "Encounter":
                var encounters = await LoadFHIRDataAsync<Encounter>("Encounter");
                DisplayFHIRData("Encounter", encounters, lvBundle,
                    e => e.Id ?? string.Empty);
                break;
            case "Patient":
                var patients = await LoadFHIRDataAsync<Patient>("Patient");
                DisplayFHIRData("Patient", patients, lvBundle,
                    p => p.Id ?? string.Empty);
                break;
            case "Practitioner":
                var practitioners = await LoadFHIRDataAsync<Practitioner>("Practitioner");
                DisplayFHIRData("Practitioner", practitioners, lvBundle,
                    p => p.Id ?? string.Empty);
                break;
            case "Organization":
                var organizations = await LoadFHIRDataAsync<Organization>("Organization");
                DisplayFHIRData("Organization", organizations, lvBundle,
                    o => o.Id ?? string.Empty);
                break;
            case "DiagnosticReport":
                var diagnosticReports = await LoadFHIRDataAsync<DiagnosticReport>("DiagnosticReport");
                DisplayFHIRData("DiagnosticReport", diagnosticReports, lvBundle,
                    d => d.Id ?? string.Empty);
                break;
            case "ImagingStudy":
                var imagingStudies = await LoadFHIRDataAsync<ImagingStudy>("ImagingStudy");
                DisplayFHIRData("ImagingStudy", imagingStudies, lvBundle,
                    i => i.Id ?? string.Empty);
                break;
            case "Media":
                var media = await LoadFHIRDataAsync<Media>("Media");
                DisplayFHIRData("Media", media, lvBundle,
                    m => m.Id ?? string.Empty);
                break;
            case "DocumentReference":
                var documentReferences = await LoadFHIRDataAsync<DocumentReference>("DocumentReference");
                DisplayFHIRData("DocumentReference", documentReferences, lvBundle,
                    d => d.Id ?? string.Empty);
                break;
            case "Substance":
                var substances = await LoadFHIRDataAsync<Substance>("Substance");
                DisplayFHIRData("Substance", substances, lvBundle,
                    s => s.Id ?? string.Empty);
                break;
            case "Coverage":
                var coverages = await LoadFHIRDataAsync<Coverage>("Coverage");
                DisplayFHIRData("Coverage", coverages, lvBundle,
                    c => c.Id ?? string.Empty);
                break;
            case "ClaimResponse":
                var claimResponses = await LoadFHIRDataAsync<ClaimResponse>("ClaimResponse");
                DisplayFHIRData("ClaimResponse", claimResponses, lvBundle,
                    c => c.Id ?? string.Empty);
                break;
            case "Claim":
                var claims = await LoadFHIRDataAsync<Claim>("Claim");
                DisplayFHIRData("Claim", claims, lvBundle,
                    c => c.Id ?? string.Empty);
                break;
            default:
                // MessageBox.Show("Profile not supported: " + profileName);
                break;
        }
    }

    private void btnStagingValidate_Click(object sender, EventArgs e)
    {
        // Validate the staging text box content
        Cursor = Cursors.WaitCursor; // Change cursor to wait state

        string fhirDataText = txtFHIRData.Text; // Renamed for clarity
        if (string.IsNullOrEmpty(fhirDataText))
        {
            MessageBox.Show("Please enter some FHIR data in the FHIR Data text box to validate.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Cursor = Cursors.Default; // Reset cursor to default state
            return;
        }
        if (resolver == null)
        {
            MessageBox.Show("Resolver is not initialized. Cannot validate staging.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Cursor = Cursors.Default; // Reset cursor to default state
            return;
        }

        var validator = CreateValidator();

        Resource? resourceToValidate = null; // Use the base Resource type
        try
        {
            resourceToValidate = new FhirJsonParser().Parse<Resource>(fhirDataText);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to parse FHIR resource from the provided JSON. Error: {ex.Message}", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Cursor = Cursors.Default; // Reset cursor to default state
            return;
        }


        if (resourceToValidate != null)
        {
            var validationResult = validator.Validate(resourceToValidate);
            // Display the validation results
            if (validationResult.Success)
            {
                Cursor = Cursors.Default; // Reset cursor to default state
                MessageBox.Show("Validation successful! No issues found.", "Validation Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Validation failed with the following issues:");
                foreach (var issue in validationResult.Issue)
                {
                    if (issue.Severity == OperationOutcome.IssueSeverity.Error)
                    {
                        sb.AppendLine($"- {issue.Severity}: {issue.Details?.Text} (at {issue.Location?.FirstOrDefault()})");
                    }
                }
                txtMsg.Text = sb.ToString(); // Display the issues in the message box
                Cursor = Cursors.Default; // Reset cursor to default state
                tabIG.SelectedTab = tabMsg; // Switch to the message tab

                //MessageBox.Show(sb.ToString(), "Validation Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        else
        {
            // This case should ideally be caught by the try-catch block during parsing
            Cursor = Cursors.Default; // Reset cursor to default state
            MessageBox.Show("Failed to parse FHIR resource from the provided JSON.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }

    private async void btnBundleCreate_Click(object sender, EventArgs e)
    {
        // Create a new Bundle from the selected items in the bundle profile listview
        Bundle bundle = new Bundle
        {
            Type = Bundle.BundleType.Collection,
            Timestamp = DateTimeOffset.Now,
            Meta = new Meta
            {
                Profile = new List<string> { "https://nhicore.nhi.gov.tw/pas/StructureDefinition/Bundle-twpas" }
            }

        };

        string reference = txtBundle.Text.Trim();
        if (string.IsNullOrEmpty(reference))
        {
            MessageBox.Show("Please enter a valid reference in the bundle text box.");
            return;
        }

        List<string> references = reference.Split(Environment.NewLine).ToList<string>();
        // remove the first two lines for header information
        references = references.Skip(2).ToList<string>();

        foreach (var refID in references)
        {
            if (client != null)
            {
                Bundle.EntryComponent entry = new Bundle.EntryComponent
                {
                    Resource = await client.ReadAsync<Resource>(refID) // Read the resource from the FHIR server
                };
                bundle.Entry.Add(entry); // Add the entry to the bundle
            }
            else
            {
                MessageBox.Show("FHIR client is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        lvBundleInfo.Clear(); // Clear the bundle info listview before adding new items
        lvBundleInfo.Columns.Clear(); // Clear the columns before adding new ones
        lvBundleInfo.Columns.Add("Index", 50); // Add a column for the resource type
        lvBundleInfo.Columns.Add("Resource Type", 150); // Add a column for the resource type
        lvBundleInfo.Columns.Add("Resource ID", 150); // Add a column for the resource ID
        lvBundleInfo.Columns.Add("Resource Path", 250); // Add a column for the resource path

        foreach (var entry in bundle.Entry)
        {
            if (entry.Resource != null)
            {
                ListViewItem item = new ListViewItem((lvBundleInfo.Items.Count).ToString()); // Resource Index
                item.SubItems.Add(entry.Resource.TypeName); // Resource Type
                item.SubItems.Add(entry.Resource.Id ?? "Unknown"); // Resource ID
                item.SubItems.Add(entry.FullUrl ?? "Unknown"); // Resource Path
                lvBundleInfo.Items.Add(item); // Add the item to the listview
            }
        }


        // Serialize the bundle to JSON
        FhirJsonSerializer serializer = new FhirJsonSerializer(new SerializerSettings()
        {
            Pretty = true,
        });
        string bundleJson = serializer.SerializeToString(bundle);
        // Display the bundle JSON in the text box
        txtBundle.Text = bundleJson;
    }

    private async void btnBundleCreate_Click2(object sender, EventArgs e)
    {
        // Create a new Bundle from the selected items in the bundle profile listview
        Bundle bundle = new Bundle
        {
            Type = Bundle.BundleType.Collection,
            Timestamp = DateTimeOffset.Now,
            Meta = new Meta
            {
                Profile = new List<string> { "https://nhicore.nhi.gov.tw/pas/StructureDefinition/Bundle-twpas" }
            }

        };

        if (lvBundle.SelectedItems.Count == 0)
        {
            MessageBox.Show("Please select an item from the bundle profile list.");
            return;
        }
        else
        {
            string profileName = lvBundle.SelectedItems[0].SubItems[3].Text;
            string resourceType = lvBundle.SelectedItems[0].SubItems[1].Text;
            string resourceId = lvBundle.SelectedItems[0].SubItems[0].Text;
            //read FHIR server to get the resource by type and id
            if (string.IsNullOrEmpty(resourceType) || string.IsNullOrEmpty(resourceId))
            {
                MessageBox.Show("Please select a valid resource type and id from the bundle profile list.");
                return;
            }
            client = new FhirClient(txtFHIRServer.Text);
            Resource? resource = null;
            string resourcePath = $"{resourceType}/{resourceId}";
            try
            {
                resource = await client.ReadAsync<Resource>(resourcePath);
                //add the resource to the bundle
                if (resource != null)
                {
                    Bundle.EntryComponent entry = new Bundle.EntryComponent
                    {
                        FullUrl = resourcePath,
                        Resource = resource,
                    };
                    bundle.Entry.Add(entry);
                    //add the resource to the listview
                    var refList = resource.Select("descendants().where($this is Reference).reference");
                    List<string> references = refList.Select(r => r?.ToString() ?? string.Empty).ToList();

                    // added each reference to the bundle
                    foreach (var reference in references)
                    {
                        if (!string.IsNullOrEmpty(reference))
                        {
                            Bundle.EntryComponent refEntry = new Bundle.EntryComponent
                            {
                                FullUrl = reference,
                                Resource = await client.ReadAsync<Resource>(reference)
                            };
                            bundle.Entry.Add(refEntry);
                        }
                    }


                }
                else
                {
                    MessageBox.Show("Resource not found: " + resourcePath);
                }
            }
            catch (FhirOperationException ex)
            {
                MessageBox.Show("Error reading resource: " + ex.Message);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return;
            }

            lvBundleInfo.Clear(); // Clear the bundle info listview before adding new items
            lvBundleInfo.Columns.Clear(); // Clear the columns before adding new ones
            lvBundleInfo.Columns.Add("Index", 50); // Add a column for the resource type
            lvBundleInfo.Columns.Add("Resource Type", 150); // Add a column for the resource type
            lvBundleInfo.Columns.Add("Resource ID", 150); // Add a column for the resource ID
            lvBundleInfo.Columns.Add("Resource Path", 250); // Add a column for the resource path

            foreach (var entry in bundle.Entry)
            {
                if (entry.Resource != null)
                {
                    ListViewItem item = new ListViewItem((lvBundleInfo.Items.Count).ToString()); // Resource Index
                    item.SubItems.Add(entry.Resource.TypeName); // Resource Type
                    item.SubItems.Add(entry.Resource.Id ?? "Unknown"); // Resource ID
                    item.SubItems.Add(entry.FullUrl ?? "Unknown"); // Resource Path
                    lvBundleInfo.Items.Add(item); // Add the item to the listview
                }
            }


            // Serialize the bundle to JSON
            FhirJsonSerializer serializer = new FhirJsonSerializer(new SerializerSettings()
            {
                Pretty = true,
            });
            string bundleJson = serializer.SerializeToString(bundle);
            // Display the bundle JSON in the text box
            txtBundle.Text = bundleJson;

        }
    }

    private void AddColumnHeaderforBundleProfile()
    {
        // Add a column for the reference if it does not exist
        bool hasReferenceColumn = false;
        foreach (ColumnHeader col in lvBundleProfile.Columns)
        {
            if (col.Text == "Reference")
            {
                hasReferenceColumn = true;
                break;
            }
        }
        if (!hasReferenceColumn)
        {
            lvBundleProfile.Columns.Add("Reference", 200); // Add a column for the reference
        }
    }
    private void btnBundleSelect_Click(object sender, EventArgs e)
    {
        string bundleElement = lvBundleProfile.SelectedItems.Count > 0 ? lvBundleProfile.SelectedItems[0].SubItems[3].Text : string.Empty;
        string reference = lvBundle.SelectedItems.Count > 0 ? lvBundle.SelectedItems[0].SubItems[0].Text : string.Empty;
        string resourceType = lvBundle.SelectedItems.Count > 0 ? lvBundle.SelectedItems[0].SubItems[1].Text : string.Empty;
        AddColumnHeaderforBundleProfile(); // Ensure the reference column is added before proceeding
        // add reference to the listview's selected item
        if (string.IsNullOrEmpty(reference))
        {
            MessageBox.Show("Please select a reference from the list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        else
        {
            lvBundleProfile.SelectedItems[0].SubItems.Add(resourceType + "/" + reference); // Add the reference to the selected item in the bundle profile listview
            lvBundleProfile.SelectedItems[0].ForeColor = Color.Green; // Change the color of the selected item to green
        }

    }
    private async Task<int> CollectResourceReferences(Resource resource, List<string> references, FhirClient client)
    {
        var directReferenceValues = resource.Select("descendants().where($this is Reference).reference");
        foreach (var value in directReferenceValues)
        {
            if (value == null) continue;
            var refStr = value.ToString();
            if (refStr != null && !references.Contains(refStr))
            {
                references.Add(refStr);
                try
                {
                    Resource? subResource = await client.ReadAsync<Resource>(refStr);
                    if (subResource != null)
                    {
                        await CollectResourceReferences(subResource, references, client);
                    }
                    else
                    {
                        MessageBox.Show("Resource not found: " + refStr);
                    }
                }
                catch (FhirOperationException ex)
                {
                    MessageBox.Show("Resource " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        return references.Count;
    }

    private async void lvBundle_DoubleClickAsync(object sender, EventArgs e)
    {
        if (lvBundle.SelectedItems.Count == 0)
        {
            MessageBox.Show("Please select an item from the list.");
            return;
        }
        string id = lvBundle.SelectedItems[0].SubItems[0].Text;
        string type = lvBundle.SelectedItems[0].SubItems[1].Text;
        if (type != "Claim")
        {
            MessageBox.Show("Please select a Claim item from the list.");
            return;
        }
        client = new FhirClient(txtFHIRServer.Text);
        DomainResource? resource = null;
        string resourcePath = $"{type}/{id}";
        try
        {
            Resource? fetchedResource = await client.ReadAsync<Resource>(resourcePath);
            DomainResource? domainResource = fetchedResource as DomainResource;
            resource = domainResource;
        }
        catch (FhirOperationException ex)
        {
            MessageBox.Show("Error reading resource: " + ex.Message);
            return;
        }

        List<string> references = new List<string>();
        references.Add(resourcePath); // Add the resource path to the references list

        if (resource != null)
        {
            var refList = resource.Select("descendants().where($this is Reference).reference");
            int refCnt = await CollectResourceReferences(resource, references, client);
        }
        string refText = "Total References: " + references.Count + Environment.NewLine + Environment.NewLine;
        refText += string.Join(Environment.NewLine, references);
        txtBundle.Text = refText;
    }

    private async void lvBundle_DoubleClickAsync2(object sender, EventArgs e)
    {
        if (lvBundle.SelectedItems.Count == 0)
        {
            MessageBox.Show("Please select an item from the list.");
            return;
        }
        // get the resource from FHIR server by the selected item in the listview
        string id = lvBundle.SelectedItems[0].SubItems[0].Text;
        string type = lvBundle.SelectedItems[0].SubItems[1].Text;
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type))
        {
            MessageBox.Show("Please select a valid item from the list.");
            return;
        }
        client = new FhirClient(txtFHIRServer.Text);
        DomainResource? resource = null;
        string resourcePath = $"{type}/{id}";
        try
        {
            Resource? fetchedResource = await client.ReadAsync<Resource>(resourcePath);
            DomainResource? domainResource = fetchedResource as DomainResource;
            resource = domainResource;
        }
        catch (FhirOperationException ex)
        {
            MessageBox.Show("Error reading resource: " + ex.Message);
            return;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
            return;
        }

        if (resource != null)
        {
            var refList = resource.Select("descendants().where($this is Reference).reference");
            txtBundle.Text = string.Join(Environment.NewLine, refList.Select(r => r?.ToString() ?? string.Empty));
            foreach (var reference in refList)
            {
                string referenceStr = reference?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(referenceStr))
                {
                    try
                    {
                        AddColumnHeaderforBundleProfile(); // Ensure the reference column is added before proceeding
                        Resource? refResource = await client.ReadAsync<Resource>(referenceStr);
                        if (refResource != null)
                        {
                            //get the resource type from the reference
                            string refType = refResource.Meta.Profile?.FirstOrDefault()?.Split('/').Last() ?? "Unknown";
                            SelectApprove(refType, referenceStr); // Call the SelectApprove method to handle the reference
                                                                  // Add the reference to string using string builder

                            //txtMsg.Text += $"Reference: {referenceStr} (Type: {refType})" + Environment.NewLine;                            
                        }
                    }
                    catch (FhirOperationException ex)
                    {
                        MessageBox.Show("Error reading reference resource: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }
    }

    private void SelectApprove(string refType, string referenceStr)
    {
        foreach (ListViewItem item in lvBundleProfile.Items)
        {
            if (item.SubItems[4].Text == refType)
            {
                item.SubItems.Add(referenceStr); // Set the reference in the selected item
                item.ForeColor = Color.Green; // Change the color of the selected item to green
                return; // Exit after finding the first match
            }
        }
    }

    private Validator CreateValidator()
    {
        if (resolver == null)
        {
            throw new InvalidOperationException("Resolver is not initialized. Cannot create validator.");
        }
        if(validator != null)
        {
            return validator; // Return the existing validator if already created
        }
        string tw_core_ig = @"D:\Hongyu\Project\data\IGAnalyzer\profiles\twcore\package-.tgz";
        FhirPackageSource tw_core = new(ModelInfo.ModelInspector, new string[] { tw_core_ig });

        var multiResolver = new MultiResolver(resolver, tw_core);

        ValueSetExpanderSettings expanderSettings = new ValueSetExpanderSettings();
        expanderSettings.MaxExpansionSize = 86400; // Set the cache size for the value set expander
        var terminologySource = new LocalTerminologyService(tw_core, expanderSettings);

        ValidationSettings settings = new ValidationSettings();
        settings.SetSkipConstraintValidation(true); // Skip constraint validation for the bundle
        settings.HandleValidateCodeServiceFailure += HandleValidateCodeServiceFailure(this, EventArgs.Empty); // Handle code validation service failures

        validator = new Validator(multiResolver, terminologySource, null, settings);
        return validator;
    }

    private static Firely.Fhir.Validation.ValidateCodeServiceFailureHandler HandleValidateCodeServiceFailure(object sender, EventArgs e)
    {
        // Handle code validation service failures
        return (sender, e) =>
        {
            return TerminologyServiceExceptionResult.Warning; // Ensure a value is returned for all code paths
        };
    }


    private void btnBundleValidate_Click(object sender, EventArgs e)
    {
        // Validate the bundle text box content
        // change the pointer to be waiting
        Cursor = Cursors.WaitCursor; // Change the cursor to a waiting cursor
        string bundleText = txtBundle.Text; // Renamed for clarity
        if (string.IsNullOrEmpty(bundleText))
        {
            MessageBox.Show("Please enter some FHIR data in the Bundle text box to validate.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Cursor = Cursors.Default; // Change the cursor back to default
            return;
        }
        if (resolver == null)
        {
            MessageBox.Show("Resolver is not initialized. Cannot validate bundle.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Cursor = Cursors.Default; // Change the cursor back to default
            return;
        }
        //var terminologySource = new LocalTerminologyService(resolver);
        // Validate the bundle
        //ValidationSettings settings = new ValidationSettings();
        //settings.SetSkipConstraintValidation(true); // Skip constraint validation for the bundle
        var validator = CreateValidator(); // Create the validator using the resolver and terminology source

        Bundle? bundleToValidate = null; // Use the base Bundle type
        try
        {
            bundleToValidate = new FhirJsonParser().Parse<Bundle>(bundleText);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to parse FHIR Bundle from the provided JSON. Error: {ex.Message}", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Cursor = Cursors.Default; // Change the cursor back to default
            return;
        }

        if (bundleToValidate != null)
        {
            OperationOutcome result = new OperationOutcome(); // Initialize the result variable
            try
            {
                result = validator.Validate(bundleToValidate);
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default; // Change the cursor back to default
                MessageBox.Show($"Error during validation: {ex.Message}", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Display the validation results
            if (result.Success)
            {
                Cursor = Cursors.Default; // Change the cursor back to default
                MessageBox.Show("Validation successful! No issues found.", "Validation Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Validation failed with the following issues:");
                foreach (var issue in result.Issue)
                {
                    if (issue.Severity == OperationOutcome.IssueSeverity.Error)
                    {
                        // Append only error severity issues
                        sb.AppendLine($"- {issue.Code}: {issue.Details?.Text} (at {issue.Expression?.FirstOrDefault()})");
                    }
                }
                txtMsg.Text = sb.ToString(); // Display the validation result in the text box
                Cursor = Cursors.Default; // Change the cursor back to default
                tabIG.SelectedTab = tabMsg; // Switch to the message tab
            }
        }
        else
        {
            // This case should ideally be caught by the try-catch block during parsing
            MessageBox.Show("Failed to parse FHIR Bundle from the provided JSON.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Cursor = Cursors.Default; // Change the cursor back to default
        }
    }

    private async void tabMajor_Enter(object sender, EventArgs e)
    {
        StructureDefinition? sd = new StructureDefinition();

        sd = await GetStructureDefinition("Claim-twpas");

        // fill the TreeView with the structure definition
        if (sd != null)
        {
            tvMajor.Nodes.Clear(); // Clear existing nodes
            TreeNode rootNode = new TreeNode(sd.Name ?? "Root");
            rootNode.Tag = sd; // Store the StructureDefinition in the tag
            tvMajor.Nodes.Add(rootNode);
            PopulateTreeView(rootNode, sd);
            tvMajor.ExpandAll(); // Expand all nodes for better visibility
        }
        else
        {
            MessageBox.Show("Failed to load StructureDefinition for Claim-pas.");
        }
    }

    private void PopulateTreeView(TreeNode parentNode, StructureDefinition sd)
    {
        // Recursively populate the TreeView with elements from the StructureDefinition

        bool isSlicing = false;
        string slicPath = string.Empty;
        TreeNode? sliceNode = null;
        TreeNode? sliceChildNode = null;
        foreach (var element in sd.Differential?.Element ?? new List<ElementDefinition>())
        {
            if (element.Max == "0")
            {
                // Skip elements with no minimum cardinality or negative minimum cardinality
                continue;
            }

            if (element.Slicing == null)
            {
                // Create a node for the element without slicing
                TreeNode node = new TreeNode(element.Path ?? "Unnamed Element");
                node.Tag = element; // Store the ElementDefinition in the tag
                if (!string.IsNullOrEmpty(element.Path) && element.Path.Contains(slicPath) && isSlicing)
                {
                    // If the element is part of a slice, add it to the current slice node
                    if (sliceNode != null)
                    {
                        if (element.Path == slicPath)
                        {
                            node.Text += "(" + element.SliceName + ")"; // Append slice name to the node text
                            sliceNode.Nodes.Add(node);
                            sliceChildNode = node;
                        }
                        else
                        {
                            string type = element.Type?.FirstOrDefault()?.Code ?? string.Empty; // Get the type code or default to "Unknown"
                            if (type != string.Empty) node.Text = $"{element.Path} [{type}]"; // Set the node text with path and type
                            //node.Text += element.Type?.FirstOrDefault()?.Code ?? string.Empty; // Append type code if available
                            sliceChildNode?.Nodes.Add(node);
                        }

                    }
                    else
                    {
                        parentNode.Nodes.Add(node); // If no slice node exists, add to parent
                    }
                }
                else if (isSlicing && (string.IsNullOrEmpty(element.Path) || !element.Path.Contains(slicPath)))
                {
                    // If slicing has ended, reset the flag and slicPath
                    isSlicing = false;
                    slicPath = string.Empty;
                    parentNode.Nodes.Add(node); // Add to parent node if not part of slicing
                }

            }
            else
            {
                // Create a node for the element
                TreeNode node = new TreeNode(element.Path ?? "Unnamed Element");
                node.Tag = element; // Store the ElementDefinition in the tag
                parentNode.Nodes.Add(node);
                isSlicing = true; // Set the flag to true if slicing is present
                slicPath = element.Path ?? string.Empty; // Store the slicing path
                // Recursively add child elements if they exist
                if (element.Slicing.Discriminator != null && element.Slicing.Discriminator.Count > 0)
                {
                    foreach (var discriminator in element.Slicing.Discriminator)
                    {
                        TreeNode discriminatorNode = new TreeNode(discriminator.Type.ToString() + ": " + discriminator.Path);
                        node.Nodes.Add(discriminatorNode);
                    }
                }
                sliceNode = node; // Store the current slice node for further processing
            }

        }
    }

    private void tabVersion_Enter(object sender, EventArgs e)
    {

    }

    private void btnStagingCopy_Click(object sender, EventArgs e)
    {
        // Copy the content of the staging text box to the clipboard
        if (!string.IsNullOrEmpty(txtFume.Text))
        {
            Clipboard.SetText(txtFume.Text);
            MessageBox.Show("Staging data copied to clipboard.", "Copy Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            MessageBox.Show("No data to copy. Please enter some FHIR data in the staging text box.", "Copy Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async void btnIGDownload_Click(object sender, EventArgs e)
    {
        Cursor = Cursors.WaitCursor; // Change cursor to wait state
        string igPackagePath = await ig.DownloadIGPackage();
        Cursor = Cursors.Default; // Reset cursor to default state
        MessageBox.Show($"IG package downloaded successfully to: {igPackagePath}", "Download Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void splitContainer5_Panel1_Paint(object sender, PaintEventArgs e)
    {

    }

    private void btnValidateLoad_Click(object sender, EventArgs e)
    {
        // Load the validation bundle from a file
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    txtValidateBundle.Clear(); // Clear the existing content in the text box
                    txtValidateEntry.Clear(); // Clear the entry text box
                    txtValidateMsg.Clear(); // Clear the message text box
                    lbValidateBundleList.Items.Clear(); // Clear the existing items in the bundle profile listview
                    txtValidateFile.Clear(); // Clear the file text box

                    txtValidateFile.Text = openFileDialog.FileName; // Set the file name in the text box
                    txtValidateFile.Enabled = false;
                    string jsonContent = File.ReadAllText(openFileDialog.FileName);
                    txtValidateBundle.Text = jsonContent; // Load the content into the bundle text box
                    List<string> entryList = GetBundleEntryList(jsonContent); // Get the list of entry references from the bundle
                    lbValidateBundleList.Items.Clear(); // Clear the existing items in the bundle profile listview
                    int entryCnt = -1;
                    foreach (var entry in entryList)
                    {
                        string entryText = "Entry" + "[" + (++entryCnt).ToString() + "]: " + entry; // Format the entry text
                        lbValidateBundleList.Items.Add(entryText); // Add each entry reference to the listview
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading file: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
    private void lbValidateBundleList_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Handle the selection change in the bundle entry list
        if (lbValidateBundleList.SelectedItem != null)
        {
            string selectedEntry = lbValidateBundleList.SelectedItem?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(selectedEntry))
            {
                // Extract the resource type and full URL from the selected entry
                string[] parts = selectedEntry.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    string resourceType = parts[0].Trim();
                    string fullUrl = parts[1].Trim();

                    // Display the selected entry details in the text box
                    statusLabel.Text = $"Selected Entry:Resource Type: {resourceType} Full URL: {fullUrl}";

                    // using FHIRPath to get the resource information from txtValidateBundle
                    try
                    {
                        Bundle bundle = new FhirJsonParser().Parse<Bundle>(txtValidateBundle.Text);
                        var entry = bundle.Entry.FirstOrDefault(e => e.FullUrl == fullUrl);
                        if (entry != null && entry.Resource != null)
                        {
                            // Display the resource details in the text box
                            // display the resource in the text box with pretty formatting
                            txtValidateEntry.Clear(); // Clear the existing content in the text box
                            FhirJsonSerializer serializer = new FhirJsonSerializer(new SerializerSettings()
                            {
                                Pretty = true,
                            });
                            string resourceJson = serializer.SerializeToString(entry.Resource);
                            txtValidateEntry.Text = resourceJson; // Set the resource JSON in the text box
                            statusLabel.Text = $"Selected entry found in the bundle: {resourceType} - {fullUrl}";
                        }
                        else
                        {
                            statusLabel.Text = "Selected entry not found in the bundle.";
                        }
                    }
                    catch (Exception ex)
                    {
                        statusLabel.Text = $"Error parsing bundle: {ex.Message}";
                    }


                }
                else
                {
                    statusLabel.Text = "Invalid entry format. Please select a valid entry.";
                }
            }
            else
            {
                statusLabel.Text = "No entry selected.";
            }
        }
    }

    private List<string> GetBundleEntryList(string bundleText)
    {
        // Parse the bundle text and return a list of entry references
        List<string> entryList = new List<string>();
        try
        {
            Bundle bundle = new FhirJsonParser().Parse<Bundle>(bundleText);
            foreach (var entry in bundle.Entry)
            {
                if (entry.FullUrl != null)
                {
                    entryList.Add(entry.Resource.TypeName + " - " + entry.FullUrl); // Add the resource type and full URL to the list
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error parsing bundle: {ex.Message}", "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        return entryList;
    }
    
    private void btnValidate_Click(object sender, EventArgs e)
    {
        // Validate the bundle in the txtValidateBundle text box
        if (string.IsNullOrEmpty(txtValidateBundle.Text))
        {
            MessageBox.Show("Please load a bundle to validate.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        txtValidateMsg.Clear(); // Clear the message text box before validation
        // Create a validator instance
        var validator = CreateValidator();

        Bundle? bundleToValidate = null; // Use the base Bundle type
        DomainResource? domainResourceToValidate = null; // Use DomainResource for the entry text box
        try
        {
            if (txtValidateEntry.Text != string.Empty)
            {
                // If the entry text box is not empty, parse the bundle from there
                domainResourceToValidate = new FhirJsonParser().Parse<DomainResource>(txtValidateEntry.Text);
            }
            else
            {
                // Otherwise, parse the bundle from the main text box
                bundleToValidate = new FhirJsonParser().Parse<Bundle>(txtValidateBundle.Text);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to parse FHIR Bundle from the provided JSON. Error: {ex.Message}", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (bundleToValidate != null)
        {
            OperationOutcome result = new OperationOutcome(); // Initialize the result variable
            try
            {
                Cursor = Cursors.WaitCursor; // Change the cursor to a waiting cursor
                // Validate the bundle
                result = validator.Validate(bundleToValidate);
                Cursor = Cursors.Default; // Change the cursor back to default after validation
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during validation: {ex.Message}", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Display the validation results
            if (result.Success)
            {
                MessageBox.Show("Validation successful! No issues found.", "Validation Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Validation failed with the following issues:");
                foreach (var issue in result.Issue)
                {
                    if (issue.Severity == OperationOutcome.IssueSeverity.Error)
                    {
                        // Append only error severity issues
                        sb.AppendLine($"*** {issue.Code}: {issue.Details?.Text} (at {issue.Expression?.FirstOrDefault()}) {Environment.NewLine}");
                    }
                }
                txtValidateMsg.Text = sb.ToString(); // Display the validation result in the text box
                //tabIG.SelectedTab = tabMsg; // Switch to the message tab
            }
        }
        else if (domainResourceToValidate != null)
        {
            // Validate a single resource entry
            OperationOutcome result = new OperationOutcome(); // Initialize the result variable
            try
            {
                Cursor = Cursors.WaitCursor; // Change the cursor to a waiting cursor
                // Validate the single resource entry
                result = validator.Validate(domainResourceToValidate);
                Cursor = Cursors.Default; // Change the cursor back to default after validation
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during validation: {ex.Message}", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Display the validation results for the single resource entry
            if (result.Success)
            {
                MessageBox.Show("Validation successful! No issues found for the selected entry.", "Validation Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Validation failed with the following issues:");
                foreach (var issue in result.Issue)
                {
                    if (issue.Severity == OperationOutcome.IssueSeverity.Error)
                    {
                        // Append only error severity issues
                        sb.AppendLine($"*** {issue.Code}: {issue.Details?.Text} (at {issue.Expression?.FirstOrDefault()}) {Environment.NewLine}");
                    }
                }
                txtValidateMsg.Text = sb.ToString(); // Display the validation result in the text box
            }
        }

    }


}