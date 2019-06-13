#pragma once
#include <ntddk.h>
#include <ntstrsafe.h>
#include <ntdef.h>

#include "FuncEx.h"
//////////////////////////////////////////////////////////////////////////////////////
// �ṹ�嶨��


// ����δ������������
typedef NTSTATUS(__fastcall *ZWRENAMEKEY)(IN HANDLE KeyHandle, IN PUNICODE_STRING ReplacementName);



// �½�KEY
NTSTATUS CreateKey(__in PWCHAR KeyName);
// ������KEY ��������汾WIN7 X64��
NTSTATUS RenameKey(__in PWCHAR OldKeyName, __in PWCHAR NewKeyName);
// ɾ��KEY
NTSTATUS DeleteKey(__in PWCHAR KeyName);
// �½�/����VALUE
NTSTATUS SetKeyValue(__in PWCHAR KeyName, __in PWCHAR ValueName, __in ULONG32 DataType, __in PVOID DataBuffer, __in ULONG32 DataLength);
// ��ȡVALUE
NTSTATUS QueryKeyValue(__in PWCHAR KeyName, __in PWCHAR ValueName, __out PKEY_VALUE_PARTIAL_INFORMATION *pkvpi);
// ɾ��VALUE
NTSTATUS DeleteKeyValue(__in PWCHAR KeyName, __in PWCHAR ValueName);
// ö����KEY����ʽ��L"\\Registry\\Machine\\Software\\xxxxxxxx"��
NTSTATUS EnumSubKey(__in PWCHAR KeyName);
// ö����VALUE����ʽ��L"\\Registry\\Machine\\Software\\xxxxxxxx"��
NTSTATUS EnumSubValue(__in PWCHAR KeyName);
