#pragma once
#include <ntddk.h>
#include <ntstrsafe.h>

#include "FuncEx.h"

#pragma intrinsic(__readmsr)
//////////////////////////////////////////////////////////////////////////////////////
// �ṹ�嶨��

// ����һ��System Service Descriptor Table������֪��SSDT��SSPT�����������ָ��
// pragma pack(1)ָ����һ���ֽڽ��ж��룬��ָ���Ļ��ᰴ��4���ֽڽ��ж���
#pragma pack(1)
typedef struct _ServiceDescriptorEntry {
	unsigned int *ServiceTableBase;
	unsigned int *ServiceCounterTableBase;	//��������checked build�汾
	unsigned int NumberOfServices;
	unsigned char *ParamTableBase;
} ServiceDescriptorTableEntry_t, *PServiceDescriptorTableEntry_t;
#pragma pack()

// ȫ�ֱ���
typedef UINT64(__fastcall *SCFN)(UINT64, UINT64);
SCFN scfn;	// shellcode����
ULONG32 gSSDT_Func_Index;
ULONG64 gSSDT_Base_Addr;
ULONG64 gSSDT_Func_Addr;
ULONG64 gKiServiceTable;
ULONG64 gServiceTableBase;

// ��ȡKeServiceDescriptorTable��ַ����SSDT��ַ
ULONG64 GetKeServiceDescriptorTable64();
// ͨ���ṹ���ȡSSDTָ����ź�����ַ
ULONG64 GetSSDTFunctionAddress64(ULONG64 qwIndex);
// 
VOID Initxxxx();
// ��ȡSSDTָ����ź�����ַ
ULONG64 GetSSDTFunctionAddress64Ex(ULONG64 qwNtApiIndex);