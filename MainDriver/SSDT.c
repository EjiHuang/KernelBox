#include "SSDT.h"

// ��ȡKeServiceDescriptorTable��ַ
ULONG64 GetKeServiceDescriptorTable64()
{
	// 4c��rexǰ׺��8d�ǲ����룬15��modrm����������Ĺؼ��Ǻ����ĸ��ֽڣ���15��00 010 101������֪�����ĸ��ֽڴ����ֵ��ʵ���������һ��ָ��ĵ�ַ��ƫ��
	PUCHAR StartSearchAddress = (PUCHAR)__readmsr(0xC0000082);	// ��ȡC0000082�Ĵ������ܹ��õ�KiSystemCall64�ĵ�ַ
	PUCHAR EndSearchAddress = StartSearchAddress + 0x500;		// ������ΧΪKiSystemCall64�׵�ַ����500�ֽ�
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
			if (0x4c == b1 && 0x8d == b2 && 0x15 == b3)			// ��λ������4c8d15
			{
				memcpy(&ulOffset, i + 3, 4);						// ��λ��4c8d15�󣬻�ȡ������4�ֽڣ������4�ֽھ��ǻ��SSDT��ַ����ڵ�ǰ��ַ��ƫ��
				ullAddr = (ULONG64)ulOffset + (ULONG64)i + 7;	// i + 7��Ϊ��ǰָ��Ľ�����ַ��Ȼ�����ƫ�ƣ���ɵõ�SSDT�Ļ���ַ
				return ullAddr;
			}
		}
	}
	return 0;
}

// ͨ���ṹ���ȡSSDTָ����ź�����ַ
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

	qwTemp = gServiceTableBase + 4 * qwIndex;	// ��ȡSSDT�����Ļ���ַ��ÿ4�ֽ�һ��
	dwTemp = *(PLONG32)qwTemp;	// ��64λ��ַת��Ϊ32λ��ַ
	dwTemp = dwTemp >> 4;			// ��32λ����ʱ��ַ����4λ
	qwRet = gServiceTableBase + (ULONG64)dwTemp;
	return qwRet;
}

// ��ȡ���shellcode����
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

// ��ȡSSDTָ����ź�����ַ
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
