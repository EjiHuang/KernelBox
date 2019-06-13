using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 这个是公用命名空间，主要处理公共函数事务
/// </summary>
namespace KernelBox.Common
{
    /// <summary>
    /// 通信控制码
    /// </summary>
    public enum IOCTL_CODE : uint
    {
        GetFuncAddr         = 0x801,
        GetKiSrvTableAddr   = 0x802,
        GetProcessNum       = 0x803,
        GetProcessList      = 0x804,
        GetProcessModules   = 0x805,
        GetProcessThreads   = 0x806,
        SuspendProcess      = 0x807,
        ResumeProcess       = 0x808,
        TerminateProcess    = 0x809,
        UninstallModule     = 0x810,
        TerminateThread     = 0x811,
        SuspendThread       = 0x812,
        ResumeThread        = 0x813
    };

    /// <summary>
    /// 公共类，记录公共方法
    /// </summary>
    class CommonFunction
    {
        /// <summary>
        /// 获取最后一次的错误信息
        /// </summary>
        /// <returns></returns>
        public static string GetLastError32()
        {
            uint errcode = NativeApi.GetLastError();
            IntPtr lpBuffer = Marshal.AllocHGlobal(4096);
            NativeApi.FormatMessage(0x00001000, (IntPtr)0, errcode, 0, lpBuffer, 1024, (IntPtr)0);
            string ret = Marshal.PtrToStringAnsi(lpBuffer);
            Marshal.FreeHGlobal(lpBuffer);
            return ret;
        }
    }

    /// <summary>
    /// 公共类，记录公共变量
    /// </summary>
    class CommonVar
    {
        /*cProcessMgr*/
        // 当前选中进程的EPROCESS
        public static ulong currentEprocess;

        /*cFile*/
        // 判断文件类是否已经初始化成功
        public static bool gIsCFileInited = false;
        // 保存当前窗口的句柄
        public static IntPtr mainHandle;
        // 保存当前选中的节点
        public static System.Windows.Forms.TreeNode gCurrentNode_File;
        // 保存CD-ROM驱动器判断结果
        public static bool gIsCDROM = true;
        // 保存文件路径
        public static string gStrFilePath;
        // 判断是否为双击打开文件夹方式，防止treeview选中触发选中后事件，从而造成listview 2次处理
        public static bool gIsDoubleClickOpen;

        /*cRegistry*/
        // 保存当前选中的节点
        public static System.Windows.Forms.TreeNode gCurrentNode_Reg;
    }
}
