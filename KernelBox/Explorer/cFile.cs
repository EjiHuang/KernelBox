using KernelBox;
using KernelBox.Common;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

/// <summary>
/// Explorer的命名空间
/// </summary>
namespace ExplorerManager
{
    /// <summary>
    /// 文件操作类
    /// </summary>
    class cFile
    {
        // 保存图标的集合
        internal static Icon[] icon;

        /// <summary>
        /// 主初始化函数
        /// </summary>
        internal static void MainInit()
        {
            CommonVar.gIsCFileInited = true;
            InitIconArray();
            GetDiskPartition();
        }

        /// <summary>
        /// 初始化图标信息
        /// </summary>
        /// <returns></returns>
        internal static bool InitIconArray()
        {
            try
            {
                icon = new Icon[]
                {
                    ExtractIcon("%SystemRoot%\\system32\\shell32.dll", 7),
                    ExtractIcon("%SystemRoot%\\system32\\shell32.dll", 8),
                    ExtractIcon("%SystemRoot%\\system32\\shell32.dll", 9),
                    ExtractIcon("%SystemRoot%\\system32\\shell32.dll", 11),
                    ExtractIcon("%SystemRoot%\\system32\\shell32.dll", 15),
                    ExtractIcon("%SystemRoot%\\system32\\shell32.dll", 3),
                    ExtractIcon("%SystemRoot%\\system32\\shell32.dll", 4),
                    ExtractIcon("%SystemRoot%\\system32\\shell32.dll", 31)
                };
                foreach (Icon v in icon)
                {
                    MainForm.main.treeViewImageList_Explorer.Images.Add(v);
                }
                return true;
            }
            catch (Exception ex)
            {
                MainForm.main.toolStripStatusLabel_Explorer.Text = "Error: " + ex.Message;
                throw;
            }
        }

        /// <summary>
        /// 获取图标
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="iIndex"></param>
        /// <returns></returns>
        internal static Icon ExtractIcon(string strFileName, int iIndex)
        {
            try
            {
                IntPtr hIcon = (IntPtr)NativeApi.ExtractIcon(CommonVar.mainHandle, strFileName, iIndex);
                if (!hIcon.Equals(null))
                {
                    Icon icon = Icon.FromHandle(hIcon);
                    return icon;
                }
            }
            catch (Exception ex)
            {
                MainForm.main.toolStripStatusLabel_Explorer.Text = "Error: " + ex.Message;
                throw;
            }
            return null;
        }

