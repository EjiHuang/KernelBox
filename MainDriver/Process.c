#include "Process.h"
#include <windef.h>

// 定义函数类型
typedef NTSTATUS(__stdcall *PSSUSPENDTHREAD)(
	IN PETHREAD Thread,
	OUT PULONG PreviousSuspendCount OPTIONAL
	);

typedef NTSTATUS(__stdcall *PSRESUMETHREAD)(
	IN PETHREAD Thread,
	OUT PULONG PreviousSuspendCount OPTIONAL
	);

PSPTERMINATETHREADBYPOINTER PspTerminateThreadByPointer = NULL;

// 获取进程列表
NTSTATUS GetProcessList()
{
	NTSTATUS status = STATUS_SUCCESS;
	PEPROCESS pep = NULL;
	ULONG index = 0;

	// 根据进程数目来分配适应大小的内存空间给用于通信的PROCESSINFO结构体
	if (!(g_pProcessInfo = (PPROCESSINFO)ExAllocatePoolWithTag(PagedPool, sizeof(PROCESSINFO) * g_dwProcNum, 'pPI')))
		return STATUS_UNSUCCESSFUL;
	// 分配成功后，对结构体进行内存清零操作
	RtlZeroMemory(g_pProcessInfo, sizeof(PROCESSINFO) * g_dwProcNum);

	// 循环遍历进程信息
	for (size_t i = 4; i < 262144; i+=4)
	{
		pep = LookupProcess((HANDLE)i);
		if (NULL != pep)
		{
			// 退出时间不为0或者句柄表为空，都表示进程已经退出
			if (*(PULONG)((PUCHAR)pep + 0x200))
			{
				// 结构体填充数据
				g_pProcessInfo[index].EProcess = (ULONG64)pep;
				g_pProcessInfo[index].Pid = (ULONG32)PsGetProcessId(pep);
				g_pProcessInfo[index].PPid = (ULONG32)PsGetProcessInheritedFromUniqueProcessId(pep);
				// 通过进程全路径获取进程名
				GetProcessNameByPath(GetProcessFullImageFileName(pep), &g_pProcessInfo[index].ImageFileName);
				// 进程全路径获取
				RtlCopyMemory(g_pProcessInfo[index].Path, GetProcessFullImageFileName(pep)->Buffer, GetProcessFullImageFileName(pep)->Length);
				// 获取的进程路径是设备路径，所以我们需要将他转换为DOS路径
				VolumeDevicePathToDosPath(&g_pProcessInfo[index].Path);
				++index;
			}
			ObDereferenceObject(pep);
		}
	}

	return status;
}

// 获取进程数量
ULONG32 GetProcessNum()
{
	PEPROCESS pep;
	// 通过一个上限for循环来遍历当前进程，存在即累加
	for (size_t i = 4; i < 262144; i+=4)
	{
		pep = LookupProcess((HANDLE)i);
		if (NULL != pep)
		{
			// 退出时间不为0或者句柄表为空，都表示进程已经退出
			if (*(PULONG)((PUCHAR)pep + 0x200))
			{
				g_dwProcNum++;
			}
			ObDereferenceObject(pep);
		}
	}
	KdPrint(("%d\n", g_dwProcNum));
	return 1;
}

// 通过Pid来获取EPROCESS
PEPROCESS LookupProcess(HANDLE Pid)
{
	PEPROCESS pep = NULL;
	if (NT_SUCCESS(PsLookupProcessByProcessId(Pid, &pep)))
		return pep;
	else
		return NULL;
}

// 通过EPROCESS来获取完整路径
PUNICODE_STRING GetProcessFullImageFileName(PEPROCESS pep)
{
	// 进程的EPROCESS结构体的0x390偏移保存了当前进程的路径
	return *(PUNICODE_STRING*)((PUCHAR)pep + 0x390);
}

