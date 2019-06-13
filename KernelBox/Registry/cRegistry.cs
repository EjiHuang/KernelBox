using KernelBox.Registry;
using KernelBox.Registry.Editors;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

/// <summary>
/// 注册表编辑器技术支持：CrackSoft
/// </summary>

namespace KernelBox.RegistryEditor
{
    /// <summary>
    /// 注册表类
    /// 用于注册表操作，同时与驱动通信
    /// </summary>
    class cRegistry
    {
        public static object GetLastError32 { get; private set; }

        /// <summary>
        /// 初始化注册表树
        /// </summary>
        internal static void InitialiszeRegistryTree()
        {
            // 展开树时绘制连线
            MainForm.main.treeView_Registry.ShowLines = true;

            MainForm.main.treeView_Registry.BeginUpdate();
            MainForm.main.treeView_Registry.Nodes.Clear();

            // 建立根节点 HKEY_CLASSES_ROOT
            AddRootKey(Microsoft.Win32.Registry.ClassesRoot);
            // 建立根节点 HKEY_CURRENT_USER
            AddRootKey(Microsoft.Win32.Registry.CurrentUser);
            // 建立根节点 HKEY_LOCAL_MACHINE
            AddRootKey(Microsoft.Win32.Registry.LocalMachine);
            // 建立根节点 HKEY_USERS
            AddRootKey(Microsoft.Win32.Registry.Users);
            // 建立根节点 HKEY_CURRENT_CONFIG
            AddRootKey(Microsoft.Win32.Registry.CurrentConfig);

            MainForm.main.treeView_Registry.EndUpdate();
        }

        /// <summary>
        /// 添加根结点方法
        /// </summary>
        /// <param name="key"></param>
        internal static void AddRootKey(RegistryKey key)
        {
            TreeNode tn = CreateNode(key.Name, key.Name, key);
            MainForm.main.treeView_Registry.Nodes.Add(tn);
            // 因为是根节点，所以添加一个空的节点
            tn.Nodes.Add(CreateNode());
        }

        /// <summary>
        /// 根据KEY信息创建一个节点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        internal static TreeNode CreateNode(string key, string text, object tag)
        {
            TreeNode tn = new TreeNode();
            tn.Text = text;
            tn.Name = key;
            tn.Tag = tag;
            return tn;
        }

        /// <summary>
        /// 创建一个空节点
        /// </summary>
        /// <returns></returns>
        internal static TreeNode CreateNode()
        {
            TreeNode tn = new TreeNode();
            return tn;
        }

        /// <summary>
        /// 读取当前父节点的子节点
        /// </summary>
        /// <param name="parentNode"></param>
        internal static void LoadSubKeys(TreeNode parentNode)
        {
            MainForm.main.treeView_Registry.SuspendLayout();

            parentNode.Nodes.Clear();
            // 获取父节点的RegistryKey关联对象
            RegistryKey key = parentNode.Tag as RegistryKey;
            // 获取父节点的子节点集合
            var subKeys = GetSubKeys(key);
            // 将子节点集合根据名称进行升序排序
            subKeys.OrderBy<cRegKey, string>(subKey => subKey.Name);
            // 将子节点集合填充到TreeView中
            foreach (cRegKey subKey in subKeys)
            {
                AddKeyToTree(parentNode, subKey);
            }

            MainForm.main.treeView_Registry.ResumeLayout();
        }

        /// <summary>
        /// 读取当前键获取VALUES
        /// </summary>
        /// <param name="key"></param>
        internal static void LoadValues(RegistryKey key)
        {
            // 状态栏显示当前操作的键路径
            MainForm.main.toolStripStatusLabel_Registry.Text = key.Name;
            // 初始化右侧ListView
            MainForm.main.listView_Registry.Items.Clear();
            // 获取KEY中的VALUE，并且保存到List中
            List<cRegValue> values = GetValues(key);
            // 判断values是否有效
            if (!values.Equals(null))
            {
                // 如果VALUES数目为0，则默认添加一个默认空值
                if (values.Count.Equals(0))
                {
                    AddValueToListViewItem(key, new cRegValue(String.Empty, RegistryValueKind.String, "(value not set)"));
                }
                else
                {
                    MainForm.main.listView_Registry.SuspendLayout();
                    // 每一项VALUE前面都需要添加一个默认值
                    cRegValue defaultValue = new cRegValue(String.Empty, RegistryValueKind.String, "(value not set)");
                    if (values.SingleOrDefault((val) => val.Name == defaultValue.Name) == null) 
                    {
                        AddValueToListViewItem(key, defaultValue);
                    }

                    foreach (cRegValue v in values)
                    {
                        AddValueToListViewItem(key, v);
                    }

                    MainForm.main.listView_Registry.ResumeLayout();
                }
            }
        }

