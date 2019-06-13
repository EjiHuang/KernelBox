#include "Process.h"
#include <windef.h>

// ���庯������
typedef NTSTATUS(__stdcall *PSSUSPENDTHREAD)(
	IN PETHREAD Thread,
	OUT PULONG PreviousSuspendCount OPTIONAL
	);

typedef NTSTATUS(__stdcall *PSRESUMETHREAD)(
	IN PETHREAD Thread,
	OUT PULONG PreviousSuspendCount OPTIONAL
	);

PSPTERMINATETHREADBYPOINTER PspTerminateThreadByPointer = NULL;

// ��ȡ�����б�
NTSTATUS GetProcessList()
{
	NTSTATUS status = STATUS_SUCCESS;
	PEPROCESS pep = NULL;
	ULONG index = 0;

	// ���ݽ�����Ŀ��������Ӧ��С���ڴ�ռ������ͨ�ŵ�PROCESSINFO�ṹ��
	if (!(g_pProcessInfo = (PPROCESSINFO)ExAllocatePoolWithTag(PagedPool, sizeof(PROCESSINFO) * g_dwProcNum, 'pPI')))
		return STATUS_UNSUCCESSFUL;
	// ����ɹ��󣬶Խṹ������ڴ��������
	RtlZeroMemory(g_pProcessInfo, sizeof(PROCESSINFO) * g_dwProcNum);

	// ѭ������������Ϣ
	for (size_t i = 4; i < 262144; i+=4)
	{
		pep = LookupProcess((HANDLE)i);
		if (NULL != pep)
		{
			// �˳�ʱ�䲻Ϊ0���߾����Ϊ�գ�����ʾ�����Ѿ��˳�
			if (*(PULONG)((PUCHAR)pep + 0x200))
			{
				// �ṹ���������
				g_pProcessInfo[index].EProcess = (ULONG64)pep;
				g_pProcessInfo[index].Pid = (ULONG32)PsGetProcessId(pep);
				g_pProcessInfo[index].PPid = (ULONG32)PsGetProcessInheritedFromUniqueProcessId(pep);
				// ͨ������ȫ·����ȡ������
				GetProcessNameByPath(GetProcessFullImageFileName(pep), &g_pProcessInfo[index].ImageFileName);
				// ����ȫ·����ȡ
				RtlCopyMemory(g_pProcessInfo[index].Path, GetProcessFullImageFileName(pep)->Buffer, GetProcessFullImageFileName(pep)->Length);
				// ��ȡ�Ľ���·�����豸·��������������Ҫ����ת��ΪDOS·��
				VolumeDevicePathToDosPath(&g_pProcessInfo[index].Path);
				++index;
			}
			ObDereferenceObject(pep);
		}
	}

	return status;
}

