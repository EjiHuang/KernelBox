using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// SSDT管理命名空间
/// </summary>
namespace KernelBox.SSDTMgr
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SYSTEM_MODULE_INFORMATION
    {

        /// DWORD->unsigned int
        public uint reserved1;

        /// DWORD->unsigned int
        public uint reserved2;

        /// DWORD->unsigned int
        public uint reserved3;

        /// DWORD->unsigned int
        public uint reserved4;

        /// PVOID->void*
        public IntPtr Base;

        /// ULONG->unsigned int
        public uint Size;

        /// ULONG->unsigned int
        public uint Flags;

        /// USHORT->unsigned short
        public ushort Index;

        /// USHORT->unsigned short
        public ushort NameLength;

        /// USHORT->unsigned short
        public ushort LoadCount;

        /// USHORT->unsigned short
        public ushort ModuleNameOffset;

        /// CHAR[256]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string ImageName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MODULE_LIST
    {
        /// DWORD->unsigned int
        public uint ModuleCount;

        /// SYSTEM_MODULE_INFORMATION[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public SYSTEM_MODULE_INFORMATION[] Modules;
    }

    /// <summary>
    /// SSDT管理类
    /// </summary>
    class cSSDTMgr
    {
        // KiServiceTable地址
        internal static ulong KiServiceTableAddr;


        /// <summary>
        /// 获取SSDT函数索引，参数为Zw函数非Nt
        /// </summary>
        /// <param name="FunctionName"></param>
        /// <returns></returns>
        internal static uint GetSSDTFunctionIndex(string FunctionName)
        {
            IntPtr ptr = NativeApi.GetProcAddress(NativeApi.GetModuleHandle("ntdll.dll"), FunctionName);
            uint index = (uint)Marshal.ReadInt32(ptr + 4);
            return index;
        }

        /// <summary>
        /// 获取KiServiceTable地址
        /// </summary>
        /// <returns></returns>
        internal static ulong GetKiServiceTableAddr()
        {
            // 定义用于与驱动通信的变量
            ulong[] IoReturnBuffer = new ulong[1];
            uint BytesReturned = new uint();
            System.Threading.NativeOverlapped lpOverlapped = new System.Threading.NativeOverlapped();

            // R0层获取KiServiceTable地址
            bool bRet = DriverManager.IoControl(DriverManager.hDrv, (uint)Common.IOCTL_CODE.GetKiSrvTableAddr, null, 0, IoReturnBuffer,
                sizeof(ulong), ref BytesReturned, ref lpOverlapped);
            if (!bRet)
                MessageBox.Show("[x_x]:" + Common.CommonFunction.GetLastError32());
            return IoReturnBuffer[0];
        }

        /// <summary>
        /// 获取SSDT函数的地址
        /// </summary>
        /// <param name="iIndex"></param>
        /// <returns></returns>
        internal static ulong GetSSDTFuncAddr(uint iIndex)
        {
            // 定义用于与驱动通信的变量
            ulong[] IoReturnBuffer = new ulong[1];
            uint BytesReturned = new uint();
            System.Threading.NativeOverlapped lpOverlapped = new System.Threading.NativeOverlapped();

            // R0层获取SSDT函数的地址
            bool bRet = DriverManager.IoControl(DriverManager.hDrv, (uint)Common.IOCTL_CODE.GetFuncAddr, iIndex, sizeof(uint), IoReturnBuffer,
                sizeof(ulong), ref BytesReturned, ref lpOverlapped);
            if (!bRet)
                MessageBox.Show("[x_x]:" + Common.CommonFunction.GetLastError32());
            return IoReturnBuffer[0];
        }

        /// <summary>
        /// 显示SSDT列表
        /// </summary>
        internal static void ShowSSDTList()
        {
            string strFuncOriAddr;
            string strFuncSSDTAddr;
            uint FuncIndex;


            OtherFunction.GetNtoskrnlBase();
            OtherFunction.GetNtoskrnlImageBase();
            OtherFunction.GetNtdllFuncNames();

            MainForm.main.ListView_SSDT.Columns.Clear();
            MainForm.main.ListView_SSDT.Columns.Add("", 0, HorizontalAlignment.Left);

            MainForm.main.ListView_SSDT.Columns.Add("Index", 50, HorizontalAlignment.Left);
            MainForm.main.ListView_SSDT.Columns.Add("Function", 50, HorizontalAlignment.Left);
            MainForm.main.ListView_SSDT.Columns.Add("Current Address", 50, HorizontalAlignment.Left);
            MainForm.main.ListView_SSDT.Columns.Add("Hook Status", 50, HorizontalAlignment.Left);
            MainForm.main.ListView_SSDT.Columns.Add("Original Address", 50, HorizontalAlignment.Left);
            MainForm.main.ListView_SSDT.BeginUpdate();
            MainForm.main.ListView_SSDT.Items.Clear();

            foreach (string v in OtherFunction.arrNtFuncNames)
            {
                ListViewItem lvi = new ListViewItem();

                FuncIndex = cSSDTMgr.GetSSDTFunctionIndex("Zw" + v.Substring(2));

                // 这玩意是个二次跳转，会跳到RtlQuerySystemTime，这里就手动定位索引
                if (v.Equals("NtQuerySystemTime"))
                    FuncIndex = 0x57;

                strFuncSSDTAddr = "0x" + cSSDTMgr.GetSSDTFuncAddr(FuncIndex).ToString("X16");
                strFuncOriAddr = "0x" + OtherFunction.GetSSDTFuncOriginalAddr(FuncIndex).ToString("X16");

                lvi.SubItems.Add(FuncIndex.ToString());
                lvi.SubItems.Add(v);
                lvi.SubItems.Add(strFuncSSDTAddr);

                if (strFuncSSDTAddr.Equals(strFuncOriAddr))
                    lvi.SubItems.Add("Normal");
                else
                    lvi.SubItems.Add("Hooked");

                lvi.SubItems.Add(strFuncOriAddr);

                MainForm.main.ListView_SSDT.Items.Add(lvi);

            }

            MainForm.main.ListView_SSDT.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            MainForm.main.ListView_SSDT.Columns[0].Width = 0;
            MainForm.main.ListView_SSDT.EndUpdate();
        }
    }

    /// <summary>
    /// SSDT其他函数操作类
    /// </summary>
    class OtherFunction
    {
        // ntoskrnl的基址
        internal static IntPtr NtosBase;
        // ntoskrnl的映像基址
        internal static ulong NtosImageBase;
        // 模块名
        internal static string strNtosExeFullPath = "";
        // Ntdll路径
        internal static string strNtdllFullPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\system32\\ntdll.dll";
        // 记录Ntdll函数名称的数组
        internal static string[] arrNtFuncNames;


        /// <summary>
        /// 获取文件的长度
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        internal static long GetFileLength(string FilePath)
        {
            return new FileInfo(FilePath).Length;
        }

        /// <summary>
        /// 读取DLL文件的内容
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        internal static FileStream LoadDllContext(string FilePath)
        {
            return File.OpenRead(FilePath);
        }

        /// <summary>
        /// 获取ntoskrnl的基址
        /// </summary>
        /// <returns></returns>
        internal static bool GetNtoskrnlBase()
        {
            uint uiNeedSize, uiModuleCount, uiBufferSize = 0x5000;
            IntPtr pBuffer = IntPtr.Zero;
            NativeApiEx.NtStatus status;
            MODULE_LIST _ModuleInfo = new MODULE_LIST();

            // 分配内存
            pBuffer = Marshal.AllocHGlobal((IntPtr)uiBufferSize);
            if (pBuffer.Equals(IntPtr.Zero))
                return false;
            // 查询模块信息
            status = NativeApi.NtQuerySystemInformation(
                NativeApiEx.SYSTEM_INFORMATION_CLASS.SystemModuleInformation,
                pBuffer, uiBufferSize, out uiNeedSize);

            if (status.Equals(NativeApiEx.NtStatus.InfoLengthMismatch))
            {
                Marshal.FreeHGlobal(pBuffer);
                try
                {
                    pBuffer = Marshal.AllocHGlobal((int)uiNeedSize);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("[x_x]:" + ex.Message);
                    return false;
                }

                status = NativeApi.NtQuerySystemInformation(
                NativeApiEx.SYSTEM_INFORMATION_CLASS.SystemModuleInformation,
                pBuffer, uiNeedSize, out uiNeedSize);

                if (!status.Equals(NativeApiEx.NtStatus.Success))
                {
                    Marshal.FreeHGlobal(pBuffer);
                    MessageBox.Show("[x_x]:" + Common.CommonFunction.GetLastError32());
                    return false;
                }
            }

            try
            {
                _ModuleInfo = (MODULE_LIST)Marshal.PtrToStructure(pBuffer, _ModuleInfo.GetType());
            }
            catch (Exception ex)
            {
                Marshal.FreeHGlobal(pBuffer);
                MessageBox.Show("[x_x]:" + ex.Message);
                return false;
            }

            // 获取模块总数量
            uiModuleCount = _ModuleInfo.ModuleCount;
            // 获取模块名
            if (strNtosExeFullPath.Equals(""))
            {
                strNtosExeFullPath = _ModuleInfo.Modules[0].ImageName;
                strNtosExeFullPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\system32\\" + Path.GetFileName(strNtosExeFullPath);
            }
            // 获取模块基址
            NtosBase = _ModuleInfo.Modules[0].Base;
            // 释放内存空间
            Marshal.FreeHGlobal(pBuffer);

            return true;
        }

        /// <summary>
        /// Stream转Struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal static T StreamToStruct<T>(Stream stream)
        {
            int iStructSize = Marshal.SizeOf(typeof(T));
            IntPtr pTempMemory = IntPtr.Zero;
            byte[] buffer = new byte[iStructSize];
            int iBytesLength = 0;

            try
            {
                pTempMemory = Marshal.AllocHGlobal(iStructSize);

                if (pTempMemory.Equals(IntPtr.Zero))
                    throw new InvalidOperationException();

                // 从当前流中读取字节序列
                iBytesLength = stream.Read(buffer, 0, iStructSize);
                // 校验
                if (!iBytesLength.Equals(iStructSize))
                    throw new InvalidOperationException();
                // 拷贝字节序列
                Marshal.Copy(buffer, 0, pTempMemory, iStructSize);

                Marshal.FreeHGlobal(pTempMemory);
                return (T)Marshal.PtrToStructure(pTempMemory, typeof(T));
            }
            catch (Exception ex)
            {
                MessageBox.Show("[x_x]:" + ex.Message);
                return default(T);
            }
        }

        /// <summary>
        /// 获取ntoskrnl的映像基址
        /// </summary>
        /// <returns></returns>
        internal static bool GetNtoskrnlImageBase()
        {
            NativeApiEx.IMAGE_NT_HEADERS64 _ImageNtHeader64 = new NativeApiEx.IMAGE_NT_HEADERS64();
            NativeApiEx.IMAGE_DOS_HEADER _ImageDosHeader = new NativeApiEx.IMAGE_DOS_HEADER();

            FileStream NtosFileStream = LoadDllContext(strNtosExeFullPath);
            NtosFileStream.Seek(0, SeekOrigin.Begin);

            // 获取IMAGE_DOS_HEADER结构
            _ImageDosHeader = StreamToStruct<NativeApiEx.IMAGE_DOS_HEADER>(NtosFileStream);
            // 获取IMAGE_NT_HEADERS64结构
            NtosFileStream.Seek(_ImageDosHeader.e_lfanew, SeekOrigin.Begin);
            _ImageNtHeader64 = StreamToStruct<NativeApiEx.IMAGE_NT_HEADERS64>(NtosFileStream);

            NtosImageBase = _ImageNtHeader64.OptionalHeader.ImageBase;

            NtosFileStream.Dispose();
            return true;
        }

        /// <summary>
        /// 获取ntdll.dll中的Zw函数
        /// </summary>
        /// <returns></returns>
        internal static bool GetNtdllFuncNames()
        {
            int iNtFuncStartPos = 0, iNtFuncEndPos = 0;
            byte[] arrTemp1 = Encoding.Default.GetBytes("ZwAcceptConnectPort");
            byte[] arrTemp2 = Encoding.Default.GetBytes("ZwYieldExecution");
            // 将NTDLL.DLL读取到内存流中
            MemoryStream memStream = new MemoryStream();
            LoadDllContext(strNtdllFullPath).CopyTo(memStream);
            // 将内存流转换成字节数组
            byte[] NtdllBuffer = memStream.GetBuffer();
            // 在字节数组中搜索第一个函数和最后一个函数的位置
            iNtFuncStartPos = SearchByteArray(NtdllBuffer, arrTemp1);
            iNtFuncEndPos = SearchByteArray(NtdllBuffer, arrTemp2);
            // 最后将字节数组转换成字符串数组，方便处理
            arrNtFuncNames = ByteArryToStringArray(NtdllBuffer, iNtFuncStartPos, iNtFuncEndPos);
            // 将数组中的Zw函数名改成Nt函数名
            ZwToNt(ref arrNtFuncNames);

            memStream.Dispose();
            return true;
        }

        /// <summary>
        /// 获取SSDT函数的原始地址
        /// </summary>
        /// <param name="iIndex"></param>
        /// <returns></returns>
        internal static ulong GetSSDTFuncOriginalAddr(uint iIndex)
        {
            // 获取ntoskrnl加载进内存的地址
            IntPtr pNtos = NativeApi.LoadLibrary(strNtosExeFullPath);
            // 获取KiServiceTable的地址
            cSSDTMgr.KiServiceTableAddr = cSSDTMgr.GetKiServiceTableAddr();
            // 计算ntoskrnl的相对虚拟地址
            ulong ulRva = cSSDTMgr.KiServiceTableAddr - (ulong)NtosBase.ToInt64();

            ulong ulTemp = (ulong)pNtos.ToInt64() + ulRva + 8 * iIndex;
            ulTemp = (ulong)Marshal.ReadIntPtr((IntPtr)ulTemp);
            
            ulong ulRvaIndex = ulTemp - NtosImageBase; // IMAGE_OPTIONAL_HEADER64.ImageBase = 0x140000000（这个值基本是固定的）

            return ulRvaIndex + (ulong)NtosBase.ToInt64();
        }

        /// <summary>
        /// 字节数组搜索指定字节数组，返回位置
        /// </summary>
        /// <param name="TargetArr"></param>
        /// <param name="NeedleArr"></param>
        /// <returns></returns>
        internal static int SearchByteArray(byte[] TargetArr,byte[] NeedleArr)
        {
            for (int i = 0; i <= TargetArr.Length - NeedleArr.Length; i++)
            {
                if (CheckByteArrayMatch(TargetArr, NeedleArr, i)) 
                {
                    // 如果匹配，就返回索引值
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 寻找字节数组中是否存在匹配的字节数组
        /// </summary>
        /// <param name="TargetArr"></param>
        /// <param name="NeedleArr"></param>
        /// <param name="StartPos"></param>
        /// <returns></returns>
        internal static bool CheckByteArrayMatch(byte[] TargetArr, byte[] NeedleArr, int StartPos)
        {
            if (NeedleArr.Length + StartPos > TargetArr.Length) 
            {
                return false;
            }
            else
            {
                for (int i = 0; i < NeedleArr.Length; i++)
                {
                    if (NeedleArr[i] != TargetArr[i + StartPos])
                    {
                        return false;
                    }
                }
                // 全部匹配了，这里才返回true
                return true;
            }
        }

        /// <summary>
        /// 字节数组转换成字符串数组
        /// </summary>
        /// <param name="TargetArr"></param>
        /// <param name="StartPos"></param>
        /// <param name="EndPos"></param>
        /// <returns></returns>
        internal static string[] ByteArryToStringArray(byte[] TargetArr, int StartPos, int EndPos)
        {
            string[] arr = Encoding.Default.GetString(TargetArr, StartPos, EndPos - StartPos - 1).
                Split(new char[] { '\0' }, StringSplitOptions.None);
            return arr;
        }

        /// <summary>
        /// 将数组中的Zw函数名改成Nt函数名
        /// </summary>
        /// <param name="arr"></param>
        internal static void ZwToNt(ref string[] arr)
        {
            int i = 0;
            foreach (string v in arr)
            {
                arr[i] = "Nt" + v.Substring(2);
                i++;
            }
        }
    }
}