        /// <summary>
        /// 获取KEY的VALUE，用cRegValue对象保存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static List<cRegValue> GetValues(RegistryKey key)
        {
            // 判断这个KEY里是否包含VALUE
            int valueCount = key.ValueCount;
            if (valueCount.Equals(0))
                return new List<cRegValue>();
            // 若包含VALUE，则创建valueCount个cRegValue用于保存
            List<cRegValue> values = new List<cRegValue>(valueCount);
            string[] valueNames = key.GetValueNames();
            for (int i = 0; i < valueNames.Length; i++)
            {
                values.Add(new cRegValue(key, valueNames[i]));
            }

            return values;
        }

        /// <summary>
        /// 获取当前键的子键，用cRegKey对象保存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static List<cRegKey> GetSubKeys(RegistryKey key)
        {
            // 获取父节点的子节点数目
            int subKeyCount = key.SubKeyCount;
            // 如果父节点下没有子节点，就返回一个空cRegKey对象
            if (subKeyCount.Equals(0))
            {
                return new List<cRegKey>();
            }
            // 根据子节点数目创建对象集合
            List<cRegKey> subKeys = new List<cRegKey>(subKeyCount);
            // 保存子节点名称字符串集合，用于遍历操作
            string[] subKeyNames = key.GetSubKeyNames();
            // 遍历父节点的子节点集合，填充到subKeys集合中
            for (int i = 0; i < subKeyNames.Length; i++)
            {
                try
                {
                    string keyName = subKeyNames[i];
                    cRegKey item = new cRegKey(keyName, key.OpenSubKey(keyName));
                    subKeys.Add(item);
                }
                catch { }
            }

            return subKeys;
        }

        /// <summary>
        /// 将VALUE添加到ListViewItem中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="regValue"></param>
        /// <returns></returns>
        internal static ListViewItem AddValueToListViewItem(RegistryKey key,cRegValue regValue)
        {
            // 添加名称到ListView，并且获取其ListViewItem进行操作
            ListViewItem lvi = MainForm.main.listView_Registry.Items.Add(regValue.Name);
            // 根据regValue的种类定位图片索引
            lvi.ImageKey = GetValueTypeIcon(regValue.Kind);
            // 绑定相关属性
            lvi.Name = regValue.Name;
            lvi.Tag = key;
            // 填充数据的类型到LsitViewItem
            lvi.SubItems.Add(regValue.Kind.ToDataType());
            // 填充VAULUE的DATA到LsitViewItem，然后转换为ListViewSubItem
            ListViewItem.ListViewSubItem lvsi = lvi.SubItems.Add(regValue.ToString());
            // 绑定相关属性
            lvsi.Tag = regValue;
            return lvi;
        }

        /// <summary>
        /// 添加KEY到TreeView中
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="subKey"></param>
        internal static void AddKeyToTree(TreeNode parentNode,cRegKey subKey)
        {
            RegistryKey key = subKey.Key;
            // 根据子节点属性来创建一个新的节点
            TreeNode newNode = CreateNode(key.Name, subKey.Name, key);
            parentNode.Nodes.Add(newNode);
            // 如果子节点下游子节点的话，就先填充一个空节点
            if (key.SubKeyCount > 0) 
            {
                newNode.Nodes.Add(CreateNode());
            }
        }

