using KernelBox.Common;
using System;

using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

/// <summary>
/// 进程管理命名空间
/// </summary>
namespace KernelBox.ProcessMgr
{
    // 用于与驱动通信的结构体声明
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PROCESSINFO // 进程信息
    {
        public UInt64 EProcess;         // 进程eprocess地址
        public UInt32 Pid;              // 进程Pid
        public UInt32 PPid;             // 进程父进程Pid
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
        public byte[] ImageFileName;    // 进程名
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string Path;             // 进程路径
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct MODULEINFO // 进程模块信息
    {
        public UInt64 Base;     // 模块基地址
        public UInt32 Size;     // 模块大小
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string Path;     // 模块路径
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct THREADINFO // 进程线程信息
    {
        public UInt64 EThread;      // 模块基地址
        public UInt32 Tid;          // 模块大小
        public UInt32 Priority;     // 线程优先级
        public UInt64 Teb;          // 线程Teb地址
        public UInt64 Entry;        // 线程入口地址
        public UInt64 Switch;       // 线程切换次数
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] State;        // 线程状态
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct MODULE_UNINSTALL // 卸载模块
    {
        public UInt64 Eprocess;      // 进程eprocess地址
        public UInt64 Base;          // 模块基地址
    }

    /// <summary>
    /// 进程管理类
    /// </summary>
    class cProcessMgr
    {
        static public UInt32 ProcNum;                           // 记录进程数量
        static public UInt32[] ProcStatus = new UInt32[2];      // {[pid],[status]}记录进程状态(1：正常状态 0：暂停状态)
        static public UInt32 ModuleNum = 1024;                  // 记录模块数量
        static public UInt32 ThreadNum = 256;                   // 记录线程数量

        /// <summary>
        /// 初始化进程列表
        /// </summary>
        static public void InitializeProcessList()
        {
            // R0层获取进程数目
            uint[] IoReturnBuffer = new uint[1];
            uint BytesReturned = new uint();
            System.Threading.NativeOverlapped lpOverlapped = new System.Threading.NativeOverlapped();
            bool bRet = DriverManager.IoControl(DriverManager.hDrv, (uint)IOCTL_CODE.GetProcessNum, null, sizeof(uint), IoReturnBuffer, sizeof(uint), ref BytesReturned, ref lpOverlapped);
            ProcNum = IoReturnBuffer[0];
            if (!bRet)
            {
                MessageBox.Show("Failed to get the number of processes!");
                return;
            }

            // R0层获取进程列表
            byte[] bIoRetBuffer = new byte[Marshal.SizeOf(typeof(PROCESSINFO)) * ProcNum];
            bRet = DriverManager.IoControl(DriverManager.hDrv, (uint)IOCTL_CODE.GetProcessList, null, 0, bIoRetBuffer,
                (uint)Marshal.SizeOf(typeof(PROCESSINFO)) * ProcNum,
                ref BytesReturned, ref lpOverlapped);

            // 将驱动传过来的字节流转换成进程信息结构体
            PROCESSINFO[] _procArr = fromBytes(bIoRetBuffer, (int)ProcNum);

            // ListView中添加进程数据
            MainForm.main.ListView_Process.BeginUpdate();
            MainForm.main.ListView_Process.Items.Clear();
            for (int i = 0; i < ProcNum; i++)
            {
                ListViewItem lvi = new ListViewItem();

                // 为进程添加图标
                if (0 < _procArr[i].Path.Length)
                {
                    MainForm.main.imgList_icon_proc.Images.Add(Icon.ExtractAssociatedIcon(_procArr[i].Path));
                }
                lvi.ImageIndex = i - 1; // 通过与imageList绑定，显示imageList中第i项图标

                // 添加Pid
                lvi.SubItems.Add(_procArr[i].Pid.ToString());
                // 添加PPid
                lvi.SubItems.Add(_procArr[i].PPid.ToString());
                // 添加ProcName
                lvi.SubItems.Add(Encoding.ASCII.GetString(_procArr[i].ImageFileName));
                // 添加FullPath
                lvi.SubItems.Add(_procArr[i].Path);
                // 添加Eprocess
                lvi.SubItems.Add("0x" + _procArr[i].EProcess.ToString("X16"));
                // 添加Active
                if (System.Diagnostics.Process.GetProcessById((int)_procArr[i].Pid).Threads[0].ThreadState.Equals(System.Diagnostics.ThreadState.Wait))
                {
                    lvi.SubItems.Add(System.Diagnostics.Process.GetProcessById((int)_procArr[i].Pid).Threads[0].WaitReason.ToString());
                    if (ProcStatus[0].Equals(_procArr[i].Pid) && ProcStatus[1].Equals(0)) 
                    {
                        lvi.ForeColor = Color.Red;
                    }
                }

                MainForm.main.ListView_Process.Items.Add(lvi);
            }

            MainForm.main.ListView_Process.EndUpdate(); // 结束数据处理，UI界面一次性绘制
        }

        /// <summary>
        /// 恢复选中线程
        /// </summary>
        internal static void ResumeThread()
        {
            // 定义用于与驱动通信的变量
            uint BytesReturned = new uint();
            System.Threading.NativeOverlapped lpOverlapped = new System.Threading.NativeOverlapped();

            // 获取当前选中的线程tid
            uint tid;
            uint.TryParse(MainForm.main.ListView_ProcessOther.SelectedItems[0].SubItems[2].Text, out tid);

            // R0层终结指定线程
            bool bRet = DriverManager.IoControl(DriverManager.hDrv, (uint)Common.IOCTL_CODE.ResumeThread, tid, sizeof(uint), null,
                0, ref BytesReturned, ref lpOverlapped);
        }

        /// <summary>
        /// 挂起选中线程
        /// </summary>
        internal static void SuspendThread()
        {
            // 定义用于与驱动通信的变量
            uint BytesReturned = new uint();
            System.Threading.NativeOverlapped lpOverlapped = new System.Threading.NativeOverlapped();

            // 获取当前选中的线程tid
            uint tid;
            uint.TryParse(MainForm.main.ListView_ProcessOther.SelectedItems[0].SubItems[2].Text, out tid);

            // R0层终结指定线程
            bool bRet = DriverManager.IoControl(DriverManager.hDrv, (uint)Common.IOCTL_CODE.SuspendThread, tid, sizeof(uint), null,
                0, ref BytesReturned, ref lpOverlapped);
        }

        /// <summary>
        /// 刷新线程列表
        /// </summary>
        internal static void RefreshThreadsList()
        {
            EnumThreads();
        }

        /// <summary>
        /// 终结指定线程
        /// </summary>
        internal static void TerminateThread()
        {
            // 定义用于与驱动通信的变量
            uint BytesReturned = new uint();
            System.Threading.NativeOverlapped lpOverlapped = new System.Threading.NativeOverlapped();

            // 获取当前选中的线程tid
            uint tid;
            uint.TryParse(MainForm.main.ListView_ProcessOther.SelectedItems[0].SubItems[2].Text, out tid);

            // R0层终结指定线程
            bool bRet = DriverManager.IoControl(DriverManager.hDrv, (uint)Common.IOCTL_CODE.TerminateThread, tid, sizeof(uint), null,
                0, ref BytesReturned, ref lpOverlapped);
        }

        /// <summary>
        /// 卸载指定模块
        /// </summary>
        internal static void UninstallModule()
        {
            // 定义用于与驱动通信的变量
            MODULE_UNINSTALL _moduleUnstall = new MODULE_UNINSTALL();
            uint BytesReturned = new uint();
            System.Threading.NativeOverlapped lpOverlapped = new System.Threading.NativeOverlapped();

            // 填充当前选中模块的信息（模块所在进程的Erpocess和模块基地址）
            _moduleUnstall.Eprocess = Common.CommonVar.currentEprocess;
            _moduleUnstall.Base = Convert.ToUInt64(MainForm.main.ListView_ProcessOther.SelectedItems[0].SubItems[1].Text.Substring(2), 16);

            // 将结构体放到非托管内存中去
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MODULE_UNINSTALL)));
            Marshal.StructureToPtr(_moduleUnstall, ptr, true);

            // R0层卸载指定进程模块
            bool bRet = DriverManager.IoControl(DriverManager.hDrv, (uint)Common.IOCTL_CODE.UninstallModule, ptr, (uint)Marshal.SizeOf(typeof(MODULE_UNINSTALL)), null,
                0, ref BytesReturned, ref lpOverlapped);

            // 需要释放临时非托管区内存
            Marshal.FreeHGlobal(ptr);
        }