// 通过进程pid获取进程名（此方法在本程序中弃用，改用通过进程路径获取进程名）
VOID GetProcessNameByPid(IN ULONG32 pid, IN PCHAR pszProcName)
{
	HANDLE hProc;
	CLIENT_ID ClientID;
	ULONG32 dwRetValue;
	
	ClientID.UniqueProcess = pid;
	ClientID.UniqueThread = 0;
	
	OBJECT_ATTRIBUTES oa;
	InitializeObjectAttributes(&oa, 0, OBJ_CASE_INSENSITIVE | OBJ_KERNEL_HANDLE, 0, 0);
	// 获取指定PID的进程句柄
	ZwOpenProcess(&hProc, PROCESS_ALL_ACCESS, &oa, &ClientID);
	// 查询这个进程名所占的内存长度
	ZwQueryInformationProcess(hProc, ProcessImageFileName, NULL, 0, &dwRetValue);
	// 分配适当大小的内存用于保存进程名字符串
	PVOID pBuffer = ExAllocatePool(PagedPool, dwRetValue);
	ZwQueryInformationProcess(hProc, ProcessImageFileName, pBuffer, dwRetValue, &dwRetValue);

	//RtlCopyMemory(pszProcName, ((PUNICODE_STRING)pBuffer)->Buffer, dwRetValue);
	KdPrint(("%wZ %d", (PUNICODE_STRING)pBuffer,dwRetValue));
	
	// 内存申请并使用完，需要进行释放
	ExFreePool(pBuffer);
	ZwClose(hProc);
}

// 通过进程路径获取进程名
VOID GetProcessNameByPath(IN PUNICODE_STRING pusProcPath, OUT PCHAR pszProcName)
{
	// 获取当前路径字符串的长度，并且分配指定内存空间进行操作
	ULONG64 dwLen = pusProcPath->MaximumLength;
	PWCHAR pwzProcPath = ExAllocatePool(PagedPool, dwLen);
	PCHAR pszProcPath = ExAllocatePool(PagedPool, dwLen);
	// 将us类型的字符串转换为PWCHAR类型进行操作
	RtlCopyMemory(pwzProcPath, pusProcPath->Buffer, dwLen);	
	// 再将PWCHAR转换为PCHAR，这样更容易进行内存操作
	WcharToChar(pwzProcPath, pszProcPath);
	// 获取这段PCHAR字符串内存的大小
	dwLen = sizeof(CHAR) * strlen(pszProcPath);
	// pszProcPath是头地址，pszProcPath + dwLen即为尾部，这里从字符串尾部开始判断'\\'的位置
	while ((DWORD)(*(pszProcPath + dwLen)) != '\\')
	{
		// 当找到倒数第一个'\\'时，就找到了进程名了，跳出循环
		if (0 >= dwLen || (DWORD)(*(pszProcPath + dwLen)) == '\\')
			break;
		dwLen--;
	}
	// 判断地址有效后，将进程名写到指定内存指针
	if (MmIsAddressValid(pszProcName))
	{
		RtlCopyMemory(pszProcName, pszProcPath + dwLen + 1, 60);
	}

	//KdPrint(("%s\n", ProcName + dwLen + 1));
	//KdPrint(("%ws\n", pwzProcPath + dwLen + 1));
	
	ExFreePool(pwzProcPath);
	ExFreePool(pszProcPath);
}

// 获取文件路径所在的盘符，当前传入参数为设备路径
BOOLEAN GetDiskSym(IN PUNICODE_STRING usFilePath)
{
	OBJECT_ATTRIBUTES obj_attrib;
	IO_STATUS_BLOCK io_status_block = { 0 };
	HANDLE hFile;
	NTSTATUS status;
	PFILE_OBJECT fileObj;

	InitializeObjectAttributes(&obj_attrib, usFilePath, OBJ_CASE_INSENSITIVE | OBJ_KERNEL_HANDLE, NULL, NULL);
	// 创建一个文件句柄
	status = ZwCreateFile(&hFile, GENERIC_READ, &obj_attrib, &io_status_block, NULL, FILE_ATTRIBUTE_NORMAL, FILE_SHARE_READ,
		FILE_OPEN, FILE_NON_DIRECTORY_FILE | FILE_SYNCHRONOUS_IO_NONALERT, NULL, 0);
	if (!NT_SUCCESS(status))
	{
		KdPrint(("error 1 %wZ",usFilePath));
		return FALSE;
	}
	// 根据文件句柄，获取内核态文件句柄的FILE_OBJECT结构体
	status = ObReferenceObjectByHandle(hFile, FILE_ALL_ACCESS, 0, KernelMode, &fileObj, 0);
	if (!NT_SUCCESS(status))
	{
		KdPrint(("error 2"));
		ZwClose(hFile);
		return FALSE;
	}
	// 用IoVolumeDeviceToDosName从FileObject中的DeviceObject得到形如“C:”这样的盘符
	status = IoVolumeDeviceToDosName(fileObj->DeviceObject, usFilePath);
	if (!NT_SUCCESS(status))
	{
		KdPrint(("error 3"));
		ZwClose(hFile);
		return FALSE;
	}
	ZwClose(hFile);
	return TRUE;
}

