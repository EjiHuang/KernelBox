#include <ntddk.h>
#include <ntstrsafe.h>

#include "FuncEx.h"
#include "SSDT.h"

// ͨ���ýṹ��
typedef struct _PROCESS_INFO
{
	ULONG64 EProcess;		// ����eprocess��ַ
	ULONG32 Pid;			// ����Pid
	ULONG32 PPid;			// ���̸�����Pid
	CHAR ImageFileName[60];	// ������
	WCHAR Path[512];		// ����·��
}PROCESSINFO, *PPROCESSINFO;	// ������Ϣ

typedef struct _MODULE_INFO
{
	ULONG64 Base;			// ģ�����ַ
	ULONG32 Size;			// ģ���С
	WCHAR	Path[1024];		// ģ��·��
}MODULEINFO, *PMODULEINFO;		// ����ģ����Ϣ

typedef struct _THREAD_INFO
{
	ULONG64 ETHREAD;		// �߳�ethread��ַ
	ULONG32 TID;			// �߳�ID
	ULONG32 PRIORITY;		// �߳����ȼ�
	ULONG64 TEB;			// �߳�Teb��ַ
	ULONG64 ENTRY;			// �߳���ڵ�ַ
	ULONG64 SWITCH;			// �߳��л�����
	CHAR STATE[20];			// �߳�״̬
}THREADINFO, *PTHREADINFO;		// �����߳���Ϣ

typedef struct _MODULE_UNINSTALL
{
	ULONG64 Eprocess;		// ����eprocess��ַ
	ULONG64 Base;			// ģ�����ַ
}MODULEUNINSTALL, *PMODULEUNINSTALL;


// ������Ϣ&������Ŀ
PPROCESSINFO g_pProcessInfo;
ULONG32 g_dwProcNum;
// ����״̬(1������״̬ 0����ͣ״̬)
ULONG32 g_dwProcStatus;
// ģ����Ϣ&ģ����Ŀ&ģ��ж����Ϣ
PMODULEINFO g_pModuleInfo;
static ULONG32 g_dwModuleNum = 512;
PMODULEUNINSTALL g_pModulelUninstall;
// �߳���Ϣ&�߳���Ŀ
PTHREADINFO g_pThreadInfo;
static ULONG32 g_dwThreadNum = 256;
// PsSuspendThread��ַ
static ULONG64 g_qwPsSuspendThreadAddr = 0;
// PsResumeThread��ַ
static ULONG64 g_qwPsResumeThreadAddr = 0;

// ��ȡ�����б�
NTSTATUS GetProcessList();
// ��ȡ������Ŀ
ULONG32 GetProcessNum();
// ͨ��Pid����ȡEPROCESS
PEPROCESS LookupProcess(__in HANDLE Pid);
// ͨ��EPROCESS����ȡ����·��
PUNICODE_STRING GetProcessFullImageFileName(__in PEPROCESS pep);
// ͨ��Pid����ȡ���̵�����
VOID GetProcessNameByPid(IN ULONG32 pid, IN PCHAR pszProcName);
// ͨ������·������ȡ��������
VOID GetProcessNameByPath(IN PUNICODE_STRING pusProcPath, OUT PCHAR pszProcName);
// ��ȡ�������̷�
BOOLEAN GetDiskSym(IN PUNICODE_STRING usFilePath);
//ȡ�ַ��������ַ���(startIndexΪ��ʼλ��, stopIndexΪ��ֹλ��, ����stopIndexλ�õ��ַ�)   
VOID KStr_Sub(IN PUNICODE_STRING str, IN USHORT startIndex, IN USHORT stopIndex, OUT PUNICODE_STRING str_result);
// Volume Device Path To Dos Path
VOID VolumeDevicePathToDosPath(IN PWCHAR pwzDesStr);
//WCHAR*ת��Ϊ CHAR*
//������ַ����׵�ַ�����խ�ַ�����BUFFER ��Ҫ�Ѿ�����ÿռ�
VOID WcharToChar(__in PWCHAR src, __out PCHAR dst);
// ö�ٽ���ģ��
NTSTATUS EnumModule(__in HANDLE Pid);
// �����߳�ID����ETHREAD
PETHREAD LookupThread(__in HANDLE Tid);
// ö�ٽ��̵��߳�
NTSTATUS EnumThread(__in HANDLE Pid);
// ��������
NTSTATUS TerminateProcess(__in ULONG32 Pid);
// ��ȡ�߳�״̬
PCHAR GetThreadState(__in THREAD_STATE threadState);
// ��ȡ�߳̾��
NTSTATUS GetThreadHandle(__in PETHREAD pet, __out PHANDLE phThread);
// ��ȡ�߳̾������֪��ʲôԭ���ʧ��
HANDLE OpenThread(__in HANDLE tid);
// ��ȡ�̵߳������л������͵�ǰ״̬
NTSTATUS GetThreadSwitchAndState(__in HANDLE Pid, __in HANDLE Tid, __out ULONG32 *dwSwitchCount, __out CHAR *szThreadState);
// ��ͣ����
NTSTATUS SuspendProcess(__in ULONG32 pid);
// �ָ�����
NTSTATUS ResumeProcess(__in ULONG32 pid);
// ж��ָ��ģ��
NTSTATUS UninstallModule();
// �ս�ָ���߳�
BOOLEAN TerminateThread(__in ULONG64 tid);
NTSTATUS TerminateThread64(__in ULONG64 tid);	// ��������TA���
// APC��ֹ�߳�����
VOID KernelTerminateThreadRoutine(
	IN struct _KAPC *Apc,
	IN OUT PKNORMAL_ROUTINE *NormalRoutine,
	IN OUT PVOID *NormalContext,
	IN OUT PVOID *SystemArgument1,
	IN OUT PVOID *SystemArgument2
);
// ��ȡ�̱߳�־ƫ��
ULONG64 GetThreadFlagsOffset();
// ��ȡ������ַ
PVOID GetFunctionAddr(__in PCWSTR FunctionName);
// �����߳�
NTSTATUS SuspendThread(__in ULONG32 tid);
// �ָ��߳�
NTSTATUS ResumeThread(__in ULONG32 tid);