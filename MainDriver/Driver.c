//驱动开发模板
//模板作者：Tesla.Angela(GDUT.HWL)

#include <ntddk.h>
#include <windef.h>
#include <stdlib.h>
#include <ntimage.h>
#include "Driver.h"

// 驱动卸载
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

// 创建派遣
NTSTATUS DispatchCreate(PDEVICE_OBJECT pDevObj, PIRP pIrp)
{
	pIrp->IoStatus.Status = STATUS_SUCCESS;
	pIrp->IoStatus.Information = 0;
	IoCompleteRequest(pIrp, IO_NO_INCREMENT);
	return STATUS_SUCCESS;
}

// 结束派遣
NTSTATUS DispatchClose(PDEVICE_OBJECT pDevObj, PIRP pIrp)
{
	pIrp->IoStatus.Status = STATUS_SUCCESS;
	pIrp->IoStatus.Information = 0;
	IoCompleteRequest(pIrp, IO_NO_INCREMENT);
	return STATUS_SUCCESS;
}

// 派遣函数
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
		// 测试使用
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
		// 获取进程数目
		case IOCTL_GetProcessNum:
		{
			GetProcessNum();
			ULONG32 size = sizeof(PROCESSINFO);
			KdPrint(("[IOCTL_GetProcessNum]:%d size:%d\n", g_dwProcNum, size));
			RtlCopyMemory(lpIoBuffer, &g_dwProcNum, sizeof(ULONG32));
			status = STATUS_SUCCESS;
			break;
		}
		// 获取进程列表
		case IOCTL_GetProcessList:
		{
			GetProcessList();
			RtlCopyMemory(lpIoBuffer, g_pProcessInfo, sizeof(PROCESSINFO)*g_dwProcNum);
			status = STATUS_SUCCESS;

			ExFreePool(g_pProcessInfo);
			g_dwProcNum = 0;
			break;
		}
		// 获取进程模块
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
		// 获取进程线程
		case IOCTL_GetProcessThread:
		{
			ULONG32 pid;
			RtlCopyMemory(&pid, lpIoBuffer, sizeof(ULONG32));
			status = EnumThread((HANDLE)pid);
			RtlCopyMemory(lpIoBuffer, g_pThreadInfo, sizeof(THREADINFO)*g_dwThreadNum);
			ExFreePool(g_pThreadInfo);
			break;
		}
		// 暂停进程
		case IOCTL_SuspendProcess:
		{
			ULONG32 pid;
			RtlCopyMemory(&pid, lpIoBuffer, sizeof(ULONG32));
			status = SuspendProcess(pid);
			RtlCopyMemory(lpIoBuffer, &g_dwProcStatus, sizeof(ULONG32));
			break;
		}
		// 恢复进程
		case IOCTL_ResumeProcess:
		{
			ULONG32 pid;
			RtlCopyMemory(&pid, lpIoBuffer, sizeof(ULONG32));
			status = ResumeProcess(pid);
			RtlCopyMemory(lpIoBuffer, &g_dwProcStatus, sizeof(ULONG32));
			break;
		}
		// 终止进程
		case IOCTL_TerminateProcess:
		{
			ULONG32 pid;
			RtlCopyMemory(&pid, lpIoBuffer, sizeof(ULONG32));
			status = TerminateProcess(pid);
			break;
		}
		// 卸载进程指定模块
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
		// 终结进程指定线程
		case IOCTL_TerminateThread:
		{
			ULONG32 tid;
			RtlCopyMemory(&tid, lpIoBuffer, sizeof(ULONG32));
			TerminateThread64(tid);
			status = STATUS_SUCCESS;
			break;
		}
		// 挂起进程指定线程
		case IOCTL_SuspendThread:
		{
			ULONG32 tid;
			RtlCopyMemory(&tid, lpIoBuffer, sizeof(ULONG32));
			status = SuspendThread(tid);
			break;
		}
		// 恢复进程指定线程
		case IOCTL_ResumeThread:
		{
			ULONG32 tid;
			RtlCopyMemory(&tid, lpIoBuffer, sizeof(ULONG32));
			status = ResumeThread(tid);
			break;
		}
		// 获取KiServiceTable的地址
		case IOCTL_GetKiSrvTableAddr:
		{
			if (0 == gSSDT_Base_Addr)
				gSSDT_Base_Addr = GetKeServiceDescriptorTable64();
			gKiServiceTable = (ULONG64)((PServiceDescriptorTableEntry_t)gSSDT_Base_Addr)->ServiceTableBase;
			RtlCopyMemory(lpIoBuffer, &gKiServiceTable, sizeof(ULONG64));
			status = STATUS_SUCCESS;
			break;
		}
		// 获取SSDT函数地址
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

// 驱动入口
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