        /// <summary>
        /// 刷新进程模块列表
        /// </summary>
        internal static void RefreshModulesList()
        {
            EnumModules();
        }

        /// <summary>
        /// 终止进程
        /// </summary>
        internal static void TerminateProcess()
        {
            // 定义用于与驱动通信的变量
            uint BytesReturned = new uint();
            System.Threading.NativeOverlapped lpOverlapped = new System.Threading.NativeOverlapped();

            // 获取当前选中的进程pid
            uint pid;
            uint.TryParse(MainForm.main.ListView_Process.SelectedItems[0].SubItems[1].Text, out pid);

            // R0层终结指定进程
            bool bRet = DriverManager.IoControl(DriverManager.hDrv, (uint)Common.IOCTL_CODE.TerminateProcess, pid, sizeof(uint), null,
                0, ref BytesReturned, ref lpOverlapped);
        }

        /// <summary>
        /// 恢复进程
        /// </summary>
        internal static void ResumeProcess()
        {
            // 定义用于与驱动通信的变量
            uint[] IoReturnBuffer = new uint[1];
            uint BytesReturned = new uint();
            System.Threading.NativeOverlapped lpOverlapped = new System.Threading.NativeOverlapped();

            // 获取当前选中的进程pid
            uint pid;
            uint.TryParse(MainForm.main.ListView_Process.SelectedItems[0].SubItems[1].Text, out pid);

            // R0层暂停指定进程
            bool bRet = DriverManager.IoControl(DriverManager.hDrv, (uint)IOCTL_CODE.ResumeProcess, pid, sizeof(uint), IoReturnBuffer,
                sizeof(uint), ref BytesReturned, ref lpOverlapped);
            // {[pid],[status]}进程状态(1：正常状态 0：暂停状态)
            ProcStatus[0] = pid;
            ProcStatus[1] = IoReturnBuffer[0];
            if (ProcStatus[1].Equals(1))
            {
                MainForm.main.ListView_Process.SelectedItems[0].ForeColor = Color.Black;
            }
            else
            {
                MainForm.main.ListView_Process.SelectedItems[0].ForeColor = Color.Red;
            }
            
        }

