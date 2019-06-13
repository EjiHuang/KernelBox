using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KernelBox.Registry
{
    class cRegContextMenuStrip
    {

        /// <summary>
        /// 根据不同的活动控件，创建不同的右键菜单
        /// </summary>
        internal static void CreatePopupMenu()
        {
            // 判断当前鼠标右键是否显示New选项
            MainForm.main.contextMenuStrip_Registry.Visible =
                MainForm.main.popupMenuSeparatorNew.Visible = GetNewMenuState();
            // 判断当前鼠标右键是否显示Modify选项
            MainForm.main.modifyPopupMenuItem.Visible =
                MainForm.main.popupMenuSeperatorModify.Visible = GetModifyMenuState();

            // 判断当前鼠标右键菜单在TreeView时的状态
            if (MainForm.main.ActiveControl == MainForm.main.treeView_Registry)
            {
                MainForm.main.expandPopupMenuItem.Visible = true;
                MainForm.main.expandPopupMenuItem.Enabled = MainForm.main.treeView_Registry.SelectedNode.Nodes.Count > 0;
                MainForm.main.expandPopupMenuItem.Text = MainForm.main.treeView_Registry.SelectedNode.IsExpanded ? "Collapse" : "Expand";
            }
            else
            {
                // 如果鼠标右键菜单不在TreeView上时，就没有Expand选项
                MainForm.main.expandPopupMenuItem.Visible = false;
            }

            // 判断当前鼠标右键是否显示Refresh选项
            MainForm.main.refreshPopupMenuItem.Enabled = GetRefreshMenuState();
            // 判断当前鼠标右键是否显示Delete选项
            MainForm.main.deletePopupMenuItem.Enabled = GetDeleteMenuState();
        }

        /// <summary>
        /// 根据不同的活动控件，判断当前鼠标右键是否显示New选项
        /// </summary>
        /// <returns></returns>
        internal static bool GetNewMenuState()
        {
            if ((MainForm.main.ActiveControl == MainForm.main.treeView_Registry || (MainForm.main.listView_Registry.Items.Count > 0))
                && MainForm.main.treeView_Registry.SelectedNode != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据不同的活动控件，判断当前鼠标右键是否显示Modify选项
        /// </summary>
        /// <returns></returns>
        internal static bool GetModifyMenuState()
        {
            if (MainForm.main.ActiveControl == MainForm.main.listView_Registry && MainForm.main.listView_Registry.SelectedItems.Count == 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据不同的活动控件，判断当前鼠标右键是否显示Delete选项
        /// </summary>
        /// <returns></returns>
        internal static bool GetDeleteMenuState()
        {
            if (MainForm.main.ActiveControl == MainForm.main.treeView_Registry && MainForm.main.treeView_Registry.SelectedNode != null)
            {
                return (MainForm.main.treeView_Registry.SelectedNode.Level != 0);
            }
            else if (MainForm.main.ActiveControl is ListView && ((ListView)MainForm.main.ActiveControl).SelectedItems.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据不同的活动控件，判断当前鼠标右键是否显示Refresh选项
        /// </summary>
        /// <returns></returns>
        internal static bool GetRefreshMenuState()
        {
            if ((MainForm.main.ActiveControl == MainForm.main.treeView_Registry || (MainForm.main.ActiveControl == MainForm.main.listView_Registry && MainForm.main.listView_Registry.Items.Count > 0))
                && MainForm.main.treeView_Registry.SelectedNode != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据不同的活动控件，判断当前鼠标右键是否显示Copy选项
        /// </summary>
        /// <returns></returns>
        internal static bool GetCopyMenuState()
        {
            if (MainForm.main.ActiveControl == MainForm.main.treeView_Registry && MainForm.main.treeView_Registry.SelectedNode != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
