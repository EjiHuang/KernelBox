#pragma once
#include <ntddk.h>
#include <ntstrsafe.h>
#include <ntdef.h>

#include "FuncEx.h"
//////////////////////////////////////////////////////////////////////////////////////
// 结构体定义


// 定义未导出函数类型
typedef NTSTATUS(__fastcall *ZWRENAMEKEY)(IN HANDLE KeyHandle, IN PUNICODE_STRING ReplacementName);



// 新建KEY
NTSTATUS CreateKey(__in PWCHAR KeyName);
// 重命名KEY （特征码版本WIN7 X64）
NTSTATUS RenameKey(__in PWCHAR OldKeyName, __in PWCHAR NewKeyName);
// 删除KEY
NTSTATUS DeleteKey(__in PWCHAR KeyName);
// 新建/设置VALUE
NTSTATUS SetKeyValue(__in PWCHAR KeyName, __in PWCHAR ValueName, __in ULONG32 DataType, __in PVOID DataBuffer, __in ULONG32 DataLength);
// 读取VALUE
NTSTATUS QueryKeyValue(__in PWCHAR KeyName, __in PWCHAR ValueName, __out PKEY_VALUE_PARTIAL_INFORMATION *pkvpi);
// 删除VALUE
NTSTATUS DeleteKeyValue(__in PWCHAR KeyName, __in PWCHAR ValueName);
// 枚举子KEY（格式：L"\\Registry\\Machine\\Software\\xxxxxxxx"）
NTSTATUS EnumSubKey(__in PWCHAR KeyName);
// 枚举子VALUE（格式：L"\\Registry\\Machine\\Software\\xxxxxxxx"）
NTSTATUS EnumSubValue(__in PWCHAR KeyName);