        /// <summary>
        /// 暂停进程
        /// </summary>
        internal static void SuspendProcess()
        {
            // 定义用于与驱动通信的变量
            uint[] IoReturnBuffer = new uint[1];
            uint BytesReturned = new uint();
            System.Threading.NativeOverlapped lpOverlapped = new System.Threading.NativeOverlapped();

            // 获取当前选中的进程pid
            uint pid;
            uint.TryParse(MainForm.main.ListView_Process.SelectedItems[0].SubItems[1].Text, out pid);

            // R0层暂停指定进程
            bool bRet = DriverManager.IoControl(DriverManager.hDrv, (uint)IOCTL_CODE.SuspendProcess, pid, sizeof(uint), IoReturnBuffer,
                sizeof(uint), ref BytesReturned, ref lpOverlapped);
            // {[pid],[status]}进程状态(1：正常状态 0：暂停状态)
            ProcStatus[0] = pid;
            ProcStatus[1] = IoReturnBuffer[0];
            if (ProcStatus[1].Equals(1))
            {
                MainForm.main.ListView_Process.SelectedItems[0].ForeColor = Color.Black;
            }
            else
            {
                MainForm.main.ListView_Process.SelectedItems[0].ForeColor = Color.Red;
            }

        }

        /// <summary>
        /// 枚举进程模块
        /// </summary>
        static public void EnumModules()
        {
            // 定义用于与驱动通信的变量
            byte[] IoReturnBuffer = new byte[Marshal.SizeOf(typeof(MODULEINFO)) * ModuleNum];
            uint BytesReturned = new uint();
            System.Threading.NativeOverlapped lpOverlapped = new System.Threading.NativeOverlapped();

            // 获取当前选中的进程pid
            uint pid;
            uint.TryParse(MainForm.main.ListView_Process.SelectedItems[0].SubItems[1].Text, out pid);
            // 保存当前进程的Eprocess供卸载模块使用
            Common.CommonVar.currentEprocess = Convert.ToUInt64(MainForm.main.ListView_Process.SelectedItems[0].SubItems[5].Text.Substring(2), 16);

            // R0层获取进程的模块信息
            bool bRet = DriverManager.IoControl(DriverManager.hDrv, (uint)IOCTL_CODE.GetProcessModules, pid, sizeof(uint), IoReturnBuffer,
                (uint)Marshal.SizeOf(typeof(MODULEINFO)) * ModuleNum, ref BytesReturned, ref lpOverlapped);

            // 将驱动传过来的字节流转换成进程模块信息结构体
            MODULEINFO[] _moduleArr = new MODULEINFO[ModuleNum];
            fromBytes(IoReturnBuffer, (int)ModuleNum, ref _moduleArr);

            // 如果进程列表框占满tab容器
            MainForm.main.ListView_ProcessOther.Visible = true;
            if (MainForm.isProListViewDock)
            {
                MainForm.main.ListView_Process.Height = MainForm.main.ListView_Process.Height - MainForm.main.ListView_ProcessOther.Height;
                MainForm.isProListViewDock = false;
            }

            // 设置mProcOtherListView的表头
            MainForm.main.ListView_ProcessOther.Columns.Clear();
            MainForm.main.ListView_ProcessOther.Columns.Add("", 0, HorizontalAlignment.Left);
            MainForm.main.ListView_ProcessOther.Columns.Add("Base", 50, HorizontalAlignment.Left);
            MainForm.main.ListView_ProcessOther.Columns.Add("Size", 50, HorizontalAlignment.Left);
            MainForm.main.ListView_ProcessOther.Columns.Add("Company", 50, HorizontalAlignment.Left);
            MainForm.main.ListView_ProcessOther.Columns.Add("Path", 50, HorizontalAlignment.Left);

            // 定位到指定的右键菜单
            MainForm.main.ListView_ProcessOther.ContextMenuStrip = MainForm.main.contextMenuStrip_procModulesList;

            // 将数据插入到mProcOtherListView中
            MainForm.main.ListView_ProcessOther.BeginUpdate();
            MainForm.main.ListView_ProcessOther.Items.Clear();
            MainForm.main.imgList_icon_module.Images.Clear();
            MainForm.main.ListView_ProcessOther.SmallImageList = MainForm.main.imgList_icon_module;
            for (int i = 0; _moduleArr[i].Size != 0; i++)
            {
                ListViewItem lvi = new ListViewItem();

                if (0 < _moduleArr[i].Path.Length)
                {
                    MainForm.main.imgList_icon_module.Images.Add(Icon.ExtractAssociatedIcon(_moduleArr[i].Path));
                }
                lvi.ImageIndex = i;

                lvi.SubItems.Add("0x" + _moduleArr[i].Base.ToString("X16"));
                lvi.SubItems.Add(_moduleArr[i].Size.ToString());
                lvi.SubItems.Add("unknow");
                lvi.SubItems.Add(_moduleArr[i].Path);

                MainForm.main.ListView_ProcessOther.Items.Add(lvi);
            }
            MainForm.main.ListView_ProcessOther.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            MainForm.main.ListView_ProcessOther.EndUpdate();
        }

