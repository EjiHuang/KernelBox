#include "Registry.h"

// �½�KEY
NTSTATUS CreateKey(PWCHAR KeyName)
{
	NTSTATUS status;
	HANDLE hReg;
	UNICODE_STRING usKeyName = RTL_CONSTANT_STRING(KeyName);
	OBJECT_ATTRIBUTES oa = RTL_CONSTANT_OBJECT_ATTRIBUTES(&usKeyName, OBJ_CASE_INSENSITIVE);

	__try
	{
		status = ZwCreateKey(&hReg, KEY_ALL_ACCESS, &oa, 0, NULL, REG_OPTION_NON_VOLATILE, NULL);
		ZwClose(hReg);
		return status;
	}
	__except (1)
	{
		KdPrint(("[T]ERROR: CreateKey"));
		return STATUS_UNSUCCESSFUL;
	}
	
}

// ������KEY ��������汾WIN7 X64��
NTSTATUS RenameKey(PWCHAR OldKeyName, PWCHAR ReplacementName)
{
	HANDLE hReg;
	NTSTATUS status;
	UNICODE_STRING usOldKeyName = RTL_CONSTANT_STRING(OldKeyName);
	UNICODE_STRING usReplacementName = RTL_CONSTANT_STRING(ReplacementName);
	OBJECT_ATTRIBUTES oa = RTL_CONSTANT_OBJECT_ATTRIBUTES(&usOldKeyName, OBJ_CASE_INSENSITIVE);

	ZWRENAMEKEY ZwRenameKey = 0xffffffffffffffff;

	__try
	{
		status = ZwOpenKey(&hReg, KEY_ALL_ACCESS, &oa);
		if (!NT_SUCCESS(status))
		{
			return status;
		}
		status = ZwRenameKey(hReg, &usReplacementName);
		ZwFlushKey(hReg);
		ZwClose(hReg);
		return status;
	}
	__except(1)
	{
		KdPrint(("[T]ERROR: RenameKey"));
		return STATUS_UNSUCCESSFUL;
	}
}

// ɾ��KEY
NTSTATUS DeleteKey(PWCHAR KeyName)
{
	NTSTATUS status;
	HANDLE hReg;
	UNICODE_STRING usKeyName = RTL_CONSTANT_STRING(KeyName);
	OBJECT_ATTRIBUTES oa = RTL_CONSTANT_OBJECT_ATTRIBUTES(&usKeyName, OBJ_CASE_INSENSITIVE);

	__try
	{
		status = ZwOpenKey(&hReg, KEY_ALL_ACCESS, &oa);
		if (!NT_SUCCESS(status))
		{
			return status;
		}
		status = ZwDeleteKey(hReg);
		ZwClose(hReg);
		return status;
	}
	__except (1)
	{
		KdPrint(("[T]ERROR: DeleteKey"));
		return STATUS_UNSUCCESSFUL;
	}
}

// �½�/����VALUE
NTSTATUS SetKeyValue(PWCHAR KeyName, PWCHAR ValueName, ULONG32 DataType, PVOID DataBuffer, ULONG32 DataLength)
{
	NTSTATUS status;
	HANDLE hReg;
	ULONG32 dwType;
	UNICODE_STRING usKeyName = RTL_CONSTANT_STRING(KeyName);
	UNICODE_STRING usValueName = RTL_CONSTANT_STRING(ValueName);
	OBJECT_ATTRIBUTES oa = RTL_CONSTANT_OBJECT_ATTRIBUTES(&usKeyName, OBJ_CASE_INSENSITIVE);

	__try
	{
		status = ZwOpenKey(&hReg, KEY_ALL_ACCESS, &oa);
		if (!NT_SUCCESS(status))
		{
			return status;
		}
		status = ZwSetValueKey(hReg, &usValueName, 0, DataType, DataBuffer, DataLength);
		ZwFlushKey(hReg);
		ZwClose(hReg);
		return status;
	}
	__except (1)
	{
		KdPrint(("[T]ERROR: SetKeyValue"));
		return STATUS_UNSUCCESSFUL;
	}
}

