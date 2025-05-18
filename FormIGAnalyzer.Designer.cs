namespace IGAnalyzer;




partial class FormIGAnalyzer : Form
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
  

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }


    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        btnClose = new Button();
        btnSelect = new Button();
        txtPackage = new TextBox();
        label1 = new Label();
        ofdPackage = new OpenFileDialog();
        tabIG = new TabControl();
        tabProfile = new TabPage();
        splitContainer8 = new SplitContainer();
        lbProfile = new ListBox();
        splitContainer9 = new SplitContainer();
        lvElement = new ListView();
        lvProfile = new ListView();
        tabApplyModel = new TabPage();
        splitContainer7 = new SplitContainer();
        lbApplyModel = new ListBox();
        splitContainer10 = new SplitContainer();
        lvApplyModel = new ListView();
        splitQuestionnaire = new SplitContainer();
        txtQuestionnaire = new TextBox();
        btnRendering = new Button();
        btnSave = new Button();
        btnLoad = new Button();
        btnRefresh = new Button();
        tabStaging = new TabPage();
        splitContainer1 = new SplitContainer();
        lbStaging = new ListBox();
        splitContainer2 = new SplitContainer();
        lvStaging = new ListView();
        splitContainer3 = new SplitContainer();
        splitContainer4 = new SplitContainer();
        txtStaging = new TextBox();
        btnFHIRData = new Button();
        txtFume = new TextBox();
        txtFHIRData = new TextBox();
        tabBundle = new TabPage();
        splitContainer5 = new SplitContainer();
        splitContainer6 = new SplitContainer();
        lbBundleList = new ListBox();
        lvBundleProfile = new ListView();
        tabMsg = new TabPage();
        txtMsg = new TextBox();
        tabConfiguration = new TabPage();
        rbUser = new RadioButton();
        rbAdmin = new RadioButton();
        label4 = new Label();
        btnDataDir = new Button();
        txtDataDirectory = new TextBox();
        label3 = new Label();
        cmbIG = new ComboBox();
        label2 = new Label();
        cmbHosp = new ComboBox();
        lblHosp = new Label();
        txtFHIRServer = new TextBox();
        lblFHIRServer = new Label();
        statusBar = new StatusStrip();
        columnHeader1 = new ColumnHeader();
        rbDifferential = new RadioButton();
        rbSnapshot = new RadioButton();
        rbApplyModel = new RadioButton();
        btnFUME = new Button();
        btnSaveFHIR = new Button();
        tabIG.SuspendLayout();
        tabProfile.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer8).BeginInit();
        splitContainer8.Panel1.SuspendLayout();
        splitContainer8.Panel2.SuspendLayout();
        splitContainer8.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer9).BeginInit();
        splitContainer9.Panel1.SuspendLayout();
        splitContainer9.Panel2.SuspendLayout();
        splitContainer9.SuspendLayout();
        tabApplyModel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer7).BeginInit();
        splitContainer7.Panel1.SuspendLayout();
        splitContainer7.Panel2.SuspendLayout();
        splitContainer7.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer10).BeginInit();
        splitContainer10.Panel1.SuspendLayout();
        splitContainer10.Panel2.SuspendLayout();
        splitContainer10.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitQuestionnaire).BeginInit();
        splitQuestionnaire.Panel1.SuspendLayout();
        splitQuestionnaire.SuspendLayout();
        tabStaging.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.Panel2.SuspendLayout();
        splitContainer1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
        splitContainer2.Panel1.SuspendLayout();
        splitContainer2.Panel2.SuspendLayout();
        splitContainer2.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
        splitContainer3.Panel1.SuspendLayout();
        splitContainer3.Panel2.SuspendLayout();
        splitContainer3.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer4).BeginInit();
        splitContainer4.Panel1.SuspendLayout();
        splitContainer4.Panel2.SuspendLayout();
        splitContainer4.SuspendLayout();
        tabBundle.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer5).BeginInit();
        splitContainer5.Panel1.SuspendLayout();
        splitContainer5.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer6).BeginInit();
        splitContainer6.Panel1.SuspendLayout();
        splitContainer6.Panel2.SuspendLayout();
        splitContainer6.SuspendLayout();
        tabMsg.SuspendLayout();
        tabConfiguration.SuspendLayout();
        SuspendLayout();
        // 
        // btnClose
        // 
        btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnClose.BackColor = Color.LightBlue;
        btnClose.Location = new Point(849, 472);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(100, 40);
        btnClose.TabIndex = 2;
        btnClose.Text = "結束";
        btnClose.UseVisualStyleBackColor = false;
        btnClose.Click += btnClose_Click;
        // 
        // btnSelect
        // 
        btnSelect.BackColor = Color.LightBlue;
        btnSelect.Location = new Point(849, 10);
        btnSelect.Name = "btnSelect";
        btnSelect.Size = new Size(100, 40);
        btnSelect.TabIndex = 3;
        btnSelect.Text = "選擇";
        btnSelect.UseVisualStyleBackColor = false;
        btnSelect.Click += btnSelect_ClickAsync;
        // 
        // txtPackage
        // 
        txtPackage.BorderStyle = BorderStyle.FixedSingle;
        txtPackage.Font = new Font("Microsoft JhengHei UI", 9F);
        txtPackage.Location = new Point(135, 13);
        txtPackage.Name = "txtPackage";
        txtPackage.Size = new Size(708, 30);
        txtPackage.TabIndex = 0;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(39, 19);
        label1.Name = "label1";
        label1.Size = new Size(90, 23);
        label1.TabIndex = 5;
        label1.Text = "Package :";
        // 
        // tabIG
        // 
        tabIG.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        tabIG.Controls.Add(tabProfile);
        tabIG.Controls.Add(tabApplyModel);
        tabIG.Controls.Add(tabStaging);
        tabIG.Controls.Add(tabBundle);
        tabIG.Controls.Add(tabMsg);
        tabIG.Controls.Add(tabConfiguration);
        tabIG.Location = new Point(12, 65);
        tabIG.Name = "tabIG";
        tabIG.SelectedIndex = 0;
        tabIG.Size = new Size(945, 401);
        tabIG.TabIndex = 6;
        // 
        // tabProfile
        // 
        tabProfile.Controls.Add(splitContainer8);
        tabProfile.Location = new Point(4, 32);
        tabProfile.Name = "tabProfile";
        tabProfile.Size = new Size(937, 365);
        tabProfile.TabIndex = 6;
        tabProfile.Text = "Profile";
        tabProfile.UseVisualStyleBackColor = true;
        // 
        // splitContainer8
        // 
        splitContainer8.Dock = DockStyle.Fill;
        splitContainer8.Location = new Point(0, 0);
        splitContainer8.Name = "splitContainer8";
        // 
        // splitContainer8.Panel1
        // 
        splitContainer8.Panel1.Controls.Add(lbProfile);
        // 
        // splitContainer8.Panel2
        // 
        splitContainer8.Panel2.Controls.Add(splitContainer9);
        splitContainer8.Size = new Size(937, 365);
        splitContainer8.SplitterDistance = 153;
        splitContainer8.TabIndex = 0;
        // 
        // lbProfile
        // 
        lbProfile.Dock = DockStyle.Fill;
        lbProfile.FormattingEnabled = true;
        lbProfile.Location = new Point(0, 0);
        lbProfile.Name = "lbProfile";
        lbProfile.Size = new Size(153, 365);
        lbProfile.TabIndex = 0;
        lbProfile.SelectedIndexChanged += lbProfile_SelectedIndexChanged;
        // 
        // splitContainer9
        // 
        splitContainer9.Dock = DockStyle.Fill;
        splitContainer9.Location = new Point(0, 0);
        splitContainer9.Name = "splitContainer9";
        splitContainer9.Orientation = Orientation.Horizontal;
        // 
        // splitContainer9.Panel1
        // 
        splitContainer9.Panel1.Controls.Add(lvElement);
        // 
        // splitContainer9.Panel2
        // 
        splitContainer9.Panel2.Controls.Add(lvProfile);
        splitContainer9.Size = new Size(780, 365);
        splitContainer9.SplitterDistance = 207;
        splitContainer9.TabIndex = 0;
        // 
        // lvElement
        // 
        lvElement.Dock = DockStyle.Fill;
        lvElement.FullRowSelect = true;
        lvElement.GridLines = true;
        lvElement.Location = new Point(0, 0);
        lvElement.Name = "lvElement";
        lvElement.Size = new Size(780, 207);
        lvElement.TabIndex = 0;
        lvElement.UseCompatibleStateImageBehavior = false;
        lvElement.View = View.Details;
        // 
        // lvProfile
        // 
        lvProfile.Dock = DockStyle.Fill;
        lvProfile.FullRowSelect = true;
        lvProfile.GridLines = true;
        lvProfile.Location = new Point(0, 0);
        lvProfile.Name = "lvProfile";
        lvProfile.Size = new Size(780, 154);
        lvProfile.TabIndex = 0;
        lvProfile.UseCompatibleStateImageBehavior = false;
        lvProfile.View = View.Details;
        // 
        // tabApplyModel
        // 
        tabApplyModel.Controls.Add(splitContainer7);
        tabApplyModel.Location = new Point(4, 32);
        tabApplyModel.Name = "tabApplyModel";
        tabApplyModel.Padding = new Padding(3);
        tabApplyModel.Size = new Size(937, 365);
        tabApplyModel.TabIndex = 1;
        tabApplyModel.Text = "ApplyModel";
        tabApplyModel.UseVisualStyleBackColor = true;
        // 
        // splitContainer7
        // 
        splitContainer7.Dock = DockStyle.Fill;
        splitContainer7.Location = new Point(3, 3);
        splitContainer7.Name = "splitContainer7";
        // 
        // splitContainer7.Panel1
        // 
        splitContainer7.Panel1.Controls.Add(lbApplyModel);
        // 
        // splitContainer7.Panel2
        // 
        splitContainer7.Panel2.Controls.Add(splitContainer10);
        splitContainer7.Size = new Size(931, 359);
        splitContainer7.SplitterDistance = 143;
        splitContainer7.TabIndex = 0;
        // 
        // lbApplyModel
        // 
        lbApplyModel.Dock = DockStyle.Fill;
        lbApplyModel.FormattingEnabled = true;
        lbApplyModel.Location = new Point(0, 0);
        lbApplyModel.Name = "lbApplyModel";
        lbApplyModel.Size = new Size(143, 359);
        lbApplyModel.TabIndex = 0;
        lbApplyModel.SelectedIndexChanged += lbApplyModel_SelectedIndexChanged;
        // 
        // splitContainer10
        // 
        splitContainer10.Dock = DockStyle.Fill;
        splitContainer10.Location = new Point(0, 0);
        splitContainer10.Name = "splitContainer10";
        splitContainer10.Orientation = Orientation.Horizontal;
        // 
        // splitContainer10.Panel1
        // 
        splitContainer10.Panel1.Controls.Add(lvApplyModel);
        // 
        // splitContainer10.Panel2
        // 
        splitContainer10.Panel2.Controls.Add(splitQuestionnaire);
        splitContainer10.Size = new Size(784, 359);
        splitContainer10.SplitterDistance = 128;
        splitContainer10.TabIndex = 0;
        // 
        // lvApplyModel
        // 
        lvApplyModel.Dock = DockStyle.Fill;
        lvApplyModel.FullRowSelect = true;
        lvApplyModel.GridLines = true;
        lvApplyModel.Location = new Point(0, 0);
        lvApplyModel.Name = "lvApplyModel";
        lvApplyModel.Size = new Size(784, 128);
        lvApplyModel.TabIndex = 0;
        lvApplyModel.UseCompatibleStateImageBehavior = false;
        lvApplyModel.View = View.Details;
        lvApplyModel.SelectedIndexChanged += lvApplyModel_SelectedIndexChanged;
        lvApplyModel.DoubleClick += lvApplyModel_DoubleClick;
        // 
        // splitQuestionnaire
        // 
        splitQuestionnaire.Dock = DockStyle.Fill;
        splitQuestionnaire.Location = new Point(0, 0);
        splitQuestionnaire.Name = "splitQuestionnaire";
        // 
        // splitQuestionnaire.Panel1
        // 
        splitQuestionnaire.Panel1.Controls.Add(txtQuestionnaire);
        splitQuestionnaire.Panel1.Controls.Add(btnRendering);
        splitQuestionnaire.Panel1.Controls.Add(btnSave);
        splitQuestionnaire.Panel1.Controls.Add(btnLoad);
        splitQuestionnaire.Panel1.Controls.Add(btnRefresh);
        splitQuestionnaire.Size = new Size(784, 227);
        splitQuestionnaire.SplitterDistance = 480;
        splitQuestionnaire.TabIndex = 0;
        // 
        // txtQuestionnaire
        // 
        txtQuestionnaire.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtQuestionnaire.Location = new Point(11, 49);
        txtQuestionnaire.Multiline = true;
        txtQuestionnaire.Name = "txtQuestionnaire";
        txtQuestionnaire.ScrollBars = ScrollBars.Both;
        txtQuestionnaire.Size = new Size(457, 125);
        txtQuestionnaire.TabIndex = 4;
        // 
        // btnRendering
        // 
        btnRendering.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnRendering.Location = new Point(356, 180);
        btnRendering.Name = "btnRendering";
        btnRendering.Size = new Size(112, 34);
        btnRendering.TabIndex = 3;
        btnRendering.Text = "產生界面";
        btnRendering.UseVisualStyleBackColor = true;
        btnRendering.Click += btnRendering_Click;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(272, 10);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(112, 34);
        btnSave.TabIndex = 2;
        btnSave.Text = "儲存";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnLoad
        // 
        btnLoad.Location = new Point(135, 9);
        btnLoad.Name = "btnLoad";
        btnLoad.Size = new Size(112, 34);
        btnLoad.TabIndex = 1;
        btnLoad.Text = "修正";
        btnLoad.UseVisualStyleBackColor = true;
        btnLoad.Click += btnLoad_Click;
        // 
        // btnRefresh
        // 
        btnRefresh.Location = new Point(11, 10);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(112, 34);
        btnRefresh.TabIndex = 0;
        btnRefresh.Text = "更新";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // tabStaging
        // 
        tabStaging.Controls.Add(splitContainer1);
        tabStaging.Location = new Point(4, 32);
        tabStaging.Name = "tabStaging";
        tabStaging.Padding = new Padding(3);
        tabStaging.Size = new Size(937, 365);
        tabStaging.TabIndex = 4;
        tabStaging.Text = "Staging";
        tabStaging.UseVisualStyleBackColor = true;
        tabStaging.Enter += tabStaging_Enter;
        // 
        // splitContainer1
        // 
        splitContainer1.Dock = DockStyle.Fill;
        splitContainer1.Location = new Point(3, 3);
        splitContainer1.Name = "splitContainer1";
        // 
        // splitContainer1.Panel1
        // 
        splitContainer1.Panel1.Controls.Add(lbStaging);
        // 
        // splitContainer1.Panel2
        // 
        splitContainer1.Panel2.Controls.Add(splitContainer2);
        splitContainer1.Size = new Size(931, 359);
        splitContainer1.SplitterDistance = 153;
        splitContainer1.TabIndex = 0;
        // 
        // lbStaging
        // 
        lbStaging.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lbStaging.FormattingEnabled = true;
        lbStaging.Location = new Point(4, 3);
        lbStaging.Name = "lbStaging";
        lbStaging.Size = new Size(147, 349);
        lbStaging.TabIndex = 0;
        lbStaging.SelectedIndexChanged += lbStaging_SelectedIndexChanged;
        // 
        // splitContainer2
        // 
        splitContainer2.Dock = DockStyle.Fill;
        splitContainer2.Location = new Point(0, 0);
        splitContainer2.Name = "splitContainer2";
        splitContainer2.Orientation = Orientation.Horizontal;
        // 
        // splitContainer2.Panel1
        // 
        splitContainer2.Panel1.Controls.Add(lvStaging);
        // 
        // splitContainer2.Panel2
        // 
        splitContainer2.Panel2.Controls.Add(splitContainer3);
        splitContainer2.Size = new Size(774, 359);
        splitContainer2.SplitterDistance = 102;
        splitContainer2.TabIndex = 0;
        // 
        // lvStaging
        // 
        lvStaging.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lvStaging.FullRowSelect = true;
        lvStaging.GridLines = true;
        lvStaging.Location = new Point(0, 3);
        lvStaging.Name = "lvStaging";
        lvStaging.Size = new Size(771, 96);
        lvStaging.TabIndex = 0;
        lvStaging.UseCompatibleStateImageBehavior = false;
        lvStaging.View = View.Details;
        // 
        // splitContainer3
        // 
        splitContainer3.Dock = DockStyle.Fill;
        splitContainer3.Location = new Point(0, 0);
        splitContainer3.Name = "splitContainer3";
        // 
        // splitContainer3.Panel1
        // 
        splitContainer3.Panel1.Controls.Add(splitContainer4);
        // 
        // splitContainer3.Panel2
        // 
        splitContainer3.Panel2.Controls.Add(btnSaveFHIR);
        splitContainer3.Panel2.Controls.Add(txtFHIRData);
        splitContainer3.Size = new Size(774, 253);
        splitContainer3.SplitterDistance = 336;
        splitContainer3.TabIndex = 0;
        // 
        // splitContainer4
        // 
        splitContainer4.Dock = DockStyle.Fill;
        splitContainer4.Location = new Point(0, 0);
        splitContainer4.Name = "splitContainer4";
        splitContainer4.Orientation = Orientation.Horizontal;
        // 
        // splitContainer4.Panel1
        // 
        splitContainer4.Panel1.Controls.Add(txtStaging);
        // 
        // splitContainer4.Panel2
        // 
        splitContainer4.Panel2.Controls.Add(btnFUME);
        splitContainer4.Panel2.Controls.Add(btnFHIRData);
        splitContainer4.Panel2.Controls.Add(txtFume);
        splitContainer4.Size = new Size(336, 253);
        splitContainer4.SplitterDistance = 93;
        splitContainer4.TabIndex = 0;
        // 
        // txtStaging
        // 
        txtStaging.BorderStyle = BorderStyle.FixedSingle;
        txtStaging.Dock = DockStyle.Fill;
        txtStaging.Location = new Point(0, 0);
        txtStaging.Multiline = true;
        txtStaging.Name = "txtStaging";
        txtStaging.ScrollBars = ScrollBars.Both;
        txtStaging.Size = new Size(336, 93);
        txtStaging.TabIndex = 0;
        txtStaging.TextChanged += textBox1_TextChanged;
        // 
        // btnFHIRData
        // 
        btnFHIRData.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnFHIRData.Location = new Point(221, 119);
        btnFHIRData.Name = "btnFHIRData";
        btnFHIRData.Size = new Size(112, 34);
        btnFHIRData.TabIndex = 1;
        btnFHIRData.Text = "FHIR";
        btnFHIRData.UseVisualStyleBackColor = true;
        btnFHIRData.Click += btnFHIRData_Click;
        // 
        // txtFume
        // 
        txtFume.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtFume.BorderStyle = BorderStyle.FixedSingle;
        txtFume.Location = new Point(3, -1);
        txtFume.Multiline = true;
        txtFume.Name = "txtFume";
        txtFume.ScrollBars = ScrollBars.Both;
        txtFume.Size = new Size(330, 118);
        txtFume.TabIndex = 0;
        // 
        // txtFHIRData
        // 
        txtFHIRData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtFHIRData.BorderStyle = BorderStyle.FixedSingle;
        txtFHIRData.Location = new Point(0, 0);
        txtFHIRData.Multiline = true;
        txtFHIRData.Name = "txtFHIRData";
        txtFHIRData.ScrollBars = ScrollBars.Both;
        txtFHIRData.Size = new Size(431, 214);
        txtFHIRData.TabIndex = 0;
        txtFHIRData.TextChanged += txtFHIRData_TextChanged;
        // 
        // tabBundle
        // 
        tabBundle.Controls.Add(splitContainer5);
        tabBundle.Location = new Point(4, 32);
        tabBundle.Name = "tabBundle";
        tabBundle.Size = new Size(937, 365);
        tabBundle.TabIndex = 5;
        tabBundle.Text = "Bundle";
        tabBundle.UseVisualStyleBackColor = true;
        // 
        // splitContainer5
        // 
        splitContainer5.Dock = DockStyle.Fill;
        splitContainer5.Location = new Point(0, 0);
        splitContainer5.Name = "splitContainer5";
        // 
        // splitContainer5.Panel1
        // 
        splitContainer5.Panel1.Controls.Add(splitContainer6);
        splitContainer5.Size = new Size(937, 365);
        splitContainer5.SplitterDistance = 312;
        splitContainer5.TabIndex = 0;
        // 
        // splitContainer6
        // 
        splitContainer6.Dock = DockStyle.Fill;
        splitContainer6.Location = new Point(0, 0);
        splitContainer6.Name = "splitContainer6";
        splitContainer6.Orientation = Orientation.Horizontal;
        // 
        // splitContainer6.Panel1
        // 
        splitContainer6.Panel1.Controls.Add(lbBundleList);
        // 
        // splitContainer6.Panel2
        // 
        splitContainer6.Panel2.Controls.Add(lvBundleProfile);
        splitContainer6.Size = new Size(312, 365);
        splitContainer6.SplitterDistance = 182;
        splitContainer6.TabIndex = 0;
        // 
        // lbBundleList
        // 
        lbBundleList.Dock = DockStyle.Fill;
        lbBundleList.FormattingEnabled = true;
        lbBundleList.Location = new Point(0, 0);
        lbBundleList.Name = "lbBundleList";
        lbBundleList.Size = new Size(312, 182);
        lbBundleList.TabIndex = 0;
        lbBundleList.SelectedIndexChanged += lbBundleList_SelectedIndexChanged;
        // 
        // lvBundleProfile
        // 
        lvBundleProfile.Dock = DockStyle.Fill;
        lvBundleProfile.Location = new Point(0, 0);
        lvBundleProfile.Name = "lvBundleProfile";
        lvBundleProfile.Size = new Size(312, 179);
        lvBundleProfile.TabIndex = 0;
        lvBundleProfile.UseCompatibleStateImageBehavior = false;
        lvBundleProfile.View = View.Details;
        // 
        // tabMsg
        // 
        tabMsg.Controls.Add(txtMsg);
        tabMsg.Location = new Point(4, 32);
        tabMsg.Name = "tabMsg";
        tabMsg.Size = new Size(937, 365);
        tabMsg.TabIndex = 3;
        tabMsg.Text = "Message";
        tabMsg.UseVisualStyleBackColor = true;
        // 
        // txtMsg
        // 
        txtMsg.Dock = DockStyle.Fill;
        txtMsg.Location = new Point(0, 0);
        txtMsg.Multiline = true;
        txtMsg.Name = "txtMsg";
        txtMsg.ScrollBars = ScrollBars.Both;
        txtMsg.Size = new Size(937, 365);
        txtMsg.TabIndex = 0;
        // 
        // tabConfiguration
        // 
        tabConfiguration.Controls.Add(rbUser);
        tabConfiguration.Controls.Add(rbAdmin);
        tabConfiguration.Controls.Add(label4);
        tabConfiguration.Controls.Add(btnDataDir);
        tabConfiguration.Controls.Add(txtDataDirectory);
        tabConfiguration.Controls.Add(label3);
        tabConfiguration.Controls.Add(cmbIG);
        tabConfiguration.Controls.Add(label2);
        tabConfiguration.Controls.Add(cmbHosp);
        tabConfiguration.Controls.Add(lblHosp);
        tabConfiguration.Controls.Add(txtFHIRServer);
        tabConfiguration.Controls.Add(lblFHIRServer);
        tabConfiguration.Location = new Point(4, 32);
        tabConfiguration.Name = "tabConfiguration";
        tabConfiguration.Size = new Size(937, 365);
        tabConfiguration.TabIndex = 2;
        tabConfiguration.Text = "Configuration";
        tabConfiguration.UseVisualStyleBackColor = true;
        // 
        // rbUser
        // 
        rbUser.AutoSize = true;
        rbUser.Location = new Point(371, 211);
        rbUser.Name = "rbUser";
        rbUser.Size = new Size(73, 27);
        rbUser.TabIndex = 11;
        rbUser.Text = "User";
        rbUser.UseVisualStyleBackColor = true;
        // 
        // rbAdmin
        // 
        rbAdmin.AutoSize = true;
        rbAdmin.Checked = true;
        rbAdmin.Location = new Point(212, 213);
        rbAdmin.Name = "rbAdmin";
        rbAdmin.Size = new Size(153, 27);
        rbAdmin.TabIndex = 10;
        rbAdmin.TabStop = true;
        rbAdmin.Text = "Administrator";
        rbAdmin.UseVisualStyleBackColor = true;
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(54, 213);
        label4.Name = "label4";
        label4.Size = new Size(100, 23);
        label4.TabIndex = 9;
        label4.Text = "操作模式：";
        // 
        // btnDataDir
        // 
        btnDataDir.Location = new Point(618, 167);
        btnDataDir.Name = "btnDataDir";
        btnDataDir.Size = new Size(66, 34);
        btnDataDir.TabIndex = 8;
        btnDataDir.Text = "設定";
        btnDataDir.UseVisualStyleBackColor = true;
        // 
        // txtDataDirectory
        // 
        txtDataDirectory.Location = new Point(212, 167);
        txtDataDirectory.Name = "txtDataDirectory";
        txtDataDirectory.Size = new Size(400, 30);
        txtDataDirectory.TabIndex = 7;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(54, 167);
        label3.Name = "label3";
        label3.Size = new Size(100, 23);
        label3.TabIndex = 6;
        label3.Text = "資料目錄：";
        // 
        // cmbIG
        // 
        cmbIG.FormattingEnabled = true;
        cmbIG.Items.AddRange(new object[] { "pas", "ngs", "emr-dms", "emr-pmr", "emr-ic", "emr-image", "emr-ep", "emr-ds" });
        cmbIG.Location = new Point(212, 119);
        cmbIG.Name = "cmbIG";
        cmbIG.Size = new Size(200, 31);
        cmbIG.TabIndex = 5;
        cmbIG.Text = "pas";
        cmbIG.SelectedIndexChanged += cmbIG_SelectedIndexChanged;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(54, 122);
        label2.Name = "label2";
        label2.Size = new Size(100, 23);
        label2.TabIndex = 4;
        label2.Text = "實作指引：";
        // 
        // cmbHosp
        // 
        cmbHosp.FormattingEnabled = true;
        cmbHosp.Location = new Point(212, 76);
        cmbHosp.Name = "cmbHosp";
        cmbHosp.Size = new Size(146, 31);
        cmbHosp.TabIndex = 3;
        // 
        // lblHosp
        // 
        lblHosp.AutoSize = true;
        lblHosp.Location = new Point(54, 76);
        lblHosp.Name = "lblHosp";
        lblHosp.Size = new Size(136, 23);
        lblHosp.TabIndex = 2;
        lblHosp.Text = "醫事機構代碼：";
        lblHosp.Click += lblHosp_Click;
        // 
        // txtFHIRServer
        // 
        txtFHIRServer.Location = new Point(212, 30);
        txtFHIRServer.Name = "txtFHIRServer";
        txtFHIRServer.Size = new Size(263, 30);
        txtFHIRServer.TabIndex = 1;
        txtFHIRServer.Text = "http://localhost:8080/fhir/";
        // 
        // lblFHIRServer
        // 
        lblFHIRServer.AutoSize = true;
        lblFHIRServer.Location = new Point(54, 33);
        lblFHIRServer.Name = "lblFHIRServer";
        lblFHIRServer.Size = new Size(117, 23);
        lblFHIRServer.TabIndex = 0;
        lblFHIRServer.Text = "FHIR Server :";
        // 
        // statusBar
        // 
        statusBar.ImageScalingSize = new Size(24, 24);
        statusBar.Location = new Point(0, 515);
        statusBar.Name = "statusBar";
        statusBar.Size = new Size(961, 22);
        statusBar.TabIndex = 7;
        statusBar.Text = "statusStrip1";
        // 
        // rbDifferential
        // 
        rbDifferential.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        rbDifferential.AutoSize = true;
        rbDifferential.Checked = true;
        rbDifferential.Location = new Point(15, 469);
        rbDifferential.Name = "rbDifferential";
        rbDifferential.Size = new Size(131, 27);
        rbDifferential.TabIndex = 8;
        rbDifferential.TabStop = true;
        rbDifferential.Text = "Differential";
        rbDifferential.UseVisualStyleBackColor = true;
        rbDifferential.Visible = false;
        rbDifferential.CheckedChanged += lbProfile_SelectedIndexChanged;
        // 
        // rbSnapshot
        // 
        rbSnapshot.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        rbSnapshot.AutoSize = true;
        rbSnapshot.Location = new Point(152, 469);
        rbSnapshot.Name = "rbSnapshot";
        rbSnapshot.Size = new Size(118, 27);
        rbSnapshot.TabIndex = 9;
        rbSnapshot.Text = "Snapshop";
        rbSnapshot.UseVisualStyleBackColor = true;
        rbSnapshot.Visible = false;
        rbSnapshot.CheckedChanged += lbProfile_SelectedIndexChanged;
        // 
        // rbApplyModel
        // 
        rbApplyModel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        rbApplyModel.AutoSize = true;
        rbApplyModel.Location = new Point(276, 468);
        rbApplyModel.Name = "rbApplyModel";
        rbApplyModel.Size = new Size(142, 27);
        rbApplyModel.TabIndex = 10;
        rbApplyModel.Text = "Apply Model";
        rbApplyModel.UseVisualStyleBackColor = true;
        rbApplyModel.Visible = false;
        rbApplyModel.CheckedChanged += lbProfile_SelectedIndexChanged;
        // 
        // btnFUME
        // 
        btnFUME.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnFUME.Location = new Point(103, 119);
        btnFUME.Name = "btnFUME";
        btnFUME.Size = new Size(112, 34);
        btnFUME.TabIndex = 2;
        btnFUME.Text = "FUME";
        btnFUME.UseVisualStyleBackColor = true;
        btnFUME.Click += btnFUME_Click;
        // 
        // btnSaveFHIR
        // 
        btnSaveFHIR.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnSaveFHIR.Location = new Point(319, 216);
        btnSaveFHIR.Name = "btnSaveFHIR";
        btnSaveFHIR.Size = new Size(112, 34);
        btnSaveFHIR.TabIndex = 1;
        btnSaveFHIR.Text = "Upload";
        btnSaveFHIR.UseVisualStyleBackColor = true;
        btnSaveFHIR.Click += btnSaveFHIR_Click;
        // 
        // FormIGAnalyzer
        // 
        AutoScaleDimensions = new SizeF(11F, 23F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(961, 537);
        Controls.Add(rbSnapshot);
        Controls.Add(rbDifferential);
        Controls.Add(rbApplyModel);
        Controls.Add(statusBar);
        Controls.Add(tabIG);
        Controls.Add(label1);
        Controls.Add(txtPackage);
        Controls.Add(btnClose);
        Controls.Add(btnSelect);
        Name = "FormIGAnalyzer";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "IG Analyzer";
        WindowState = FormWindowState.Maximized;
        Load += FormIGAnalyzer_Load;
        tabIG.ResumeLayout(false);
        tabProfile.ResumeLayout(false);
        splitContainer8.Panel1.ResumeLayout(false);
        splitContainer8.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer8).EndInit();
        splitContainer8.ResumeLayout(false);
        splitContainer9.Panel1.ResumeLayout(false);
        splitContainer9.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer9).EndInit();
        splitContainer9.ResumeLayout(false);
        tabApplyModel.ResumeLayout(false);
        splitContainer7.Panel1.ResumeLayout(false);
        splitContainer7.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer7).EndInit();
        splitContainer7.ResumeLayout(false);
        splitContainer10.Panel1.ResumeLayout(false);
        splitContainer10.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer10).EndInit();
        splitContainer10.ResumeLayout(false);
        splitQuestionnaire.Panel1.ResumeLayout(false);
        splitQuestionnaire.Panel1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitQuestionnaire).EndInit();
        splitQuestionnaire.ResumeLayout(false);
        tabStaging.ResumeLayout(false);
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        splitContainer2.Panel1.ResumeLayout(false);
        splitContainer2.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
        splitContainer2.ResumeLayout(false);
        splitContainer3.Panel1.ResumeLayout(false);
        splitContainer3.Panel2.ResumeLayout(false);
        splitContainer3.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
        splitContainer3.ResumeLayout(false);
        splitContainer4.Panel1.ResumeLayout(false);
        splitContainer4.Panel1.PerformLayout();
        splitContainer4.Panel2.ResumeLayout(false);
        splitContainer4.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer4).EndInit();
        splitContainer4.ResumeLayout(false);
        tabBundle.ResumeLayout(false);
        splitContainer5.Panel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer5).EndInit();
        splitContainer5.ResumeLayout(false);
        splitContainer6.Panel1.ResumeLayout(false);
        splitContainer6.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer6).EndInit();
        splitContainer6.ResumeLayout(false);
        tabMsg.ResumeLayout(false);
        tabMsg.PerformLayout();
        tabConfiguration.ResumeLayout(false);
        tabConfiguration.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private void FormIGAnalyzer_Load(object sender, EventArgs e)
    {

    }

    // add this method to handle the form closing event
    private void FormIGAnalyzer_Closing(object sender, FormClosingEventArgs e)
    {
        // Set the form to normal window state before closing
        this.WindowState = FormWindowState.Normal;
        this.FormBorderStyle = FormBorderStyle.Sizable; // Optional: Set the border style back to Sizable
    }
 
    #endregion

    private Button btnClose;
    private Button btnSelect;
    private TextBox txtPackage;
    private Label label1;
    private OpenFileDialog ofdPackage;
    private TabControl tabIG;
    private TabPage tabApplyModel;
    private StatusStrip statusBar;
    private ColumnHeader columnHeader1;
    private RadioButton rbDifferential;
    private RadioButton rbSnapshot;
    private RadioButton rbApplyModel;
    private TabPage tabConfiguration;
    private TextBox txtMsg;
    private TabPage tabMsg;
    private Label lblHosp;
    private TextBox txtFHIRServer;
    private Label lblFHIRServer;
    private ComboBox cmbHosp;
    private ComboBox cmbIG;
    private Label label2;
    private TabPage tabStaging;
    private SplitContainer splitContainer1;
    private ListBox lbStaging;
    private SplitContainer splitContainer2;
    private ListView lvStaging;
    private TextBox txtDataDirectory;
    private Label label3;
    private Button btnDataDir;
    private SplitContainer splitContainer3;
    private SplitContainer splitContainer4;
    private TextBox txtStaging;
    private TextBox txtFume;
    private Button btnFHIRData;
    private TextBox txtFHIRData;
    private TabPage tabBundle;
    private Label label4;
    private RadioButton rbUser;
    private RadioButton rbAdmin;
    private SplitContainer splitContainer5;
    private SplitContainer splitContainer6;
    private ListBox lbBundleList;
    private ListView lvBundleProfile;
    private SplitContainer splitContainer7;
    private ListBox lbApplyModel;
    private TabPage tabProfile;
    private SplitContainer splitContainer8;
    private SplitContainer splitContainer9;
    private ListBox lbProfile;
    private ListView lvElement;
    private ListView lvProfile;
    private SplitContainer splitContainer10;
    private ListView lvApplyModel;
    private SplitContainer splitQuestionnaire;
    private Button btnRendering;
    private Button btnSave;
    private Button btnLoad;
    private Button btnRefresh;
    private TextBox txtQuestionnaire;
    private Button btnFUME;
    private Button btnSaveFHIR;
}
