#include <ntddk.h>
#include <ntstrsafe.h>

#include "FuncEx.h"
#include "SSDT.h"

// 通信用结构体
typedef struct _PROCESS_INFO
{
	ULONG64 EProcess;		// 进程eprocess地址
	ULONG32 Pid;			// 进程Pid
	ULONG32 PPid;			// 进程父进程Pid
	CHAR ImageFileName[60];	// 进程名
	WCHAR Path[512];		// 进程路径
}PROCESSINFO, *PPROCESSINFO;	// 进程信息

typedef struct _MODULE_INFO
{
	ULONG64 Base;			// 模块基地址
	ULONG32 Size;			// 模块大小
	WCHAR	Path[1024];		// 模块路径
}MODULEINFO, *PMODULEINFO;		// 进程模块信息

typedef struct _THREAD_INFO
{
	ULONG64 ETHREAD;		// 线程ethread地址
	ULONG32 TID;			// 线程ID
	ULONG32 PRIORITY;		// 线程优先级
	ULONG64 TEB;			// 线程Teb地址
	ULONG64 ENTRY;			// 线程入口地址
	ULONG64 SWITCH;			// 线程切换次数
	CHAR STATE[20];			// 线程状态
}THREADINFO, *PTHREADINFO;		// 进程线程信息

typedef struct _MODULE_UNINSTALL
{
	ULONG64 Eprocess;		// 进程eprocess地址
	ULONG64 Base;			// 模块基地址
}MODULEUNINSTALL, *PMODULEUNINSTALL;


// 进程信息&进程数目
PPROCESSINFO g_pProcessInfo;
ULONG32 g_dwProcNum;
// 进程状态(1：正常状态 0：暂停状态)
ULONG32 g_dwProcStatus;
// 模块信息&模块数目&模块卸载信息
PMODULEINFO g_pModuleInfo;
static ULONG32 g_dwModuleNum = 512;
PMODULEUNINSTALL g_pModulelUninstall;
// 线程信息&线程数目
PTHREADINFO g_pThreadInfo;
static ULONG32 g_dwThreadNum = 256;
// PsSuspendThread地址
static ULONG64 g_qwPsSuspendThreadAddr = 0;
// PsResumeThread地址
static ULONG64 g_qwPsResumeThreadAddr = 0;

// 获取进程列表
NTSTATUS GetProcessList();
// 获取进程数目
ULONG32 GetProcessNum();
// 通过Pid来获取EPROCESS
PEPROCESS LookupProcess(__in HANDLE Pid);
// 通过EPROCESS来获取完整路径
PUNICODE_STRING GetProcessFullImageFileName(__in PEPROCESS pep);
// 通过Pid来获取进程的名称
VOID GetProcessNameByPid(IN ULONG32 pid, IN PCHAR pszProcName);
// 通过进程路径来获取进程名称
VOID GetProcessNameByPath(IN PUNICODE_STRING pusProcPath, OUT PCHAR pszProcName);
// 获取驱动器盘符
BOOLEAN GetDiskSym(IN PUNICODE_STRING usFilePath);
//取字符串的子字符串(startIndex为起始位置, stopIndex为终止位置, 包含stopIndex位置的字符)   
VOID KStr_Sub(IN PUNICODE_STRING str, IN USHORT startIndex, IN USHORT stopIndex, OUT PUNICODE_STRING str_result);
// Volume Device Path To Dos Path
VOID VolumeDevicePathToDosPath(IN PWCHAR pwzDesStr);
//WCHAR*转换为 CHAR*
//输入宽字符串首地址，输出窄字符串，BUFFER 需要已经分配好空间
VOID WcharToChar(__in PWCHAR src, __out PCHAR dst);
// 枚举进程模块
NTSTATUS EnumModule(__in HANDLE Pid);
// 根据线程ID返回ETHREAD
PETHREAD LookupThread(__in HANDLE Tid);
// 枚举进程的线程
NTSTATUS EnumThread(__in HANDLE Pid);
// 结束进程
NTSTATUS TerminateProcess(__in ULONG32 Pid);
// 获取线程状态
PCHAR GetThreadState(__in THREAD_STATE threadState);
// 获取线程句柄
NTSTATUS GetThreadHandle(__in PETHREAD pet, __out PHANDLE phThread);
// 获取线程句柄，不知道什么原因会失败
HANDLE OpenThread(__in HANDLE tid);
// 获取线程的内文切换次数和当前状态
NTSTATUS GetThreadSwitchAndState(__in HANDLE Pid, __in HANDLE Tid, __out ULONG32 *dwSwitchCount, __out CHAR *szThreadState);
// 暂停进程
NTSTATUS SuspendProcess(__in ULONG32 pid);
// 恢复进程
NTSTATUS ResumeProcess(__in ULONG32 pid);
// 卸载指定模块
NTSTATUS UninstallModule();
// 终结指定线程
BOOLEAN TerminateThread(__in ULONG64 tid);
NTSTATUS TerminateThread64(__in ULONG64 tid);	// 方法来自TA大大
// APC终止线程例程
VOID KernelTerminateThreadRoutine(
	IN struct _KAPC *Apc,
	IN OUT PKNORMAL_ROUTINE *NormalRoutine,
	IN OUT PVOID *NormalContext,
	IN OUT PVOID *SystemArgument1,
	IN OUT PVOID *SystemArgument2
);
// 获取线程标志偏移
ULONG64 GetThreadFlagsOffset();
// 获取函数地址
PVOID GetFunctionAddr(__in PCWSTR FunctionName);
// 挂起线程
NTSTATUS SuspendThread(__in ULONG32 tid);
// 恢复线程
NTSTATUS ResumeThread(__in ULONG32 tid);