// 取字符串的子字符串(startIndex为起始位置, stopIndex为终止位置, 包含stopIndex位置的字符)   
VOID KStr_Sub(IN PUNICODE_STRING str, IN USHORT startIndex, IN USHORT stopIndex, OUT PUNICODE_STRING str_result) 
{
	USHORT startIndex2 = startIndex;
	USHORT stopIndex2 = stopIndex;

	if (startIndex2 < 0)
		startIndex2 = 0;
	if (startIndex2 > str->Length - 1)
		startIndex2 = str->Length - 1;
	if (stopIndex2 < startIndex2)
		stopIndex2 = startIndex2;
	if (stopIndex2 > str->Length - 1)
		stopIndex2 = str->Length - 1;

	RtlCopyMemory(str_result->Buffer, str->Buffer + startIndex2 / sizeof(WCHAR), stopIndex2 - startIndex2 + 1);

	str_result->Length = stopIndex2 - startIndex2 + 1;
}

// Volume Device Path To Dos Path
VOID VolumeDevicePathToDosPath(IN PWCHAR pwzDesStr)
{
	UNICODE_STRING DriverLetter;
	UNICODE_STRING TempPath1, TempPath2;
	UNICODE_STRING Result;
	
	RtlInitUnicodeString(&DriverLetter, pwzDesStr);
	RtlInitUnicodeString(&TempPath1, pwzDesStr);
	RtlInitUnicodeString(&TempPath2, pwzDesStr);

	// 获取路径所在驱动的盘符
	if (GetDiskSym(&DriverLetter))
	{
		// 截取卷设备路径前面的设备编号名
		KStr_Sub(&TempPath1, 46, TempPath1.Length - 1, &TempPath2);
		// 初始化字符串长度
		Result.MaximumLength = DriverLetter.MaximumLength + TempPath2.MaximumLength;
		// 根据DriverLetter和TempPath2来分配指定的内存大小
		Result.Buffer = (PWSTR)ExAllocatePool(PagedPool, 2 * wcslen(DriverLetter.Buffer) + 2 * wcslen(TempPath2.Buffer));
		// 获取缓冲区长度
		Result.Length = 2 * wcslen(DriverLetter.Buffer) + 2 * wcslen(TempPath2.Buffer);
		// 将2个字符串合成，并且拷贝到Result字符串内存中
		RtlCopyUnicodeString(&Result, &DriverLetter);
		RtlAppendUnicodeStringToString(&Result, &TempPath2);
		
		RtlZeroMemory(pwzDesStr, wcslen(pwzDesStr) * 2);
		RtlCopyMemory(pwzDesStr, Result.Buffer, Result.Length);

		ExFreePool(Result.Buffer);
	}	
}

// 宽字节转换成窄字节
VOID WcharToChar(PWCHAR src, PCHAR dst)
{
	UNICODE_STRING uString;
	ANSI_STRING aString;
	RtlInitUnicodeString(&uString, src);
	RtlUnicodeStringToAnsiString(&aString, &uString, TRUE);
	strcpy(dst, aString.Buffer);
	RtlFreeAnsiString(&aString);
}

