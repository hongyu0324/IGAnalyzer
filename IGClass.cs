namespace IGAnalyzer;

using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using Firely.Fhir.Packages;
using Hl7.Fhir.Model;
using System.Configuration;

public class IGClass
{
    public string Name { get; set; } = string.Empty;
    public string SubName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string IGName { get; set; } = string.Empty;

    public string canonical { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Package { get; set; } = string.Empty;

    public List<string> Profiles { get; set; } = new List<string>();
    public List<string> Bundles { get; set; } = new List<string>();
    public string LogicModel { get; set; } = string.Empty;

    public List<string> LogicModelList { get; set; } = new List<string>();
    public string ErrorMessage { get; set; } = string.Empty;
    public List<Tuple<string, string, string, string>> QList { get; set; } = new List<Tuple<string, string, string, string>>();

    public List<Tuple<string, string, string>> SliceList { get; set; } = new List<Tuple<string, string, string>>();

    public Dictionary<string, string> Binding { get; set; } = new Dictionary<string, string>();

    private FhirPackageSource? resolver;

    public string ICD10PCSVersion { get; set; } = string.Empty;
    public IGClass()
    {
        Name = string.Empty;
        SubName = string.Empty;
        Version = string.Empty;
        Url = string.Empty;
        Status = string.Empty;
        Publisher = string.Empty;
        Package = string.Empty;

        Profiles = new List<string>();
        Bundles = new List<string>();
        LogicModel = string.Empty;

        QList = new List<Tuple<string, string, string, string>>();

        SliceList = new List<Tuple<string, string, string>>();

        Binding = new Dictionary<string, string>();
    }

    public void AddProfile(string profileName)
    {
        if (!Profiles.Contains(profileName))
        {
            Profiles.Add(profileName);
        }
    }
    public void AddBundle(string bundleName)
    {
        if (!Bundles.Contains(bundleName))
        {
            Bundles.Add(bundleName);
        }
    }

    public void InsertQItem(int pos, string name, string profile, string path, string type)
    {
        var item = Tuple.Create(name, profile, path, type);
        if (pos < 0 || pos > QList.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(pos), "Position is out of range.");
        }
        QList.Insert(pos, item);
    }

    public void AddQItem(string name, string profile, string path, string type)
    {
        var item = Tuple.Create(name, profile, path, type);
        if (!QList.Contains(item))
        {
            QList.Add(item);
        }
    }

    public void AddSliceItem(string name, string path, string description)
    {
        var item = Tuple.Create(name, path, description);
        if (!SliceList.Contains(item))
        {
            SliceList.Add(item);
        }
    }

    public void AddLogicModelItem(string logicModel)
    {
        if (!LogicModelList.Contains(logicModel))
        {
            LogicModelList.Add(logicModel);
        }
    }

    public void AddBindItem(string name, string path)
    {

    }

    public void ClearProfiles()
    {
        Profiles.Clear();
    }

    public void ClearBundles()
    {
        Bundles.Clear();
    }

    public void ClearQList()
    {
        QList.Clear();
    }

    public void ClearSliceList()
    {
        SliceList.Clear();
    }

    public void ClearBinding()
    {
        Binding.Clear();
    }

    public void ClearLogicModelList()
    {
        LogicModelList.Clear();
    }

    public void Clear()
    {
        ClearProfiles();
        ClearBundles();
        ClearQList();
        ClearSliceList();
        ClearBinding();
        ClearLogicModelList();
        Name = string.Empty;
        SubName = string.Empty;
        LogicModel = string.Empty;
    }