        /// <summary>
        /// 获取磁盘分区
        /// </summary>
        /// <returns></returns>
        internal static bool GetDiskPartition()
        {
            // 左侧树形浏览器初始化
            MainForm.main.treeViewLeft.ImageList = MainForm.main.treeViewImageList_Explorer;
            MainForm.main.treeViewLeft.BeginUpdate();
            MainForm.main.treeViewLeft.Nodes.Clear();

            // 建立根节点
            TreeNode rootNode = new TreeNode("MyPc", 4, 4);
            MainForm.main.treeViewLeft.Nodes.Add(rootNode);
            MainForm.main.treeViewLeft.SelectedNode = rootNode;
            CommonVar.gCurrentNode_File = rootNode;
            // 展开根节点
            rootNode.Expand();

            try
            {
                int iImageIndex = 0;
                int iSelectedIndex = 0;
                DriveInfo[] Drives = DriveInfo.GetDrives();
                foreach (DriveInfo v in Drives)
                {
                    string strV;

                    // 判断此驱动器是否是可移动存储设备
                    if (v.DriveType.Equals(DriveType.Removable))
                    {
                        if (!v.VolumeLabel.Length.Equals(0))
                        {
                            // 直接获取驱动器的卷标
                            strV = v.VolumeLabel;
                        }
                        else
                        {
                            strV = "Removable Disk";
                        }
                        iImageIndex = 0;
                        iSelectedIndex = 0;
                        strV = strV + " (" + v.ToString().Substring(0, 2) + ")";
                    }
                    // 判断此驱动器是否是一个固定的磁盘
                    else if (v.DriveType.Equals(DriveType.Fixed))
                    {
                        if (!v.VolumeLabel.Length.Equals(0))
                        {
                            // 直接获取驱动器的卷标
                            strV = v.VolumeLabel;
                        }
                        else
                        {
                            strV = "Local Disk";
                        }
                        iImageIndex = 1;
                        iSelectedIndex = 1;
                        strV = strV + " (" + v.ToString().Substring(0, 2) + ")";
                    }
                    // 判断此驱动器是否是网络驱动器
                    else if (v.DriveType.Equals(DriveType.Network))
                    {
                        if (!v.VolumeLabel.Length.Equals(0))
                        {
                            // 直接获取驱动器的卷标
                            strV = v.VolumeLabel;
                        }
                        else
                        {
                            strV = "Network Drive";
                        }
                        iImageIndex = 2;
                        iSelectedIndex = 2;
                        strV = strV + " (" + v.ToString().Substring(0, 2) + ")";
                    }
                    // 判断此驱动器是否是光盘设备
                    else if (v.DriveType.Equals(DriveType.CDRom))
                    {
                        // 判断光盘驱动器里是否有加载光盘
                        if (v.IsReady.Equals(true))
                        {
                            if (!v.VolumeLabel.Length.Equals(0))
                            {
                                // 直接获取驱动器的卷标
                                strV = v.VolumeLabel;
                            }
                            else
                            {
                                strV = "CD-ROM Device";
                            }
                        }
                        else
                        {
                            strV = "CD-ROM Device";
                        }
                        iImageIndex = 3;
                        iSelectedIndex = 3;
                        strV = strV + " (" + v.ToString().Substring(0, 2) + ")";
                        CommonVar.gIsCDROM = true;
                    }
                    // 判断此驱动器是否是无法识别设备
                    else if (v.DriveType.Equals(DriveType.Unknown))
                    {
                        iImageIndex = 0;
                        iImageIndex = 0;
                        strV = v.VolumeLabel + v.ToString();
                    }
                    // 其他特殊情况
                    else
                    {
                        iImageIndex = 0;
                        iSelectedIndex = 0;
                        strV = v.ToString();
                    }
                    TreeNode tnDrive = new TreeNode(strV, iImageIndex, iSelectedIndex);
                    MainForm.main.treeViewLeft.Nodes[0].Nodes.Add(tnDrive);
                    // 添加文件夹树
                    AddDirectories(tnDrive);

                    MainForm.main.treeViewLeft.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                MainForm.main.toolStripStatusLabel_Explorer.Text = "Error: " + ex.Message;
                throw;
            }

            // 同时显示右侧ListView内容
            ShowDriveRight();

            MainForm.main.treeViewLeft.EndUpdate();
            return true;
        }

        /// <summary>
        /// 右侧显示磁盘分区
        /// </summary>
        internal static void ShowDriveRight()
        {
            int iIdex = 0;
            MainForm.main.imageListRight1_Explorer.Images.Clear();

            // 对应shell32.dll中的7，8，9，11
            for (; iIdex < 4; iIdex++) 
            {
                MainForm.main.imageListRight1_Explorer.Images.Add(icon[iIdex]);
            }

            // 设置右侧的ListView
            MainForm.main.listView_Explorer.Clear();
            MainForm.main.listView_Explorer.Columns.Add("FileName", 120, HorizontalAlignment.Left);
            MainForm.main.listView_Explorer.Columns.Add("Type", 120, HorizontalAlignment.Left);
            MainForm.main.listView_Explorer.Columns.Add("Size", 100, HorizontalAlignment.Right);
            MainForm.main.listView_Explorer.Columns.Add("AvailableSpace", 100, HorizontalAlignment.Right);
            MainForm.main.listView_Explorer.Columns.Add("Remarks", 120, HorizontalAlignment.Left);

            int iItem = 0;
            int iIcon = 1;
            try
            {
                DriveInfo[] Drives = DriveInfo.GetDrives();
                foreach (DriveInfo v in Drives)
                {
                    string strV;
                    string[] arrSubItem = new string[5];

                    if (v.DriveType.Equals(DriveType.Removable))
                    {
                        if (v.VolumeLabel.Length.Equals(0))
                        {
                            strV = "Removable Disk";
                        }
                        else
                        {
                            strV = v.VolumeLabel;
                        }
                        iIcon = 0;
                        strV = strV + " (" + v.ToString().Substring(0, 2) + ")";
                        arrSubItem[0] = strV;
                        arrSubItem[1] = "Removable Disk";
                        arrSubItem[2] = GetDriveTotalSize(v);
                        arrSubItem[3] = GetDriveTotalFreeSpace(v);
                        arrSubItem[4] = "";
                    }
                    else if (v.DriveType.Equals(DriveType.Fixed))
                    {
                        if (v.VolumeLabel.Length.Equals(0))
                        {
                            strV = "Local Disk";
                        }
                        else
                        {
                            strV = v.VolumeLabel;
                        }
                        iIcon = 1;
                        strV = strV + " (" + v.ToString().Substring(0, 2) + ")";
                        arrSubItem[0] = strV;
                        arrSubItem[1] = "Local Disk";
                        arrSubItem[2] = GetDriveTotalSize(v);
                        arrSubItem[3] = GetDriveTotalFreeSpace(v);
                        arrSubItem[4] = "";
                    }
                    else if (v.DriveType.Equals(DriveType.Network))
                    {
                        if (v.VolumeLabel.Length.Equals(0))
                        {
                            strV = "Network Drive";
                        }
                        else
                        {
                            strV = v.VolumeLabel;
                        }
                        iIcon = 2;
                        strV = strV + " (" + v.ToString().Substring(0, 2) + ")";
                        arrSubItem[0] = strV;
                        arrSubItem[1] = "Network Drive";
                        arrSubItem[2] = GetDriveTotalSize(v);
                        arrSubItem[3] = GetDriveTotalFreeSpace(v);
                        arrSubItem[4] = "";
                    }
                    else if (v.DriveType.Equals(DriveType.CDRom))
                    {
                        iIcon = 3;
                        if (v.IsReady.Equals(true))
                        {
                            if (v.VolumeLabel.Length.Equals(0))
                            {
                                strV = "CD-ROM Device";
                            }
                            else
                            {
                                strV = v.VolumeLabel;
                            }
                            iIcon = 2;
                            strV = strV + " (" + v.ToString().Substring(0, 2) + ")";
                            arrSubItem[0] = strV;
                            arrSubItem[1] = "CD-ROM Device";
                            arrSubItem[2] = GetDriveTotalSize(v);
                            arrSubItem[3] = GetDriveTotalFreeSpace(v);
                            arrSubItem[4] = "";
                        }
                        else
                        {
                            strV = "CD-ROM Device";
                            strV = strV + " (" + v.ToString().Substring(0, 2) + ")";
                            arrSubItem[0] = strV;
                            arrSubItem[1] = "CD-ROM Device";
                            arrSubItem[2] = "";
                            arrSubItem[3] = "";
                            arrSubItem[4] = "";
                        }
                    }
                    else if (v.DriveType.Equals(DriveType.Unknown))
                    {
                        iIcon = 0;
                        strV = v.VolumeLabel + v.ToString();
                        strV = v.ToString();
                        arrSubItem[0] = strV;
                        arrSubItem[1] = "Unknown Type";
                        arrSubItem[2] = "Unknown Size";
                        arrSubItem[3] = "Unknown Size";
                        arrSubItem[4] = "";
                    }

                    // 向ListView中插入数据
                    ListViewItem lvi = new ListViewItem(arrSubItem, iIcon);
                    MainForm.main.listView_Explorer.Items.Insert(iItem, lvi);
                    // 插入的数据进行递增，防止覆盖插入
                    iItem++;
                }
                CommonVar.gStrFilePath = "";
                MainForm.main.listView_Explorer.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                MainForm.main.textBox_FilePathOuput.Text = "MyPc";
                MainForm.main.toolStripStatusLabel_Explorer.Text = iItem.ToString() + " Objects";
            }
            catch (IOException ex)
            {
                MainForm.main.toolStripStatusLabel_Explorer.Text = "Error: " + ex.Message;
                throw;
            }
        }

        /// <summary>
        /// 左侧添加文件夹树
        /// </summary>
        /// <param name="tn"></param>
        internal static void AddDirectories(TreeNode tn)
        {
            tn.Nodes.Clear();

            string strPath = tn.FullPath;
            strPath = strPath.Remove(0, 5);
            strPath = CorrectPath(strPath);

            // 获取当前的目录
            DirectoryInfo DirInfo = new DirectoryInfo(strPath);
            DirectoryInfo[] arrDirInfo;
            try
            {
                if (CommonVar.gIsCDROM.Equals(true))
                {
                    CommonVar.gIsCDROM = false;
                    return;
                }
                else
                {
                    // 获取当前目录的子目录
                    arrDirInfo = DirInfo.GetDirectories();
                }
            }
            catch
            {
                return;
            }

            int iImageIndex = 5;
            int iSelectedIndex = 5;
            foreach (DirectoryInfo v in arrDirInfo)
            {
                // 判断是否为回收站
                if (v.Name.Equals("RECYCLER") || v.Name.Equals("RECYCLED") || v.Name.Equals("recycled")) 
                {
                    iImageIndex = 7;
                    iSelectedIndex = 7;
                }
                else
                {
                    iImageIndex = 5;
                    iSelectedIndex = 5;
                }
                TreeNode tnDir = new TreeNode(v.Name, iImageIndex, iSelectedIndex);
                tn.Nodes.Add(tnDir);
            }
        }

        /// <summary>
        /// 左侧展开节点，右侧显示信息
        /// </summary>
        /// <param name="tn"></param>
        internal static void ExpandNode(TreeNode tn)
        {
            // 获取节点的完整路径
            string strV = tn.FullPath;
            strV = strV.Remove(0, 5);
            strV = CorrectPath(strV);

            if (strV.Substring(strV.Length - 2, 1).Equals(":"))
            {
                DriveInfo di = new DriveInfo(strV);
                // 先判断选中的是否是光驱
                if (di.IsReady.Equals(false))
                {
                    if (di.DriveType.Equals(DriveType.CDRom))
                    {
                        MessageBox.Show("Please insert the disc into the " + strV + ".", "Information", 0, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("The " + strV + " is not ready", "Error", 0, MessageBoxIcon.Error);
                    }
                    return;
                }
            }
            // 下面判断选中的是文件夹
            MainForm.main.toolStripStatusLabel_Explorer.Text = "Refreshing folder, please wait ...";
            MainForm.main.treeViewLeft.BeginUpdate();
            MainForm.main.Cursor = Cursors.WaitCursor;
            MainForm.main.imageListRight1_Explorer.Images.Clear();
            // 获取文件夹的图标shell32.dll索引3
            MainForm.main.imageListRight1_Explorer.Images.Add(icon[5]);
            // 设置列表框的显示表头
            MainForm.main.listView_Explorer.Clear();
            MainForm.main.listView_Explorer.Columns.Add("FileName", 160, HorizontalAlignment.Left);
            MainForm.main.listView_Explorer.Columns.Add("Size", 120, HorizontalAlignment.Left);
            MainForm.main.listView_Explorer.Columns.Add("Creation Time", 120, HorizontalAlignment.Left);
            MainForm.main.listView_Explorer.Columns.Add("Last Access Time", 120, HorizontalAlignment.Left);
            // 取得目录对象
            DirectoryInfo diri = new DirectoryInfo(strV);
            // 获取目录下的所有文件
            FileInfo[] fi;
            try
            {
                fi = diri.GetFiles();
            }
            catch (Exception ex)
            {
                MainForm.main.toolStripStatusLabel_Explorer.Text = "Error: " + ex.Message;
                MainForm.main.Cursor = Cursors.Arrow;
                MainForm.main.treeViewLeft.EndUpdate();
                return;
            }
            // 用于记录文件信息的数组
            string[] arrSubItems = new string[4];
            // 获取文件的创建时间和访问时间
            int iFileCount = 0;
            int iIconIndex = 1;
            foreach (FileInfo v in fi)
            {
                arrSubItems[0] = v.Name;
                arrSubItems[1] = GetFileTotalSize(v);
                arrSubItems[2] = v.CreationTime.ToString();
                arrSubItems[3] = v.LastAccessTime.ToString();
                // 获取每个文件的图标
                try
                {
                    SetIcon(MainForm.main.imageListRight1_Explorer, v.FullName, false);
                }
                catch (Exception ex)
                {
                    MainForm.main.toolStripStatusLabel_Explorer.Text = "Error: " + ex.Message;
                    MainForm.main.Cursor = Cursors.Arrow;
                    MainForm.main.treeViewLeft.EndUpdate();
                    return;
                }
                // 插入到列表框中
                MainForm.main.listView_Explorer.Items.Insert(iFileCount, new ListViewItem(arrSubItems, iIconIndex));
                iFileCount++;
                iIconIndex++;
            }
            // 向列表框中插入文件夹，即获取当前目录下的子目录
            int iFolderCount = 0;
            try
            {
                foreach (DirectoryInfo v in diri.GetDirectories())
                {
                    MainForm.main.listView_Explorer.Items.Insert(iFolderCount, new ListViewItem(v.Name, 0));
                    iFolderCount++;
                }
            }
            catch (Exception ex)
            {
                MainForm.main.toolStripStatusLabel_Explorer.Text = "Error: " + ex.Message;
                MainForm.main.Cursor = Cursors.Arrow;
                MainForm.main.treeViewLeft.EndUpdate();
                return;
            }
            MainForm.main.treeViewLeft.EndUpdate();

            CommonVar.gStrFilePath = strV;
            MainForm.main.textBox_FilePathOuput.Text = CommonVar.gStrFilePath;
            MainForm.main.toolStripStatusLabel_Explorer.Text = iFolderCount + iFileCount + " Objects (" + iFolderCount.ToString() + " folders," + iFileCount.ToString() + " files)";
            MainForm.main.Cursor = Cursors.Arrow;
            MainForm.main.listView_Explorer.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        /// <summary>
        /// 双击列表框中文件夹，显示子目录
        /// </summary>
        /// <param name="strPath"></param>
        internal static void ExpandFolder(string strPath)
        {
            MainForm.main.toolStripStatusLabel_Explorer.Text = "Refreshing folder, please wait ...";
            MainForm.main.listView_Explorer.BeginUpdate();
            MainForm.main.Cursor = Cursors.WaitCursor;

            MainForm.main.imageListRight1_Explorer.Images.Clear();
            // 获取文件夹的图标shell32.dll索引3
            MainForm.main.imageListRight1_Explorer.Images.Add(icon[5]);
            // 设置列表框的显示表头
            MainForm.main.listView_Explorer.Clear();
            MainForm.main.listView_Explorer.Columns.Add("FileName", 160, HorizontalAlignment.Left);
            MainForm.main.listView_Explorer.Columns.Add("Size", 120, HorizontalAlignment.Left);
            MainForm.main.listView_Explorer.Columns.Add("Creation Time", 120, HorizontalAlignment.Left);
            MainForm.main.listView_Explorer.Columns.Add("Last Access Time", 120, HorizontalAlignment.Left);

            strPath = CorrectFile(strPath);
            DirectoryInfo diri = new DirectoryInfo(strPath);

            // 获取目录下的所有文件
            FileInfo[] fi;
            try
            {
                fi = diri.GetFiles();
            }
            catch (Exception ex)
            {
                MainForm.main.toolStripStatusLabel_Explorer.Text = "Error: " + ex.Message;
                MainForm.main.Cursor = Cursors.Arrow;
                MainForm.main.listView_Explorer.EndUpdate();
                return;
            }
            // 用于记录文件信息的数组
            string[] arrSubItems = new string[4];
            // 获取文件的创建时间和访问时间
            int iFileCount = 0;
            int iIconIndex = 1;
            foreach (FileInfo v in fi)
            {
                arrSubItems[0] = v.Name;
                arrSubItems[1] = GetFileTotalSize(v);
                arrSubItems[2] = v.CreationTime.ToString();
                arrSubItems[3] = v.LastAccessTime.ToString();
                // 获取每个文件的图标
                try
                {
                    // 建立一条线程来执行
                    //ThreadStart starter = delegate { mSetIcon(MainForm.main.imageListRight1, v.FullName, false); };
                    //Thread th = new Thread(starter);
                    //th.Start();
                    SetIcon(MainForm.main.imageListRight1_Explorer, v.FullName, false);
                }
                catch (Exception ex)
                {
                    MainForm.main.toolStripStatusLabel_Explorer.Text = "Error: " + ex.Message;
                    MainForm.main.Cursor = Cursors.Arrow;
                    MainForm.main.listView_Explorer.EndUpdate();
                    return;
                }
                // 插入到列表框中
                MainForm.main.listView_Explorer.Items.Insert(iFileCount, new ListViewItem(arrSubItems, iIconIndex));
                iFileCount++;
                iIconIndex++;
            }
            // 向列表框中插入文件夹，即获取当前目录下的子目录
            int iFolderCount = 0;
            try
            {
                foreach (DirectoryInfo v in diri.GetDirectories())
                {
                    MainForm.main.listView_Explorer.Items.Insert(iFolderCount, new ListViewItem(v.Name, 0));
                    iFolderCount++;
                }
            }
            catch (Exception ex)
            {
                MainForm.main.toolStripStatusLabel_Explorer.Text = "Error: " + ex.Message;
                MainForm.main.Cursor = Cursors.Arrow;
                MainForm.main.listView_Explorer.EndUpdate();
                return;
            }
            MainForm.main.listView_Explorer.EndUpdate();
            CommonVar.gStrFilePath = strPath;
            MainForm.main.toolStripStatusLabel_Explorer.Text = iFolderCount + iFileCount + " Objects (" + iFolderCount.ToString() + " folders," + iFileCount.ToString() + " files)";
            MainForm.main.Cursor = Cursors.Arrow;
            MainForm.main.listView_Explorer.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        /// <summary>
        /// 为列表框的文件设置图标
        /// </summary>
        /// <param name="imageList"></param>
        /// <param name="fileName"></param>
        /// <param name="tf"></param>
        private static void SetIcon(ImageList imageList, string fileName, bool tf)
        {
            NativeApiEx.SHFILEINFO _shFileInfo = new NativeApiEx.SHFILEINFO();
            if (tf.Equals(true))
            {
                int iTotal = (int)NativeApi.SHGetFileInfo(fileName, 0, ref _shFileInfo, 100, 16640);
                if (_shFileInfo.hIcon.Equals(IntPtr.Zero))
                    return;
                try
                {
                    if (0 < iTotal)
                    {
                        Icon icon = Icon.FromHandle(_shFileInfo.hIcon);
                        imageList.Images.Add(icon);
                    }
                }
                catch (Exception ex)
                {
                    MainForm.main.toolStripStatusLabel_Explorer.Text = "Error: " + ex.Message;
                    return;
                }
            }
            else
            {
                int iTotal = (int)NativeApi.SHGetFileInfo(fileName, 0, ref _shFileInfo, 100, 257);
                if (_shFileInfo.hIcon.Equals(IntPtr.Zero))
                    return;
                try
                {
                    if (0 < iTotal)
                    {
                        Icon icon = Icon.FromHandle(_shFileInfo.hIcon);
                        imageList.Images.Add(icon);
                    }
                }
                catch (Exception ex)
                {
                    MainForm.main.toolStripStatusLabel_Explorer.Text = "Error: " + ex.Message;
                    return;
                }
            }
        }

        /// <summary>
        /// 获取文件的总大小
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        private static string GetFileTotalSize(FileInfo fi)
        {
            string strV;

            if (fi.Length.Equals(0))
            {
                return "0 byte";
            }
            else if (fi.Length < 1024)
            {
                return fi.Length.ToString() + " byte";
            }
            else if (fi.Length < 1048576)
            {
                strV = (float.Parse(fi.Length.ToString()) / 1024).ToString();
                if (strV.IndexOf(".").Equals(-1))
                {
                    return strV + " KB";
                }
                else
                {
                    return strV.Substring(0, strV.LastIndexOf(".")) + " KB";
                }
            }
            else if (fi.Length < 1073741824)
            {
                strV = (float.Parse(fi.Length.ToString()) / 1048576).ToString();
                if (strV.IndexOf(".").Equals(-1))
                {
                    return strV + " MB";
                }
                else
                {
                    return strV.Substring(0, strV.LastIndexOf(".")) + " MB";
                }
            }
            else
            {
                strV = (float.Parse(fi.Length.ToString()) / 1073741824).ToString();
                if (strV.IndexOf(".").Equals(-1))
                {
                    return strV + " GB";
                }
                else
                {
                    return strV.Substring(0, strV.LastIndexOf(".")) + " GB";
                }
            }
        }

        /// <summary>
        /// 获取磁盘分区的总大小
        /// </summary>
        /// <param name="di"></param>
        /// <returns></returns>
        private static string GetDriveTotalSize(DriveInfo di)
        {
            string strV;

            if (di.TotalSize.Equals(0))
            {
                return "0 byte";
            }
            else if (di.TotalSize < 1024)
            {
                return di.TotalSize.ToString() + " byte";
            }
            else if (di.TotalSize < 1048576)
            {
                strV = (float.Parse(di.TotalSize.ToString()) / 1024).ToString();
                if (strV.IndexOf(".").Equals(-1))
                {
                    return strV + " KB";
                }
                else
                {
                    return strV.Substring(0, strV.LastIndexOf(".")) + " KB";
                }
            }
            else if (di.TotalSize < 1073741824)
            {
                strV = (float.Parse(di.TotalSize.ToString()) / 1048576).ToString();
                if (strV.IndexOf(".").Equals(-1))
                {
                    return strV + " MB";
                }
                else
                {
                    return strV.Substring(0, strV.LastIndexOf(".")) + " MB";
                }
            }
            else
            {
                strV = (float.Parse(di.TotalSize.ToString()) / 1073741824).ToString();
                if (strV.IndexOf(".").Equals(-1))
                {
                    return strV + " GB";
                }
                else
                {
                    return strV.Substring(0, strV.LastIndexOf(".")) + " GB";
                }
            }
        }

        /// <summary>
        /// 获取磁盘分区的可用空间
        /// </summary>
        /// <param name="di"></param>
        /// <returns></returns>
        private static string GetDriveTotalFreeSpace(DriveInfo di)
        {
            string strV;

            if (di.TotalFreeSpace.Equals(0))
            {
                return "0 byte";
            }
            else if (di.TotalFreeSpace < 1024)
            {
                return di.TotalFreeSpace.ToString() + " byte";
            }
            else if (di.TotalFreeSpace < 1048576)
            {
                strV = (float.Parse(di.TotalFreeSpace.ToString()) / 1024).ToString();
                if (strV.IndexOf(".").Equals(-1))
                {
                    return strV + " KB";
                }
                else
                {
                    return strV.Substring(0, strV.LastIndexOf(".")) + " KB";
                }
            }
            else if (di.TotalFreeSpace < 1073741824)
            {
                strV = (float.Parse(di.TotalFreeSpace.ToString()) / 1048576).ToString();
                if (strV.IndexOf(".").Equals(-1))
                {
                    return strV + " MB";
                }
                else
                {
                    return strV.Substring(0, strV.LastIndexOf(".")) + " MB";
                }
            }
            else
            {
                strV = (float.Parse(di.TotalFreeSpace.ToString()) / 1073741824).ToString();
                if (strV.IndexOf(".").Equals(-1))
                {
                    return strV + " GB";
                }
                else
                {
                    return strV.Substring(0, strV.LastIndexOf(".")) + " GB";
                }
            }
        }

        /// <summary>
        /// 将文件夹路径改为正确的格式
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        private static string CorrectPath(string strPath)
        {
            if (strPath.Substring(strPath.Length - 2, 1) == ":")
            {
                strPath = strPath.Substring(strPath.Length - 3, 2) + "\\";
            }
            else
            {
                int a = strPath.IndexOf("(") + 1;
                int b = strPath.IndexOf(")") + 1;
                strPath = strPath.Substring(a, 2) + strPath.Substring(b, strPath.Length - b);
            }
            return strPath;
        }

        /// <summary>
        /// 将文件路径改为正确的格式
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        internal static string CorrectFile(string strPath)
        {
            if (strPath.Substring(strPath.Length - 2, 1) == ":")
            {
                strPath = strPath.Substring(strPath.Length - 3, 2) + "\\";
            }
            return strPath;
        }
    }
}
