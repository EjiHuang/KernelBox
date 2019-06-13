#pragma once
#include <ntddk.h>
#include <ntstrsafe.h>

#include "FuncEx.h"

#pragma intrinsic(__readmsr)
//////////////////////////////////////////////////////////////////////////////////////
// 结构体定义

// 声明一个System Service Descriptor Table，我们知道SSDT及SSPT都从这个表中指向
// pragma pack(1)指定以一个字节进行对齐，不指定的话会按照4个字节进行对齐
#pragma pack(1)
typedef struct _ServiceDescriptorEntry {
	unsigned int *ServiceTableBase;
	unsigned int *ServiceCounterTableBase;	//仅适用于checked build版本
	unsigned int NumberOfServices;
	unsigned char *ParamTableBase;
} ServiceDescriptorTableEntry_t, *PServiceDescriptorTableEntry_t;
#pragma pack()

// 全局变量
typedef UINT64(__fastcall *SCFN)(UINT64, UINT64);
SCFN scfn;	// shellcode函数
ULONG32 gSSDT_Func_Index;
ULONG64 gSSDT_Base_Addr;
ULONG64 gSSDT_Func_Addr;
ULONG64 gKiServiceTable;
ULONG64 gServiceTableBase;

// 获取KeServiceDescriptorTable地址，即SSDT基址
ULONG64 GetKeServiceDescriptorTable64();
// 通过结构体获取SSDT指定序号函数地址
ULONG64 GetSSDTFunctionAddress64(ULONG64 qwIndex);
// 
VOID Initxxxx();
// 获取SSDT指定序号函数地址
ULONG64 GetSSDTFunctionAddress64Ex(ULONG64 qwNtApiIndex);