        /// <summary>
        /// 枚举线程
        /// </summary>
        static public void EnumThreads()
        {
            // 定义用于与驱动通信的变量
            byte[] IoReturnBuffer = new byte[Marshal.SizeOf(typeof(THREADINFO)) * ThreadNum];
            uint BytesReturned = new uint();
            System.Threading.NativeOverlapped lpOverlapped = new System.Threading.NativeOverlapped();
            // 获取当前选中的进程的pid
            uint pid;
            uint.TryParse(MainForm.main.ListView_Process.SelectedItems[0].SubItems[1].Text, out pid);

            // R0层获取进程的模块信息
            bool bRet = DriverManager.IoControl(DriverManager.hDrv, (uint)IOCTL_CODE.GetProcessThreads, pid, sizeof(uint), IoReturnBuffer,
                (uint)Marshal.SizeOf(typeof(THREADINFO)) * ThreadNum, ref BytesReturned, ref lpOverlapped);

            // 将驱动传过来的字节流转换成进程模块信息结构体
            THREADINFO[] _threadArr = new THREADINFO[ThreadNum];
            fromBytes(IoReturnBuffer, (int)ThreadNum, ref _threadArr);

            // 如果进程列表框占满tab容器
            MainForm.main.ListView_ProcessOther.Visible = true;
            if (MainForm.isProListViewDock)
            {
                MainForm.main.ListView_Process.Height = MainForm.main.ListView_Process.Height - MainForm.main.ListView_ProcessOther.Height;
                MainForm.isProListViewDock = false;
            }

            // 枚举线程不需要用到图标
            MainForm.main.ListView_ProcessOther.SmallImageList = null;

            // 设置mProcOtherListView的表头
            MainForm.main.ListView_ProcessOther.Columns.Clear();
            MainForm.main.ListView_ProcessOther.Columns.Add("", 0, HorizontalAlignment.Left);
            MainForm.main.ListView_ProcessOther.Columns.Add("EThread", 50, HorizontalAlignment.Left);
            MainForm.main.ListView_ProcessOther.Columns.Add("Tid", 50, HorizontalAlignment.Left);
            MainForm.main.ListView_ProcessOther.Columns.Add("Priority", 50, HorizontalAlignment.Left);
            MainForm.main.ListView_ProcessOther.Columns.Add("Teb", 50, HorizontalAlignment.Left);
            MainForm.main.ListView_ProcessOther.Columns.Add("Entry", 50, HorizontalAlignment.Left);
            MainForm.main.ListView_ProcessOther.Columns.Add("Switch", 50, HorizontalAlignment.Left);
            MainForm.main.ListView_ProcessOther.Columns.Add("State", 50, HorizontalAlignment.Left);

            // 定位到指定的右键菜单
            MainForm.main.ListView_ProcessOther.ContextMenuStrip = MainForm.main.contextMenuStrip_procThreadList;

            // 将数据插入到mProcOtherListView中
            MainForm.main.ListView_ProcessOther.BeginUpdate();
            MainForm.main.ListView_ProcessOther.Items.Clear();

            foreach (THREADINFO arr in _threadArr)
            {
                if (!arr.EThread.Equals(0))
                {
                    ListViewItem lvi = new ListViewItem();

                    lvi.SubItems.Add("0x" + arr.EThread.ToString("X16"));
                    lvi.SubItems.Add(arr.Tid.ToString());
                    lvi.SubItems.Add(arr.Priority.ToString());
                    lvi.SubItems.Add("0x" + arr.Teb.ToString("X16"));
                    lvi.SubItems.Add("0x" + arr.Entry.ToString("X16"));
                    lvi.SubItems.Add(arr.Switch.ToString());
                    lvi.SubItems.Add(Encoding.ASCII.GetString(arr.State));

                    MainForm.main.ListView_ProcessOther.Items.Add(lvi);
                }
                else
                {
                    break;
                }
            }

            MainForm.main.ListView_ProcessOther.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            MainForm.main.ListView_ProcessOther.EndUpdate();
        }