// ��ȡVALUE
NTSTATUS QueryKeyValue(PWCHAR KeyName, PWCHAR ValueName, PKEY_VALUE_PARTIAL_INFORMATION *pkvpi)
{
	ULONG32 dwSize;
	NTSTATUS status;
	PKEY_VALUE_PARTIAL_INFORMATION p_kvpi;
	HANDLE hReg;
	UNICODE_STRING usKeyName = RTL_CONSTANT_STRING(KeyName);
	UNICODE_STRING usValueName = RTL_CONSTANT_STRING(ValueName);
	OBJECT_ATTRIBUTES oa = RTL_CONSTANT_OBJECT_ATTRIBUTES(&usKeyName, OBJ_CASE_INSENSITIVE);

	__try 
	{
		status = ZwOpenKey(&hReg, KEY_ALL_ACCESS, &oa);
		if (!NT_SUCCESS(status))
		{
			return status;
		}
		// ��һ�β�ѯ���ڻ�ȡ��С
		status = ZwQueryValueKey(hReg, &usValueName, KeyValuePartialInformation, NULL, 0, &dwSize);
		// ���������δ�ҵ����߷��ش�СΪ0���˳�
		if (STATUS_OBJECT_NAME_NOT_FOUND == status || 0 == dwSize)
		{
			ZwClose(hReg);
			return STATUS_UNSUCCESSFUL;
		}
		// ���ṹ�������ȷ��С���ڴ�
		p_kvpi = (PKEY_VALUE_PARTIAL_INFORMATION)ExAllocatePool(PagedPool, dwSize);
		// �ڶ��β�ѯ����ȡ���Ÿ�ֵ���ṹ����
		status = ZwQueryValueKey(hReg, &usValueName, KeyValuePartialInformation, p_kvpi, dwSize, &dwSize);
		if (!NT_SUCCESS(status))
		{
			ZwClose(hReg);
			ExFreePool(p_kvpi);
			return STATUS_UNSUCCESSFUL;
		}

		*pkvpi = p_kvpi;

		ZwClose(hReg);
		ExFreePool(p_kvpi);
		return status;
	}
	__except (1)
	{
		KdPrint(("[T]ERROR: QueryKeyValue"));
		return STATUS_UNSUCCESSFUL;
	}
}

// ɾ��VALUE
NTSTATUS DeleteKeyValue(__in PWCHAR KeyName, __in PWCHAR ValueName)
{
	NTSTATUS status;
	HANDLE hReg;
	UNICODE_STRING usKeyName = RTL_CONSTANT_STRING(KeyName);
	UNICODE_STRING usValueName = RTL_CONSTANT_STRING(ValueName);
	OBJECT_ATTRIBUTES oa = RTL_CONSTANT_OBJECT_ATTRIBUTES(&usKeyName, OBJ_CASE_INSENSITIVE);

	__try
	{
		status = ZwOpenKey(&hReg, KEY_ALL_ACCESS, &oa);
		if (!NT_SUCCESS(status))
		{
			return STATUS_UNSUCCESSFUL;
		}
		ZwDeleteValueKey(hReg, &usValueName);
		ZwFlushKey(hReg);
		ZwClose(hReg);
	}
	__except (1)
	{
		KdPrint(("[T]ERROR: DeleteKeyValue"));
		return STATUS_UNSUCCESSFUL;
	}
}

// ö����KEY
NTSTATUS EnumSubKey(__in PWCHAR KeyName)
{
	NTSTATUS status;
	HANDLE hReg;
	ULONG32 dwSize;
	UNICODE_STRING usKeyName = RTL_CONSTANT_STRING(KeyName);
	OBJECT_ATTRIBUTES oa = RTL_CONSTANT_OBJECT_ATTRIBUTES(&usKeyName, OBJ_CASE_INSENSITIVE);

	__try
	{
		status = ZwOpenKey(&hReg, KEY_ALL_ACCESS, &oa);
		if (!NT_SUCCESS(status))
		{
			return STATUS_UNSUCCESSFUL;
		}
		// ��һ�β�ѯ���ڻ�ȡ��С
		status = ZwQueryKey(hReg, KeyFullInformation, NULL, 0, &dwSize);
		// ���������δ�ҵ����߷��ش�СΪ0���˳�
		if (STATUS_OBJECT_NAME_NOT_FOUND == status || 0 == dwSize)
		{
			ZwClose(hReg);
			return STATUS_UNSUCCESSFUL;
		}
		// ������ܽṹ�壬���ҷ�����Ӧ��С���ڴ�
		PKEY_FULL_INFORMATION p_kfi = (PKEY_FULL_INFORMATION)ExAllocatePool(PagedPool, dwSize);
		// �ڶ��β�ѯ����ȡ���Ÿ�ֵ���ṹ����
		status = ZwQueryKey(hReg, KeyFullInformation, p_kfi, dwSize, &dwSize);

		// ��������
		for (size_t i = 0; i < p_kfi->SubKeys; i++)
		{
			// ��һ�β�ѯ���ڻ�ȡ��С
			status = ZwEnumerateKey(hReg, i, KeyBasicInformation, NULL, 0, &dwSize);
			if (STATUS_OBJECT_NAME_NOT_FOUND == status || 0 == dwSize)
			{
				ZwClose(hReg);
				return STATUS_UNSUCCESSFUL;
			}
			// ������ܽṹ�壬���ҷ�����Ӧ��С���ڴ�
			PKEY_BASIC_INFORMATION p_kbi = (PKEY_BASIC_INFORMATION)ExAllocatePool(PagedPool, dwSize);
			// �ڶ��β�ѯ����ȡ���Ÿ�ֵ���ṹ����
			status = ZwEnumerateKey(hReg, i, KeyBasicInformation, p_kbi, dwSize, &dwSize);

			UNICODE_STRING usTemp;
			usTemp.Length = usTemp.MaximumLength = p_kbi->NameLength;
			usTemp.Buffer = p_kbi->Name;
			KdPrint(("The %d sub item name:%wZ\n", i, &usTemp));

			ExFreePool(p_kbi);
		}

		
		ExFreePool(p_kfi);
		ZwClose(hReg);
	}
	__except (1)
	{
		KdPrint(("[T]ERROR: EnumSubKey"));
		return STATUS_UNSUCCESSFUL;
	}
}

