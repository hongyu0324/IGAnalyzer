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
        ofdPackage = new OpenFileDialog();
        tabIG = new TabControl();
        tabConfiguration = new TabPage();
        splitBase = new SplitContainer();
        txtFUMEServer = new TextBox();
        label5 = new Label();
        rbUser = new RadioButton();
        rbAdmin = new RadioButton();
        label4 = new Label();
        btnDataDir = new Button();
        txtDataDirectory = new TextBox();
        label3 = new Label();
        cmbIG = new ComboBox();
        label2 = new Label();
        txtFHIRServer = new TextBox();
        lblFHIRServer = new Label();
        splitContainer14 = new SplitContainer();
        btnExample = new Button();
        btnUpload = new Button();
        lbBase = new ListBox();
        splitBase3 = new SplitContainer();
        splitBase4 = new SplitContainer();
        txtBase = new TextBox();
        lvBase = new ListView();
        tabProfile = new TabPage();
        splitContainer8 = new SplitContainer();
        lbProfile = new ListBox();
        splitContainer9 = new SplitContainer();
        lvElement = new ListView();
        splitContainer11 = new SplitContainer();
        lvProfile = new ListView();
        lvConstraint = new ListView();
        tabApplyModel = new TabPage();
        splitContainer7 = new SplitContainer();
        lbApplyModel = new ListBox();
        splitContainer10 = new SplitContainer();
        lvApplyModel = new ListView();
        splitQuestionnaire = new SplitContainer();
        btnApplyLoad = new Button();
        txtQuestionnaire = new TextBox();
        btnRendering = new Button();
        btnSave = new Button();
        btnLoad = new Button();
        btnRefresh = new Button();
        tabMaster = new TabPage();
        splitMaster = new SplitContainer();
        lbMaster = new ListBox();
        splitMaster1 = new SplitContainer();
        lvMaster = new ListView();
        splitMaster2 = new SplitContainer();
        lvReference = new ListView();
        btnMasterSelect = new Button();
        txtMasterFHIR = new TextBox();
        tabStaging = new TabPage();
        splitContainer1 = new SplitContainer();
        splitContainer12 = new SplitContainer();
        lbStaging = new ListBox();
        txtStaging = new TextBox();
        splitContainer2 = new SplitContainer();
        lvStaging = new ListView();
        splitContainer3 = new SplitContainer();
        splitStaging4 = new SplitContainer();
        btnConfirm = new Button();
        btnFUMECheck = new Button();
        btnStagingLoad = new Button();
        btnStagingCopy = new Button();
        lvFUME = new ListView();
        btnFUME = new Button();
        btnFHIRData = new Button();
        txtFume = new TextBox();
        btnStagingValidate = new Button();
        btnSaveFHIR = new Button();
        txtFHIRData = new TextBox();
        tabBundle = new TabPage();
        splitBundle1 = new SplitContainer();
        splitContainer6 = new SplitContainer();
        lbBundleList = new ListBox();
        lvBundleProfile = new ListView();
        splitBundle2 = new SplitContainer();
        splitBundle3 = new SplitContainer();
        lvBundleConstraint = new ListView();
        splitBundle4 = new SplitContainer();
        lvBundle = new ListView();
        btnBundleCreate = new Button();
        txtBundle = new TextBox();
        tabMsg = new TabPage();
        txtMsg = new TextBox();
        tabVersion = new TabPage();
        btnIGCompare = new Button();
        btnIGCheck = new Button();
        splitIGList1 = new SplitContainer();
        splitIGList2 = new SplitContainer();
        lbIG = new ListBox();
        lvIG = new ListView();
        lvIGNext = new ListView();
        cmdIGList = new ComboBox();
        label6 = new Label();
        statusBar = new StatusStrip();
        lblSatatusBar = new ToolStripStatusLabel();
        columnHeader1 = new ColumnHeader();
        rbDifferential = new RadioButton();
        rbSnapshot = new RadioButton();
        rbApplyModel = new RadioButton();
        btnBundleSelect = new Button();
        tabIG.SuspendLayout();
        tabConfiguration.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitBase).BeginInit();
        splitBase.Panel1.SuspendLayout();
        splitBase.Panel2.SuspendLayout();
        splitBase.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer14).BeginInit();
        splitContainer14.Panel1.SuspendLayout();
        splitContainer14.Panel2.SuspendLayout();
        splitContainer14.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitBase3).BeginInit();
        splitBase3.Panel1.SuspendLayout();
        splitBase3.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitBase4).BeginInit();
        splitBase4.Panel1.SuspendLayout();
        splitBase4.Panel2.SuspendLayout();
        splitBase4.SuspendLayout();
        tabProfile.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer8).BeginInit();
        splitContainer8.Panel1.SuspendLayout();
        splitContainer8.Panel2.SuspendLayout();
        splitContainer8.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer9).BeginInit();
        splitContainer9.Panel1.SuspendLayout();
        splitContainer9.Panel2.SuspendLayout();
        splitContainer9.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer11).BeginInit();
        splitContainer11.Panel1.SuspendLayout();
        splitContainer11.Panel2.SuspendLayout();
        splitContainer11.SuspendLayout();
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
        tabMaster.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitMaster).BeginInit();
        splitMaster.Panel1.SuspendLayout();
        splitMaster.Panel2.SuspendLayout();
        splitMaster.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitMaster1).BeginInit();
        splitMaster1.Panel1.SuspendLayout();
        splitMaster1.Panel2.SuspendLayout();
        splitMaster1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitMaster2).BeginInit();
        splitMaster2.Panel1.SuspendLayout();
        splitMaster2.Panel2.SuspendLayout();
        splitMaster2.SuspendLayout();
        tabStaging.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.Panel2.SuspendLayout();
        splitContainer1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer12).BeginInit();
        splitContainer12.Panel1.SuspendLayout();
        splitContainer12.Panel2.SuspendLayout();
        splitContainer12.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
        splitContainer2.Panel1.SuspendLayout();
        splitContainer2.Panel2.SuspendLayout();
        splitContainer2.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
        splitContainer3.Panel1.SuspendLayout();
        splitContainer3.Panel2.SuspendLayout();
        splitContainer3.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitStaging4).BeginInit();
        splitStaging4.Panel1.SuspendLayout();
        splitStaging4.Panel2.SuspendLayout();
        splitStaging4.SuspendLayout();
        tabBundle.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitBundle1).BeginInit();
        splitBundle1.Panel1.SuspendLayout();
        splitBundle1.Panel2.SuspendLayout();
        splitBundle1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer6).BeginInit();
        splitContainer6.Panel1.SuspendLayout();
        splitContainer6.Panel2.SuspendLayout();
        splitContainer6.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitBundle2).BeginInit();
        splitBundle2.Panel1.SuspendLayout();
        splitBundle2.Panel2.SuspendLayout();
        splitBundle2.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitBundle3).BeginInit();
        splitBundle3.Panel1.SuspendLayout();
        splitBundle3.Panel2.SuspendLayout();
        splitBundle3.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitBundle4).BeginInit();
        splitBundle4.Panel1.SuspendLayout();
        splitBundle4.Panel2.SuspendLayout();
        splitBundle4.SuspendLayout();
        tabMsg.SuspendLayout();
        tabVersion.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitIGList1).BeginInit();
        splitIGList1.Panel1.SuspendLayout();
        splitIGList1.Panel2.SuspendLayout();
        splitIGList1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitIGList2).BeginInit();
        splitIGList2.Panel1.SuspendLayout();
        splitIGList2.Panel2.SuspendLayout();
        splitIGList2.SuspendLayout();
        statusBar.SuspendLayout();
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
        btnSelect.Location = new Point(347, 111);
        btnSelect.Name = "btnSelect";
        btnSelect.Size = new Size(100, 40);
        btnSelect.TabIndex = 3;
        btnSelect.Text = "匯入";
        btnSelect.UseVisualStyleBackColor = false;
        btnSelect.Click += btnSelect_ClickAsync;
        // 
        // txtPackage
        // 
        txtPackage.BorderStyle = BorderStyle.FixedSingle;
        txtPackage.Enabled = false;
        txtPackage.Font = new Font("Microsoft JhengHei UI", 9F);
        txtPackage.Location = new Point(462, 117);
        txtPackage.Name = "txtPackage";
        txtPackage.Size = new Size(600, 30);
        txtPackage.TabIndex = 0;
        // 
        // tabIG
        // 
        tabIG.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        tabIG.Controls.Add(tabConfiguration);
        tabIG.Controls.Add(tabProfile);
        tabIG.Controls.Add(tabApplyModel);
        tabIG.Controls.Add(tabMaster);
        tabIG.Controls.Add(tabStaging);
        tabIG.Controls.Add(tabBundle);
        tabIG.Controls.Add(tabMsg);
        tabIG.Controls.Add(tabVersion);
        tabIG.Location = new Point(12, 12);
        tabIG.Name = "tabIG";
        tabIG.SelectedIndex = 0;
        tabIG.Size = new Size(945, 454);
        tabIG.TabIndex = 6;
        // 
        // tabConfiguration
        // 
        tabConfiguration.Controls.Add(splitBase);
        tabConfiguration.Location = new Point(4, 32);
        tabConfiguration.Name = "tabConfiguration";
        tabConfiguration.Size = new Size(937, 418);
        tabConfiguration.TabIndex = 2;
        tabConfiguration.Text = "Configuration";
        tabConfiguration.UseVisualStyleBackColor = true;
        tabConfiguration.Enter += tabConfiguration_Enter;
        // 
        // splitBase
        // 
        splitBase.Dock = DockStyle.Fill;
        splitBase.FixedPanel = FixedPanel.Panel1;
        splitBase.Location = new Point(0, 0);
        splitBase.Name = "splitBase";
        splitBase.Orientation = Orientation.Horizontal;
        // 
        // splitBase.Panel1
        // 
        splitBase.Panel1.Controls.Add(txtFUMEServer);
        splitBase.Panel1.Controls.Add(label5);
        splitBase.Panel1.Controls.Add(rbUser);
        splitBase.Panel1.Controls.Add(rbAdmin);
        splitBase.Panel1.Controls.Add(label4);
        splitBase.Panel1.Controls.Add(btnDataDir);
        splitBase.Panel1.Controls.Add(txtPackage);
        splitBase.Panel1.Controls.Add(txtDataDirectory);
        splitBase.Panel1.Controls.Add(label3);
        splitBase.Panel1.Controls.Add(btnSelect);
        splitBase.Panel1.Controls.Add(cmbIG);
        splitBase.Panel1.Controls.Add(label2);
        splitBase.Panel1.Controls.Add(txtFHIRServer);
        splitBase.Panel1.Controls.Add(lblFHIRServer);
        // 
        // splitBase.Panel2
        // 
        splitBase.Panel2.Controls.Add(splitContainer14);
        splitBase.Panel2.Paint += splitContainer13_Panel2_Paint;
        splitBase.Size = new Size(937, 418);
        splitBase.SplitterDistance = 232;
        splitBase.TabIndex = 14;
        splitBase.SplitterMoved += splitContainer13_SplitterMoved;
        // 
        // txtFUMEServer
        // 
        txtFUMEServer.Location = new Point(184, 68);
        txtFUMEServer.Name = "txtFUMEServer";
        txtFUMEServer.Size = new Size(263, 30);
        txtFUMEServer.TabIndex = 27;
        txtFUMEServer.Text = "http://localhost:42020/";
        // 
        // label5
        // 
        label5.AutoSize = true;
        label5.Location = new Point(23, 64);
        label5.Name = "label5";
        label5.Size = new Size(126, 23);
        label5.TabIndex = 26;
        label5.Text = "FUME Server :";
        // 
        // rbUser
        // 
        rbUser.AutoSize = true;
        rbUser.Location = new Point(343, 199);
        rbUser.Name = "rbUser";
        rbUser.Size = new Size(73, 27);
        rbUser.TabIndex = 25;
        rbUser.Text = "User";
        rbUser.UseVisualStyleBackColor = true;
        // 
        // rbAdmin
        // 
        rbAdmin.AutoSize = true;
        rbAdmin.Checked = true;
        rbAdmin.Location = new Point(184, 199);
        rbAdmin.Name = "rbAdmin";
        rbAdmin.Size = new Size(153, 27);
        rbAdmin.TabIndex = 24;
        rbAdmin.TabStop = true;
        rbAdmin.Text = "Administrator";
        rbAdmin.UseVisualStyleBackColor = true;
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(23, 201);
        label4.Name = "label4";
        label4.Size = new Size(100, 23);
        label4.TabIndex = 23;
        label4.Text = "操作模式：";
        // 
        // btnDataDir
        // 
        btnDataDir.Location = new Point(600, 154);
        btnDataDir.Name = "btnDataDir";
        btnDataDir.Size = new Size(66, 34);
        btnDataDir.TabIndex = 22;
        btnDataDir.Text = "設定";
        btnDataDir.UseVisualStyleBackColor = true;
        // 
        // txtDataDirectory
        // 
        txtDataDirectory.Location = new Point(184, 154);
        txtDataDirectory.Name = "txtDataDirectory";
        txtDataDirectory.Size = new Size(400, 30);
        txtDataDirectory.TabIndex = 21;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(23, 157);
        label3.Name = "label3";
        label3.Size = new Size(100, 23);
        label3.TabIndex = 20;
        label3.Text = "資料目錄：";
        // 
        // cmbIG
        // 
        cmbIG.Enabled = false;
        cmbIG.FormattingEnabled = true;
        cmbIG.Items.AddRange(new object[] { "pas", "ngs", "ci", "emr-dms", "emr-pmr", "emr-ic", "emr-image", "emr-ep", "emr-ds" });
        cmbIG.Location = new Point(184, 117);
        cmbIG.Name = "cmbIG";
        cmbIG.Size = new Size(146, 31);
        cmbIG.TabIndex = 19;
        cmbIG.Text = "pas";
        cmbIG.SelectedIndexChanged += cmbIG_SelectedIndexChanged;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(23, 120);
        label2.Name = "label2";
        label2.Size = new Size(100, 23);
        label2.TabIndex = 18;
        label2.Text = "實作指引：";
        // 
        // txtFHIRServer
        // 
        txtFHIRServer.Location = new Point(184, 25);
        txtFHIRServer.Name = "txtFHIRServer";
        txtFHIRServer.Size = new Size(263, 30);
        txtFHIRServer.TabIndex = 15;
        txtFHIRServer.Text = "http://localhost:8080/fhir/";
        // 
        // lblFHIRServer
        // 
        lblFHIRServer.AutoSize = true;
        lblFHIRServer.Location = new Point(23, 28);
        lblFHIRServer.Name = "lblFHIRServer";
        lblFHIRServer.Size = new Size(117, 23);
        lblFHIRServer.TabIndex = 14;
        lblFHIRServer.Text = "FHIR Server :";
        // 
        // splitContainer14
        // 
        splitContainer14.Dock = DockStyle.Fill;
        splitContainer14.Location = new Point(0, 0);
        splitContainer14.Name = "splitContainer14";
        // 
        // splitContainer14.Panel1
        // 
        splitContainer14.Panel1.Controls.Add(btnExample);
        splitContainer14.Panel1.Controls.Add(btnUpload);
        splitContainer14.Panel1.Controls.Add(lbBase);
        // 
        // splitContainer14.Panel2
        // 
        splitContainer14.Panel2.Controls.Add(splitBase3);
        splitContainer14.Size = new Size(937, 182);
        splitContainer14.SplitterDistance = 217;
        splitContainer14.TabIndex = 0;
        // 
        // btnExample
        // 
        btnExample.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnExample.Location = new Point(0, 145);
        btnExample.Name = "btnExample";
        btnExample.Size = new Size(112, 34);
        btnExample.TabIndex = 2;
        btnExample.Text = "Example";
        btnExample.UseVisualStyleBackColor = true;
        // 
        // btnUpload
        // 
        btnUpload.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnUpload.Location = new Point(105, 145);
        btnUpload.Name = "btnUpload";
        btnUpload.Size = new Size(112, 34);
        btnUpload.TabIndex = 1;
        btnUpload.Text = "Upload";
        btnUpload.UseVisualStyleBackColor = true;
        btnUpload.Click += btnUpload_Click;
        // 
        // lbBase
        // 
        lbBase.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lbBase.FormattingEnabled = true;
        lbBase.Location = new Point(0, 0);
        lbBase.Name = "lbBase";
        lbBase.Size = new Size(217, 142);
        lbBase.TabIndex = 0;
        lbBase.SelectedIndexChanged += lbBase_SelectedIndexChanged;
        // 
        // splitBase3
        // 
        splitBase3.Dock = DockStyle.Fill;
        splitBase3.Location = new Point(0, 0);
        splitBase3.Name = "splitBase3";
        // 
        // splitBase3.Panel1
        // 
        splitBase3.Panel1.Controls.Add(splitBase4);
        // 
        // splitBase3.Panel2
        // 
        splitBase3.Panel2.BackColor = Color.LightGray;
        splitBase3.Size = new Size(716, 182);
        splitBase3.SplitterDistance = 431;
        splitBase3.TabIndex = 0;
        // 
        // splitBase4
        // 
        splitBase4.Dock = DockStyle.Fill;
        splitBase4.Location = new Point(0, 0);
        splitBase4.Name = "splitBase4";
        splitBase4.Orientation = Orientation.Horizontal;
        // 
        // splitBase4.Panel1
        // 
        splitBase4.Panel1.Controls.Add(txtBase);
        // 
        // splitBase4.Panel2
        // 
        splitBase4.Panel2.Controls.Add(lvBase);
        splitBase4.Size = new Size(431, 182);
        splitBase4.SplitterDistance = 117;
        splitBase4.TabIndex = 0;
        // 
        // txtBase
        // 
        txtBase.BorderStyle = BorderStyle.FixedSingle;
        txtBase.Dock = DockStyle.Fill;
        txtBase.Location = new Point(0, 0);
        txtBase.Multiline = true;
        txtBase.Name = "txtBase";
        txtBase.ScrollBars = ScrollBars.Both;
        txtBase.Size = new Size(431, 117);
        txtBase.TabIndex = 0;
        // 
        // lvBase
        // 
        lvBase.Dock = DockStyle.Fill;
        lvBase.FullRowSelect = true;
        lvBase.GridLines = true;
        lvBase.Location = new Point(0, 0);
        lvBase.Name = "lvBase";
        lvBase.Size = new Size(431, 61);
        lvBase.TabIndex = 1;
        lvBase.UseCompatibleStateImageBehavior = false;
        lvBase.View = View.Details;
        lvBase.SelectedIndexChanged += lvBase_SelectedIndexChanged;
        // 
        // tabProfile
        // 
        tabProfile.Controls.Add(splitContainer8);
        tabProfile.Location = new Point(4, 32);
        tabProfile.Name = "tabProfile";
        tabProfile.Size = new Size(937, 418);
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
        splitContainer8.Size = new Size(937, 418);
        splitContainer8.SplitterDistance = 153;
        splitContainer8.TabIndex = 0;
        // 
        // lbProfile
        // 
        lbProfile.Dock = DockStyle.Fill;
        lbProfile.FormattingEnabled = true;
        lbProfile.Location = new Point(0, 0);
        lbProfile.Name = "lbProfile";
        lbProfile.Size = new Size(153, 418);
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
        splitContainer9.Panel2.Controls.Add(splitContainer11);
        splitContainer9.Size = new Size(780, 418);
        splitContainer9.SplitterDistance = 237;
        splitContainer9.TabIndex = 0;
        // 
        // lvElement
        // 
        lvElement.Dock = DockStyle.Fill;
        lvElement.FullRowSelect = true;
        lvElement.GridLines = true;
        lvElement.Location = new Point(0, 0);
        lvElement.Name = "lvElement";
        lvElement.Size = new Size(780, 237);
        lvElement.TabIndex = 0;
        lvElement.UseCompatibleStateImageBehavior = false;
        lvElement.View = View.Details;
        // 
        // splitContainer11
        // 
        splitContainer11.Dock = DockStyle.Fill;
        splitContainer11.Location = new Point(0, 0);
        splitContainer11.Name = "splitContainer11";
        splitContainer11.Orientation = Orientation.Horizontal;
        // 
        // splitContainer11.Panel1
        // 
        splitContainer11.Panel1.Controls.Add(lvProfile);
        // 
        // splitContainer11.Panel2
        // 
        splitContainer11.Panel2.Controls.Add(lvConstraint);
        splitContainer11.Size = new Size(780, 177);
        splitContainer11.SplitterDistance = 71;
        splitContainer11.TabIndex = 0;
        // 
        // lvProfile
        // 
        lvProfile.Dock = DockStyle.Fill;
        lvProfile.FullRowSelect = true;
        lvProfile.GridLines = true;
        lvProfile.Location = new Point(0, 0);
        lvProfile.Name = "lvProfile";
        lvProfile.Size = new Size(780, 71);
        lvProfile.TabIndex = 1;
        lvProfile.UseCompatibleStateImageBehavior = false;
        lvProfile.View = View.Details;
        // 
        // lvConstraint
        // 
        lvConstraint.Dock = DockStyle.Fill;
        lvConstraint.FullRowSelect = true;
        lvConstraint.GridLines = true;
        lvConstraint.Location = new Point(0, 0);
        lvConstraint.Name = "lvConstraint";
        lvConstraint.Size = new Size(780, 102);
        lvConstraint.TabIndex = 0;
        lvConstraint.UseCompatibleStateImageBehavior = false;
        lvConstraint.View = View.Details;
        // 
        // tabApplyModel
        // 
        tabApplyModel.Controls.Add(splitContainer7);
        tabApplyModel.Location = new Point(4, 32);
        tabApplyModel.Name = "tabApplyModel";
        tabApplyModel.Padding = new Padding(3);
        tabApplyModel.Size = new Size(937, 418);
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
        splitContainer7.Size = new Size(931, 412);
        splitContainer7.SplitterDistance = 143;
        splitContainer7.TabIndex = 0;
        // 
        // lbApplyModel
        // 
        lbApplyModel.Dock = DockStyle.Fill;
        lbApplyModel.FormattingEnabled = true;
        lbApplyModel.Location = new Point(0, 0);
        lbApplyModel.Name = "lbApplyModel";
        lbApplyModel.Size = new Size(143, 412);
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
        splitContainer10.Size = new Size(784, 412);
        splitContainer10.SplitterDistance = 146;
        splitContainer10.TabIndex = 0;
        // 
        // lvApplyModel
        // 
        lvApplyModel.Dock = DockStyle.Fill;
        lvApplyModel.FullRowSelect = true;
        lvApplyModel.GridLines = true;
        lvApplyModel.Location = new Point(0, 0);
        lvApplyModel.Name = "lvApplyModel";
        lvApplyModel.Size = new Size(784, 146);
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
        splitQuestionnaire.Panel1.Controls.Add(btnApplyLoad);
        splitQuestionnaire.Panel1.Controls.Add(txtQuestionnaire);
        splitQuestionnaire.Panel1.Controls.Add(btnRendering);
        splitQuestionnaire.Panel1.Controls.Add(btnSave);
        splitQuestionnaire.Panel1.Controls.Add(btnLoad);
        splitQuestionnaire.Panel1.Controls.Add(btnRefresh);
        splitQuestionnaire.Size = new Size(784, 262);
        splitQuestionnaire.SplitterDistance = 480;
        splitQuestionnaire.TabIndex = 0;
        // 
        // btnApplyLoad
        // 
        btnApplyLoad.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnApplyLoad.Location = new Point(238, 215);
        btnApplyLoad.Name = "btnApplyLoad";
        btnApplyLoad.Size = new Size(112, 34);
        btnApplyLoad.TabIndex = 5;
        btnApplyLoad.Text = "匯入";
        btnApplyLoad.UseVisualStyleBackColor = true;
        // 
        // txtQuestionnaire
        // 
        txtQuestionnaire.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtQuestionnaire.Location = new Point(11, 49);
        txtQuestionnaire.Multiline = true;
        txtQuestionnaire.Name = "txtQuestionnaire";
        txtQuestionnaire.ScrollBars = ScrollBars.Both;
        txtQuestionnaire.Size = new Size(457, 160);
        txtQuestionnaire.TabIndex = 4;
        // 
        // btnRendering
        // 
        btnRendering.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnRendering.Location = new Point(356, 215);
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
        // tabMaster
        // 
        tabMaster.Controls.Add(splitMaster);
        tabMaster.Location = new Point(4, 32);
        tabMaster.Name = "tabMaster";
        tabMaster.Size = new Size(937, 418);
        tabMaster.TabIndex = 8;
        tabMaster.Text = "Master";
        tabMaster.UseVisualStyleBackColor = true;
        tabMaster.Enter += tabStaging_Enter;
        // 
        // splitMaster
        // 
        splitMaster.Dock = DockStyle.Fill;
        splitMaster.Location = new Point(0, 0);
        splitMaster.Name = "splitMaster";
        // 
        // splitMaster.Panel1
        // 
        splitMaster.Panel1.Controls.Add(lbMaster);
        // 
        // splitMaster.Panel2
        // 
        splitMaster.Panel2.Controls.Add(splitMaster1);
        splitMaster.Size = new Size(937, 418);
        splitMaster.SplitterDistance = 312;
        splitMaster.TabIndex = 0;
        // 
        // lbMaster
        // 
        lbMaster.Dock = DockStyle.Fill;
        lbMaster.FormattingEnabled = true;
        lbMaster.Location = new Point(0, 0);
        lbMaster.Name = "lbMaster";
        lbMaster.ScrollAlwaysVisible = true;
        lbMaster.Size = new Size(312, 418);
        lbMaster.TabIndex = 1;
        lbMaster.SelectedIndexChanged += lbMaster_SelectedIndexChanged;
        // 
        // splitMaster1
        // 
        splitMaster1.Dock = DockStyle.Fill;
        splitMaster1.Location = new Point(0, 0);
        splitMaster1.Name = "splitMaster1";
        splitMaster1.Orientation = Orientation.Horizontal;
        // 
        // splitMaster1.Panel1
        // 
        splitMaster1.Panel1.Controls.Add(lvMaster);
        // 
        // splitMaster1.Panel2
        // 
        splitMaster1.Panel2.Controls.Add(splitMaster2);
        splitMaster1.Size = new Size(621, 418);
        splitMaster1.SplitterDistance = 207;
        splitMaster1.TabIndex = 0;
        // 
        // lvMaster
        // 
        lvMaster.Dock = DockStyle.Fill;
        lvMaster.FullRowSelect = true;
        lvMaster.GridLines = true;
        lvMaster.Location = new Point(0, 0);
        lvMaster.Name = "lvMaster";
        lvMaster.Size = new Size(621, 207);
        lvMaster.TabIndex = 0;
        lvMaster.UseCompatibleStateImageBehavior = false;
        lvMaster.View = View.Details;
        lvMaster.SelectedIndexChanged += lvMaster_SelectedIndexChanged;
        lvMaster.DoubleClick += LvMaster_DoubleClickAsync;
        // 
        // splitMaster2
        // 
        splitMaster2.Dock = DockStyle.Fill;
        splitMaster2.Location = new Point(0, 0);
        splitMaster2.Name = "splitMaster2";
        // 
        // splitMaster2.Panel1
        // 
        splitMaster2.Panel1.Controls.Add(lvReference);
        splitMaster2.Panel1.Controls.Add(btnMasterSelect);
        // 
        // splitMaster2.Panel2
        // 
        splitMaster2.Panel2.Controls.Add(txtMasterFHIR);
        splitMaster2.Size = new Size(621, 207);
        splitMaster2.SplitterDistance = 277;
        splitMaster2.TabIndex = 0;
        // 
        // lvReference
        // 
        lvReference.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lvReference.FullRowSelect = true;
        lvReference.Location = new Point(0, 46);
        lvReference.MultiSelect = false;
        lvReference.Name = "lvReference";
        lvReference.Size = new Size(275, 161);
        lvReference.TabIndex = 1;
        lvReference.UseCompatibleStateImageBehavior = false;
        lvReference.View = View.Details;
        // 
        // btnMasterSelect
        // 
        btnMasterSelect.Location = new Point(9, 6);
        btnMasterSelect.Name = "btnMasterSelect";
        btnMasterSelect.Size = new Size(112, 34);
        btnMasterSelect.TabIndex = 0;
        btnMasterSelect.Text = "確認";
        btnMasterSelect.UseVisualStyleBackColor = true;
        btnMasterSelect.Click += btnMasterSelect_ClickAsync;
        // 
        // txtMasterFHIR
        // 
        txtMasterFHIR.Dock = DockStyle.Fill;
        txtMasterFHIR.Location = new Point(0, 0);
        txtMasterFHIR.Multiline = true;
        txtMasterFHIR.Name = "txtMasterFHIR";
        txtMasterFHIR.ScrollBars = ScrollBars.Both;
        txtMasterFHIR.Size = new Size(340, 207);
        txtMasterFHIR.TabIndex = 0;
        // 
        // tabStaging
        // 
        tabStaging.Controls.Add(splitContainer1);
        tabStaging.Location = new Point(4, 32);
        tabStaging.Name = "tabStaging";
        tabStaging.Padding = new Padding(3);
        tabStaging.Size = new Size(937, 418);
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
        splitContainer1.Panel1.Controls.Add(splitContainer12);
        // 
        // splitContainer1.Panel2
        // 
        splitContainer1.Panel2.Controls.Add(splitContainer2);
        splitContainer1.Size = new Size(931, 412);
        splitContainer1.SplitterDistance = 153;
        splitContainer1.TabIndex = 0;
        // 
        // splitContainer12
        // 
        splitContainer12.Dock = DockStyle.Fill;
        splitContainer12.Location = new Point(0, 0);
        splitContainer12.Name = "splitContainer12";
        splitContainer12.Orientation = Orientation.Horizontal;
        // 
        // splitContainer12.Panel1
        // 
        splitContainer12.Panel1.Controls.Add(lbStaging);
        // 
        // splitContainer12.Panel2
        // 
        splitContainer12.Panel2.Controls.Add(txtStaging);
        splitContainer12.Size = new Size(153, 412);
        splitContainer12.SplitterDistance = 228;
        splitContainer12.TabIndex = 1;
        // 
        // lbStaging
        // 
        lbStaging.Dock = DockStyle.Fill;
        lbStaging.FormattingEnabled = true;
        lbStaging.Location = new Point(0, 0);
        lbStaging.Name = "lbStaging";
        lbStaging.ScrollAlwaysVisible = true;
        lbStaging.Size = new Size(153, 228);
        lbStaging.TabIndex = 1;
        lbStaging.SelectedIndexChanged += lbStaging_SelectedIndexChanged;
        // 
        // txtStaging
        // 
        txtStaging.BorderStyle = BorderStyle.FixedSingle;
        txtStaging.Dock = DockStyle.Fill;
        txtStaging.Location = new Point(0, 0);
        txtStaging.Multiline = true;
        txtStaging.Name = "txtStaging";
        txtStaging.ScrollBars = ScrollBars.Both;
        txtStaging.Size = new Size(153, 180);
        txtStaging.TabIndex = 1;
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
        splitContainer2.Size = new Size(774, 412);
        splitContainer2.SplitterDistance = 117;
        splitContainer2.TabIndex = 0;
        // 
        // lvStaging
        // 
        lvStaging.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lvStaging.FullRowSelect = true;
        lvStaging.GridLines = true;
        lvStaging.Location = new Point(0, 3);
        lvStaging.Name = "lvStaging";
        lvStaging.Size = new Size(771, 111);
        lvStaging.TabIndex = 0;
        lvStaging.UseCompatibleStateImageBehavior = false;
        lvStaging.View = View.Details;
        lvStaging.Click += lvStaging_SelectedIndexChanged;
        lvStaging.DoubleClick += lvStaging_DoubleClick;
        // 
        // splitContainer3
        // 
        splitContainer3.Dock = DockStyle.Fill;
        splitContainer3.Location = new Point(0, 0);
        splitContainer3.Name = "splitContainer3";
        // 
        // splitContainer3.Panel1
        // 
        splitContainer3.Panel1.Controls.Add(splitStaging4);
        // 
        // splitContainer3.Panel2
        // 
        splitContainer3.Panel2.Controls.Add(btnStagingValidate);
        splitContainer3.Panel2.Controls.Add(btnSaveFHIR);
        splitContainer3.Panel2.Controls.Add(txtFHIRData);
        splitContainer3.Size = new Size(774, 291);
        splitContainer3.SplitterDistance = 391;
        splitContainer3.TabIndex = 0;
        // 
        // splitStaging4
        // 
        splitStaging4.Dock = DockStyle.Fill;
        splitStaging4.FixedPanel = FixedPanel.Panel1;
        splitStaging4.Location = new Point(0, 0);
        splitStaging4.Name = "splitStaging4";
        splitStaging4.Orientation = Orientation.Horizontal;
        // 
        // splitStaging4.Panel1
        // 
        splitStaging4.Panel1.Controls.Add(btnConfirm);
        splitStaging4.Panel1.Controls.Add(btnFUMECheck);
        splitStaging4.Panel1.Controls.Add(btnStagingLoad);
        // 
        // splitStaging4.Panel2
        // 
        splitStaging4.Panel2.Controls.Add(btnStagingCopy);
        splitStaging4.Panel2.Controls.Add(lvFUME);
        splitStaging4.Panel2.Controls.Add(btnFUME);
        splitStaging4.Panel2.Controls.Add(btnFHIRData);
        splitStaging4.Panel2.Controls.Add(txtFume);
        splitStaging4.Size = new Size(391, 291);
        splitStaging4.SplitterDistance = 55;
        splitStaging4.TabIndex = 0;
        // 
        // btnConfirm
        // 
        btnConfirm.Location = new Point(3, 13);
        btnConfirm.Name = "btnConfirm";
        btnConfirm.Size = new Size(112, 34);
        btnConfirm.TabIndex = 3;
        btnConfirm.Text = "檢查";
        btnConfirm.UseVisualStyleBackColor = true;
        btnConfirm.Click += btnConfirm_Click;
        // 
        // btnFUMECheck
        // 
        btnFUMECheck.Location = new Point(266, 13);
        btnFUMECheck.Name = "btnFUMECheck";
        btnFUMECheck.Size = new Size(112, 34);
        btnFUMECheck.TabIndex = 2;
        btnFUMECheck.Text = "切換";
        btnFUMECheck.UseVisualStyleBackColor = true;
        btnFUMECheck.Click += btnFUMECheck_Click;
        // 
        // btnStagingLoad
        // 
        btnStagingLoad.Location = new Point(121, 13);
        btnStagingLoad.Name = "btnStagingLoad";
        btnStagingLoad.Size = new Size(112, 34);
        btnStagingLoad.TabIndex = 1;
        btnStagingLoad.Text = "修正";
        btnStagingLoad.UseVisualStyleBackColor = true;
        btnStagingLoad.Click += btnStagingLoad_Click;
        // 
        // btnStagingCopy
        // 
        btnStagingCopy.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnStagingCopy.Location = new Point(40, 195);
        btnStagingCopy.Name = "btnStagingCopy";
        btnStagingCopy.Size = new Size(112, 34);
        btnStagingCopy.TabIndex = 4;
        btnStagingCopy.Text = "Copy";
        btnStagingCopy.UseVisualStyleBackColor = true;
        // 
        // lvFUME
        // 
        lvFUME.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lvFUME.BorderStyle = BorderStyle.FixedSingle;
        lvFUME.FullRowSelect = true;
        lvFUME.GridLines = true;
        lvFUME.Location = new Point(0, 0);
        lvFUME.Name = "lvFUME";
        lvFUME.Size = new Size(391, 194);
        lvFUME.TabIndex = 3;
        lvFUME.UseCompatibleStateImageBehavior = false;
        lvFUME.View = View.Details;
        lvFUME.Visible = false;
        lvFUME.DoubleClick += lvFUME_DoubleClick;
        // 
        // btnFUME
        // 
        btnFUME.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnFUME.Location = new Point(158, 195);
        btnFUME.Name = "btnFUME";
        btnFUME.Size = new Size(112, 34);
        btnFUME.TabIndex = 2;
        btnFUME.Text = "FUME";
        btnFUME.UseVisualStyleBackColor = true;
        btnFUME.Click += btnFUME_Click;
        // 
        // btnFHIRData
        // 
        btnFHIRData.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnFHIRData.Location = new Point(276, 195);
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
        txtFume.Size = new Size(385, 194);
        txtFume.TabIndex = 0;
        // 
        // btnStagingValidate
        // 
        btnStagingValidate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnStagingValidate.Location = new Point(146, 254);
        btnStagingValidate.Name = "btnStagingValidate";
        btnStagingValidate.Size = new Size(112, 34);
        btnStagingValidate.TabIndex = 2;
        btnStagingValidate.Text = "Validate";
        btnStagingValidate.UseVisualStyleBackColor = true;
        btnStagingValidate.Click += btnStagingValidate_Click;
        // 
        // btnSaveFHIR
        // 
        btnSaveFHIR.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnSaveFHIR.Location = new Point(264, 254);
        btnSaveFHIR.Name = "btnSaveFHIR";
        btnSaveFHIR.Size = new Size(112, 34);
        btnSaveFHIR.TabIndex = 1;
        btnSaveFHIR.Text = "Upload";
        btnSaveFHIR.UseVisualStyleBackColor = true;
        btnSaveFHIR.Click += btnSaveFHIR_Click;
        // 
        // txtFHIRData
        // 
        txtFHIRData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtFHIRData.BorderStyle = BorderStyle.FixedSingle;
        txtFHIRData.Location = new Point(0, 0);
        txtFHIRData.Multiline = true;
        txtFHIRData.Name = "txtFHIRData";
        txtFHIRData.ScrollBars = ScrollBars.Both;
        txtFHIRData.Size = new Size(376, 252);
        txtFHIRData.TabIndex = 0;
        txtFHIRData.TextChanged += txtFHIRData_TextChanged;
        // 
        // tabBundle
        // 
        tabBundle.Controls.Add(splitBundle1);
        tabBundle.Location = new Point(4, 32);
        tabBundle.Name = "tabBundle";
        tabBundle.Size = new Size(937, 418);
        tabBundle.TabIndex = 5;
        tabBundle.Text = "Bundle";
        tabBundle.UseVisualStyleBackColor = true;
        // 
        // splitBundle1
        // 
        splitBundle1.Dock = DockStyle.Fill;
        splitBundle1.Location = new Point(0, 0);
        splitBundle1.Name = "splitBundle1";
        // 
        // splitBundle1.Panel1
        // 
        splitBundle1.Panel1.Controls.Add(splitContainer6);
        // 
        // splitBundle1.Panel2
        // 
        splitBundle1.Panel2.Controls.Add(splitBundle2);
        splitBundle1.Size = new Size(937, 418);
        splitBundle1.SplitterDistance = 312;
        splitBundle1.TabIndex = 0;
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
        splitContainer6.Size = new Size(312, 418);
        splitContainer6.SplitterDistance = 51;
        splitContainer6.TabIndex = 0;
        // 
        // lbBundleList
        // 
        lbBundleList.Dock = DockStyle.Fill;
        lbBundleList.FormattingEnabled = true;
        lbBundleList.Location = new Point(0, 0);
        lbBundleList.Name = "lbBundleList";
        lbBundleList.Size = new Size(312, 51);
        lbBundleList.TabIndex = 0;
        lbBundleList.SelectedIndexChanged += lbBundleList_SelectedIndexChanged;
        // 
        // lvBundleProfile
        // 
        lvBundleProfile.Dock = DockStyle.Fill;
        lvBundleProfile.FullRowSelect = true;
        lvBundleProfile.GridLines = true;
        lvBundleProfile.Location = new Point(0, 0);
        lvBundleProfile.Name = "lvBundleProfile";
        lvBundleProfile.Size = new Size(312, 363);
        lvBundleProfile.TabIndex = 0;
        lvBundleProfile.UseCompatibleStateImageBehavior = false;
        lvBundleProfile.View = View.Details;
        lvBundleProfile.SelectedIndexChanged += lvBundleProfile_SelectedIndexChanged;
        // 
        // splitBundle2
        // 
        splitBundle2.Dock = DockStyle.Fill;
        splitBundle2.Location = new Point(0, 0);
        splitBundle2.Name = "splitBundle2";
        // 
        // splitBundle2.Panel1
        // 
        splitBundle2.Panel1.Controls.Add(splitBundle3);
        // 
        // splitBundle2.Panel2
        // 
        splitBundle2.Panel2.Controls.Add(txtBundle);
        splitBundle2.Size = new Size(621, 418);
        splitBundle2.SplitterDistance = 387;
        splitBundle2.TabIndex = 0;
        // 
        // splitBundle3
        // 
        splitBundle3.Dock = DockStyle.Fill;
        splitBundle3.Location = new Point(0, 0);
        splitBundle3.Name = "splitBundle3";
        splitBundle3.Orientation = Orientation.Horizontal;
        // 
        // splitBundle3.Panel1
        // 
        splitBundle3.Panel1.Controls.Add(lvBundleConstraint);
        // 
        // splitBundle3.Panel2
        // 
        splitBundle3.Panel2.Controls.Add(splitBundle4);
        splitBundle3.Size = new Size(387, 418);
        splitBundle3.SplitterDistance = 155;
        splitBundle3.TabIndex = 0;
        // 
        // lvBundleConstraint
        // 
        lvBundleConstraint.Dock = DockStyle.Fill;
        lvBundleConstraint.Location = new Point(0, 0);
        lvBundleConstraint.Name = "lvBundleConstraint";
        lvBundleConstraint.Size = new Size(387, 155);
        lvBundleConstraint.TabIndex = 0;
        lvBundleConstraint.UseCompatibleStateImageBehavior = false;
        lvBundleConstraint.View = View.Details;
        // 
        // splitBundle4
        // 
        splitBundle4.Dock = DockStyle.Fill;
        splitBundle4.FixedPanel = FixedPanel.Panel2;
        splitBundle4.Location = new Point(0, 0);
        splitBundle4.Name = "splitBundle4";
        splitBundle4.Orientation = Orientation.Horizontal;
        // 
        // splitBundle4.Panel1
        // 
        splitBundle4.Panel1.Controls.Add(lvBundle);
        // 
        // splitBundle4.Panel2
        // 
        splitBundle4.Panel2.Controls.Add(btnBundleSelect);
        splitBundle4.Panel2.Controls.Add(btnBundleCreate);
        splitBundle4.Size = new Size(387, 259);
        splitBundle4.SplitterDistance = 208;
        splitBundle4.TabIndex = 0;
        // 
        // lvBundle
        // 
        lvBundle.Dock = DockStyle.Fill;
        lvBundle.FullRowSelect = true;
        lvBundle.GridLines = true;
        lvBundle.Location = new Point(0, 0);
        lvBundle.Name = "lvBundle";
        lvBundle.Size = new Size(387, 208);
        lvBundle.TabIndex = 0;
        lvBundle.UseCompatibleStateImageBehavior = false;
        lvBundle.View = View.Details;
        lvBundle.DoubleClick += lvBundle_DoubleClickAsync;
        // 
        // btnBundleCreate
        // 
        btnBundleCreate.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        btnBundleCreate.Location = new Point(272, 7);
        btnBundleCreate.Name = "btnBundleCreate";
        btnBundleCreate.Size = new Size(112, 34);
        btnBundleCreate.TabIndex = 0;
        btnBundleCreate.Text = "產生";
        btnBundleCreate.UseVisualStyleBackColor = true;
        btnBundleCreate.Click += btnBundleCreate_Click;
        // 
        // txtBundle
        // 
        txtBundle.Dock = DockStyle.Fill;
        txtBundle.Location = new Point(0, 0);
        txtBundle.Multiline = true;
        txtBundle.Name = "txtBundle";
        txtBundle.Size = new Size(230, 418);
        txtBundle.TabIndex = 0;
        // 
        // tabMsg
        // 
        tabMsg.Controls.Add(txtMsg);
        tabMsg.Location = new Point(4, 32);
        tabMsg.Name = "tabMsg";
        tabMsg.Size = new Size(937, 418);
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
        txtMsg.Size = new Size(937, 418);
        txtMsg.TabIndex = 0;
        // 
        // tabVersion
        // 
        tabVersion.Controls.Add(btnIGCompare);
        tabVersion.Controls.Add(btnIGCheck);
        tabVersion.Controls.Add(splitIGList1);
        tabVersion.Controls.Add(cmdIGList);
        tabVersion.Controls.Add(label6);
        tabVersion.Location = new Point(4, 32);
        tabVersion.Name = "tabVersion";
        tabVersion.Size = new Size(937, 418);
        tabVersion.TabIndex = 7;
        tabVersion.Text = "Version";
        tabVersion.UseVisualStyleBackColor = true;
        // 
        // btnIGCompare
        // 
        btnIGCompare.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnIGCompare.Location = new Point(821, 18);
        btnIGCompare.Name = "btnIGCompare";
        btnIGCompare.Size = new Size(112, 34);
        btnIGCompare.TabIndex = 4;
        btnIGCompare.Text = "比較";
        btnIGCompare.UseVisualStyleBackColor = true;
        // 
        // btnIGCheck
        // 
        btnIGCheck.Location = new Point(340, 18);
        btnIGCheck.Name = "btnIGCheck";
        btnIGCheck.Size = new Size(112, 34);
        btnIGCheck.TabIndex = 3;
        btnIGCheck.Text = "檢查";
        btnIGCheck.UseVisualStyleBackColor = true;
        btnIGCheck.Click += btnIGCheck_Click;
        // 
        // splitIGList1
        // 
        splitIGList1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        splitIGList1.Location = new Point(0, 54);
        splitIGList1.Name = "splitIGList1";
        // 
        // splitIGList1.Panel1
        // 
        splitIGList1.Panel1.Controls.Add(splitIGList2);
        // 
        // splitIGList1.Panel2
        // 
        splitIGList1.Panel2.Controls.Add(lvIGNext);
        splitIGList1.Size = new Size(937, 364);
        splitIGList1.SplitterDistance = 574;
        splitIGList1.TabIndex = 2;
        // 
        // splitIGList2
        // 
        splitIGList2.Dock = DockStyle.Fill;
        splitIGList2.Location = new Point(0, 0);
        splitIGList2.Name = "splitIGList2";
        // 
        // splitIGList2.Panel1
        // 
        splitIGList2.Panel1.Controls.Add(lbIG);
        // 
        // splitIGList2.Panel2
        // 
        splitIGList2.Panel2.Controls.Add(lvIG);
        splitIGList2.Size = new Size(574, 364);
        splitIGList2.SplitterDistance = 190;
        splitIGList2.TabIndex = 0;
        // 
        // lbIG
        // 
        lbIG.Dock = DockStyle.Fill;
        lbIG.FormattingEnabled = true;
        lbIG.Location = new Point(0, 0);
        lbIG.Name = "lbIG";
        lbIG.Size = new Size(190, 364);
        lbIG.TabIndex = 0;
        // 
        // lvIG
        // 
        lvIG.Dock = DockStyle.Fill;
        lvIG.Location = new Point(0, 0);
        lvIG.Name = "lvIG";
        lvIG.Size = new Size(380, 364);
        lvIG.TabIndex = 0;
        lvIG.UseCompatibleStateImageBehavior = false;
        // 
        // lvIGNext
        // 
        lvIGNext.Dock = DockStyle.Fill;
        lvIGNext.Location = new Point(0, 0);
        lvIGNext.Name = "lvIGNext";
        lvIGNext.Size = new Size(359, 364);
        lvIGNext.TabIndex = 0;
        lvIGNext.UseCompatibleStateImageBehavior = false;
        // 
        // cmdIGList
        // 
        cmdIGList.FormattingEnabled = true;
        cmdIGList.Location = new Point(136, 18);
        cmdIGList.Name = "cmdIGList";
        cmdIGList.Size = new Size(182, 31);
        cmdIGList.TabIndex = 1;
        cmdIGList.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.Location = new Point(7, 21);
        label6.Name = "label6";
        label6.Size = new Size(130, 23);
        label6.TabIndex = 0;
        label6.Text = "實作指引(IG)：";
        // 
        // statusBar
        // 
        statusBar.ImageScalingSize = new Size(24, 24);
        statusBar.Items.AddRange(new ToolStripItem[] { lblSatatusBar });
        statusBar.Location = new Point(0, 515);
        statusBar.Name = "statusBar";
        statusBar.Size = new Size(961, 22);
        statusBar.TabIndex = 7;
        statusBar.Text = "statusStrip1";
        // 
        // lblSatatusBar
        // 
        lblSatatusBar.Name = "lblSatatusBar";
        lblSatatusBar.Size = new Size(0, 15);
        lblSatatusBar.TextImageRelation = TextImageRelation.Overlay;
        lblSatatusBar.Click += toolStripStatusLabel1_Click;
        // 
        // rbDifferential
        // 
        rbDifferential.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        rbDifferential.AutoSize = true;
        rbDifferential.Checked = true;
        rbDifferential.Location = new Point(16, 479);
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
        rbSnapshot.Location = new Point(152, 479);
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
        rbApplyModel.Location = new Point(276, 479);
        rbApplyModel.Name = "rbApplyModel";
        rbApplyModel.Size = new Size(142, 27);
        rbApplyModel.TabIndex = 10;
        rbApplyModel.Text = "Apply Model";
        rbApplyModel.UseVisualStyleBackColor = true;
        rbApplyModel.Visible = false;
        rbApplyModel.CheckedChanged += lbProfile_SelectedIndexChanged;
        // 
        // btnBundleSelect
        // 
        btnBundleSelect.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        btnBundleSelect.Location = new Point(3, 7);
        btnBundleSelect.Name = "btnBundleSelect";
        btnBundleSelect.Size = new Size(112, 34);
        btnBundleSelect.TabIndex = 1;
        btnBundleSelect.Text = "選擇";
        btnBundleSelect.UseVisualStyleBackColor = true;
        btnBundleSelect.Click += btnBundleSelect_Click;
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
        Controls.Add(btnClose);
        Name = "FormIGAnalyzer";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "IG Analyzer";
        WindowState = FormWindowState.Maximized;
        Load += FormIGAnalyzer_Load;
        tabIG.ResumeLayout(false);
        tabConfiguration.ResumeLayout(false);
        splitBase.Panel1.ResumeLayout(false);
        splitBase.Panel1.PerformLayout();
        splitBase.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitBase).EndInit();
        splitBase.ResumeLayout(false);
        splitContainer14.Panel1.ResumeLayout(false);
        splitContainer14.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer14).EndInit();
        splitContainer14.ResumeLayout(false);
        splitBase3.Panel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitBase3).EndInit();
        splitBase3.ResumeLayout(false);
        splitBase4.Panel1.ResumeLayout(false);
        splitBase4.Panel1.PerformLayout();
        splitBase4.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitBase4).EndInit();
        splitBase4.ResumeLayout(false);
        tabProfile.ResumeLayout(false);
        splitContainer8.Panel1.ResumeLayout(false);
        splitContainer8.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer8).EndInit();
        splitContainer8.ResumeLayout(false);
        splitContainer9.Panel1.ResumeLayout(false);
        splitContainer9.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer9).EndInit();
        splitContainer9.ResumeLayout(false);
        splitContainer11.Panel1.ResumeLayout(false);
        splitContainer11.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer11).EndInit();
        splitContainer11.ResumeLayout(false);
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
        tabMaster.ResumeLayout(false);
        splitMaster.Panel1.ResumeLayout(false);
        splitMaster.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitMaster).EndInit();
        splitMaster.ResumeLayout(false);
        splitMaster1.Panel1.ResumeLayout(false);
        splitMaster1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitMaster1).EndInit();
        splitMaster1.ResumeLayout(false);
        splitMaster2.Panel1.ResumeLayout(false);
        splitMaster2.Panel2.ResumeLayout(false);
        splitMaster2.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitMaster2).EndInit();
        splitMaster2.ResumeLayout(false);
        tabStaging.ResumeLayout(false);
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        splitContainer12.Panel1.ResumeLayout(false);
        splitContainer12.Panel2.ResumeLayout(false);
        splitContainer12.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer12).EndInit();
        splitContainer12.ResumeLayout(false);
        splitContainer2.Panel1.ResumeLayout(false);
        splitContainer2.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
        splitContainer2.ResumeLayout(false);
        splitContainer3.Panel1.ResumeLayout(false);
        splitContainer3.Panel2.ResumeLayout(false);
        splitContainer3.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
        splitContainer3.ResumeLayout(false);
        splitStaging4.Panel1.ResumeLayout(false);
        splitStaging4.Panel2.ResumeLayout(false);
        splitStaging4.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitStaging4).EndInit();
        splitStaging4.ResumeLayout(false);
        tabBundle.ResumeLayout(false);
        splitBundle1.Panel1.ResumeLayout(false);
        splitBundle1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitBundle1).EndInit();
        splitBundle1.ResumeLayout(false);
        splitContainer6.Panel1.ResumeLayout(false);
        splitContainer6.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer6).EndInit();
        splitContainer6.ResumeLayout(false);
        splitBundle2.Panel1.ResumeLayout(false);
        splitBundle2.Panel2.ResumeLayout(false);
        splitBundle2.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitBundle2).EndInit();
        splitBundle2.ResumeLayout(false);
        splitBundle3.Panel1.ResumeLayout(false);
        splitBundle3.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitBundle3).EndInit();
        splitBundle3.ResumeLayout(false);
        splitBundle4.Panel1.ResumeLayout(false);
        splitBundle4.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitBundle4).EndInit();
        splitBundle4.ResumeLayout(false);
        tabMsg.ResumeLayout(false);
        tabMsg.PerformLayout();
        tabVersion.ResumeLayout(false);
        tabVersion.PerformLayout();
        splitIGList1.Panel1.ResumeLayout(false);
        splitIGList1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitIGList1).EndInit();
        splitIGList1.ResumeLayout(false);
        splitIGList2.Panel1.ResumeLayout(false);
        splitIGList2.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitIGList2).EndInit();
        splitIGList2.ResumeLayout(false);
        statusBar.ResumeLayout(false);
        statusBar.PerformLayout();
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
    private TabPage tabStaging;
    private SplitContainer splitContainer1;
    private SplitContainer splitContainer2;
    private ListView lvStaging;
    private SplitContainer splitContainer3;
    private SplitContainer splitStaging4;
    private TextBox txtFume;
    private Button btnFHIRData;
    private TextBox txtFHIRData;
    private TabPage tabBundle;
    private SplitContainer splitBundle1;
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
    private SplitContainer splitContainer11;
    private ListView lvProfile;
    private ListView lvConstraint;
    private Button btnConfirm;
    private Button btnFUMECheck;
    private Button btnStagingLoad;
    private ListView lvFUME;
    private TabPage tabVersion;
    private ComboBox cmdIGList;
    private Label label6;
    private SplitContainer splitIGList1;
    private Button btnIGCheck;
    private ListView lvIGNext;
    private SplitContainer splitContainer12;
    private ListBox lbStaging;
    private SplitContainer splitBase;
    private TextBox txtFUMEServer;
    private Label label5;
    private RadioButton rbUser;
    private RadioButton rbAdmin;
    private Label label4;
    private Button btnDataDir;
    private TextBox txtDataDirectory;
    private Label label3;
    private ComboBox cmbIG;
    private Label label2;
    private TextBox txtFHIRServer;
    private Label lblFHIRServer;
    private SplitContainer splitContainer14;
    private ListBox lbBase;
    private SplitContainer splitBase3;
    private SplitContainer splitBase4;
    private TextBox txtBase;
    private ListView lvBase;
    private Button btnUpload;
    private TabPage tabMaster;
    private SplitContainer splitMaster;
    private ListBox lbMaster;
    private SplitContainer splitMaster1;
    private ListView lvMaster;
    private SplitContainer splitMaster2;
    private TextBox txtMasterFHIR;
    private Button btnMasterSelect;
    private SplitContainer splitIGList2;
    private ListBox lbIG;
    private ListView lvIG;
    private Button btnIGCompare;
    private ToolStripStatusLabel lblSatatusBar;
    private ListView lvReference;
    private Button btnExample;
    private Button btnStagingCopy;
    private Button btnApplyLoad;
    private SplitContainer splitBundle2;
    private SplitContainer splitBundle3;
    private ListView lvBundle;
    private TextBox txtBundle;
    private Button btnStagingValidate;
    private TextBox txtStaging;
    internal SplitContainer splitBundle4;
    private ListView lvBundleConstraint;
    private Button btnBundleCreate;
    private Button btnBundleSelect;
}
