// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;

Show();

Console.ReadKey();

unsafe void Show()
{
    Pointer();

    ConvertBytes();
}

unsafe void Pointer()
{
    int x = 10;
    short y = -1;
    byte y2 = 4;
    double z = 1.5;
    int* pX = &x;
    short* pY = &y;
    double* pZ = &z;

    Console.WriteLine($"Address of x is 0x{(ulong)&x:X}, size is {sizeof(int)}, value is {x}");
    Console.WriteLine($"Address of y is 0x{(ulong)&y:X}, size is {sizeof(short)}, value is {y}");
    Console.WriteLine($"Address of y2 is 0x{(ulong)&y2:X}, size is {sizeof(byte)}, value is {y2}");
    Console.WriteLine($"Address of z is 0x{(ulong)&z:X}, size is {sizeof(double)}, value is {z}");
    Console.WriteLine($"Address of pX=&x is 0x{(ulong)&pX:X}, size is {sizeof(int*)}, value is 0x{(ulong)pX:X}");
    Console.WriteLine($"Address of pY=&y is 0x{(ulong)&pY:X}, size is {sizeof(short*)}, value is 0x{(ulong)pY:X}");
    Console.WriteLine($"Address of pZ=&z is 0x{(ulong)&pZ:X}, size is {sizeof(double*)}, value is 0x{(ulong)pZ:X}");
    *pX = 20;
    Console.WriteLine($"After setting *pX, x = {x}");
    Console.WriteLine($"*pX = {*pX}");

    pZ = (double*)pX;
    Console.WriteLine($"x treated as a double = {*pZ}");
}

void ConvertBytes()
{
    byte[] managedArray = { 1, 2, 3, 4, 5 };
    GCHandle hObject = GCHandle.Alloc(managedArray, GCHandleType.Pinned);
    try
    {
        // byte[] --> IntPtr 
        IntPtr pBuffer = hObject.AddrOfPinnedObject();

        // IntPtr --> byte[]
        int size = managedArray.Length;
        byte[] managedArray2 = new byte[size];
        Marshal.Copy(pBuffer, managedArray2, 0, size);
    }
    finally
    {
        if (hObject.IsAllocated)
            hObject.Free();
    }
}