// 通过pid枚举模块
NTSTATUS EnumModule(HANDLE Pid)
{
	//声明偏移
	ULONG64 LdrInPebOffset = 0x018; // peb.ldr
	ULONG64 ModListInPebOffset = 0x010; // peb.ldr.InLoadOrderModuleList

	ULONG64 Peb = 0;
	ULONG64 Ldr = 0;
	PLIST_ENTRY ModListHead = 0;
	PLIST_ENTRY Module = 0;
	ANSI_STRING AnsiString;
	KAPC_STATE _kApc_State;
	PEPROCESS pep;
	ULONG32 index = 0;
	
	// 根据进程模块的数目，分配内存创建MODULEINFO结构体数组
	if (!(g_pModuleInfo = (PMODULEINFO)ExAllocatePoolWithTag(PagedPool, sizeof(MODULEINFO) * g_dwModuleNum, 'pMd')))
		return STATUS_UNSUCCESSFUL;
	// 对着串内存清零
	RtlZeroMemory(g_pModuleInfo, sizeof(MODULEINFO) * g_dwModuleNum);

	// 获取指定PID进程的EPROCESS
	pep = LookupProcess(Pid);
	// 判断EPROCESS是否有效
	if (!MmIsAddressValid(pep))
		return STATUS_UNSUCCESSFUL;
	// 获取PEB地址
	Peb = PsGetProcessPeb(pep);
	if (!Peb)
		return STATUS_UNSUCCESSFUL;
	// 依附进程，获取这个进程的KAPC_STATE结构体
	KeStackAttachProcess(pep, &_kApc_State);
	__try
	{
		// 获取LDR地址，Ped.ldr在Peb的0x018偏移处
		Ldr = Peb+ (ULONG64)LdrInPebOffset;
		// 测试是否可读，不可读则抛出异常退出
		ProbeForRead((CONST PVOID)Ldr, 8, 8);
		// 获得链表头peb.ldr.InLoadOrderModuleList，偏移为0x010
		ModListHead = (PLIST_ENTRY)(*(PULONG64)Ldr + ModListInPebOffset);
		// 再次测试可读性
		ProbeForRead((CONST PVOID)ModListHead, 8, 8);
		// 获得第一个模块的信息
		Module = ModListHead->Flink;
		// 对模块链表头进行遍历
		while (ModListHead != Module)
		{
			g_pModuleInfo[index].Base = (ULONG64)(((PLDR_DATA_TABLE_ENTRY)Module)->DllBase);
			g_pModuleInfo[index].Size = (ULONG32)(((PLDR_DATA_TABLE_ENTRY)Module)->SizeOfImage);
			RtlCopyMemory(g_pModuleInfo[index].Path, ((PLDR_DATA_TABLE_ENTRY)Module)->FullDllName.Buffer, ((PLDR_DATA_TABLE_ENTRY)Module)->FullDllName.Length);

			//KdPrint(("Base=%llx,Size=%ld,Path=%ws",
			//	g_pModuleInfo[index].Base,
			//	g_pModuleInfo[index].Size,
			//	&g_pModuleInfo[index].Path
			//	));

			// 指向下一个
			Module = Module->Flink;
			index++;
			// 测试下一个模块信息的可读性
			ProbeForRead((CONST PVOID)Module, 80, 8);
		}
	}
	__except (EXCEPTION_EXECUTE_HANDLER)
	{
		KdPrint(("[EnumModule]__except(EXCEPTION_EXECUTE_HANDLER)"));
	}
	// 取消依附进程
	KeUnstackDetachProcess(&_kApc_State);
	// 此处释放内存为测试所用
	//ExFreePool(g_pModuleInfo);
	return STATUS_SUCCESS;
}

// 根据线程ID返回ETHREAD
PETHREAD LookupThread(HANDLE Tid)
{
	PETHREAD pet;
	if (NT_SUCCESS(PsLookupThreadByThreadId(Tid, &pet)))
		return pet;
	else
		return NULL;
}

// 根据进程ID枚举线程
NTSTATUS EnumThread(HANDLE Pid)
{
	PEPROCESS pep;
	PETHREAD pet = NULL;
	HANDLE hThread;
	ULONG64 qwThreadStartAddr;
	ULONG32 dwThreadSwitch;
	ULONG64 qwReturnLength;
	CHAR	szThreadState[50];
	ULONG32 index = 0;

	// 字符串数组内存清零，防止不必要的乱码输出
	RtlZeroMemory(&szThreadState, 50);

	// 初始化线程信息结构体
	if (!(g_pThreadInfo = (PTHREADINFO)ExAllocatePoolWithTag(PagedPool, sizeof(THREADINFO) * g_dwThreadNum, 'pTd')))
		return STATUS_UNSUCCESSFUL;
	RtlZeroMemory(g_pThreadInfo, sizeof(THREADINFO) * g_dwThreadNum);

	// 获取这个进程的EPROCESS指针
	pep = LookupProcess(Pid);
	// 判断EPROCESS是否有效
	if (!MmIsAddressValid(pep))
		return STATUS_UNSUCCESSFUL;

	// 开始遍历进程的线程，这里用的是穷举法
	for (size_t i = 4; i < 262144; i+=4)
	{
		pet = LookupThread((HANDLE)i);
		if (NULL != pet)
		{
			// 获得线程所属进程
			if (pep == IoThreadToProcess(pet))
			{
				// 根据PETHREAD获取线程句柄
				GetThreadHandle(pet, &hThread);
				// 查询当前线程的开始地址
				ZwQueryInformationThread(hThread, ThreadQuerySetWin32StartAddress, &qwThreadStartAddr, sizeof(ULONG64), &qwReturnLength);
				// 查询当前线程的交换次数
				ZwQueryInformationThread(hThread, ThreadCSwitchPmu, &dwThreadSwitch, sizeof(ULONG32), &qwReturnLength);
				// 获取线程交换次数和线程状态
				GetThreadSwitchAndState(Pid, PsGetThreadId(pet), &dwThreadSwitch, &szThreadState);
				// 填充结构体
				g_pThreadInfo[index].TID = (ULONG64)PsGetThreadId(pet);
				g_pThreadInfo[index].ETHREAD = pet;
				g_pThreadInfo[index].PRIORITY = KeQueryPriorityThread(pet);
				g_pThreadInfo[index].TEB = PsGetThreadTeb(pet);
				g_pThreadInfo[index].ENTRY = qwThreadStartAddr;
				g_pThreadInfo[index].SWITCH = dwThreadSwitch;
				RtlCopyMemory(&g_pThreadInfo[index].STATE, &szThreadState, strlen(&szThreadState) * sizeof(CHAR));

				KdPrint((
					"[T]TID=%ld,ETHREAD=%p,Priority:%d,Teb=%p,Entry=%p,switch=%d,state:%s,index=%d\n",
					g_pThreadInfo[index].TID,
					g_pThreadInfo[index].ETHREAD,
					g_pThreadInfo[index].PRIORITY,
					g_pThreadInfo[index].TEB,
					g_pThreadInfo[index].ENTRY,
					g_pThreadInfo[index].SWITCH,
					&g_pThreadInfo[index].STATE,
					index
					));
				
				index++;
			}
			ObDereferenceObject(pet);
		}
	}

	return STATUS_SUCCESS;
}

