//��������ģ��
//ģ�����ߣ�Tesla.Angela(GDUT.HWL)

#include <ntddk.h>
#include <windef.h>
#include <stdlib.h>
#include <ntimage.h>
#include "Driver.h"

// ����ж��
VOID DriverUnload(PDRIVER_OBJECT pDriverObj)
{	
	UNICODE_STRING strLink;
	if (IoIsWdmVersionAvailable(1, 0x10))
		RtlInitUnicodeString(&strLink, LINK_GLOBAL_NAME);
	else
		RtlInitUnicodeString(&strLink, LINK_NAME);
	IoDeleteSymbolicLink(&strLink);
	IoDeleteDevice(pDriverObj->DeviceObject);
	DbgPrint("[DriverUnload]\n");
}

// ������ǲ
NTSTATUS DispatchCreate(PDEVICE_OBJECT pDevObj, PIRP pIrp)
{
	pIrp->IoStatus.Status = STATUS_SUCCESS;
	pIrp->IoStatus.Information = 0;
	IoCompleteRequest(pIrp, IO_NO_INCREMENT);
	return STATUS_SUCCESS;
}

// ������ǲ
NTSTATUS DispatchClose(PDEVICE_OBJECT pDevObj, PIRP pIrp)
{
	pIrp->IoStatus.Status = STATUS_SUCCESS;
	pIrp->IoStatus.Information = 0;
	IoCompleteRequest(pIrp, IO_NO_INCREMENT);
	return STATUS_SUCCESS;
}

// ��ǲ����
NTSTATUS DispatchIoctl(PDEVICE_OBJECT pDevObj, PIRP pIrp)
{
	NTSTATUS status = STATUS_INVALID_DEVICE_REQUEST;
	PIO_STACK_LOCATION pIrpStack;
	ULONG uIoControlCode;
	PVOID lpIoBuffer;
	ULONG uInSize;
	ULONG uOutSize;
	//
	pIrpStack = IoGetCurrentIrpStackLocation(pIrp);
	uIoControlCode = pIrpStack->Parameters.DeviceIoControl.IoControlCode;
	lpIoBuffer = pIrp->AssociatedIrp.SystemBuffer;
	uInSize = pIrpStack->Parameters.DeviceIoControl.InputBufferLength;
	uOutSize = pIrpStack->Parameters.DeviceIoControl.OutputBufferLength;
	//
	switch(uIoControlCode)
	{
		// ����ʹ��
		case IOCTL_TEST:
		{
			DWORD dw;
			memcpy(&dw,lpIoBuffer,sizeof(dw));
			DbgPrint("[IOCTL_TEST]dw=%ld\n",dw);
			dw++;
			memcpy(lpIoBuffer,&dw,sizeof(dw));
			status = STATUS_SUCCESS;
			break;
		}
		// ��ȡ������Ŀ
		case IOCTL_GetProcessNum:
		{
			GetProcessNum();
			ULONG32 size = sizeof(PROCESSINFO);
			KdPrint(("[IOCTL_GetProcessNum]:%d size:%d\n", g_dwProcNum, size));
			RtlCopyMemory(lpIoBuffer, &g_dwProcNum, sizeof(ULONG32));
			status = STATUS_SUCCESS;
			break;
		}
		// ��ȡ�����б�
		case IOCTL_GetProcessList:
		{
			GetProcessList();
			RtlCopyMemory(lpIoBuffer, g_pProcessInfo, sizeof(PROCESSINFO)*g_dwProcNum);
			status = STATUS_SUCCESS;

			ExFreePool(g_pProcessInfo);
			g_dwProcNum = 0;
			break;
		}
		// ��ȡ����ģ��
		case IOCTL_GetProcessModules:
		{
			ULONG32 pid;
			RtlCopyMemory(&pid, lpIoBuffer, sizeof(ULONG32));
			KdPrint(("pid:%d\n", pid));
			status = EnumModule((HANDLE)pid);
			RtlCopyMemory(lpIoBuffer, g_pModuleInfo, sizeof(MODULEINFO)*g_dwModuleNum);
			ExFreePool(g_pModuleInfo);
			break;
		}
		// ��ȡ�����߳�
		case IOCTL_GetProcessThread:
		{
			ULONG32 pid;
			RtlCopyMemory(&pid, lpIoBuffer, sizeof(ULONG32));
			status = EnumThread((HANDLE)pid);
			RtlCopyMemory(lpIoBuffer, g_pThreadInfo, sizeof(THREADINFO)*g_dwThreadNum);
			ExFreePool(g_pThreadInfo);
			break;
		}
		// ��ͣ����
		case IOCTL_SuspendProcess:
		{
			ULONG32 pid;
			RtlCopyMemory(&pid, lpIoBuffer, sizeof(ULONG32));
			status = SuspendProcess(pid);
			RtlCopyMemory(lpIoBuffer, &g_dwProcStatus, sizeof(ULONG32));
			break;
		}
		// �ָ�����
		case IOCTL_ResumeProcess:
		{
			ULONG32 pid;
			RtlCopyMemory(&pid, lpIoBuffer, sizeof(ULONG32));
			status = ResumeProcess(pid);
			RtlCopyMemory(lpIoBuffer, &g_dwProcStatus, sizeof(ULONG32));
			break;
		}
		// ��ֹ����
		case IOCTL_TerminateProcess:
		{
			ULONG32 pid;
			RtlCopyMemory(&pid, lpIoBuffer, sizeof(ULONG32));
			status = TerminateProcess(pid);
			break;
		}
		// ж�ؽ���ָ��ģ��
		case IOCTL_UninstallModule:
		{
			if (!(g_pModulelUninstall = (PMODULEUNINSTALL)ExAllocatePoolWithTag(PagedPool, sizeof(MODULEUNINSTALL), 'MUI')))
			{
				status = STATUS_UNSUCCESSFUL;
				break;
			}
			RtlZeroMemory(g_pModulelUninstall, sizeof(MODULEUNINSTALL));
			RtlCopyMemory(&g_pModulelUninstall, lpIoBuffer, sizeof(MODULEUNINSTALL));
			status = UninstallModule();
			break;
		}
		// �ս����ָ���߳�
		case IOCTL_TerminateThread:
		{
			ULONG32 tid;
			RtlCopyMemory(&tid, lpIoBuffer, sizeof(ULONG32));
			TerminateThread64(tid);
			status = STATUS_SUCCESS;
			break;
		}
		// �������ָ���߳�
		case IOCTL_SuspendThread:
		{
			ULONG32 tid;
			RtlCopyMemory(&tid, lpIoBuffer, sizeof(ULONG32));
			status = SuspendThread(tid);
			break;
		}
		// �ָ�����ָ���߳�
		case IOCTL_ResumeThread:
		{
			ULONG32 tid;
			RtlCopyMemory(&tid, lpIoBuffer, sizeof(ULONG32));
			status = ResumeThread(tid);
			break;
		}
		// ��ȡKiServiceTable�ĵ�ַ
		case IOCTL_GetKiSrvTableAddr:
		{
			if (0 == gSSDT_Base_Addr)
				gSSDT_Base_Addr = GetKeServiceDescriptorTable64();
			gKiServiceTable = (ULONG64)((PServiceDescriptorTableEntry_t)gSSDT_Base_Addr)->ServiceTableBase;
			RtlCopyMemory(lpIoBuffer, &gKiServiceTable, sizeof(ULONG64));
			status = STATUS_SUCCESS;
			break;
		}
		// ��ȡSSDT������ַ
		case IOCTL_GetFuncAddr:
		{
			RtlCopyMemory(&gSSDT_Func_Index, lpIoBuffer, sizeof(ULONG32));
			gSSDT_Func_Addr = GetSSDTFunctionAddress64Ex((ULONG64)gSSDT_Func_Index);
			RtlCopyMemory(lpIoBuffer, &gSSDT_Func_Addr, sizeof(ULONG64));
			status = STATUS_SUCCESS;
			break;
		}
	}
	if(status == STATUS_SUCCESS)
		pIrp->IoStatus.Information = uOutSize;
	else
		pIrp->IoStatus.Information = 0;	
	pIrp->IoStatus.Status = status;
	IoCompleteRequest(pIrp, IO_NO_INCREMENT);
	return status;
}

