using System;
using System.IO;
using System.Runtime.InteropServices;

using SystemProg;

public class Lab2
{
    private const uint GENERIC_READ = 0x80000000;
    private const uint GENERIC_WRITE = 0x40000000;
    private const uint OPEN_EXISTING = 3;

    [DllImport("kernel32.dll")]
    private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint GetFileSize(IntPtr hFile, out uint lpFileSizeHigh);

    public static void Lab_2()
    {
        IntPtr file_lab_2 = CreateFile("C:\\Users\\tugai\\source\\repos\\chevapchichx\\system_prog\\system_prog\\lab_2.txt", GENERIC_READ | GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
        Console.WriteLine("descriptor: {0}", file_lab_2);

        uint fileSizeHigh;
        uint fileSizeLow = GetFileSize(file_lab_2, out fileSizeHigh);
        long fileSize = ((long)fileSizeHigh << 32) + fileSizeLow;
        Console.WriteLine("file size in bytes: {0}", fileSizeLow);


    }

}
