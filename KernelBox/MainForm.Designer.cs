namespace KernelBox
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabProcess = new System.Windows.Forms.TabPage();
            this.ListView_ProcessOther = new System.Windows.Forms.ListView();
            this.ListView_Process = new System.Windows.Forms.ListView();
            this.colIcon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPid = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPPid = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colProcName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFullPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colEpe = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActive = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip_procList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshProcListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.viewModulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewThreadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewHandlesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.operationProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.suspendProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resumeProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.terminateProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imgList_icon_proc = new System.Windows.Forms.ImageList(this.components);
            this.tabSSDT = new System.Windows.Forms.TabPage();
            this.ListView_SSDT = new System.Windows.Forms.ListView();
            this.tabExplorer = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.textBox_FilePathOuput = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeViewLeft = new System.Windows.Forms.TreeView();
            this.ResourceTreeView = new System.Windows.Forms.TreeView();
            this.listView_Explorer = new System.Windows.Forms.ListView();
            this.imageListRight1_Explorer = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip_Explorer = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel_Explorer = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabRegistry = new System.Windows.Forms.TabPage();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.listView_Registry = new System.Windows.Forms.ListView();
            this.colValueName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colValueData = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList_Registry = new System.Windows.Forms.ImageList(this.components);
            this.treeView_Registry = new System.Windows.Forms.TreeView();
            this.statusStrip_Registry = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel_Registry = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.labelProductName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelCompanyName = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.imgList_icon_module = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip_procModulesList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshModulesListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip_procThreadList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshThreadsListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.terminateThreadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.suspendThreadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resumeThreadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.treeViewImageList_Explorer = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip_Registry = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.modifyPopupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.popupMenuSeperatorModify = new System.Windows.Forms.ToolStripSeparator();
            this.newPopupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyPopupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.stringValuePopupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.binaryValuePopupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dWORDValuePopupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qWORDValuePopupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiStringValuePopupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandableStringPopupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.popupMenuSeparatorNew = new System.Windows.Forms.ToolStripSeparator();
            this.expandPopupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshPopupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.popupMenuSeperatorRefresh = new System.Windows.Forms.ToolStripSeparator();
            this.deletePopupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.popupMenuSeperatorCopyKeyName = new System.Windows.Forms.ToolStripSeparator();
            this.popupMenuSeperatorExport = new System.Windows.Forms.ToolStripSeparator();
            this.tabControl1.SuspendLayout();
            this.tabProcess.SuspendLayout();
            this.contextMenuStrip_procList.SuspendLayout();
            this.tabSSDT.SuspendLayout();
            this.tabExplorer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip_Explorer.SuspendLayout();
            this.tabRegistry.SuspendLayout();
            this.statusStrip_Registry.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.contextMenuStrip_procModulesList.SuspendLayout();
            this.contextMenuStrip_procThreadList.SuspendLayout();
            this.contextMenuStrip_Registry.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabProcess);
            this.tabControl1.Controls.Add(this.tabSSDT);
            this.tabControl1.Controls.Add(this.tabExplorer);
            this.tabControl1.Controls.Add(this.tabRegistry);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1206, 721);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // tabProcess
            // 
            this.tabProcess.Controls.Add(this.ListView_ProcessOther);
            this.tabProcess.Controls.Add(this.ListView_Process);
            this.tabProcess.Location = new System.Drawing.Point(4, 28);
            this.tabProcess.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tabProcess.Name = "tabProcess";
            this.tabProcess.Padding = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tabProcess.Size = new System.Drawing.Size(1198, 689);
            this.tabProcess.TabIndex = 0;
            this.tabProcess.Text = "Process";
            this.tabProcess.UseVisualStyleBackColor = true;
            // 
            // ListView_ProcessOther
            // 
            this.ListView_ProcessOther.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListView_ProcessOther.BackColor = System.Drawing.SystemColors.Window;
            this.ListView_ProcessOther.FullRowSelect = true;
            this.ListView_ProcessOther.GridLines = true;
            this.ListView_ProcessOther.Location = new System.Drawing.Point(3, 457);
            this.ListView_ProcessOther.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.ListView_ProcessOther.Name = "ListView_ProcessOther";
            this.ListView_ProcessOther.Size = new System.Drawing.Size(1186, 226);
            this.ListView_ProcessOther.TabIndex = 1;
            this.ListView_ProcessOther.UseCompatibleStateImageBehavior = false;
            this.ListView_ProcessOther.View = System.Windows.Forms.View.Details;
            this.ListView_ProcessOther.Visible = false;
            // 
            // ListView_Process
            // 
            this.ListView_Process.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListView_Process.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIcon,
            this.colPid,
            this.colPPid,
            this.colProcName,
            this.colFullPath,
            this.colEpe,
            this.colActive});
            this.ListView_Process.ContextMenuStrip = this.contextMenuStrip_procList;
            this.ListView_Process.FullRowSelect = true;
            this.ListView_Process.GridLines = true;
            this.ListView_Process.Location = new System.Drawing.Point(3, 5);
            this.ListView_Process.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.ListView_Process.MultiSelect = false;
            this.ListView_Process.Name = "ListView_Process";
            this.ListView_Process.Size = new System.Drawing.Size(1186, 677);
            this.ListView_Process.SmallImageList = this.imgList_icon_proc;
            this.ListView_Process.TabIndex = 0;
            this.ListView_Process.UseCompatibleStateImageBehavior = false;
            this.ListView_Process.View = System.Windows.Forms.View.Details;
            // 
            // colIcon
            // 
            this.colIcon.Text = "ICON";
            this.colIcon.Width = 25;
            // 
            // colPid
            // 
            this.colPid.Text = "Pid";
            this.colPid.Width = 50;
            // 
            // colPPid
            // 
            this.colPPid.Text = "PPid";
            this.colPPid.Width = 50;
            // 
            // colProcName
            // 
            this.colProcName.Text = "ProcName";
            this.colProcName.Width = 150;
            // 
            // colFullPath
            // 
            this.colFullPath.Text = "FullPath";
            this.colFullPath.Width = 302;
            // 
            // colEpe
            // 
            this.colEpe.Text = "Eprocess";
            this.colEpe.Width = 130;
            // 
            // colActive
            // 
            this.colActive.Text = "Active";
            this.colActive.Width = 77;
            // 
            // contextMenuStrip_procList
            // 
            this.contextMenuStrip_procList.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip_procList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshProcListToolStripMenuItem,
            this.toolStripSeparator1,
            this.viewModulesToolStripMenuItem,
            this.viewThreadsToolStripMenuItem,
            this.viewHandlesToolStripMenuItem,
            this.toolStripSeparator2,
            this.operationProcessToolStripMenuItem});
            this.contextMenuStrip_procList.Name = "menuStrip_procList";
            this.contextMenuStrip_procList.Size = new System.Drawing.Size(251, 166);
            // 
            // refreshProcListToolStripMenuItem
            // 
            this.refreshProcListToolStripMenuItem.Name = "refreshProcListToolStripMenuItem";
            this.refreshProcListToolStripMenuItem.Size = new System.Drawing.Size(250, 30);
            this.refreshProcListToolStripMenuItem.Text = "Refresh ProcList";
            this.refreshProcListToolStripMenuItem.Click += new System.EventHandler(this.refresh_process);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(247, 6);
            // 
            // viewModulesToolStripMenuItem
            // 
            this.viewModulesToolStripMenuItem.Name = "viewModulesToolStripMenuItem";
            this.viewModulesToolStripMenuItem.Size = new System.Drawing.Size(250, 30);
            this.viewModulesToolStripMenuItem.Text = "View Modules";
            this.viewModulesToolStripMenuItem.Click += new System.EventHandler(this.view_modules);
            // 
            // viewThreadsToolStripMenuItem
            // 
            this.viewThreadsToolStripMenuItem.Name = "viewThreadsToolStripMenuItem";
            this.viewThreadsToolStripMenuItem.Size = new System.Drawing.Size(250, 30);
            this.viewThreadsToolStripMenuItem.Text = "View Threads";
            this.viewThreadsToolStripMenuItem.Click += new System.EventHandler(this.view_threads);
            // 
            // viewHandlesToolStripMenuItem
            // 
            this.viewHandlesToolStripMenuItem.Name = "viewHandlesToolStripMenuItem";
            this.viewHandlesToolStripMenuItem.Size = new System.Drawing.Size(250, 30);
            this.viewHandlesToolStripMenuItem.Text = "View Handles";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(247, 6);
            // 
            // operationProcessToolStripMenuItem
            // 
            this.operationProcessToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.suspendProcessToolStripMenuItem,
            this.resumeProcessToolStripMenuItem,
            this.terminateProcessToolStripMenuItem});
            this.operationProcessToolStripMenuItem.Name = "operationProcessToolStripMenuItem";
            this.operationProcessToolStripMenuItem.Size = new System.Drawing.Size(250, 30);
            this.operationProcessToolStripMenuItem.Text = "Operation Process";
            // 
            // suspendProcessToolStripMenuItem
            // 
            this.suspendProcessToolStripMenuItem.Name = "suspendProcessToolStripMenuItem";
            this.suspendProcessToolStripMenuItem.Size = new System.Drawing.Size(249, 30);
            this.suspendProcessToolStripMenuItem.Text = "Suspend Process";
            this.suspendProcessToolStripMenuItem.Click += new System.EventHandler(this.suspendProcessToolStripMenuItem_Click);
            // 
            // resumeProcessToolStripMenuItem
            // 
            this.resumeProcessToolStripMenuItem.Name = "resumeProcessToolStripMenuItem";
            this.resumeProcessToolStripMenuItem.Size = new System.Drawing.Size(249, 30);
            this.resumeProcessToolStripMenuItem.Text = "Resume Process";
            this.resumeProcessToolStripMenuItem.Click += new System.EventHandler(this.resumeProcessToolStripMenuItem_Click);
            // 
            // terminateProcessToolStripMenuItem
            // 
            this.terminateProcessToolStripMenuItem.Name = "terminateProcessToolStripMenuItem";
            this.terminateProcessToolStripMenuItem.Size = new System.Drawing.Size(249, 30);
            this.terminateProcessToolStripMenuItem.Text = "Terminate Process";
            this.terminateProcessToolStripMenuItem.Click += new System.EventHandler(this.terminateProcessToolStripMenuItem_Click);
            // 
            // imgList_icon_proc
            // 
            this.imgList_icon_proc.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imgList_icon_proc.ImageSize = new System.Drawing.Size(16, 16);
            this.imgList_icon_proc.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // tabSSDT
            // 
            this.tabSSDT.Controls.Add(this.ListView_SSDT);
            this.tabSSDT.Location = new System.Drawing.Point(4, 28);
            this.tabSSDT.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tabSSDT.Name = "tabSSDT";
            this.tabSSDT.Padding = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tabSSDT.Size = new System.Drawing.Size(1198, 689);
            this.tabSSDT.TabIndex = 1;
            this.tabSSDT.Text = "SSDT";
            this.tabSSDT.UseVisualStyleBackColor = true;
            // 
            // ListView_SSDT
            // 
            this.ListView_SSDT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListView_SSDT.FullRowSelect = true;
            this.ListView_SSDT.GridLines = true;
            this.ListView_SSDT.Location = new System.Drawing.Point(3, 5);
            this.ListView_SSDT.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.ListView_SSDT.MultiSelect = false;
            this.ListView_SSDT.Name = "ListView_SSDT";
            this.ListView_SSDT.Size = new System.Drawing.Size(1192, 679);
            this.ListView_SSDT.TabIndex = 0;
            this.ListView_SSDT.UseCompatibleStateImageBehavior = false;
            this.ListView_SSDT.View = System.Windows.Forms.View.Details;
            // 
            // tabExplorer
            // 
            this.tabExplorer.Controls.Add(this.splitContainer3);
            this.tabExplorer.Location = new System.Drawing.Point(4, 28);
            this.tabExplorer.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tabExplorer.Name = "tabExplorer";
            this.tabExplorer.Size = new System.Drawing.Size(1198, 689);
            this.tabExplorer.TabIndex = 2;
            this.tabExplorer.Text = "Explorer";
            this.tabExplorer.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.statusStrip_Explorer);
            this.splitContainer3.Size = new System.Drawing.Size(1198, 689);
            this.splitContainer3.SplitterDistance = 663;
            this.splitContainer3.SplitterWidth = 1;
            this.splitContainer3.TabIndex = 6;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.textBox_FilePathOuput);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(1198, 663);
            this.splitContainer2.SplitterDistance = 25;
            this.splitContainer2.SplitterWidth = 1;
            this.splitContainer2.TabIndex = 3;
            // 
            // textBox_FilePathOuput
            // 
            this.textBox_FilePathOuput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_FilePathOuput.Location = new System.Drawing.Point(0, 2);
            this.textBox_FilePathOuput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_FilePathOuput.Name = "textBox_FilePathOuput";
            this.textBox_FilePathOuput.ReadOnly = true;
            this.textBox_FilePathOuput.Size = new System.Drawing.Size(1196, 28);
            this.textBox_FilePathOuput.TabIndex = 98;
            this.textBox_FilePathOuput.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewLeft);
            this.splitContainer1.Panel1.Controls.Add(this.ResourceTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listView_Explorer);
            this.splitContainer1.Size = new System.Drawing.Size(1198, 637);
            this.splitContainer1.SplitterDistance = 232;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeViewLeft
            // 
            this.treeViewLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewLeft.Location = new System.Drawing.Point(0, 0);
            this.treeViewLeft.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.treeViewLeft.Name = "treeViewLeft";
            this.treeViewLeft.Size = new System.Drawing.Size(232, 637);
            this.treeViewLeft.TabIndex = 1;
            this.treeViewLeft.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewLeft_BeforeExpand);
            this.treeViewLeft.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewLeft_AfterSelect);
            // 
            // ResourceTreeView
            // 
            this.ResourceTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResourceTreeView.Location = new System.Drawing.Point(0, 0);
            this.ResourceTreeView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ResourceTreeView.Name = "ResourceTreeView";
            this.ResourceTreeView.Size = new System.Drawing.Size(232, 637);
            this.ResourceTreeView.TabIndex = 0;
            // 
            // listView_Explorer
            // 
            this.listView_Explorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_Explorer.HideSelection = false;
            this.listView_Explorer.Location = new System.Drawing.Point(0, 0);
            this.listView_Explorer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listView_Explorer.Name = "listView_Explorer";
            this.listView_Explorer.Size = new System.Drawing.Size(963, 637);
            this.listView_Explorer.SmallImageList = this.imageListRight1_Explorer;
            this.listView_Explorer.TabIndex = 99;
            this.listView_Explorer.UseCompatibleStateImageBehavior = false;
            this.listView_Explorer.View = System.Windows.Forms.View.Details;
            this.listView_Explorer.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // imageListRight1_Explorer
            // 
            this.imageListRight1_Explorer.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageListRight1_Explorer.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListRight1_Explorer.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // statusStrip_Explorer
            // 
            this.statusStrip_Explorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusStrip_Explorer.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip_Explorer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_Explorer});
            this.statusStrip_Explorer.Location = new System.Drawing.Point(0, 0);
            this.statusStrip_Explorer.Name = "statusStrip_Explorer";
            this.statusStrip_Explorer.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip_Explorer.Size = new System.Drawing.Size(1198, 25);
            this.statusStrip_Explorer.TabIndex = 4;
            this.statusStrip_Explorer.Text = "statusStrip1";
            // 
            // toolStripStatusLabel_Explorer
            // 
            this.toolStripStatusLabel_Explorer.Name = "toolStripStatusLabel_Explorer";
            this.toolStripStatusLabel_Explorer.Size = new System.Drawing.Size(64, 20);
            this.toolStripStatusLabel_Explorer.Text = "Ready";
            // 
            // tabRegistry
            // 
            this.tabRegistry.Controls.Add(this.splitter1);
            this.tabRegistry.Controls.Add(this.listView_Registry);
            this.tabRegistry.Controls.Add(this.treeView_Registry);
            this.tabRegistry.Controls.Add(this.statusStrip_Registry);
            this.tabRegistry.Location = new System.Drawing.Point(4, 28);
            this.tabRegistry.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabRegistry.Name = "tabRegistry";
            this.tabRegistry.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabRegistry.Size = new System.Drawing.Size(1198, 689);
            this.tabRegistry.TabIndex = 3;
            this.tabRegistry.Text = "Registry";
            this.tabRegistry.UseVisualStyleBackColor = true;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(294, 4);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 652);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // listView_Registry
            // 
            this.listView_Registry.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colValueName,
            this.colType,
            this.colValueData});
            this.listView_Registry.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_Registry.Location = new System.Drawing.Point(294, 4);
            this.listView_Registry.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listView_Registry.Name = "listView_Registry";
            this.listView_Registry.Size = new System.Drawing.Size(901, 652);
            this.listView_Registry.SmallImageList = this.imageList_Registry;
            this.listView_Registry.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView_Registry.TabIndex = 0;
            this.listView_Registry.UseCompatibleStateImageBehavior = false;
            this.listView_Registry.View = System.Windows.Forms.View.Details;
            this.listView_Registry.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listView_Registry_AfterLabelEdit);
            this.listView_Registry.DoubleClick += new System.EventHandler(this.listView_Registry_DoubleClick);
            this.listView_Registry.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listView_Registry_MouseUp);
            // 
            // colValueName
            // 
            this.colValueName.Text = "Name";
            this.colValueName.Width = 120;
            // 
            // colType
            // 
            this.colType.Text = "Type";
            this.colType.Width = 120;
            // 
            // colValueData
            // 
            this.colValueData.Text = "Data";
            this.colValueData.Width = 120;
            // 
            // imageList_Registry
            // 
            this.imageList_Registry.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_Registry.ImageStream")));
            this.imageList_Registry.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_Registry.Images.SetKeyName(0, "ascii");
            this.imageList_Registry.Images.SetKeyName(1, "binary");
            this.imageList_Registry.Images.SetKeyName(2, "fold_close");
            this.imageList_Registry.Images.SetKeyName(3, "fold_open");
            // 
            // treeView_Registry
            // 
            this.treeView_Registry.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView_Registry.HideSelection = false;
            this.treeView_Registry.ImageKey = "fold_close";
            this.treeView_Registry.ImageList = this.imageList_Registry;
            this.treeView_Registry.Location = new System.Drawing.Point(3, 4);
            this.treeView_Registry.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeView_Registry.Name = "treeView_Registry";
            this.treeView_Registry.SelectedImageKey = "fold_open";
            this.treeView_Registry.Size = new System.Drawing.Size(291, 652);
            this.treeView_Registry.TabIndex = 0;
            this.treeView_Registry.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_Registry_AfterLabelEdit);
            this.treeView_Registry.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_Registry_BeforeExpand);
            this.treeView_Registry.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Registry_AfterSelect);
            this.treeView_Registry.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView_Registry_MouseUp);
            // 
            // statusStrip_Registry
            // 
            this.statusStrip_Registry.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip_Registry.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_Registry});
            this.statusStrip_Registry.Location = new System.Drawing.Point(3, 656);
            this.statusStrip_Registry.Name = "statusStrip_Registry";
            this.statusStrip_Registry.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip_Registry.Size = new System.Drawing.Size(1192, 29);
            this.statusStrip_Registry.TabIndex = 1;
            this.statusStrip_Registry.Text = "statusStrip1";
            // 
            // toolStripStatusLabel_Registry
            // 
            this.toolStripStatusLabel_Registry.Name = "toolStripStatusLabel_Registry";
            this.toolStripStatusLabel_Registry.Size = new System.Drawing.Size(64, 24);
            this.toolStripStatusLabel_Registry.Text = "Ready";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1198, 689);
            this.tabPage1.TabIndex = 4;
            this.tabPage1.Text = "About";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.2651F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.7349F));
            this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelProductName, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelVersion, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.labelCopyright, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.labelCompanyName, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.textBoxDescription, 1, 4);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 58.41874F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1.756955F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1192, 683);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.Location = new System.Drawing.Point(4, 4);
            this.logoPictureBox.Margin = new System.Windows.Forms.Padding(4);
            this.logoPictureBox.Name = "logoPictureBox";
            this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 6);
            this.logoPictureBox.Size = new System.Drawing.Size(316, 675);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logoPictureBox.TabIndex = 12;
            this.logoPictureBox.TabStop = false;
            // 
            // labelProductName
            // 
            this.labelProductName.AutoEllipsis = true;
            this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelProductName.Font = new System.Drawing.Font("微软雅黑", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelProductName.Location = new System.Drawing.Point(333, 0);
            this.labelProductName.Margin = new System.Windows.Forms.Padding(9, 0, 4, 0);
            this.labelProductName.MaximumSize = new System.Drawing.Size(0, 45);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(855, 45);
            this.labelProductName.TabIndex = 19;
            this.labelProductName.Text = "Product：Kernel Box";
            this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelVersion.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelVersion.Location = new System.Drawing.Point(333, 68);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(9, 0, 4, 0);
            this.labelVersion.MaximumSize = new System.Drawing.Size(0, 45);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(855, 45);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "Version：1.0";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCopyright
            // 
            this.labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCopyright.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCopyright.Location = new System.Drawing.Point(333, 136);
            this.labelCopyright.Margin = new System.Windows.Forms.Padding(9, 0, 4, 0);
            this.labelCopyright.MaximumSize = new System.Drawing.Size(0, 45);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(855, 45);
            this.labelCopyright.TabIndex = 21;
            this.labelCopyright.Text = "Source address：";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCompanyName
            // 
            this.labelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCompanyName.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCompanyName.Location = new System.Drawing.Point(333, 204);
            this.labelCompanyName.Margin = new System.Windows.Forms.Padding(9, 0, 4, 0);
            this.labelCompanyName.MaximumSize = new System.Drawing.Size(0, 45);
            this.labelCompanyName.Name = "labelCompanyName";
            this.labelCompanyName.Size = new System.Drawing.Size(855, 45);
            this.labelCompanyName.TabIndex = 22;
            this.labelCompanyName.Text = "Author：JerryAJ";
            this.labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxDescription.Location = new System.Drawing.Point(333, 276);
            this.textBoxDescription.Margin = new System.Windows.Forms.Padding(9, 4, 4, 4);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDescription.Size = new System.Drawing.Size(855, 390);
            this.textBoxDescription.TabIndex = 23;
            this.textBoxDescription.TabStop = false;
            this.textBoxDescription.Text = "Here, I need to thank these people and the site to bring me help :\r\n\r\n+Tesla.Ange" +
    "la and his www.m5home.com\r\n+mengwuji and his www.mengwuji.net/forum.php\r\n+www.cr" +
    "acksoft.net\r\n\r\n\r\n";
            // 
            // imgList_icon_module
            // 
            this.imgList_icon_module.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imgList_icon_module.ImageSize = new System.Drawing.Size(16, 16);
            this.imgList_icon_module.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // contextMenuStrip_procModulesList
            // 
            this.contextMenuStrip_procModulesList.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_procModulesList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshModulesListToolStripMenuItem,
            this.uninstallModuleToolStripMenuItem});
            this.contextMenuStrip_procModulesList.Name = "contextMenuStrip_procModulesList";
            this.contextMenuStrip_procModulesList.Size = new System.Drawing.Size(280, 64);
            // 
            // refreshModulesListToolStripMenuItem
            // 
            this.refreshModulesListToolStripMenuItem.Name = "refreshModulesListToolStripMenuItem";
            this.refreshModulesListToolStripMenuItem.Size = new System.Drawing.Size(279, 30);
            this.refreshModulesListToolStripMenuItem.Text = "Refresh ModulesList";
            this.refreshModulesListToolStripMenuItem.Click += new System.EventHandler(this.refreshModulesListToolStripMenuItem_Click);
            // 
            // uninstallModuleToolStripMenuItem
            // 
            this.uninstallModuleToolStripMenuItem.Name = "uninstallModuleToolStripMenuItem";
            this.uninstallModuleToolStripMenuItem.Size = new System.Drawing.Size(279, 30);
            this.uninstallModuleToolStripMenuItem.Text = "Uninstall This Module";
            this.uninstallModuleToolStripMenuItem.Click += new System.EventHandler(this.uninstallModuleToolStripMenuItem_Click);
            // 
            // contextMenuStrip_procThreadList
            // 
            this.contextMenuStrip_procThreadList.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_procThreadList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshThreadsListToolStripMenuItem,
            this.terminateThreadToolStripMenuItem,
            this.suspendThreadToolStripMenuItem,
            this.resumeThreadToolStripMenuItem});
            this.contextMenuStrip_procThreadList.Name = "contextMenuStrip_procThreadList";
            this.contextMenuStrip_procThreadList.Size = new System.Drawing.Size(265, 124);
            // 
            // refreshThreadsListToolStripMenuItem
            // 
            this.refreshThreadsListToolStripMenuItem.Name = "refreshThreadsListToolStripMenuItem";
            this.refreshThreadsListToolStripMenuItem.Size = new System.Drawing.Size(264, 30);
            this.refreshThreadsListToolStripMenuItem.Text = "Refresh Threads List";
            this.refreshThreadsListToolStripMenuItem.Click += new System.EventHandler(this.refreshThreadsListToolStripMenuItem_Click);
            // 
            // terminateThreadToolStripMenuItem
            // 
            this.terminateThreadToolStripMenuItem.Name = "terminateThreadToolStripMenuItem";
            this.terminateThreadToolStripMenuItem.Size = new System.Drawing.Size(264, 30);
            this.terminateThreadToolStripMenuItem.Text = "Terminate Thread";
            this.terminateThreadToolStripMenuItem.Click += new System.EventHandler(this.terminateThreadToolStripMenuItem_Click);
            // 
            // suspendThreadToolStripMenuItem
            // 
            this.suspendThreadToolStripMenuItem.Name = "suspendThreadToolStripMenuItem";
            this.suspendThreadToolStripMenuItem.Size = new System.Drawing.Size(264, 30);
            this.suspendThreadToolStripMenuItem.Text = "Suspend Thread";
            this.suspendThreadToolStripMenuItem.Click += new System.EventHandler(this.suspendThreadToolStripMenuItem_Click);
            // 
            // resumeThreadToolStripMenuItem
            // 
            this.resumeThreadToolStripMenuItem.Name = "resumeThreadToolStripMenuItem";
            this.resumeThreadToolStripMenuItem.Size = new System.Drawing.Size(264, 30);
            this.resumeThreadToolStripMenuItem.Text = "Resume Thread";
            this.resumeThreadToolStripMenuItem.Click += new System.EventHandler(this.resumeThreadToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(74, 4);
            // 
            // treeViewImageList_Explorer
            // 
            this.treeViewImageList_Explorer.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.treeViewImageList_Explorer.ImageSize = new System.Drawing.Size(16, 16);
            this.treeViewImageList_Explorer.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // contextMenuStrip_Registry
            // 
            this.contextMenuStrip_Registry.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_Registry.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modifyPopupMenuItem,
            this.popupMenuSeperatorModify,
            this.newPopupMenuItem,
            this.popupMenuSeparatorNew,
            this.expandPopupMenuItem,
            this.refreshPopupMenuItem,
            this.popupMenuSeperatorRefresh,
            this.deletePopupMenuItem,
            this.popupMenuSeperatorCopyKeyName});
            this.contextMenuStrip_Registry.Name = "contextMenuStrip1";
            this.contextMenuStrip_Registry.Size = new System.Drawing.Size(158, 178);
            // 
            // modifyPopupMenuItem
            // 
            this.modifyPopupMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.modifyPopupMenuItem.Name = "modifyPopupMenuItem";
            this.modifyPopupMenuItem.Size = new System.Drawing.Size(157, 30);
            this.modifyPopupMenuItem.Text = "&Modify";
            this.modifyPopupMenuItem.Click += new System.EventHandler(this.modifyPopupMenuItem_Click);
            // 
            // popupMenuSeperatorModify
            // 
            this.popupMenuSeperatorModify.Name = "popupMenuSeperatorModify";
            this.popupMenuSeperatorModify.Size = new System.Drawing.Size(154, 6);
            // 
            // newPopupMenuItem
            // 
            this.newPopupMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keyPopupMenuItem,
            this.toolStripSeparator3,
            this.stringValuePopupMenuItem,
            this.binaryValuePopupMenuItem,
            this.dWORDValuePopupMenuItem,
            this.qWORDValuePopupMenuItem,
            this.multiStringValuePopupMenuItem,
            this.expandableStringPopupMenuItem});
            this.newPopupMenuItem.Name = "newPopupMenuItem";
            this.newPopupMenuItem.Size = new System.Drawing.Size(157, 30);
            this.newPopupMenuItem.Text = "&New";
            // 
            // keyPopupMenuItem
            // 
            this.keyPopupMenuItem.Name = "keyPopupMenuItem";
            this.keyPopupMenuItem.Size = new System.Drawing.Size(304, 30);
            this.keyPopupMenuItem.Text = "&Key";
            this.keyPopupMenuItem.Click += new System.EventHandler(this.keyPopupMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(301, 6);
            // 
            // stringValuePopupMenuItem
            // 
            this.stringValuePopupMenuItem.Name = "stringValuePopupMenuItem";
            this.stringValuePopupMenuItem.Size = new System.Drawing.Size(304, 30);
            this.stringValuePopupMenuItem.Text = "&String Value";
            this.stringValuePopupMenuItem.Click += new System.EventHandler(this.stringValuePopupMenuItem_Click);
            // 
            // binaryValuePopupMenuItem
            // 
            this.binaryValuePopupMenuItem.Name = "binaryValuePopupMenuItem";
            this.binaryValuePopupMenuItem.Size = new System.Drawing.Size(304, 30);
            this.binaryValuePopupMenuItem.Text = "&Binary Value";
            this.binaryValuePopupMenuItem.Click += new System.EventHandler(this.binaryValuePopupMenuItem_Click);
            // 
            // dWORDValuePopupMenuItem
            // 
            this.dWORDValuePopupMenuItem.Name = "dWORDValuePopupMenuItem";
            this.dWORDValuePopupMenuItem.Size = new System.Drawing.Size(304, 30);
            this.dWORDValuePopupMenuItem.Text = "&DWORD Value";
            this.dWORDValuePopupMenuItem.Click += new System.EventHandler(this.dWORDValuePopupMenuItem_Click);
            // 
            // qWORDValuePopupMenuItem
            // 
            this.qWORDValuePopupMenuItem.Name = "qWORDValuePopupMenuItem";
            this.qWORDValuePopupMenuItem.Size = new System.Drawing.Size(304, 30);
            this.qWORDValuePopupMenuItem.Text = "&QWORD Value";
            this.qWORDValuePopupMenuItem.Click += new System.EventHandler(this.qWORDValuePopupMenuItem_Click);
            // 
            // multiStringValuePopupMenuItem
            // 
            this.multiStringValuePopupMenuItem.Name = "multiStringValuePopupMenuItem";
            this.multiStringValuePopupMenuItem.Size = new System.Drawing.Size(304, 30);
            this.multiStringValuePopupMenuItem.Text = "&Multi-String Value";
            this.multiStringValuePopupMenuItem.Click += new System.EventHandler(this.multiStringValuePopupMenuItem_Click);
            // 
            // expandableStringPopupMenuItem
            // 
            this.expandableStringPopupMenuItem.Name = "expandableStringPopupMenuItem";
            this.expandableStringPopupMenuItem.Size = new System.Drawing.Size(304, 30);
            this.expandableStringPopupMenuItem.Text = "&Expandable String Value";
            this.expandableStringPopupMenuItem.Click += new System.EventHandler(this.expandableStringPopupMenuItem_Click);
            // 
            // popupMenuSeparatorNew
            // 
            this.popupMenuSeparatorNew.Name = "popupMenuSeparatorNew";
            this.popupMenuSeparatorNew.Size = new System.Drawing.Size(154, 6);
            // 
            // expandPopupMenuItem
            // 
            this.expandPopupMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.expandPopupMenuItem.Name = "expandPopupMenuItem";
            this.expandPopupMenuItem.Size = new System.Drawing.Size(157, 30);
            this.expandPopupMenuItem.Text = "Expand";
            this.expandPopupMenuItem.Click += new System.EventHandler(this.expandPopupMenuItem_Click);
            // 
            // refreshPopupMenuItem
            // 
            this.refreshPopupMenuItem.Name = "refreshPopupMenuItem";
            this.refreshPopupMenuItem.Size = new System.Drawing.Size(157, 30);
            this.refreshPopupMenuItem.Text = "Refresh";
            this.refreshPopupMenuItem.Click += new System.EventHandler(this.refreshPopupMenuItem_Click);
            // 
            // popupMenuSeperatorRefresh
            // 
            this.popupMenuSeperatorRefresh.Name = "popupMenuSeperatorRefresh";
            this.popupMenuSeperatorRefresh.Size = new System.Drawing.Size(154, 6);
            // 
            // deletePopupMenuItem
            // 
            this.deletePopupMenuItem.Name = "deletePopupMenuItem";
            this.deletePopupMenuItem.Size = new System.Drawing.Size(157, 30);
            this.deletePopupMenuItem.Text = "&Delete";
            this.deletePopupMenuItem.Click += new System.EventHandler(this.deletePopupMenuItem_Click);
            // 
            // popupMenuSeperatorCopyKeyName
            // 
            this.popupMenuSeperatorCopyKeyName.Name = "popupMenuSeperatorCopyKeyName";
            this.popupMenuSeperatorCopyKeyName.Size = new System.Drawing.Size(154, 6);
            // 
            // popupMenuSeperatorExport
            // 
            this.popupMenuSeperatorExport.Name = "popupMenuSeperatorExport";
            this.popupMenuSeperatorExport.Size = new System.Drawing.Size(228, 6);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1206, 721);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "MainForm";
            this.Text = "Kernel Box v1.0";
            this.tabControl1.ResumeLayout(false);
            this.tabProcess.ResumeLayout(false);
            this.contextMenuStrip_procList.ResumeLayout(false);
            this.tabSSDT.ResumeLayout(false);
            this.tabExplorer.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip_Explorer.ResumeLayout(false);
            this.statusStrip_Explorer.PerformLayout();
            this.tabRegistry.ResumeLayout(false);
            this.tabRegistry.PerformLayout();
            this.statusStrip_Registry.ResumeLayout(false);
            this.statusStrip_Registry.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.contextMenuStrip_procModulesList.ResumeLayout(false);
            this.contextMenuStrip_procThreadList.ResumeLayout(false);
            this.contextMenuStrip_Registry.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabPage tabProcess;
        private System.Windows.Forms.TabPage tabSSDT;
        private System.Windows.Forms.TabPage tabExplorer;
        private System.Windows.Forms.ColumnHeader colIcon;
        private System.Windows.Forms.ColumnHeader colPid;
        private System.Windows.Forms.ColumnHeader colPPid;
        private System.Windows.Forms.ColumnHeader colProcName;
        private System.Windows.Forms.ColumnHeader colFullPath;
        private System.Windows.Forms.ColumnHeader colEpe;
        private System.Windows.Forms.ToolStripMenuItem refreshProcListToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem viewModulesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewThreadsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewHandlesToolStripMenuItem;
        public System.Windows.Forms.TabControl tabControl1;
        public System.Windows.Forms.ListView ListView_Process;
        public System.Windows.Forms.ListView ListView_ProcessOther;
        public System.Windows.Forms.ImageList imgList_icon_proc;
        public System.Windows.Forms.ContextMenuStrip contextMenuStrip_procList;
        public System.Windows.Forms.ImageList imgList_icon_module;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem operationProcessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem suspendProcessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resumeProcessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem terminateProcessToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader colActive;
        public System.Windows.Forms.ContextMenuStrip contextMenuStrip_procModulesList;
        private System.Windows.Forms.ToolStripMenuItem refreshModulesListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uninstallModuleToolStripMenuItem;
        public System.Windows.Forms.ContextMenuStrip contextMenuStrip_procThreadList;
        private System.Windows.Forms.ToolStripMenuItem terminateThreadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem suspendThreadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resumeThreadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshThreadsListToolStripMenuItem;
        public System.Windows.Forms.ListView ListView_SSDT;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer2;
        public System.Windows.Forms.TextBox textBox_FilePathOuput;
        private System.Windows.Forms.SplitContainer splitContainer1;
        public System.Windows.Forms.TreeView treeViewLeft;
        private System.Windows.Forms.TreeView ResourceTreeView;
        public System.Windows.Forms.ListView listView_Explorer;
        private System.Windows.Forms.StatusStrip statusStrip_Explorer;
        public System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_Explorer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        public System.Windows.Forms.ImageList treeViewImageList_Explorer;
        public System.Windows.Forms.ImageList imageListRight1_Explorer;
        private System.Windows.Forms.TabPage tabRegistry;
        public System.Windows.Forms.TreeView treeView_Registry;
        private System.Windows.Forms.StatusStrip statusStrip_Registry;
        public System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_Registry;
        private System.Windows.Forms.ImageList imageList_Registry;
        private System.Windows.Forms.ToolStripMenuItem keyPopupMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem stringValuePopupMenuItem;
        private System.Windows.Forms.ToolStripMenuItem binaryValuePopupMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dWORDValuePopupMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qWORDValuePopupMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multiStringValuePopupMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandableStringPopupMenuItem;
        public System.Windows.Forms.ContextMenuStrip contextMenuStrip_Registry;
        public System.Windows.Forms.ToolStripMenuItem modifyPopupMenuItem;
        public System.Windows.Forms.ToolStripSeparator popupMenuSeperatorModify;
        public System.Windows.Forms.ToolStripMenuItem newPopupMenuItem;
        public System.Windows.Forms.ToolStripSeparator popupMenuSeparatorNew;
        public System.Windows.Forms.ToolStripMenuItem expandPopupMenuItem;
        public System.Windows.Forms.ToolStripMenuItem refreshPopupMenuItem;
        public System.Windows.Forms.ToolStripSeparator popupMenuSeperatorRefresh;
        public System.Windows.Forms.ToolStripMenuItem deletePopupMenuItem;
        public System.Windows.Forms.ListView listView_Registry;
        private System.Windows.Forms.ColumnHeader colValueName;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colValueData;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Splitter splitter1;
        public System.Windows.Forms.ToolStripSeparator popupMenuSeperatorCopyKeyName;
        public System.Windows.Forms.ToolStripSeparator popupMenuSeperatorExport;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.Label labelCompanyName;
        private System.Windows.Forms.TextBox textBoxDescription;
    }
}