// ö����VALUE����ʽ��L"\\Registry\\Machine\\Software\\xxxxxxxx"��
NTSTATUS EnumSubValue(PWCHAR KeyName)
{
	NTSTATUS status;
	HANDLE hReg;
	ULONG32 dwSize;
	UNICODE_STRING usKeyName = RTL_CONSTANT_STRING(KeyName);
	OBJECT_ATTRIBUTES oa = RTL_CONSTANT_OBJECT_ATTRIBUTES(&usKeyName, OBJ_CASE_INSENSITIVE);

	__try
	{
		status = ZwOpenKey(&hReg, KEY_ALL_ACCESS, &oa);
		if (!NT_SUCCESS(status))
		{
			return STATUS_UNSUCCESSFUL;
		}
		// ��һ�β�ѯ���ڻ�ȡ��С
		status = ZwQueryKey(hReg, KeyFullInformation, NULL, 0, &dwSize);
		// ���������δ�ҵ����߷��ش�СΪ0���˳�
		if (STATUS_OBJECT_NAME_NOT_FOUND == status || 0 == dwSize)
		{
			ZwClose(hReg);
			return STATUS_UNSUCCESSFUL;
		}
		// ������ܽṹ�壬���ҷ�����Ӧ��С���ڴ�
		PKEY_FULL_INFORMATION p_kfi = (PKEY_FULL_INFORMATION)ExAllocatePool(PagedPool, dwSize);
		// �ڶ��β�ѯ����ȡ���Ÿ�ֵ���ṹ����
		status = ZwQueryKey(hReg, KeyFullInformation, p_kfi, dwSize, &dwSize);
		// �����ṹ��
		for (size_t i = 0; i < p_kfi->Values; i++)
		{
			// ��ѯ���VALUE����ʵ��С�����ڷ����ڴ�
			ZwEnumerateKey(hReg, i, KeyValueBasicInformation, NULL, 0, &dwSize);
			// ������ܽṹ�壬���ҷ�����Ӧ��С���ڴ�
			PKEY_VALUE_BASIC_INFORMATION p_kvbi = (PKEY_VALUE_BASIC_INFORMATION)ExAllocatePool(PagedPool, dwSize);
			
			UNICODE_STRING usTemp;
			usTemp.Length = usTemp.MaximumLength = p_kvbi->NameLength;
			usTemp.Buffer = p_kvbi->Name;
			KdPrint(("The %d sub value name:%wZ\n", i, &usTemp));
			switch (p_kvbi->Type)
			{
			case REG_SZ: {
				KdPrint(("The sub value type:REG_SZ\n"));
				break;
			}
			case REG_MULTI_SZ: {
				KdPrint(("The sub value type:REG_MULTI_SZ\n"));
				break;
			}
			case REG_DWORD: {
				KdPrint(("The sub value type:REG_DWORD\n"));
				break;
			}
			case REG_BINARY: {
				KdPrint(("The sub value type:REG_BINARY\n"));
				break;
			}
			default:
				break;
			}

			ExFreePool(p_kvbi);
		}
		
		ExFreePool(p_kfi);
		ZwClose(hReg);
	}
	__except (1)
	{
		KdPrint(("[T]ERROR: EnumSubValue"));
		return STATUS_UNSUCCESSFUL;
	}
}