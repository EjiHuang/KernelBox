#include "Registry.h"

// 新建KEY
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

// 重命名KEY （特征码版本WIN7 X64）
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

// 删除KEY
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

// 新建/设置VALUE
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

// 读取VALUE
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
		// 第一次查询用于获取大小
		status = ZwQueryValueKey(hReg, &usValueName, KeyValuePartialInformation, NULL, 0, &dwSize);
		// 如果对象名未找到或者返回大小为0就退出
		if (STATUS_OBJECT_NAME_NOT_FOUND == status || 0 == dwSize)
		{
			ZwClose(hReg);
			return STATUS_UNSUCCESSFUL;
		}
		// 给结构体分配正确大小的内存
		p_kvpi = (PKEY_VALUE_PARTIAL_INFORMATION)ExAllocatePool(PagedPool, dwSize);
		// 第二次查询将获取的信赋值到结构体中
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

// 删除VALUE
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

// 枚举子KEY
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
		// 第一次查询用于获取大小
		status = ZwQueryKey(hReg, KeyFullInformation, NULL, 0, &dwSize);
		// 如果对象名未找到或者返回大小为0就退出
		if (STATUS_OBJECT_NAME_NOT_FOUND == status || 0 == dwSize)
		{
			ZwClose(hReg);
			return STATUS_UNSUCCESSFUL;
		}
		// 定义接受结构体，并且分配相应大小的内存
		PKEY_FULL_INFORMATION p_kfi = (PKEY_FULL_INFORMATION)ExAllocatePool(PagedPool, dwSize);
		// 第二次查询将获取的信赋值到结构体中
		status = ZwQueryKey(hReg, KeyFullInformation, p_kfi, dwSize, &dwSize);

		// 遍历数据
		for (size_t i = 0; i < p_kfi->SubKeys; i++)
		{
			// 第一次查询用于获取大小
			status = ZwEnumerateKey(hReg, i, KeyBasicInformation, NULL, 0, &dwSize);
			if (STATUS_OBJECT_NAME_NOT_FOUND == status || 0 == dwSize)
			{
				ZwClose(hReg);
				return STATUS_UNSUCCESSFUL;
			}
			// 定义接受结构体，并且分配相应大小的内存
			PKEY_BASIC_INFORMATION p_kbi = (PKEY_BASIC_INFORMATION)ExAllocatePool(PagedPool, dwSize);
			// 第二次查询将获取的信赋值到结构体中
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

// 枚举子VALUE（格式：L"\\Registry\\Machine\\Software\\xxxxxxxx"）
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
		// 第一次查询用于获取大小
		status = ZwQueryKey(hReg, KeyFullInformation, NULL, 0, &dwSize);
		// 如果对象名未找到或者返回大小为0就退出
		if (STATUS_OBJECT_NAME_NOT_FOUND == status || 0 == dwSize)
		{
			ZwClose(hReg);
			return STATUS_UNSUCCESSFUL;
		}
		// 定义接受结构体，并且分配相应大小的内存
		PKEY_FULL_INFORMATION p_kfi = (PKEY_FULL_INFORMATION)ExAllocatePool(PagedPool, dwSize);
		// 第二次查询将获取的信赋值到结构体中
		status = ZwQueryKey(hReg, KeyFullInformation, p_kfi, dwSize, &dwSize);
		// 遍历结构体
		for (size_t i = 0; i < p_kfi->Values; i++)
		{
			// 查询这个VALUE的真实大小，用于分配内存
			ZwEnumerateKey(hReg, i, KeyValueBasicInformation, NULL, 0, &dwSize);
			// 定义接受结构体，并且分配相应大小的内存
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