// ��ȡ��������
ULONG32 GetProcessNum()
{
	PEPROCESS pep;
	// ͨ��һ������forѭ����������ǰ���̣����ڼ��ۼ�
	for (size_t i = 4; i < 262144; i+=4)
	{
		pep = LookupProcess((HANDLE)i);
		if (NULL != pep)
		{
			// �˳�ʱ�䲻Ϊ0���߾����Ϊ�գ�����ʾ�����Ѿ��˳�
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

// ͨ��Pid����ȡEPROCESS
PEPROCESS LookupProcess(HANDLE Pid)
{
	PEPROCESS pep = NULL;
	if (NT_SUCCESS(PsLookupProcessByProcessId(Pid, &pep)))
		return pep;
	else
		return NULL;
}

// ͨ��EPROCESS����ȡ����·��
PUNICODE_STRING GetProcessFullImageFileName(PEPROCESS pep)
{
	// ���̵�EPROCESS�ṹ���0x390ƫ�Ʊ����˵�ǰ���̵�·��
	return *(PUNICODE_STRING*)((PUCHAR)pep + 0x390);
}

// ͨ������pid��ȡ���������˷����ڱ����������ã�����ͨ������·����ȡ��������
VOID GetProcessNameByPid(IN ULONG32 pid, IN PCHAR pszProcName)
{
	HANDLE hProc;
	CLIENT_ID ClientID;
	ULONG32 dwRetValue;
	
	ClientID.UniqueProcess = pid;
	ClientID.UniqueThread = 0;
	
	OBJECT_ATTRIBUTES oa;
	InitializeObjectAttributes(&oa, 0, OBJ_CASE_INSENSITIVE | OBJ_KERNEL_HANDLE, 0, 0);
	// ��ȡָ��PID�Ľ��̾��
	ZwOpenProcess(&hProc, PROCESS_ALL_ACCESS, &oa, &ClientID);
	// ��ѯ�����������ռ���ڴ泤��
	ZwQueryInformationProcess(hProc, ProcessImageFileName, NULL, 0, &dwRetValue);
	// �����ʵ���С���ڴ����ڱ���������ַ���
	PVOID pBuffer = ExAllocatePool(PagedPool, dwRetValue);
	ZwQueryInformationProcess(hProc, ProcessImageFileName, pBuffer, dwRetValue, &dwRetValue);

	//RtlCopyMemory(pszProcName, ((PUNICODE_STRING)pBuffer)->Buffer, dwRetValue);
	KdPrint(("%wZ %d", (PUNICODE_STRING)pBuffer,dwRetValue));
	
	// �ڴ����벢ʹ���꣬��Ҫ�����ͷ�
	ExFreePool(pBuffer);
	ZwClose(hProc);
}

// ͨ������·����ȡ������
VOID GetProcessNameByPath(IN PUNICODE_STRING pusProcPath, OUT PCHAR pszProcName)
{
	// ��ȡ��ǰ·���ַ����ĳ��ȣ����ҷ���ָ���ڴ�ռ���в���
	ULONG64 dwLen = pusProcPath->MaximumLength;
	PWCHAR pwzProcPath = ExAllocatePool(PagedPool, dwLen);
	PCHAR pszProcPath = ExAllocatePool(PagedPool, dwLen);
	// ��us���͵��ַ���ת��ΪPWCHAR���ͽ��в���
	RtlCopyMemory(pwzProcPath, pusProcPath->Buffer, dwLen);	
	// �ٽ�PWCHARת��ΪPCHAR�����������׽����ڴ����
	WcharToChar(pwzProcPath, pszProcPath);
	// ��ȡ���PCHAR�ַ����ڴ�Ĵ�С
	dwLen = sizeof(CHAR) * strlen(pszProcPath);
	// pszProcPath��ͷ��ַ��pszProcPath + dwLen��Ϊβ����������ַ���β����ʼ�ж�'\\'��λ��
	while ((DWORD)(*(pszProcPath + dwLen)) != '\\')
	{
		// ���ҵ�������һ��'\\'ʱ�����ҵ��˽������ˣ�����ѭ��
		if (0 >= dwLen || (DWORD)(*(pszProcPath + dwLen)) == '\\')
			break;
		dwLen--;
	}
	// �жϵ�ַ��Ч�󣬽�������д��ָ���ڴ�ָ��
	if (MmIsAddressValid(pszProcName))
	{
		RtlCopyMemory(pszProcName, pszProcPath + dwLen + 1, 60);
	}

	//KdPrint(("%s\n", ProcName + dwLen + 1));
	//KdPrint(("%ws\n", pwzProcPath + dwLen + 1));
	
	ExFreePool(pwzProcPath);
	ExFreePool(pszProcPath);
}

// ��ȡ�ļ�·�����ڵ��̷�����ǰ�������Ϊ�豸·��
BOOLEAN GetDiskSym(IN PUNICODE_STRING usFilePath)
{
	OBJECT_ATTRIBUTES obj_attrib;
	IO_STATUS_BLOCK io_status_block = { 0 };
	HANDLE hFile;
	NTSTATUS status;
	PFILE_OBJECT fileObj;

	InitializeObjectAttributes(&obj_attrib, usFilePath, OBJ_CASE_INSENSITIVE | OBJ_KERNEL_HANDLE, NULL, NULL);
	// ����һ���ļ����
	status = ZwCreateFile(&hFile, GENERIC_READ, &obj_attrib, &io_status_block, NULL, FILE_ATTRIBUTE_NORMAL, FILE_SHARE_READ,
		FILE_OPEN, FILE_NON_DIRECTORY_FILE | FILE_SYNCHRONOUS_IO_NONALERT, NULL, 0);
	if (!NT_SUCCESS(status))
	{
		KdPrint(("error 1 %wZ",usFilePath));
		return FALSE;
	}
	// �����ļ��������ȡ�ں�̬�ļ������FILE_OBJECT�ṹ��
	status = ObReferenceObjectByHandle(hFile, FILE_ALL_ACCESS, 0, KernelMode, &fileObj, 0);
	if (!NT_SUCCESS(status))
	{
		KdPrint(("error 2"));
		ZwClose(hFile);
		return FALSE;
	}
	// ��IoVolumeDeviceToDosName��FileObject�е�DeviceObject�õ����硰C:���������̷�
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

// ȡ�ַ��������ַ���(startIndexΪ��ʼλ��, stopIndexΪ��ֹλ��, ����stopIndexλ�õ��ַ�)   
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

	// ��ȡ·�������������̷�
	if (GetDiskSym(&DriverLetter))
	{
		// ��ȡ���豸·��ǰ����豸�����
		KStr_Sub(&TempPath1, 46, TempPath1.Length - 1, &TempPath2);
		// ��ʼ���ַ�������
		Result.MaximumLength = DriverLetter.MaximumLength + TempPath2.MaximumLength;
		// ����DriverLetter��TempPath2������ָ�����ڴ��С
		Result.Buffer = (PWSTR)ExAllocatePool(PagedPool, 2 * wcslen(DriverLetter.Buffer) + 2 * wcslen(TempPath2.Buffer));
		// ��ȡ����������
		Result.Length = 2 * wcslen(DriverLetter.Buffer) + 2 * wcslen(TempPath2.Buffer);
		// ��2���ַ����ϳɣ����ҿ�����Result�ַ����ڴ���
		RtlCopyUnicodeString(&Result, &DriverLetter);
		RtlAppendUnicodeStringToString(&Result, &TempPath2);
		
		RtlZeroMemory(pwzDesStr, wcslen(pwzDesStr) * 2);
		RtlCopyMemory(pwzDesStr, Result.Buffer, Result.Length);

		ExFreePool(Result.Buffer);
	}	
}

// ���ֽ�ת����խ�ֽ�
VOID WcharToChar(PWCHAR src, PCHAR dst)
{
	UNICODE_STRING uString;
	ANSI_STRING aString;
	RtlInitUnicodeString(&uString, src);
	RtlUnicodeStringToAnsiString(&aString, &uString, TRUE);
	strcpy(dst, aString.Buffer);
	RtlFreeAnsiString(&aString);
}

// ͨ��pidö��ģ��
NTSTATUS EnumModule(HANDLE Pid)
{
	//����ƫ��
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
	
	// ���ݽ���ģ�����Ŀ�������ڴ洴��MODULEINFO�ṹ������
	if (!(g_pModuleInfo = (PMODULEINFO)ExAllocatePoolWithTag(PagedPool, sizeof(MODULEINFO) * g_dwModuleNum, 'pMd')))
		return STATUS_UNSUCCESSFUL;
	// ���Ŵ��ڴ�����
	RtlZeroMemory(g_pModuleInfo, sizeof(MODULEINFO) * g_dwModuleNum);

	// ��ȡָ��PID���̵�EPROCESS
	pep = LookupProcess(Pid);
	// �ж�EPROCESS�Ƿ���Ч
	if (!MmIsAddressValid(pep))
		return STATUS_UNSUCCESSFUL;
	// ��ȡPEB��ַ
	Peb = PsGetProcessPeb(pep);
	if (!Peb)
		return STATUS_UNSUCCESSFUL;
	// �������̣���ȡ������̵�KAPC_STATE�ṹ��
	KeStackAttachProcess(pep, &_kApc_State);
	__try
	{
		// ��ȡLDR��ַ��Ped.ldr��Peb��0x018ƫ�ƴ�
		Ldr = Peb+ (ULONG64)LdrInPebOffset;
		// �����Ƿ�ɶ������ɶ����׳��쳣�˳�
		ProbeForRead((CONST PVOID)Ldr, 8, 8);
		// �������ͷpeb.ldr.InLoadOrderModuleList��ƫ��Ϊ0x010
		ModListHead = (PLIST_ENTRY)(*(PULONG64)Ldr + ModListInPebOffset);
		// �ٴβ��Կɶ���
		ProbeForRead((CONST PVOID)ModListHead, 8, 8);
		// ��õ�һ��ģ�����Ϣ
		Module = ModListHead->Flink;
		// ��ģ������ͷ���б���
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

			// ָ����һ��
			Module = Module->Flink;
			index++;
			// ������һ��ģ����Ϣ�Ŀɶ���
			ProbeForRead((CONST PVOID)Module, 80, 8);
		}
	}
	__except (EXCEPTION_EXECUTE_HANDLER)
	{
		KdPrint(("[EnumModule]__except(EXCEPTION_EXECUTE_HANDLER)"));
	}
	// ȡ����������
	KeUnstackDetachProcess(&_kApc_State);
	// �˴��ͷ��ڴ�Ϊ��������
	//ExFreePool(g_pModuleInfo);
	return STATUS_SUCCESS;
}

// �����߳�ID����ETHREAD
PETHREAD LookupThread(HANDLE Tid)
{
	PETHREAD pet;
	if (NT_SUCCESS(PsLookupThreadByThreadId(Tid, &pet)))
		return pet;
	else
		return NULL;
}

// ���ݽ���IDö���߳�
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

	// �ַ��������ڴ����㣬��ֹ����Ҫ���������
	RtlZeroMemory(&szThreadState, 50);

	// ��ʼ���߳���Ϣ�ṹ��
	if (!(g_pThreadInfo = (PTHREADINFO)ExAllocatePoolWithTag(PagedPool, sizeof(THREADINFO) * g_dwThreadNum, 'pTd')))
		return STATUS_UNSUCCESSFUL;
	RtlZeroMemory(g_pThreadInfo, sizeof(THREADINFO) * g_dwThreadNum);

	// ��ȡ������̵�EPROCESSָ��
	pep = LookupProcess(Pid);
	// �ж�EPROCESS�Ƿ���Ч
	if (!MmIsAddressValid(pep))
		return STATUS_UNSUCCESSFUL;

	// ��ʼ�������̵��̣߳������õ�����ٷ�
	for (size_t i = 4; i < 262144; i+=4)
	{
		pet = LookupThread((HANDLE)i);
		if (NULL != pet)
		{
			// ����߳���������
			if (pep == IoThreadToProcess(pet))
			{
				// ����PETHREAD��ȡ�߳̾��
				GetThreadHandle(pet, &hThread);
				// ��ѯ��ǰ�̵߳Ŀ�ʼ��ַ
				ZwQueryInformationThread(hThread, ThreadQuerySetWin32StartAddress, &qwThreadStartAddr, sizeof(ULONG64), &qwReturnLength);
				// ��ѯ��ǰ�̵߳Ľ�������
				ZwQueryInformationThread(hThread, ThreadCSwitchPmu, &dwThreadSwitch, sizeof(ULONG32), &qwReturnLength);
				// ��ȡ�߳̽����������߳�״̬
				GetThreadSwitchAndState(Pid, PsGetThreadId(pet), &dwThreadSwitch, &szThreadState);
				// ���ṹ��
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

// Ring0���������
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

// ��ȡ�߳�״̬
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

// ͨ��ETHREAD��ȡ�߳̾��
NTSTATUS GetThreadHandle(PETHREAD pet, PHANDLE phThread)
{
	NTSTATUS status;
	
	status = ObOpenObjectByPointer(pet, 0, NULL, THREAD_ALL_ACCESS, (PVOID)*PsThreadType, KernelMode, phThread);

	return status;
}

// ͨ���߳�ID��ȡ�߳̾��
HANDLE OpenThread(HANDLE tid)
{
	HANDLE hThread;
	CLIENT_ID _ClientId;
	OBJECT_ATTRIBUTES _oa;
	//��� CID
	_ClientId.UniqueProcess = 0;
	_ClientId.UniqueThread = tid; //�����޸�Ϊ��Ҫ�� TID

	InitializeObjectAttributes(&_oa, NULL, 0, NULL, NULL);
	ZwOpenProcess(&hThread, 1, &_oa, &_ClientId);
	return hThread;
}

// ��ȡ�߳̽����������߳�״̬
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
	// ��ѯSYSTEM_PROCESS_INFORMATION�ṹ���������ʵ�ڴ��С
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
			// �жϲ�ȷ��Ϊ��ǰ������PID
			if (_pSysProcInfo->UniqueProcessId == Pid)
			{
				// �����߳���Ϣ
				for (size_t i = 0; i < _pSysProcInfo->NumberOfThreads; i++)
				{
					// ��λ��ָ����tid�����һ�ȡ���������ͽ���״̬
					if (_pSysProcInfo->Threads[i].ClientId.UniqueThread == Tid)
					{
						*dwSwitchCount = _pSysProcInfo->Threads[i].ContextSwitchCount;
						RtlCopyMemory(szThreadState, GetThreadState(_pSysProcInfo->Threads[i].State), strlen(GetThreadState(_pSysProcInfo->Threads[i].State)) * sizeof(CHAR));
					}		
				}
				// ��ȡ�꣬����ѭ��
				break;
			}
			// ָ����һ��
			_pSysProcInfo = (PSYSTEM_PROCESS_INFORMATION)((ULONG64)_pSysProcInfo + _pSysProcInfo->NextEntryOffset);
			// �¸����ƫ��Ϊ0ʱ����Ϊ�Լ�����
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

// ��ͣ����
NTSTATUS SuspendProcess(ULONG32 pid)
{
	PEPROCESS pep = LookupProcess((HANDLE)pid);
	NTSTATUS status = PsSuspendProcess(pep);

	switch (status)
	{
	case STATUS_SUCCESS:
	{
		// ����״̬(1������״̬ 0����ͣ״̬)
		g_dwProcStatus = 0;
		break;
	}
	case STATUS_PROCESS_IS_TERMINATING:
	{
		// ����״̬(1������״̬ 0����ͣ״̬)
		g_dwProcStatus = 0;
		break;
	}
	}
	return status;
}

// �ָ�����
NTSTATUS ResumeProcess(ULONG32 pid)
{
	PEPROCESS pep = LookupProcess((HANDLE)pid);
	NTSTATUS status = PsResumeProcess(pep);

	switch (status)
	{
	case STATUS_SUCCESS:
	{
		// ����״̬(1������״̬ 0����ͣ״̬)
		g_dwProcStatus = 1;		
		break;
	}
	case STATUS_PROCESS_IS_TERMINATING:
	{
		// ����״̬(1������״̬ 0����ͣ״̬)
		g_dwProcStatus = 0;
		break;
	}
	}
	return status;
}

// ж��ָ��ģ��
NTSTATUS UninstallModule()
{
	NTSTATUS status;
	status = MmUnmapViewOfSection(g_pModulelUninstall->Eprocess, g_pModulelUninstall->Base);
	return status;
}

// ��ȡ�̱߳�־ƫ�ƣ�����APC��ֹ�߳�
ULONG64 GetThreadFlagsOffset()
{
	PUCHAR cPtr;
	ULONG64 qwOffset;

	for (cPtr = (PUCHAR)PsTerminateSystemThread; cPtr < (PUCHAR)PsTerminateSystemThread + PAGE_SIZE; cPtr++)
	{
		// ���������붨λ
		if (*(PUSHORT)cPtr == 0x80F6)
		{
			qwOffset = *(PULONG64)(cPtr + 2);
			return qwOffset;
		}
	}
	return 0;
}

// APC��ֹ�߳�����
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

// �ս�ָ���߳�
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

	// �̵߳�ַ����Ƿ���Ч
	if (!MmIsAddressValid(pet))
		return bRet;

	// �����ڴ�
	pApc = ExAllocatePoolWithTag(NonPagedPool, sizeof(KAPC), 'apc');
	
	pThread2 = (PKTHREAD)pet;
	// ��ʼ��APC
	
	KeInitializeApc(pApc, pet, OriginalApcEnvironment, KernelTerminateThreadRoutine,
		NULL, NULL, KernelMode, NULL);

	bRet = KeInsertQueueApc(pApc, NULL, NULL, 0);
	return bRet;
}

// �ս�ָ���߳� (WIN7 SP1 X64������汾)
NTSTATUS TerminateThread64(__in ULONG64 tid)
{
	ULONG32 dwCallCode = 0;
	ULONG64 qwAddrOfPsTTBP = 0, qwAddrOfPsTST = 0;
	PETHREAD pet = NULL;
	NTSTATUS status = NULL;
	
	// ��ȡPsTerminateSystemThreadByPointer��ָ��
	if (PspTerminateThreadByPointer ==  NULL)
	{
		// ͨ��Windbg����֪��PsTerminateSystemThreadByPointer��PsTerminateSystemThread + 0x1c������������3������
		// ������Ϊ01e8

		// ��ȡPsTerminateSystemThread�ĺ�����ַ
		qwAddrOfPsTST = (ULONG64)GetFunctionAddr(L"PsTerminateSystemThread");
		if (qwAddrOfPsTST == 0)
		{
			return STATUS_UNSUCCESSFUL;
		}
		// ����PsTerminateSystemThread�������ڴ�����
		for (size_t i = 1; i < 0xff; i++)
		{
			if (MmIsAddressValid((PVOID)(qwAddrOfPsTST + i)) != FALSE)
			{
				if (*(BYTE *)(qwAddrOfPsTST + i) == 0x01 && *(BYTE *)(qwAddrOfPsTST + i + 1) == 0xe8)	
				{
					// �����ڴ�Ϊ 41b001 e8d0590500 -> dwCallCodeӦ�ô��00 05 59 d0
					RtlMoveMemory(&dwCallCode, (PVOID)(qwAddrOfPsTST + i + 2), 4);
					// KdPrint(("[T]dwCallCode=%p qwAddrOfPsTST=%p", dwCallCode, qwAddrOfPsTST));

					// Ŀ���ַ - ԭʼ��ַ - 5 = ������ ==> Ŀ���ַ = ������ + 5 + ԭʼ��ַ
					qwAddrOfPsTTBP = (ULONG64)dwCallCode + 5 + qwAddrOfPsTST + i;
				}
			}
		}
		PspTerminateThreadByPointer = (PSPTERMINATETHREADBYPOINTER)qwAddrOfPsTTBP;
	}

	status = PsLookupThreadByThreadId((HANDLE)tid, &pet);
	// �̵߳�ַ����Ƿ���Ч
	if (!MmIsAddressValid(pet))
		return status;
	PspTerminateThreadByPointer(pet, 0, 1);
	// ���ǵ�����lookup��������ҪDereferenceObject
	ObDereferenceObject(pet);
	return status;
}

// ��ȡ������ַ
PVOID GetFunctionAddr(PCWSTR FunctionName)
{
	UNICODE_STRING usFuncName;
	RtlInitUnicodeString(&usFuncName, FunctionName);
	return MmGetSystemRoutineAddress(&usFuncName);
}

// ��ȡPsSuspendThread��ַ (WIN7 SP1 X64������汾)
VOID GetPsSuspendThreadAddr()
{
	ULONG64 NtSuspendThreadAddr;
	ULONG32 dwCallCode = 0;	// call����Ļ����룬��e8����

	// �Ȼ�ȡNtSuspendThread�ĵ�ַ
	NtSuspendThreadAddr = GetSSDTFunctionAddress64Ex(379);
	if (0 == NtSuspendThreadAddr)
		return;
	if (0 == g_qwPsSuspendThreadAddr)
	{
		// ����NtSuspendThread�������ڴ�����
		for (size_t i = 0; i < 0xff; i++)
		{
			if (FALSE != MmIsAddressValid((PVOID)(NtSuspendThreadAddr + i)))
			{
				if (*(BYTE *)(NtSuspendThreadAddr + i - 1) == 0x24 && *(BYTE *)(NtSuspendThreadAddr + i + 1) == 0xe8)
				{
					// �����ڴ�Ϊ 488b4c24 48e8 695efaff -> dwCallCodeӦ�ô��ff fa 5e 69
					// ���ﲻ����0x48���������룬��Ϊ����rsp��ƫ�ƣ����
					RtlMoveMemory(&dwCallCode, (PVOID)(NtSuspendThreadAddr + i + 2), 4);
					// Ŀ���ַ - ԭʼ��ַ - 5 = ������ ==> Ŀ���ַ = ������ + 5 + ԭʼ��ַ
					g_qwPsSuspendThreadAddr = (ULONG64)dwCallCode + 5 + NtSuspendThreadAddr + i;
					break;
				}
			}
		}
	}
}

// ��ȡKeResumeThread��ַ (WIN7 SP1 X64������汾)
VOID GetKeResumeThreadAddr()
{
	ULONG64 NtResumeThreadAddr;
	ULONG32 dwCallCode = 0;	// call����Ļ����룬��e8����

	// �Ȼ�ȡNtSuspendThread�ĵ�ַ
	NtResumeThreadAddr = GetSSDTFunctionAddress64Ex(79);
	KdPrint(("[T]: %p", NtResumeThreadAddr));
	if (0 == NtResumeThreadAddr)
		return;
	if (0 == g_qwPsResumeThreadAddr)
	{
		// ����NtSuspendThread�������ڴ�����
		for (size_t i = 0; i < 0xff; i++)
		{
			if (FALSE != MmIsAddressValid((PVOID)(NtResumeThreadAddr + i)))
			{
				if (*(BYTE *)(NtResumeThreadAddr + i - 1) == 0x24 && *(BYTE *)(NtResumeThreadAddr + i + 1) == 0xe8)
				{
					// �����ڴ�Ϊ 488b4c24 48e8 695efaff -> dwCallCodeӦ�ô��ff fa 5e 69
					// ���ﲻ����0x48���������룬��Ϊ����rsp��ƫ�ƣ����
					RtlMoveMemory(&dwCallCode, (PVOID)(NtResumeThreadAddr + i + 2), 4);
					// Ŀ���ַ - ԭʼ��ַ - 5 = ������ ==> Ŀ���ַ = ������ + 5 + ԭʼ��ַ
					// g_qwKeResumeThreadAddr = (ULONG64)dwCallCode + 5 + NtResumeThreadAddr + i;
					g_qwPsResumeThreadAddr = 0xfffff800040539b0;
					break;
				}
			}
		}
	}
}

// �����߳�
NTSTATUS SuspendThread(__in ULONG32 tid)
{
	PETHREAD pet;
	PSSUSPENDTHREAD PsSuspendThread = NULL;

	PsLookupThreadByThreadId((HANDLE)tid, &pet);

	// �ж�ETHREAD��Ч��
	if (!MmIsAddressValid(pet))
		return STATUS_UNSUCCESSFUL;
	// �ж�����߳��Ƿ��Ѿ�������
	if (PsIsThreadTerminating(pet))
		return STATUS_UNSUCCESSFUL;
	
	if (0 == g_qwPsSuspendThreadAddr)
		// ��ȡPsSuspendThread��ַ (������汾)
		GetPsSuspendThreadAddr();

	 PsSuspendThread = (PSSUSPENDTHREAD)g_qwPsSuspendThreadAddr;

	 return PsSuspendThread(pet, NULL);
}

// �ָ��߳�
NTSTATUS ResumeThread(__in ULONG32 tid)
{
	PETHREAD pet;
	PSRESUMETHREAD PsResumeThread = NULL;

	PsLookupThreadByThreadId((HANDLE)tid, &pet);

	// �ж�ETHREAD��Ч��
	if (!MmIsAddressValid(pet))
		return STATUS_UNSUCCESSFUL;
	// �ж�����߳��Ƿ��Ѿ�������
	if (PsIsThreadTerminating(pet))
		return STATUS_UNSUCCESSFUL;

	if (0 == g_qwPsResumeThreadAddr)
		// ��ȡPsSuspendThread��ַ (������汾)
		GetKeResumeThreadAddr();

	PsResumeThread = (PSRESUMETHREAD)g_qwPsResumeThreadAddr;

	return PsResumeThread(pet, NULL);
	//return STATUS_SUCCESS;
}