// Ring0层结束进程
NTSTATUS TerminateProcess(ULONG32 Pid)
{
	HANDLE hProcess;
	CLIENT_ID _clientId;
	OBJECT_ATTRIBUTES _oa;

	_clientId.UniqueProcess = (HANDLE)Pid;
	_clientId.UniqueThread = 0;

	InitializeObjectAttributes(&_oa, NULL, 0, NULL, NULL);

	ZwOpenProcess(&hProcess, PROCESS_ALL_ACCESS, &_oa, &_clientId);
	if (hProcess)
	{
		ZwTerminateProcess(hProcess, NULL);
		ZwClose(hProcess);
	}
}

// 获取线程状态
PCHAR GetThreadState(THREAD_STATE threadState)
{
	switch (threadState)
	{
	case StateInitialized:
		return "StateInitialized";
		break;
	case StateReady:
		return "StateReady";
		break;
	case StateRunning:
		return "StateRunning";
		break;
	case StateStandby:
		return "StateStandby";
		break;
	case StateTerminated:
		return "StateTerminated";
		break;
	case StateWait:
		return "StateWait";
		break;
	case StateTransition:
		return "StateTransition";
		break;
	default:
		return "StateUnknown";
		break;
	}
}

// 通过ETHREAD获取线程句柄
NTSTATUS GetThreadHandle(PETHREAD pet, PHANDLE phThread)
{
	NTSTATUS status;
	
	status = ObOpenObjectByPointer(pet, 0, NULL, THREAD_ALL_ACCESS, (PVOID)*PsThreadType, KernelMode, phThread);

	return status;
}

// 通过线程ID获取线程句柄
HANDLE OpenThread(HANDLE tid)
{
	HANDLE hThread;
	CLIENT_ID _ClientId;
	OBJECT_ATTRIBUTES _oa;
	//填充 CID
	_ClientId.UniqueProcess = 0;
	_ClientId.UniqueThread = tid; //这里修改为你要的 TID

	InitializeObjectAttributes(&_oa, NULL, 0, NULL, NULL);
	ZwOpenProcess(&hThread, 1, &_oa, &_ClientId);
	return hThread;
}