        /// <summary>
        /// 根据VALUE类型获取指定图标
        /// </summary>
        /// <param name="registryValueKind"></param>
        /// <returns></returns>
        internal static string GetValueTypeIcon(RegistryValueKind registryValueKind)
        {
            if (registryValueKind == RegistryValueKind.ExpandString ||
                registryValueKind == RegistryValueKind.MultiString ||
                registryValueKind == RegistryValueKind.String)
                return "ascii";
            else
                return "binary";
        }

        /// <summary>
        /// 右侧ListView双击事件
        /// </summary>
        internal static void OnListViewDoubleClickAction()
        {
            if (MainForm.main.listView_Registry.SelectedItems.Count.Equals(1))
            {
                cRegValue value = (cRegValue)MainForm.main.listView_Registry.SelectedItems[0].SubItems[2].Tag;
                if (value.ParentKey != null)
                {
                    Registry_ValueEditor editor = null;

                    switch (value.Kind)
                    {
                        case RegistryValueKind.String:
                        case RegistryValueKind.ExpandString:
                            editor = new StringEditor(value);
                            break;
                        case RegistryValueKind.Binary:
                            editor = new BinaryEditor(value);
                            break;
                        case RegistryValueKind.DWord:
                        case RegistryValueKind.QWord:
                            editor = new DWordQWordEditor(value);
                            break;
                        case RegistryValueKind.MultiString:
                            editor = new MultiStringEditor(value);
                            break;
                        case RegistryValueKind.Unknown:
                        case RegistryValueKind.None:
                            break;
                        default:
                            break;
                    }

                    if (editor != null)
                    {
                        if (editor.ShowDialog(MainForm.main) == DialogResult.OK)
                        {
                            RefreshValues();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 创建一个新KEY动作
        /// </summary>
        internal static void OnCreateNewKeyAction()
        {
            // 如果没选中树节点，我们就直接退出函数
            if (MainForm.main.treeView_Registry.SelectedNode == null)
                return;
            // 如果选中的节点含有子节点，并且子节点没被展开，我们就对其进行展开
            if (MainForm.main.treeView_Registry.HasChildren && !MainForm.main.treeView_Registry.SelectedNode.IsExpanded)
                MainForm.main.treeView_Registry.SelectedNode.Expand();
            // 获取当前选中的树节点对象
            RegistryKey key = MainForm.main.treeView_Registry.SelectedNode.Tag as RegistryKey;
            // 生成一个固定格式的KEY名
            string name = cRegOtherFunction.GetNewKeyName(key);
            string path = key.Name + "\\" + name;
            // 这里只是单单创建了一个空对象，最后还要在在TreeView的AfterLabelEdit事件中进行加工
            TreeNode tn = CreateNode(path, name, new object());
            MainForm.main.treeView_Registry.SelectedNode.Nodes.Add(tn);
            // 确保树节点可见，并在必要时候展开树节点和滚动TreeView控件
            tn.EnsureVisible();
            // 打开Label编辑
            MainForm.main.treeView_Registry.LabelEdit = true;
            // 开始进行编辑KEY名
            tn.BeginEdit();

        }

        /// <summary>
        /// 根据VALUE类型创建一个新的VALUE
        /// </summary>
        internal static void OnCreateNewValueAction(RegistryValueKind valueKind)
        {
            // 如果当前TreeView没有选中节点，函数直接退出
            if (MainForm.main.treeView_Registry.SelectedNode == null)
                return;
            // 获取选中节点的RegistryKey对象
            RegistryKey key = MainForm.main.treeView_Registry.SelectedNode.Tag as RegistryKey;
            // 为新VALUE创建一个名字
            string name = cRegOtherFunction.GetNewValueName(key);
            // 将新VALUE添加到ListView中
            ListViewItem lvi = AddValueToListViewItem(key, new cRegValue(name, valueKind, valueKind.GetDefaultData()));
            // 开启Lable编辑功能
            MainForm.main.listView_Registry.LabelEdit = true;
            lvi.BeginEdit();
        }

        /// <summary>
        /// 更新完VALUES之后进行的刷新操作
        /// </summary>
        internal static void RefreshValues()
        {
            string key;
            if (MainForm.main.listView_Registry.SelectedItems.Count.Equals(1))
            {
                key = MainForm.main.listView_Registry.SelectedItems[0].Name;
                RegistryKey regKey = MainForm.main.treeView_Registry.SelectedNode.Tag as RegistryKey;
                if (regKey != null)
                {
                    LoadValues(regKey);
                }
                ListViewItem lvi = MainForm.main.listView_Registry.Items[key];
                if (lvi != null)
                {
                    lvi.Selected = true;
                }
            }
            else
            {
                key = string.Empty;
                RegistryKey regKey = MainForm.main.treeView_Registry.SelectedNode.Tag as RegistryKey;
                if (regKey != null)
                {
                    LoadValues(regKey);
                }
                ListViewItem lvi = MainForm.main.listView_Registry.Items[key];
                if (lvi != null)
                {
                    lvi.Selected = true;
                }
            }
        }

        /// <summary>
        /// 注册表刷新事件
        /// </summary>
        internal static void OnRefreshAction()
        {
            if (MainForm.main.treeView_Registry.SelectedNode == null)
                return;
            if (MainForm.main.ActiveControl == MainForm.main.treeView_Registry || MainForm.main.ActiveControl == MainForm.main.listView_Registry)
            {
                string lastSelectedValue, lastSelectedKey;

                lastSelectedKey = MainForm.main.treeView_Registry.SelectedNode.FullPath;

                if (MainForm.main.listView_Registry.SelectedItems.Count == 1)
                    lastSelectedValue = MainForm.main.listView_Registry.SelectedItems[0].Name;
                else
                    lastSelectedValue = string.Empty;

                RefreshTreeView();

                TreeNode[] arrMatches = MainForm.main.treeView_Registry.Nodes.Find(lastSelectedKey, true);
                if (arrMatches.Length > 0)
                {
                    MainForm.main.treeView_Registry.SelectedNode = arrMatches[0];
                    ListViewItem lvi = MainForm.main.listView_Registry.Items[lastSelectedValue];
                    if (lvi != null)
                        lvi.Selected = true;
                }
            }

        }

        /// <summary>
        /// 删除注册表方法
        /// </summary>
        internal static void OnDeleteAction()
        {
            // 删除KEY事件
            if (MainForm.main.ActiveControl == MainForm.main.treeView_Registry)
            {
                string message = "Are you sure want to delete this key and all of it's subkeys?";
                if (DialogResult.Yes == MessageBox.Show(MainForm.main, message, "Key Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
                    if (cRegOtherFunction.DeleteKey(MainForm.main.treeView_Registry.SelectedNode.Tag.ToString()))
                        MainForm.main.treeView_Registry.SelectedNode.Remove();
                    else
                        MessageBox.Show("[x_x]:" + Common.CommonFunction.GetLastError32());
            }
            // 删除VALUE事件
            if (MainForm.main.ActiveControl == MainForm.main.listView_Registry)
            {
                string message = "Are you sure want to delete these values?";
                bool isSuccess = true;
                if (DialogResult.Yes == MessageBox.Show(MainForm.main, message, "Value Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
                {
                    foreach (ListViewItem v in MainForm.main.listView_Registry.SelectedItems)
                    {
                        if (cRegOtherFunction.DeleteValue(v.Tag.ToString(), v.Text))
                            v.Remove();
                        else
                            isSuccess = false;
                    }
                    if (!isSuccess)
                        MessageBox.Show("[x_x]:" + Common.CommonFunction.GetLastError32());
                }
            }
        }

        /// <summary>
        /// 刷新TreeView控件函数
        /// </summary>
        internal static void RefreshTreeView()
        {
            TreeNode targetNode;
            if (MainForm.main.treeView_Registry.SelectedNode == null)
                return;

            if (MainForm.main.treeView_Registry.SelectedNode.IsExpanded)
                targetNode = MainForm.main.treeView_Registry.SelectedNode;
            else if (MainForm.main.treeView_Registry.SelectedNode.Level == 0)
                return;
            else
                targetNode = MainForm.main.treeView_Registry.SelectedNode.Parent;

            bool bError = false;
            do
            {
                try
                {
                    LoadSubKeys(targetNode);
                }
                catch (IOException)
                {
                    bError = true;
                    targetNode = targetNode.Parent;
                }
            } while (bError && targetNode.Level > 0);
        }
    }
}
