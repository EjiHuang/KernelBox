using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KernelBox.ProcessMgr;
using KernelBox.Common;
using System.IO;
using System.Threading;
using KernelBox.RegistryEditor;
using Microsoft.Win32;
using KernelBox.Registry;

namespace KernelBox
{
    /// <summary>
    /// MainForm主类
    /// </summary>
    public partial class MainForm : Form
    {
        // 全局MainForm对象
        static public MainForm main;

        // **************初始化类对象************** // 
        static ProcessMgr.cProcessMgr ProcMgr = new ProcessMgr.cProcessMgr(); // 进程管理类对象

        // **************初始化全局变量************** //
        static public bool isProListViewDock = true; // 判断进程列表框是否占满整个tab容器

        // **************MainForm构造函数************** //
        public MainForm()
        {
            // 获取当前的实例对象
            main = this;
            // 初始化
            InitializeComponent();
            // 初始化进程列表
            InitializeProcessList();
        }

        // **************MainForm子函数定义************** //
        /// <summary>
        /// 初始化进程列表
        /// </summary>
        public void InitializeProcessList()
        {
            ProcessMgr.cProcessMgr.InitializeProcessList();
        }

        // 右键菜单->刷新进程事件
        private void refresh_process(object sender, EventArgs e)
        {
            // 如果进程列表框没占满整个tab容器
            if (!isProListViewDock)
            {
                ListView_Process.Height = ListView_Process.Height + ListView_ProcessOther.Height;
                isProListViewDock = true;
                ListView_ProcessOther.Visible = false;
            }
            imgList_icon_proc.Images.Clear();    // 清理一遍，防止刷新进程重复添加
            ProcessMgr.cProcessMgr.InitializeProcessList();
        }

        // 右键菜单->查看进程模块事件
        private void view_modules(object sender, EventArgs e)
        {
            ProcessMgr.cProcessMgr.EnumModules();
        }

        // 右键菜单->查看进程线程事件
        private void view_threads(object sender, EventArgs e)
        {
            ProcessMgr.cProcessMgr.EnumThreads();
        }

        // 右键菜单->操作进程->暂停进程事件
        private void suspendProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessMgr.cProcessMgr.SuspendProcess();
        }

