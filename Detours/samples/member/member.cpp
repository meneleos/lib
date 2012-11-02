//////////////////////////////////////////////////////////////////////////////
//
//  Test a detour of a member function (member.cpp of member.exe)
//
//  Microsoft Research Detours Package, Version 3.0.
//
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//
//  By default, C++ member functions use the __thiscall calling convention.
//  In order to Detour a member function, both the trampoline and the detour
//  must have exactly the same calling convention as the target function.
//  Unfortunately, the VC compiler does not support a __thiscall, so the only
//  way to create legal detour and trampoline functions is by making them
//  class members of a "detour" class.
//
//  In addition, C++ does not support converting a pointer to a member
//  function to an arbitrary pointer.  To get a raw pointer, the address of
//  the member function must be moved into a temporrary member-function
//  pointer, then passed by taking it's address, then de-referencing it.
//  Fortunately, the compiler will optimize the code to remove the extra
//  pointer operations.
//
//  If X::Target is a virtual function, the following code will *NOT* work
//  because &X::Target is the address of a thunk that does a virtual call,
//  not the real address of the X::Target.  You can get the real address
//  of X::Target by looking directly in the VTBL for class X, but there
//  is no legal way to 1) get the address of X's VTBL or 2) get the offset
//  of ::Target within that VTBL.  You can of course, figure these out for
//  a particular class and function, but there is no general way to do so.
//
#include <stdio.h>

#include <windows.h>
#include <detours.h>

#include "..\slept\verify.cpp"

//////////////////////////////////////////////////////////////// Target Class.
//
class CMember
{
  public:
    void Target(void);
};

void CMember::Target(void)
{
    printf("  CMember::Target!   (this:%p)\n", this);
}

//////////////////////////////////////////////////////////////// Detour Class.
//
class CDetour /* add ": public CMember" to enable access to member variables... */
{
  public:
    void Mine_Target(void);
    static void (CDetour::* Real_Target)(void);

    // Class shouldn't have any member variables or virtual functions.
};

void CDetour::Mine_Target(void)
{
    printf("  CDetour::Mine_Target! (this:%p)\n", this);
    (this->*Real_Target)();
}

void (CDetour::* CDetour::Real_Target)(void) = (void (CDetour::*)(void))&CMember::Target;

//////////////////////////////////////////////////////////////////////////////
//
int main(int argc, char **argv)
{
    (void)argc;
    (void)argv;

    //////////////////////////////////////////////////////////////////////////
    //

#if (_MSC_VER < 1310)
    void (CMember::* pfTarget)(void) = CMember::Target;
    void (CDetour::* pfMine)(void) = CDetour::Mine_Target;

    Verify("CMember::Target", *(PBYTE*)&pfTarget);
    Verify("*CDetour::Real_Target", *(PBYTE*)&CDetour::Real_Target);
    Verify("CDetour::Mine_Target", *(PBYTE*)&pfMine);
#else
    Verify("CMember::Target", (PBYTE)(&(PBYTE&)CMember::Target));
    Verify("*CDetour::Real_Target", *(&(PBYTE&)CDetour::Real_Target));
    Verify("CDetour::Mine_Target", (PBYTE)(&(PBYTE&)CDetour::Mine_Target));
#endif

    printf("\n");

    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());

#if (_MSC_VER < 1310)
    pfMine = CDetour::Mine_Target;

    DetourAttach(&(PVOID&)CDetour::Real_Target,
                 *(PBYTE*)&pfMine);
#else
    DetourAttach(&(PVOID&)CDetour::Real_Target,
                 (PVOID)(&(PVOID&)CDetour::Mine_Target));
#endif

    LONG l = DetourTransactionCommit();
    printf("DetourTransactionCommit = %d\n", l);
    printf("\n");

#if (_MSC_VER < 1310)
    pfTarget = CMember::Target;
    pfMine = CDetour::Mine_Target;

    Verify("CMember::Target", *(PBYTE*)&pfTarget);
    Verify("*CDetour::Real_Target", *(&(PBYTE&)CDetour::Real_Target));
    Verify("CDetour::Mine_Target", *(PBYTE*)&pfMine);
#else
    Verify("CMember::Target", (PBYTE)(&(PBYTE&)CMember::Target));
    Verify("*CDetour::Real_Target", *(&(PBYTE&)CDetour::Real_Target));
    Verify("CDetour::Mine_Target", (PBYTE)(&(PBYTE&)CDetour::Mine_Target));
#endif
    printf("\n");

    //////////////////////////////////////////////////////////////////////////
    //
    CMember target;

    printf("Calling CMember (w/o Detour):\n");
    (((CDetour*)&target)->*CDetour::Real_Target)();

    printf("Calling CMember (will be detoured):\n");
    target.Target();

    return 0;
}

