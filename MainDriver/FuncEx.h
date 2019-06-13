
#if _MSC_VER > 1000
#pragma once
#endif

// ��������
#define PS_CROSS_THREAD_FLAGS_SYSTEM 0x00000010UL

// ϵͳö�����Ͷ���
typedef enum _SYSTEM_INFORMATION_CLASS
{
	SystemBasicInformation = 0,
	SystemPerformanceInformation = 2,
	SystemTimeOfDayInformation = 3,
	SystemProcessInformation = 5,
	SystemProcessorPerformanceInformation = 8,
	SystemHandleInformation = 16,
	SystemPagefileInformation = 18,
	/* There are a lot more of these... */
} SYSTEM_INFORMATION_CLASS;

typedef enum
{
	StateInitialized,
	StateReady,
	StateRunning,
	StateStandby,
	StateTerminated,
	StateWait,
	StateTransition,
	StateUnknown,
} THREAD_STATE;

// ϵͳ�ṹ��
typedef struct _KAPC_STATE
{
	LIST_ENTRY ApcListHead[2];
	PKPROCESS Process;
	UCHAR KernelApcInProgress;
	UCHAR KernelApcPending;
	UCHAR UserApcPending;
} KAPC_STATE, *PKAPC_STATE;

typedef struct _LDR_DATA_TABLE_ENTRY
{
	LIST_ENTRY64	InLoadOrderLinks;
	LIST_ENTRY64	InMemoryOrderLinks;
	LIST_ENTRY64	InInitializationOrderLinks;
	PVOID			DllBase;
	PVOID			EntryPoint;
	ULONG			SizeOfImage;
	UNICODE_STRING	FullDllName;
	UNICODE_STRING 	BaseDllName;
	ULONG			Flags;
	USHORT			LoadCount;
	USHORT			TlsIndex;
	PVOID			SectionPointer;
	ULONG			CheckSum;
	PVOID			LoadedImports;
	PVOID			EntryPointActivationContext;
	PVOID			PatchInformation;
	LIST_ENTRY64	ForwarderLinks;
	LIST_ENTRY64	ServiceTagLinks;
	LIST_ENTRY64	StaticLinks;
	PVOID			ContextInformation;
	ULONG64			OriginalBase;
	LARGE_INTEGER	LoadTime;
} LDR_DATA_TABLE_ENTRY, *PLDR_DATA_TABLE_ENTRY;

/* Checked on 64 bit. */
typedef struct _SYSTEM_THREADS
{
	LARGE_INTEGER KernelTime;	// CPU�ں�ģʽʹ��ʱ��
	LARGE_INTEGER UserTime;		// CPU�û�ģʽʹ��ʱ��
	LARGE_INTEGER CreateTime;	// �̴߳���ʱ��
	ULONG WaitTime;				// �ȴ�ʱ��
	PVOID StartAddress;			// �߳̿�ʼ�������ַ
	CLIENT_ID ClientId;			// �̱߳�ʶ��tid
	KPRIORITY Priority;			// �߳����ȼ�
	KPRIORITY BasePriority;		// �������ȼ�
	ULONG ContextSwitchCount;	// �����л���Ŀ
	THREAD_STATE State;			// ��ǰ״̬
	KWAIT_REASON WaitReason;	// �ȴ�ԭ��
	DWORD Reserved;				// ����
} SYSTEM_THREADS, *PSYSTEM_THREADS;

/* Checked on 64 bit. */
typedef struct _SYSTEM_PROCESS_INFORMATION
{
	ULONG NextEntryOffset;					// ���ɽṹ���е�ƫ����
	ULONG NumberOfThreads;					// �߳���Ŀ
	ULONG Reserved1[6];						
	LARGE_INTEGER CreateTime;				// ����ʱ��
	LARGE_INTEGER UserTime;					// �û�ģʽ��ring 3����CPUʱ��
	LARGE_INTEGER KernelTime;				// �ں�ģʽ��ring 0����CPUʱ��
	UNICODE_STRING ImageName;				// ��������
	KPRIORITY BasePriority;					// ��������Ȩ
	HANDLE UniqueProcessId;					// ���̱�ʶ��PID
	HANDLE InheritedFromUniqueProcessId;	// �����̱�ʶ��PPID
	ULONG HandleCount;						// �����Ŀ
	ULONG SessionId;
	ULONG PageDirectoryBase;
	VM_COUNTERS VirtualMemoryCounters;		// ����洢���ṹ
	SIZE_T PrivatePageCount;
	IO_COUNTERS IoCounters;					// IO�����ṹ
	SYSTEM_THREADS Threads[1];				// ��������̵߳Ľṹ����
} SYSTEM_PROCESS_INFORMATION, *PSYSTEM_PROCESS_INFORMATION;

// APC environments.
typedef enum _KAPC_ENVIRONMENT {
	OriginalApcEnvironment,		// ԭʼ�Ľ��̻���
	AttachedApcEnvironment,		// �ҿ���Ľ��̻���
	CurrentApcEnvironment,		// ��ǰ����
	InsertApcEnvironment		// ������ʱ�Ļ���
} KAPC_ENVIRONMENT;

