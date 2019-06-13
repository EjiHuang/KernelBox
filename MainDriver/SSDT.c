#include "SSDT.h"

// 获取KeServiceDescriptorTable地址
ULONG64 GetKeServiceDescriptorTable64()
{
	// 4c是rex前缀，8d是操作码，15是modrm，用来计算的关键是后面四个字节，从15（00 010 101）可以知道这四个字节代表的值其实是相对于下一条指令的地址的偏移
	PUCHAR StartSearchAddress = (PUCHAR)__readmsr(0xC0000082);	// 读取C0000082寄存器，能够得到KiSystemCall64的地址
	PUCHAR EndSearchAddress = StartSearchAddress + 0x500;		// 搜索范围为KiSystemCall64首地址往下500字节
	PUCHAR i = NULL;
	UCHAR b1 = 0, b2 = 0, b3 = 0;
	ULONG32 ulOffset = 0;
	ULONG64 ullAddr = 0;
	for (i = StartSearchAddress; i < EndSearchAddress; i++)
	{
		if (MmIsAddressValid(i) && MmIsAddressValid(i + 1) && MmIsAddressValid(i + 2))
		{
			b1 = *i;
			b2 = *(i + 1);
			b3 = *(i + 2);
			if (0x4c == b1 && 0x8d == b2 && 0x15 == b3)			// 定位特征码4c8d15
			{
				memcpy(&ulOffset, i + 3, 4);						// 定位到4c8d15后，获取他后面4字节，后面的4字节就是获得SSDT地址相对于当前地址的偏移
				ullAddr = (ULONG64)ulOffset + (ULONG64)i + 7;	// i + 7即为当前指令的结束地址，然后加上偏移，便可得到SSDT的基地址
				return ullAddr;
			}
		}
	}
	return 0;
}

// 通过结构体获取SSDT指定序号函数地址
ULONG64 GetSSDTFunctionAddress64(ULONG64 qwIndex)
{
	ULONG32 dwTemp = 0;
	ULONG64 qwTemp = 0, qwRet = 0;
	// PServiceDescriptorTableEntry_t pstcSSDT = (PServiceDescriptorTableEntry_t)GetKeServiceDescriptorTable64();
	if (0 == gServiceTableBase)
	{
		if (0 == gSSDT_Base_Addr)
			gSSDT_Base_Addr = GetKeServiceDescriptorTable64();
		gServiceTableBase = (ULONG64)(((PServiceDescriptorTableEntry_t)gSSDT_Base_Addr)->ServiceTableBase);
	}

	qwTemp = gServiceTableBase + 4 * qwIndex;	// 获取SSDT函数的基地址，每4字节一个
	dwTemp = *(PLONG32)qwTemp;	// 将64位地址转换为32位地址
	dwTemp = dwTemp >> 4;			// 将32位的临时地址右移4位
	qwRet = gServiceTableBase + (ULONG64)dwTemp;
	return qwRet;
}

// 获取这段shellcode函数
VOID Initxxxx()
{
	UCHAR shellcode[37] =
		"\x48\x8B\xC1"     //mov rax, rcx  ;rcx=index
		"\x4C\x8D\x12"     //lea r10,[rdx] ;rdx=ssdt
		"\x8B\xF8"         //mov edi,eax
		"\xC1\xEF\x07"     //shr edi,7
		"\x83\xE7\x20"     //and edi,20h
		"\x4E\x8B\x14\x17" //mov r10, qword ptr [r10+rdi]
		"\x4D\x63\x1C\x82" //movsxd r11,dword ptr [r10+rax*4]
		"\x49\x8B\xC3"     //mov rax,r11
		"\x49\xC1\xFB\x04" //sar r11,4
		"\x4D\x03\xD3"     //add r10,r11
		"\x49\x8B\xC2"     //mov rax,r10
		"\xC3";            //ret
	scfn = (SCFN)ExAllocatePool(NonPagedPool, 37);
	memcpy(scfn, shellcode, 37);
}

// 获取SSDT指定序号函数地址
ULONG64 GetSSDTFunctionAddress64Ex(ULONG64 qwNtApiIndex)
{
	ULONG64 qwRet = 0;
	if (0 == gSSDT_Base_Addr)
	{
		gSSDT_Base_Addr = GetKeServiceDescriptorTable64();
	}
	if (NULL == scfn)
	{
		Initxxxx();
	}
	qwRet = scfn(qwNtApiIndex, gSSDT_Base_Addr);
	return qwRet;
}
