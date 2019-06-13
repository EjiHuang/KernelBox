using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.ServiceProcess;
/*
    .Net driver manager class template
    Version：1.0
    Autor：JerryAJ
*/
namespace KernelBox
{
    /// <summary>
    /// 驱动管理类
    /// </summary>
    public class DriverManager
    {
        // 驱动文件的名称
        public static string g_strSysName = "MainDriver.sys";
        // 驱动文件的路径
        public static string g_strSysPath = Directory.GetCurrentDirectory() + "\\" + g_strSysName;
        // 驱动符号链接名称
        public static string g_strSysLinkName = "\\\\.\\AJ_Driver"; //格式：\\\\.\\xxoo
        // 驱动服务名称
        public static string g_strSysSvcLinkName = "AJ_Driver";
        // 驱动句柄
        public static SafeFileHandle hDrv;
        // SCM句柄
        private IntPtr hSCManager;
        // 驱动服务句柄
        private IntPtr hService;

        /// <summary>
        /// 获取驱动服务句柄
        /// </summary>
        /// <returns>bool</returns>
        public bool GetSvcHandle()
        {
            hSCManager = NativeApi.OpenSCManager(null, null, (uint)NativeApiEx.SCM_ACCESS.SC_MANAGER_ALL_ACCESS);
            if (IntPtr.Zero == hSCManager)
                return false;
            hService = NativeApi.OpenService(hSCManager, g_strSysSvcLinkName, (uint)NativeApiEx.SERVICE_ACCESS.SERVICE_ALL_ACCESS);
            if (IntPtr.Zero == hService) 
            {
                NativeApi.CloseServiceHandle(hService);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 安装驱动服务
        /// </summary>
        /// <returns>bool</returns>
        public bool Install()
        {
            hSCManager = NativeApi.OpenSCManager(null, null, (uint)NativeApiEx.SCM_ACCESS.SC_MANAGER_ALL_ACCESS);
            if (IntPtr.Zero == hSCManager)
                return false;
            hService = NativeApi.CreateService(hSCManager, g_strSysSvcLinkName, g_strSysSvcLinkName,
                (uint)NativeApiEx.SERVICE_ACCESS.SERVICE_ALL_ACCESS, (uint)NativeApiEx.SERVICE_TYPE.SERVICE_KERNEL_DRIVER,
                (uint)NativeApiEx.SERVICE_START.SERVICE_DEMAND_START, (uint)NativeApiEx.SERVICE_ERROR.SERVICE_ERROR_NORMAL,
                g_strSysPath, null, null, null, null, null);

            if (IntPtr.Zero == hService)
            {
                NativeApi.GetLastError();
                NativeApi.CloseServiceHandle(hService);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 启动驱动服务
        /// </summary>
        /// <returns>bool</returns>
        public bool Start()
        {
            if (!NativeApi.StartService(hService, 0x0, null))
                return false;
            return true;
        }

        /// <summary>
        /// 停止驱动服务
        /// </summary>
        /// <returns>bool</returns>
        public bool Stop()
        {
            try
            {
                ServiceController service = new ServiceController(g_strSysSvcLinkName);
                if (service.Status == ServiceControllerStatus.Stopped)
                    return true;
                else
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(1000 * 10);
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            return true;
        }

        /// <summary>
        /// 卸载驱动
        /// </summary>
        /// <returns>bool</returns>
        public bool Remove()
        {
            if (!NativeApi.DeleteService(hService))
                return false;
            else
            {
                NativeApi.CloseServiceHandle(hService);
                NativeApi.CloseServiceHandle(hSCManager);
            }
            return true;
        }

        /// <summary>
        /// 打开当前驱动
        /// </summary>
        /// <param name="strLinkName">驱动符号链接名称</param>
        /// <returns>SafeFileHandle</returns>
        public SafeFileHandle OpenDriver(string strLinkName)
        {
            return NativeApi.CreateFile(strLinkName, FileAccess.ReadWrite, FileShare.None, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
        }

        /// <summary>
        /// 驱动通信的IO控制器方法
        /// </summary>
        /// <param name="hDriver">驱动句柄</param>
        /// <param name="nIoCode">IO控制码</param>
        /// <param name="InBuffer">传入缓冲区</param>
        /// <param name="nInBufferSize">传入缓冲区大小</param>
        /// <param name="OutBuffer">输出缓冲区</param>
        /// <param name="nOutBufferSize">输出缓冲区大小</param>
        /// <param name="pBytesReturned">返回字节数</param>
        /// <param name="Overlapped">输入输出的信息的结构体</param>
        /// <returns>bool</returns>
        public static bool IoControl(SafeFileHandle hDriver, uint nIoCode, object InBuffer, uint nInBufferSize, object OutBuffer, uint nOutBufferSize, ref uint pBytesReturned, ref System.Threading.NativeOverlapped Overlapped)
        {
            const uint FILE_ANY_ACCESS = 0;
            const uint METHOD_BUFFERED = 0;
            bool bRet;
            nIoCode = ((int)NativeApiEx.DEVICE_TYPE.FILE_DEVICE_UNKNOWN * 65536) | (FILE_ANY_ACCESS * 16384) | (nIoCode * 4) | METHOD_BUFFERED;
            bRet = NativeApi.DeviceIoControl(hDriver, nIoCode, InBuffer, nInBufferSize, OutBuffer, nOutBufferSize, ref pBytesReturned, ref Overlapped);
            return bRet;
        }

        /// <summary>
        /// 初始化驱动
        /// </summary>
        /// <returns>bool</returns>
        public bool InitializeDriver()
        {
            bool bRet = false;
            hDrv = OpenDriver(g_strSysLinkName);
            NativeApi.GetLastError();
            if (!hDrv.IsInvalid)
                bRet = GetSvcHandle();
            else
            {
                bRet = Install();
                if (!bRet)
                    return bRet;
                else
                {
                    bRet = Start();
                    hDrv = OpenDriver(g_strSysLinkName);
                }
            }
            return bRet;
        }

        /// <summary>
        /// 卸载驱动
        /// </summary>
        /// <returns>bool</returns>
        public bool RemoveDrvier()
        {
            hDrv.Close();
            hDrv.Dispose();
            hDrv = null;

            bool bRet = false;
            bRet = Stop();
            if (!bRet)
                return bRet;

            bRet = Remove();

            return bRet;
        }

    }
}
