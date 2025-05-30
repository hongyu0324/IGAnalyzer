namespace IGAnalyzer;

using System;
using System.Collections.Generic;
using Firely.Fhir.Packages;
using Hl7.Fhir.Model;

public class IGClass
{
    public string Name { get; set; } = string.Empty;
    public string SubName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Package { get; set; } = string.Empty;



    public List<string> Profiles { get; set; } = new List<string>();
    public List<string> Bundles { get; set; } = new List<string>();
    public string LogicModel { get; set; } = string.Empty;


    public List<Tuple<string, string, string, string>> QList { get; set; } = new List<Tuple<string, string, string, string>>();

    public List<Tuple<string, string, string>> SliceList { get; set; } = new List<Tuple<string, string, string>>();

    public Dictionary<string, string> Binding { get; set; } = new Dictionary<string, string>();



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

    public void Clear()
    {
        ClearProfiles();
        ClearBundles();
        ClearQList();
        ClearSliceList();
        ClearBinding();
        Name = string.Empty;
        SubName = string.Empty;
        LogicModel = string.Empty;
    }

   
   
}