typedef
VOID
(*PKNORMAL_ROUTINE) (
	IN PVOID NormalContext,
	IN PVOID SystemArgument1,
	IN PVOID SystemArgument2
	);

typedef VOID
(*PKKERNEL_ROUTINE) (
	IN struct _KAPC *Apc,
	IN OUT PKNORMAL_ROUTINE *NormalRoutine,
	IN OUT PVOID *NormalContext,
	IN OUT PVOID *SystemArgument1,
	IN OUT PVOID *SystemArgument2
	);

typedef
VOID
(*PKRUNDOWN_ROUTINE) (
	IN struct _KAPC *Apc
	);

// PspTerminateThreadByPointer��������
typedef NTSTATUS(__fastcall *PSPTERMINATETHREADBYPOINTER)
(
	IN PETHREAD Thread,
	IN NTSTATUS ExitStatus,
	IN BOOLEAN DirectTerminate
	);


// �ں�API����
NTKERNELAPI UCHAR *PsGetProcessImageFileName(
	__in PEPROCESS Process
);

NTKERNELAPI NTSTATUS PsLookupProcessByProcessId(
	__in HANDLE ProcessId, 
	__deref_out PEPROCESS *Process
);

NTKERNELAPI NTSTATUS PsLookupThreadByThreadId(
	__in HANDLE ThreadId, 
	__deref_out PETHREAD *Thread
);

NTKERNELAPI HANDLE PsGetProcessInheritedFromUniqueProcessId(
	__in PEPROCESS Process
);

NTSYSAPI NTSTATUS NTAPI ZwQueryInformationProcess(
	__in HANDLE ProcessHandle, 
	__in PROCESSINFOCLASS ProcessInformationClass, 
	__out_bcount(ProcessInformationLength) PVOID ProcessInformation, 
	__in ULONG ProcessInformationLength, 
	__out_opt PULONG ReturnLength
);

NTSYSAPI NTSTATUS NTAPI ZwQueryInformationThread(
	__in HANDLE ThreadHandle, 
	__in THREADINFOCLASS ThreadInformationClass, 
	__out_bcount(ThreadInformationLength) PVOID ThreadInformation, 
	__in ULONG ThreadInformationLength, 
	__out_opt PULONG ReturnLength
);

NTKERNELAPI PPEB PsGetProcessPeb(
	__in PEPROCESS Process
);

NTKERNELAPI VOID KeStackAttachProcess(
	__inout PEPROCESS Process, 
	__out PKAPC_STATE ApcState
);

NTKERNELAPI VOID KeUnstackDetachProcess(
	__in PKAPC_STATE ApcState
);

NTKERNELAPI PEPROCESS IoThreadToProcess(
	IN PETHREAD Thread
);

NTKERNELAPI PVOID PsGetThreadTeb(
	__in PETHREAD Thread
);

NTSYSAPI NTSTATUS NTAPI ZwQuerySystemInformation(
	__in SYSTEM_INFORMATION_CLASS SystemInformationClass, 
	__out_bcount_opt(SystemInformationLength) PVOID SystemInformation, 
	__in ULONG SystemInformationLength, 
	__out_opt PULONG ReturnLength
);

NTKERNELAPI NTSTATUS ObOpenObjectByPointer(
	__in PVOID Object, 
	__in ULONG HandleAttributes, 
	__in_opt PACCESS_STATE PassedAccessState, 
	__in ACCESS_MASK DesiredAccess, 
	__in_opt POBJECT_TYPE ObjectType, 
	__in KPROCESSOR_MODE AccessMode, 
	__out PHANDLE Handle
);

NTKERNELAPI NTSTATUS PsSuspendProcess(
	IN PEPROCESS Process
);

NTKERNELAPI NTSTATUS PsResumeProcess(
	IN PEPROCESS Process
);

NTKERNELAPI NTSTATUS MmUnmapViewOfSection(
	__in PEPROCESS Process, 
	__in PVOID BaseAddress
);

NTKERNELAPI VOID KeInitializeApc(
	__out PRKAPC Apc, 
	__in PRKTHREAD Thread, 
	__in KAPC_ENVIRONMENT Environment, 
	__in PKKERNEL_ROUTINE KernelRoutine, 
	__in_opt PKRUNDOWN_ROUTINE RundownRoutine, 
	__in_opt PKNORMAL_ROUTINE NormalRoutine, 
	__in_opt KPROCESSOR_MODE ProcessorMode, 
	__in_opt PVOID NormalContext
);

NTKERNELAPI BOOLEAN KeInsertQueueApc(
	__inout PRKAPC Apc, 
	__in_opt PVOID SystemArgument1, 
	__in_opt PVOID SystemArgument2, 
	__in KPRIORITY Increment
);

NTKERNELAPI BOOLEAN PsIsThreadTerminating(
	__in PETHREAD Thread
);

//NTSTATUS PsSuspendThread(
//	IN PETHREAD Thread,
//	OUT PULONG PreviousSuspendCount OPTIONAL
//);
//
//NTSTATUS PsResumeThread(
//	IN PETHREAD Thread,
//	OUT PULONG PreviousSuspendCount OPTIONAL
//);