// 或取线程交换次数和线程状态
NTSTATUS GetThreadSwitchAndState(HANDLE Pid, HANDLE Tid, ULONG32 *dwSwitchCount, CHAR *szThreadState)
{
	NTSTATUS status;
	ULONG32 dwNeedSize;
	SIZE_T qwBufferSize = 4096;
	PVOID pBuffer;
	PSYSTEM_PROCESS_INFORMATION _pSysProcInfo;
	
retry:
	pBuffer = ExAllocatePool(NonPagedPool, qwBufferSize);
	if (!pBuffer)
		return STATUS_NO_MEMORY;
	// 查询SYSTEM_PROCESS_INFORMATION结构体所需的真实内存大小
	status = ZwQuerySystemInformation(SystemProcessInformation, pBuffer, qwBufferSize, &dwNeedSize);
	
	if (STATUS_INFO_LENGTH_MISMATCH == status)
	{
		ExFreePool(pBuffer);
		qwBufferSize = dwNeedSize;
		goto retry;
	}
	
	if (NT_SUCCESS(status))
	{
		_pSysProcInfo = (PSYSTEM_PROCESS_INFORMATION)pBuffer;
		do
		{
			// 判断并确定为当前操作的PID
			if (_pSysProcInfo->UniqueProcessId == Pid)
			{
				// 遍历线程信息
				for (size_t i = 0; i < _pSysProcInfo->NumberOfThreads; i++)
				{
					// 定位到指定的tid，并且获取交换次数和进程状态
					if (_pSysProcInfo->Threads[i].ClientId.UniqueThread == Tid)
					{
						*dwSwitchCount = _pSysProcInfo->Threads[i].ContextSwitchCount;
						RtlCopyMemory(szThreadState, GetThreadState(_pSysProcInfo->Threads[i].State), strlen(GetThreadState(_pSysProcInfo->Threads[i].State)) * sizeof(CHAR));
					}		
				}
				// 获取完，跳出循环
				break;
			}
			// 指向下一个
			_pSysProcInfo = (PSYSTEM_PROCESS_INFORMATION)((ULONG64)_pSysProcInfo + _pSysProcInfo->NextEntryOffset);
			// 下个入口偏移为0时，即为自己本身
			if (0 == _pSysProcInfo->NextEntryOffset)
			{
				if (_pSysProcInfo->UniqueProcessId == Pid)
				{
					for (size_t i = 0; i < _pSysProcInfo->NumberOfThreads; i++)
					{
						if (_pSysProcInfo->Threads[i].ClientId.UniqueThread == Tid)
						{
							*dwSwitchCount = _pSysProcInfo->Threads[i].ContextSwitchCount;
							RtlCopyMemory(szThreadState, GetThreadState(_pSysProcInfo->Threads[i].State), strlen(GetThreadState(_pSysProcInfo->Threads[i].State)) * sizeof(CHAR));
						}
					}
					break;
				}
			}
		} while (_pSysProcInfo->NextEntryOffset != 0);
	}
	
	ExFreePool(pBuffer);
}

// 暂停进程
NTSTATUS SuspendProcess(ULONG32 pid)
{
	PEPROCESS pep = LookupProcess((HANDLE)pid);
	NTSTATUS status = PsSuspendProcess(pep);

	switch (status)
	{
	case STATUS_SUCCESS:
	{
		// 进程状态(1：正常状态 0：暂停状态)
		g_dwProcStatus = 0;
		break;
	}
	case STATUS_PROCESS_IS_TERMINATING:
	{
		// 进程状态(1：正常状态 0：暂停状态)
		g_dwProcStatus = 0;
		break;
	}
	}
	return status;
}

// 恢复进程
NTSTATUS ResumeProcess(ULONG32 pid)
{
	PEPROCESS pep = LookupProcess((HANDLE)pid);
	NTSTATUS status = PsResumeProcess(pep);

	switch (status)
	{
	case STATUS_SUCCESS:
	{
		// 进程状态(1：正常状态 0：暂停状态)
		g_dwProcStatus = 1;		
		break;
	}
	case STATUS_PROCESS_IS_TERMINATING:
	{
		// 进程状态(1：正常状态 0：暂停状态)
		g_dwProcStatus = 0;
		break;
	}
	}
	return status;
}

// 卸载指定模块
NTSTATUS UninstallModule()
{
	NTSTATUS status;
	status = MmUnmapViewOfSection(g_pModulelUninstall->Eprocess, g_pModulelUninstall->Base);
	return status;
}

// 获取线程标志偏移，用于APC终止线程
ULONG64 GetThreadFlagsOffset()
{
	PUCHAR cPtr;
	ULONG64 qwOffset;

	for (cPtr = (PUCHAR)PsTerminateSystemThread; cPtr < (PUCHAR)PsTerminateSystemThread + PAGE_SIZE; cPtr++)
	{
		// 根据特征码定位
		if (*(PUSHORT)cPtr == 0x80F6)
		{
			qwOffset = *(PULONG64)(cPtr + 2);
			return qwOffset;
		}
	}
	return 0;
}

// APC终止线程例程
VOID KernelTerminateThreadRoutine(
	IN struct _KAPC *Apc,
	IN OUT PKNORMAL_ROUTINE *NormalRoutine,
	IN OUT PVOID *NormalContext,
	IN OUT PVOID *SystemArgument1,
	IN OUT PVOID *SystemArgument2
) 
{
	ULONG64 qwThreadFlagsOffset = GetThreadFlagsOffset();
	PULONG64 pqwThreadFlags;
	ExFreePool(Apc);
	if (qwThreadFlagsOffset)
	{
		pqwThreadFlags = (ULONG64*)((ULONG64)(PsGetCurrentThread()) + qwThreadFlagsOffset);
		*pqwThreadFlags |= PS_CROSS_THREAD_FLAGS_SYSTEM;
		PsTerminateSystemThread(STATUS_SUCCESS);
	}
	else
	{
		KdPrint(("[T]cannot get thread flags offset!"));
	}
}

