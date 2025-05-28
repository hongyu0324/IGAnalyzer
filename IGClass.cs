namespace IGAnalyzer;

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
    public List<string> LogicModels { get; set; } = new List<string>();


    public List<Tuple<string, string, string, string>> QList { get; set; } = new List<Tuple<string, string, string, string>>();

    public List<Tuple<string, string, string>> SliceList { get; set; } = new List<Tuple<string, string, string>>();

    public List<Tuple<string, string, string>> BindList { get; set; } = new List<Tuple<string, string, string>>();


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
        LogicModels = new List<string>();

        QList = new List<Tuple<string, string, string, string>>();

        SliceList = new List<Tuple<string, string, string>>();

        BindList = new List<Tuple<string, string, string>>();
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
    public void AddLogicModel(string logicModelName)
    {
        if (!LogicModels.Contains(logicModelName))
        {
            LogicModels.Add(logicModelName);
        }
    }
    public void AddQItem(string name, string type, string path, string description)
    {
        var item = Tuple.Create(name, type, path, description);
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

    public void AddBindItem(string name, string path, string description)
    {
        var item = Tuple.Create(name, path, description);
        if (!BindList.Contains(item))
        {
            BindList.Add(item);
        }
    }

    public void ClearProfiles()
    {
        Profiles.Clear();
    }

    public void ClearBundles()
    {
        Bundles.Clear();
    }

    public void ClearLogicModels()
    {
        LogicModels.Clear();
    }

    public void Clear()
    {
        ClearProfiles();
        ClearBundles();
        ClearLogicModels();
        QList.Clear();
        SliceList.Clear();
        BindList.Clear();
    }
}