// �������
NTSTATUS DriverEntry(PDRIVER_OBJECT pDriverObj, PUNICODE_STRING pRegistryString)
{
	NTSTATUS status = STATUS_SUCCESS;
	UNICODE_STRING ustrLinkName;
	UNICODE_STRING ustrDevName;  
	PDEVICE_OBJECT pDevObj;
	//
	pDriverObj->MajorFunction[IRP_MJ_CREATE] = DispatchCreate;
	pDriverObj->MajorFunction[IRP_MJ_CLOSE] = DispatchClose;
	pDriverObj->MajorFunction[IRP_MJ_DEVICE_CONTROL] = DispatchIoctl;
	pDriverObj->DriverUnload = DriverUnload;
	//
	RtlInitUnicodeString(&ustrDevName, DEVICE_NAME);
	status = IoCreateDevice(pDriverObj, 0, &ustrDevName, FILE_DEVICE_UNKNOWN, 0, FALSE, &pDevObj);
	if(!NT_SUCCESS(status))	return status;
	if(IoIsWdmVersionAvailable(1, 0x10))
		RtlInitUnicodeString(&ustrLinkName, LINK_GLOBAL_NAME);
	else
		RtlInitUnicodeString(&ustrLinkName, LINK_NAME);
	status = IoCreateSymbolicLink(&ustrLinkName, &ustrDevName);  	
	if(!NT_SUCCESS(status))
	{
		IoDeleteDevice(pDevObj); 
		return status;
	}
	DbgPrint("[DriverEntry]\n");

	return STATUS_SUCCESS;
}