// 终结指定线程
BOOLEAN TerminateThread(__in ULONG64 tid)
{
	PETHREAD pet;
	KIRQL _OldIrql;
	PKTHREAD pThread2;
	PKAPC pQueuedApc = NULL;
	PKAPC_STATE pApcState;
	PKAPC pApc = NULL;
	BOOLEAN bRet = FALSE;

	PsLookupThreadByThreadId((HANDLE)tid, &pet);

	// 线程地址检查是否有效
	if (!MmIsAddressValid(pet))
		return bRet;

	// 申请内存
	pApc = ExAllocatePoolWithTag(NonPagedPool, sizeof(KAPC), 'apc');
	
	pThread2 = (PKTHREAD)pet;
	// 初始化APC
	
	KeInitializeApc(pApc, pet, OriginalApcEnvironment, KernelTerminateThreadRoutine,
		NULL, NULL, KernelMode, NULL);

	bRet = KeInsertQueueApc(pApc, NULL, NULL, 0);
	return bRet;
}

// 终结指定线程 (WIN7 SP1 X64特征码版本)
NTSTATUS TerminateThread64(__in ULONG64 tid)
{
	ULONG32 dwCallCode = 0;
	ULONG64 qwAddrOfPsTTBP = 0, qwAddrOfPsTST = 0;
	PETHREAD pet = NULL;
	NTSTATUS status = NULL;
	
	// 获取PsTerminateSystemThreadByPointer的指针
	if (PspTerminateThreadByPointer ==  NULL)
	{
		// 通过Windbg可以知道PsTerminateSystemThreadByPointer在PsTerminateSystemThread + 0x1c附近，并且有3个参数
		// 特征码为01e8

		// 获取PsTerminateSystemThread的函数地址
		qwAddrOfPsTST = (ULONG64)GetFunctionAddr(L"PsTerminateSystemThread");
		if (qwAddrOfPsTST == 0)
		{
			return STATUS_UNSUCCESSFUL;
		}
		// 搜索PsTerminateSystemThread函数的内存区域
		for (size_t i = 1; i < 0xff; i++)
		{
			if (MmIsAddressValid((PVOID)(qwAddrOfPsTST + i)) != FALSE)
			{
				if (*(BYTE *)(qwAddrOfPsTST + i) == 0x01 && *(BYTE *)(qwAddrOfPsTST + i + 1) == 0xe8)	
				{
					// 假设内存为 41b001 e8d0590500 -> dwCallCode应该存放00 05 59 d0
					RtlMoveMemory(&dwCallCode, (PVOID)(qwAddrOfPsTST + i + 2), 4);
					// KdPrint(("[T]dwCallCode=%p qwAddrOfPsTST=%p", dwCallCode, qwAddrOfPsTST));

					// 目标地址 - 原始地址 - 5 = 机器码 ==> 目标地址 = 机器码 + 5 + 原始地址
					qwAddrOfPsTTBP = (ULONG64)dwCallCode + 5 + qwAddrOfPsTST + i;
				}
			}
		}
		PspTerminateThreadByPointer = (PSPTERMINATETHREADBYPOINTER)qwAddrOfPsTTBP;
	}

	status = PsLookupThreadByThreadId((HANDLE)tid, &pet);
	// 线程地址检查是否有效
	if (!MmIsAddressValid(pet))
		return status;
	PspTerminateThreadByPointer(pet, 0, 1);
	// 凡是调用了lookup函数，都要DereferenceObject
	ObDereferenceObject(pet);
	return status;
}

// 获取函数地址
PVOID GetFunctionAddr(PCWSTR FunctionName)
{
	UNICODE_STRING usFuncName;
	RtlInitUnicodeString(&usFuncName, FunctionName);
	return MmGetSystemRoutineAddress(&usFuncName);
}