        // 右键菜单->操作进程->恢复进程事件
        private void resumeProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessMgr.cProcessMgr.ResumeProcess();
        }

        // 右键菜单->操作进程->终止进程事件
        private void terminateProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessMgr.cProcessMgr.TerminateProcess();
        }

        // 右键菜单->操作进程模块->刷新进程模块列表事件
        private void refreshModulesListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessMgr.cProcessMgr.RefreshModulesList();
        }

        // 右键菜单->操作进程模块->卸载进程模块事件
        private void uninstallModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessMgr.cProcessMgr.UninstallModule();
        }

        // 右键菜单->操作进程线程->终结指定线程事件
        private void terminateThreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessMgr.cProcessMgr.TerminateThread();
        }

        // 右键菜单->操作进程线程->刷新线程事件
        private void refreshThreadsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessMgr.cProcessMgr.RefreshThreadsList();
        }

        // 右键菜单->操作进程线程->挂起线程事件
        private void suspendThreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessMgr.cProcessMgr.SuspendThread();
        }

        // 右键菜单->操作进程线程->恢复线程事件
        private void resumeThreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessMgr.cProcessMgr.ResumeThread();
        }

        // tab选项被选中后事件
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            // 切换到SSDT初始化事件
            if (e.TabPage.Name.Equals("tabSSDT"))
            {
                // 显示SSDT列表信息
                SSDTMgr.cSSDTMgr.ShowSSDTList();
            }

            // 切换到Explorer初始化事件
            if (e.TabPage.Name.Equals("tabExplorer"))
            {
                // 获取主窗口句柄
                Common.CommonVar.mainHandle = MainForm.main.Handle;
                // 将焦点放到右侧ListView
                MainForm.main.listView_Explorer.Focus();

                if (CommonVar.gIsCFileInited.Equals(false))
                {
                    // 初始化文件类
                    ExplorerManager.cFile.MainInit();
                }
            }

            // 切换到Registry初始化事件
            if (e.TabPage.Name.Equals("tabRegistry"))
            {
                cRegistry.InitialiszeRegistryTree();
            }
        }

        //**************************Explorer区控件事件****************************//
        // 展开选中项
        private void treeViewLeft_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Text.Equals("MyPc"))
            {
                CommonVar.gCurrentNode_File = e.Node;
                ExplorerManager.cFile.ShowDriveRight();
                return;
            }
            // 保存当前选中的节点
            CommonVar.gCurrentNode_File = e.Node;
            // 如果不是双击方式，即选中treeview节点触发的话，需要显示右侧信息
            if (CommonVar.gIsDoubleClickOpen.Equals(false))
            {
                // 展开当前选中的节点，并且右侧显示信息
                ExplorerManager.cFile.ExpandNode(CommonVar.gCurrentNode_File);
            }
            // gIsDoubleClickOpen需要复原
            CommonVar.gIsDoubleClickOpen = false;
        }

        // 展开节点前事件
        private void treeViewLeft_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            this.treeViewLeft.BeginUpdate();

            // 添加文件夹树
            foreach (TreeNode v in e.Node.Nodes)
            {
                ExplorerManager.cFile.AddDirectories(v);
            }
            this.treeViewLeft.EndUpdate();
        }

        // 右侧显示框的双击事件
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            // 当前为双击方式打开
            CommonVar.gIsDoubleClickOpen = true;
            // 获取当前焦点项目的文本，并且将他和根节点转换成完整的路径
            string strFocusItem = this.listView_Explorer.FocusedItem.Text;
            string strV = Path.Combine(CommonVar.gStrFilePath, ExplorerManager.cFile.CorrectFile(strFocusItem));
            try
            {
                // 判断当前是文件还是目录，如果是文件，就直接执行它
                if (new DirectoryInfo(strV).Exists.Equals(false))
                {
                    System.Diagnostics.Process.Start(strV);
                }
                else
                {
                    ExplorerManager.cFile.ExpandFolder(strV);
                    // ResourceManager.cFile.mExpandFolder(strV);
                    foreach (TreeNode v in CommonVar.gCurrentNode_File.Nodes)
                    {
                        // 定位到这个目录下的获取焦点的目录节点
                        if (v.Text.Equals(strFocusItem))
                        {
                            CommonVar.gCurrentNode_File = v;
                            break;
                        }
                    }
                    // TreeView选中这个获取焦点的目录节点，并且展开
                    this.treeViewLeft.SelectedNode = CommonVar.gCurrentNode_File;
                    CommonVar.gCurrentNode_File.Expand();
                    this.treeViewLeft.Focus();
                }
            }
            catch
            {
                return;
            }
        }

        //**************************Registry区控件事件****************************//
        private void treeView_Registry_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // 保存当前选中的节点
            CommonVar.gCurrentNode_Reg = e.Node;
            // 展开当前选中的节点，并且右侧显示信息
            RegistryKey key = e.Node.Tag as RegistryKey;
            cRegistry.LoadValues(key);
        }

        private void treeView_Registry_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // 保存当前选中的节点
            CommonVar.gCurrentNode_Reg = e.Node;
            // 将当前节点作为父节点
            TreeNode parentNode = e.Node;
            // 当父节点第一个节点的对象为空时，我们才获取它的子节点
            if (parentNode.FirstNode.Tag == null)
            {
                // 获取该父节点的子节点
                cRegistry.LoadSubKeys(parentNode);
            }
        }

        // 注册表ListView项目鼠标双击事件
        private void listView_Registry_DoubleClick(object sender, EventArgs e)
        {
            cRegistry.OnListViewDoubleClickAction();
        }

        // 注册表TreeView鼠标右键事件
        private void treeView_Registry_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && MainForm.main.treeView_Registry.SelectedNode != null)
            {
                cRegContextMenuStrip.CreatePopupMenu();
                contextMenuStrip_Registry.Show(treeView_Registry, e.X, e.Y);
            }
        }

        // 注册表ListView项目鼠标右键事件
        private void listView_Registry_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && treeView_Registry.SelectedNode != null)
            {
                cRegContextMenuStrip.CreatePopupMenu();
                contextMenuStrip_Registry.Show(treeView_Registry, e.X + treeView_Registry.Width, e.Y);
            }
        }

        // 树节点KEY名编辑完成事件
        private void treeView_Registry_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // 关闭TreeView的编辑功能
            MainForm.main.treeView_Registry.LabelEdit = false;
            string keyName = e.Label == null ? e.Node.Text : e.Label;
            try
            {
                // 获取其父节点的RegistryKey对象
                RegistryKey readOnlyParent = e.Node.Parent.Tag as RegistryKey;
                // 将父节点的RegistryKey对象转换为cRegKey对象
                cRegKey parent = cRegKey.Parse(readOnlyParent.Name, true);
                // 父节点下建立我们的子KEY
                parent.Key.CreateSubKey(keyName);
                // 存储子节点当前完整路径
                e.Node.Name = parent.Key.Name + "\\" + keyName;
                // 关联当前子节点对象
                e.Node.Tag = cRegKey.Parse(e.Node.Name).Key;
            }
            catch
            {
                e.Node.Remove();
                MessageBox.Show("[x_x]:" + Common.CommonFunction.GetLastError32());
            }
        }

        // 注册表ListView编辑完成时间
        private void listView_Registry_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            MainForm.main.listView_Registry.LabelEdit = false;
            ListViewItem lvi = MainForm.main.listView_Registry.Items[e.Item];
            string valueName = e.Label == null ? lvi.Text : e.Label;
            try
            {
                RegistryKey readOnlyKey = lvi.Tag as RegistryKey;
                RegistryKey key = cRegKey.Parse(readOnlyKey.Name, true).Key;
                cRegValue value = lvi.SubItems[2].Tag as cRegValue;
                key.SetValue(valueName, value.Data, value.Kind);
                lvi.Name = valueName;
                lvi.SubItems[2].Tag = new cRegValue(readOnlyKey, valueName);
            }
            catch
            {
                lvi.Remove();
                MessageBox.Show("[x_x]:" + Common.CommonFunction.GetLastError32());
            }
        }
        /************************右键菜单单击事件************************/
        // 修改VALUE值事件
        private void modifyPopupMenuItem_Click(object sender, EventArgs e)
        {
            cRegistry.OnListViewDoubleClickAction();
        }

        // 新建KEY事件
        private void keyPopupMenuItem_Click(object sender, EventArgs e)
        {
            cRegistry.OnCreateNewKeyAction();
        }

        // 新建String值事件
        private void stringValuePopupMenuItem_Click(object sender, EventArgs e)
        {
            cRegistry.OnCreateNewValueAction(RegistryValueKind.String);
        }

        // 新建Binary值事件
        private void binaryValuePopupMenuItem_Click(object sender, EventArgs e)
        {
            cRegistry.OnCreateNewValueAction(RegistryValueKind.Binary);
        }

        // 新建Dword值事件
        private void dWORDValuePopupMenuItem_Click(object sender, EventArgs e)
        {
            cRegistry.OnCreateNewValueAction(RegistryValueKind.DWord);
        }

        // 新建Qword值事件
        private void qWORDValuePopupMenuItem_Click(object sender, EventArgs e)
        {
            cRegistry.OnCreateNewValueAction(RegistryValueKind.QWord);
        }

        // 新建MultiString值事件
        private void multiStringValuePopupMenuItem_Click(object sender, EventArgs e)
        {
            cRegistry.OnCreateNewValueAction(RegistryValueKind.MultiString);
        }

        // 新建ExpandString值事件
        private void expandableStringPopupMenuItem_Click(object sender, EventArgs e)
        {
            cRegistry.OnCreateNewValueAction(RegistryValueKind.ExpandString);
        }

        // 右键菜单Expand事件
        private void expandPopupMenuItem_Click(object sender, EventArgs e)
        {
            if (MainForm.main.treeView_Registry.SelectedNode.IsExpanded)
            {
                MainForm.main.treeView_Registry.SelectedNode.Collapse();
            }
            else
            {
                MainForm.main.treeView_Registry.SelectedNode.Expand();
            }
        }

        // 右键菜单Refresh事件
        private void refreshPopupMenuItem_Click(object sender, EventArgs e)
        {
            cRegistry.OnRefreshAction();
        }

        // 右键菜单Delete事件
        private void deletePopupMenuItem_Click(object sender, EventArgs e)
        {
            cRegistry.OnDeleteAction();
        }
    }
}