    public async Task<bool> CheckIGVersion()
    {
        bool ret = false;
        HttpClient client = new HttpClient();
        string htmlContent = await client.GetStringAsync("https://nhicore.nhi.gov.tw/pas/index.html");
        //string htmlContent = await client.GetStringAsync("https://twcore.mohw.gov.tw/ig/pas/ImplementationGuide/tw.gov.mohw.nhi.pas");

        if (string.IsNullOrEmpty(htmlContent))
        {
            return false;
        }

        string version = string.Empty;

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        // XPath to find the <td> element that contains an <i> child element with the text "Version",
        // and then select the <span class="copy-text"> child of that <td>.
        var versionSpanNode = htmlDoc.DocumentNode.SelectSingleNode("//td[i[normalize-space(.)='Version']]/span[@class='copy-text']");

        if (versionSpanNode != null)
        {
            // The version number is expected to be the first text node within the span.
            foreach (var childNode in versionSpanNode.ChildNodes)
            {
                if (childNode.NodeType == HtmlNodeType.Text)
                {
                    version = childNode.InnerText.Trim();
                    if (version == Version)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                    break;
                }
            }
        }
        // You can add more complex validation logic here if needed
        return ret;
    }
    public async Task<bool> CheckIG(string packagePath)
    {
        bool ret = false;

        //string tmpDir = Path.GetTempPath();

        if (string.IsNullOrEmpty(packagePath))
        {
            return ret;
        }

        // download package file from the path https://build.fhir.org/ig/TWNHIFHIR/pas/package.tgz
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Send a GET request to the URL
                HttpResponseMessage response = await client.GetAsync(packagePath);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Read the response content as a byte array
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();

                // TODO: Define where to save the file and what to name it.
                // For example, you might want to save it in a temporary directory
                // or a specific directory related to your application.
                string localFilePath = Path.Combine(Path.GetTempPath(), "package.tgz");
                await File.WriteAllBytesAsync(localFilePath, fileBytes);

                // At this point, 'fileBytes' contains the downloaded file.
                // You can now process it (e.g., extract, validate).
                // For demonstration, we'll just set ret to true if download is successful.
                if (fileBytes.Length > 0)
                {
                    ret = true;
                    Console.WriteLine($"Successfully downloaded {fileBytes.Length} bytes.");
                    // You would typically proceed to use a library like Firely.Fhir.Packages
                    // to open and inspect the package from the downloaded bytes or the saved file.
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                // Handle any errors that occurred during the download
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        return ret;
    }

    public async void SetIinitialProperty(string igPath)
    {

        if (resolver == null )
        {
            ErrorMessage = "Resolver is not initialized or IG path is empty." + Environment.NewLine;
            return;
        }
        if(Title != string.Empty)
        {
            return; // 已經設定過了
        }
        ImplementationGuide? guide = await resolver.ResolveByUriAsync("ImplementationGuide/" + igPath) as ImplementationGuide;
        IGName = guide?.Name ?? string.Empty;
        Title = guide?.Title ?? string.Empty;
        Version = guide?.Version ?? string.Empty;
        Description = guide?.Description ?? string.Empty;
        Publisher = guide?.Publisher ?? string.Empty;
        Contact = guide?.Contact?.FirstOrDefault()?.Name ?? string.Empty;
        Url = guide?.Url ?? string.Empty;
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

    public async void LoadCanonical()
    {
        resolver = new(ModelInfo.ModelInspector, new string[] { Package });
        if (resolver == null)
        {
            ErrorMessage = "Resolver is not initialized." + Environment.NewLine;
            return;
        }
        var names = resolver.ListCanonicalUris();
        string profileName = "https://nhicore.nhi.gov.tw/" + Name + "/StructureDefinition";
        foreach (var n in names)
        {
            string logicName = GetLogicName(Name, SubName);
            try
            {
                if (n.Contains("ImplementationGuide"))
                {
                    if (n.Contains(Name)) SetIinitialProperty(n.Split("/").Last());
                    continue;
                }
                if (n.StartsWith(profileName))
                {
                    var profile = n.Split("/").Last();
                    if (profile.Contains("Bundle") && !profile.Contains("ApplyModel"))
                    {
                        // Bundle 處理
                        if (Name == "emr" && profile.Contains(logicName))
                        {
                            AddBundle(profile);
                        }
                        else if (Name != "emr")
                        {
                            AddBundle(profile);
                        }
                    }

                    else if (profile.Contains("Bundle"))
                    {
                        AddBundle(profile);
                    }
                    else if (profile.Contains(logicName) && profile.Contains("Model"))
                    {

                        LogicModel = profile;
                    }
                    else
                    {
                        if (Name == "emr")
                        {
                            if (profile.Contains(logicName))
                            {
                                AddProfile(profile);
                            }
                        }
                        else
                        {
                            AddProfile(profile);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error processing {n}: {ex.Message}" + Environment.NewLine;
            }
        }
        if (Profiles.Count == 0)
        {
            ErrorMessage = "No profiles found in the Implementation Guide." + Environment.NewLine;
            return;
        }

        // Resolve all profiles and extract bindings and slices
        foreach (string profile in Profiles)
        {
            var sd = await resolver.ResolveByUriAsync("StructureDefinition/" + profile) as StructureDefinition;
            if (sd is not StructureDefinition resolvedProfile)
            {
                ErrorMessage = "Failed to resolve the StructureDefinition." + Environment.NewLine;
                return;
            }
            //get Binding information from Differential
            foreach (var element in sd.Differential.Element)
            {
                GenerateBinding(element, profile, Binding);
            }

            //get Binding information from Snapshot
            foreach (var element in sd.Snapshot.Element)
            {
                GenerateBinding(element, profile, Binding);
            }

            // get Slicing information from Differential
            foreach (var element in sd.Differential.Element)
            {
                string slice = string.Empty;
                slice = GetElementSlicing(element);
                if (slice != string.Empty)
                {
                    string path = element.Path;
                    path = path.Replace(sd.Type + ".", "");
                    AddSliceItem(sd.Id, path, slice);
                }
            }
        }
        // 取得 ApplyModel 的定義


        var resolvedDefinition = await resolver.ResolveByUriAsync("StructureDefinition/" + LogicModel);
        if (resolvedDefinition is not StructureDefinition applyModelDef)
        {
            ErrorMessage = "Failed to resolve the ApplyModel definition." + Environment.NewLine;
            return;
        }

        foreach (var ele in applyModelDef.Differential.Element)
        {
            var elementList = ele.Path.Split('.').ToList();
            if (elementList.Count > 1)
            {
                if (elementList.Count == 2)
                {
                    AddLogicModelItem(ele.Path + " | " + ele.Definition);
                }
                //if( ele.Type.FirstOrDefault()?.Code == "BackboneElement")
                //{
                //    AddLogicModelItem(ele.Path + " | " + ele.Definition);
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
                        string q1 = ele.Short;
                        if (ele.Short.Contains("，"))
                        {
                            q1 = ele.Short.Split("，")[0];
                        }
                        //  IG Package問題，未來修改
                        AddQItem(q1 + " | " + ele.Path, GetProfileName(m.Identity, Profiles), q3, ele.Type.FirstOrDefault()?.Code ?? string.Empty);
                    }
                }
            }
        }

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
                ErrorMessage = $"Error processing {profileName} {element.Path}: {err.Message}" + Environment.NewLine;
            }
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
}