// 获取PsSuspendThread地址 (WIN7 SP1 X64特征码版本)
VOID GetPsSuspendThreadAddr()
{
	ULONG64 NtSuspendThreadAddr;
	ULONG32 dwCallCode = 0;	// call后面的机器码，即e8后面

	// 先获取NtSuspendThread的地址
	NtSuspendThreadAddr = GetSSDTFunctionAddress64Ex(379);
	if (0 == NtSuspendThreadAddr)
		return;
	if (0 == g_qwPsSuspendThreadAddr)
	{
		// 搜索NtSuspendThread函数的内存区域
		for (size_t i = 0; i < 0xff; i++)
		{
			if (FALSE != MmIsAddressValid((PVOID)(NtSuspendThreadAddr + i)))
			{
				if (*(BYTE *)(NtSuspendThreadAddr + i - 1) == 0x24 && *(BYTE *)(NtSuspendThreadAddr + i + 1) == 0xe8)
				{
					// 假设内存为 488b4c24 48e8 695efaff -> dwCallCode应该存放ff fa 5e 69
					// 这里不能用0x48来当特征码，因为它是rsp的偏移，会变
					RtlMoveMemory(&dwCallCode, (PVOID)(NtSuspendThreadAddr + i + 2), 4);
					// 目标地址 - 原始地址 - 5 = 机器码 ==> 目标地址 = 机器码 + 5 + 原始地址
					g_qwPsSuspendThreadAddr = (ULONG64)dwCallCode + 5 + NtSuspendThreadAddr + i;
					break;
				}
			}
		}
	}
}

// 获取KeResumeThread地址 (WIN7 SP1 X64特征码版本)
VOID GetKeResumeThreadAddr()
{
	ULONG64 NtResumeThreadAddr;
	ULONG32 dwCallCode = 0;	// call后面的机器码，即e8后面

	// 先获取NtSuspendThread的地址
	NtResumeThreadAddr = GetSSDTFunctionAddress64Ex(79);
	KdPrint(("[T]: %p", NtResumeThreadAddr));
	if (0 == NtResumeThreadAddr)
		return;
	if (0 == g_qwPsResumeThreadAddr)
	{
		// 搜索NtSuspendThread函数的内存区域
		for (size_t i = 0; i < 0xff; i++)
		{
			if (FALSE != MmIsAddressValid((PVOID)(NtResumeThreadAddr + i)))
			{
				if (*(BYTE *)(NtResumeThreadAddr + i - 1) == 0x24 && *(BYTE *)(NtResumeThreadAddr + i + 1) == 0xe8)
				{
					// 假设内存为 488b4c24 48e8 695efaff -> dwCallCode应该存放ff fa 5e 69
					// 这里不能用0x48来当特征码，因为它是rsp的偏移，会变
					RtlMoveMemory(&dwCallCode, (PVOID)(NtResumeThreadAddr + i + 2), 4);
					// 目标地址 - 原始地址 - 5 = 机器码 ==> 目标地址 = 机器码 + 5 + 原始地址
					// g_qwKeResumeThreadAddr = (ULONG64)dwCallCode + 5 + NtResumeThreadAddr + i;
					g_qwPsResumeThreadAddr = 0xfffff800040539b0;
					break;
				}
			}
		}
	}
}

// 挂起线程
NTSTATUS SuspendThread(__in ULONG32 tid)
{
	PETHREAD pet;
	PSSUSPENDTHREAD PsSuspendThread = NULL;

	PsLookupThreadByThreadId((HANDLE)tid, &pet);

	// 判断ETHREAD有效性
	if (!MmIsAddressValid(pet))
		return STATUS_UNSUCCESSFUL;
	// 判断这个线程是否已经结束了
	if (PsIsThreadTerminating(pet))
		return STATUS_UNSUCCESSFUL;
	
	if (0 == g_qwPsSuspendThreadAddr)
		// 获取PsSuspendThread地址 (特征码版本)
		GetPsSuspendThreadAddr();

	 PsSuspendThread = (PSSUSPENDTHREAD)g_qwPsSuspendThreadAddr;

	 return PsSuspendThread(pet, NULL);
}

// 恢复线程
NTSTATUS ResumeThread(__in ULONG32 tid)
{
	PETHREAD pet;
	PSRESUMETHREAD PsResumeThread = NULL;

	PsLookupThreadByThreadId((HANDLE)tid, &pet);

	// 判断ETHREAD有效性
	if (!MmIsAddressValid(pet))
		return STATUS_UNSUCCESSFUL;
	// 判断这个线程是否已经结束了
	if (PsIsThreadTerminating(pet))
		return STATUS_UNSUCCESSFUL;

	if (0 == g_qwPsResumeThreadAddr)
		// 获取PsSuspendThread地址 (特征码版本)
		GetKeResumeThreadAddr();

	PsResumeThread = (PSRESUMETHREAD)g_qwPsResumeThreadAddr;

	return PsResumeThread(pet, NULL);
	//return STATUS_SUCCESS;
}