        /// <summary>
        /// Struct to byte arr 结构体转换成字节数组，用于通讯信息转换
        /// </summary>
        /// <param name="_struct"></param>
        /// <returns></returns>
        static public byte[] getBytes(PROCESSINFO _struct)
        {
            int size = Marshal.SizeOf(_struct);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(_struct, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            // 需要释放临时非托管区内存
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>
        /// Byte arr to struct version 1 字节数组转换成结构体，用于通讯信息转换
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        static public PROCESSINFO fromBytes(byte[] arr)
        {
            PROCESSINFO _struct = new PROCESSINFO();

            int size = Marshal.SizeOf(_struct);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            _struct = (PROCESSINFO)Marshal.PtrToStructure(ptr, _struct.GetType());
            // 需要释放临时非托管区内存
            Marshal.FreeHGlobal(ptr);

            return _struct;
        }

        /// <summary>
        /// Byte arr to struct arr version 2 字节数组转换成结构体，用于通讯信息转换
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="desArrNum"></param>
        /// <returns></returns>
        static public PROCESSINFO[] fromBytes(byte[] arr, int desArrNum)
        {
            PROCESSINFO[] _struct = new PROCESSINFO[desArrNum];
            int size = Marshal.SizeOf(typeof(PROCESSINFO)) * desArrNum;
            IntPtr ptr = Marshal.AllocHGlobal(size);

            for (int i = 0; i < desArrNum; i++)
            {
                Marshal.Copy(arr, i * Marshal.SizeOf(typeof(PROCESSINFO)), ptr, Marshal.SizeOf(typeof(PROCESSINFO)));
                _struct[i] = (PROCESSINFO)Marshal.PtrToStructure(ptr, _struct[i].GetType());
            }

            // 需要释放临时非托管区内存
            Marshal.FreeHGlobal(ptr);
            return _struct;
        }

        /// <summary>
        /// Byte arr to struct arr version 3 字节数组转换成结构体，用于通讯信息转换
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="desArrNum"></param>
        /// <param name="desArr"></param>
        static public void fromBytes(byte[] arr, int desArrNum, ref MODULEINFO[] desArr)
        {
            MODULEINFO[] _struct = new MODULEINFO[desArrNum];
            int size = Marshal.SizeOf(typeof(MODULEINFO)) * desArrNum;
            IntPtr ptr = Marshal.AllocHGlobal(size);

            for (int i = 0; i < desArrNum; i++)
            {
                Marshal.Copy(arr, i * Marshal.SizeOf(typeof(MODULEINFO)), ptr, Marshal.SizeOf(typeof(MODULEINFO)));
                _struct[i] = (MODULEINFO)Marshal.PtrToStructure(ptr, _struct[i].GetType());
            }

            // 需要释放临时非托管区内存
            Marshal.FreeHGlobal(ptr);
            desArr = _struct;
        }

        /// <summary>
        /// Byte arr to struct arr version 4 字节数组转换成结构体，用于通讯信息转换
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="desArrNum"></param>
        /// <param name="desArr"></param>
        static internal void fromBytes(byte[] arr, int desArrNum, ref THREADINFO[] desArr)
        {
            THREADINFO[] _struct = new THREADINFO[desArrNum];
            int size = Marshal.SizeOf(typeof(THREADINFO)) * desArrNum;
            IntPtr ptr = Marshal.AllocHGlobal(size);

            for (int i = 0; i < desArrNum; i++)
            {
                Marshal.Copy(arr, i * Marshal.SizeOf(typeof(THREADINFO)), ptr, Marshal.SizeOf(typeof(THREADINFO)));
                _struct[i] = (THREADINFO)Marshal.PtrToStructure(ptr, _struct[i].GetType());
            }

            // 需要释放临时非托管区内存
            Marshal.FreeHGlobal(ptr);
            desArr = _struct;
        }
    }
}
