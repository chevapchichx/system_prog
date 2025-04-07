using System;
using System.IO;
using System.Runtime.InteropServices;

using SystemProg;

public class Lab2
{
    private const uint GENERIC_READ = 0x80000000;
    private const uint GENERIC_WRITE = 0x40000000;
    private const uint OPEN_EXISTING = 3;
    private const uint FILE_MAP_READ = 0x0004;
    private const uint FILE_MAP_WRITE = 0x0002;

    [DllImport("kernel32.dll")]
    private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetFileSize(IntPtr hFile, out IntPtr lpFileSizeHigh);

    [DllImport("kernel32.dll")]
    private static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

    [DllImport("kernel32.dll")]
    private static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, UIntPtr dwNumberOfBytesToMap);

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
    private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

    [DllImport("kernel32.dll")]
    //[return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

    [DllImport("kernel32.dll")]
    //[return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr hObject);


    public static void Lab_2()
    {
        IntPtr file_lab_2 = CreateFile("C:\\Users\\tugai\\source\\repos\\chevapchichx\\system_prog\\system_prog\\lab_2.txt", GENERIC_READ | GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
        Console.WriteLine("file descriptor: {0}", file_lab_2);

        IntPtr file_size = GetFileSize(file_lab_2, out file_size);
        Console.WriteLine("file size in bytes: {0}", file_size);

        IntPtr file_mapping = CreateFileMapping(file_lab_2, IntPtr.Zero, 0x04, 0, 0, string.Empty);
        Console.WriteLine("file mapping descriptor: {0}", file_mapping);

        IntPtr file_map_view = MapViewOfFile(file_mapping, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, UIntPtr.Zero);
        Console.WriteLine("mapped memory area: {0}", file_map_view);

        IntPtr data_ptr = Marshal.AllocHGlobal((int)file_size);
        CopyMemory(data_ptr, file_map_view, (uint)file_size);

        byte[] data = new byte[file_size];
        Marshal.Copy(data_ptr, data, 0, (int)file_size);

        string original_text = System.Text.Encoding.Default.GetString(data);
        char[] char_data = original_text.ToCharArray();
        Array.Sort(char_data);
        byte[] sorted_data = System.Text.Encoding.Default.GetBytes(char_data);

        Marshal.Copy(sorted_data, 0, data_ptr, (int)file_size);
        CopyMemory(file_map_view, data_ptr, (uint)file_size);
        Console.WriteLine("text is sorted alphabetically and written back to file");

        Console.WriteLine("original text: {0}", original_text);
        string sorted_text = System.Text.Encoding.Default.GetString(sorted_data);
        Console.WriteLine("alphabetically sorted text: {0}", sorted_text);

        UnmapViewOfFile(file_map_view);
        Console.WriteLine("mapped memory was released successfully");

        CloseHandle(file_mapping);
        Console.WriteLine("file mapping closed successfully");

        CloseHandle(file_lab_2);
        Console.WriteLine("file closed successfully");
    }

}
