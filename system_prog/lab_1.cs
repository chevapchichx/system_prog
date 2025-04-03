using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using SystemProg;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
struct OSVERSIONINFO
{
    public int dwOSVersionInfoSize;
    public int dwMajorVersion;
    public int dwMinorVersion;
    public int dwBuildNumber;
    public int dwPlatformId;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string szCSDVersion;
    public UInt16 wServicePackMajor;
    public UInt16 wServicePackMinor;
    public UInt16 wSuiteMask;
    public byte wProductType;
    public byte wReserved;
}

public enum SystemMetric
{
    SM_CXSCREEN = 0,  // 0x00
    SM_CYSCREEN = 1,  // 0x01
    SM_CXVSCROLL = 2,  // 0x02
    SM_CYHSCROLL = 3,  // 0x03
    SM_CYCAPTION = 4,  // 0x04
    SM_CXBORDER = 5,  // 0x05
    SM_CYBORDER = 6,  // 0x06
    SM_CXDLGFRAME = 7,  // 0x07
    SM_CXFIXEDFRAME = 7,  // 0x07
    SM_CYDLGFRAME = 8,  // 0x08
    SM_CYFIXEDFRAME = 8,  // 0x08
    SM_CYVTHUMB = 9,  // 0x09
    SM_CXHTHUMB = 10, // 0x0A
    SM_CXICON = 11, // 0x0B
    SM_CYICON = 12, // 0x0C
    SM_CXCURSOR = 13, // 0x0D
    SM_CYCURSOR = 14, // 0x0E
    SM_CYMENU = 15, // 0x0F
    SM_CXFULLSCREEN = 16, // 0x10
    SM_CYFULLSCREEN = 17, // 0x11
    SM_CYKANJIWINDOW = 18, // 0x12
    SM_MOUSEPRESENT = 19, // 0x13
    SM_CYVSCROLL = 20, // 0x14
    SM_CXHSCROLL = 21, // 0x15
    SM_DEBUG = 22, // 0x16
    SM_SWAPBUTTON = 23, // 0x17
    SM_CXMIN = 28, // 0x1C
    SM_CYMIN = 29, // 0x1D
    SM_CXSIZE = 30, // 0x1E
    SM_CYSIZE = 31, // 0x1F
    SM_CXSIZEFRAME = 32, // 0x20
    SM_CXFRAME = 32, // 0x20
    SM_CYSIZEFRAME = 33, // 0x21
    SM_CYFRAME = 33, // 0x21
    SM_CXMINTRACK = 34, // 0x22
    SM_CYMINTRACK = 35, // 0x23
    SM_CXDOUBLECLK = 36, // 0x24
    SM_CYDOUBLECLK = 37, // 0x25
    SM_CXICONSPACING = 38, // 0x26
    SM_CYICONSPACING = 39, // 0x27
    SM_MENUDROPALIGNMENT = 40, // 0x28
    SM_PENWINDOWS = 41, // 0x29
    SM_DBCSENABLED = 42, // 0x2A
    SM_CMOUSEBUTTONS = 43, // 0x2B
    SM_SECURE = 44, // 0x2C
    SM_CXEDGE = 45, // 0x2D
    SM_CYEDGE = 46, // 0x2E
    SM_CXMINSPACING = 47, // 0x2F
    SM_CYMINSPACING = 48, // 0x30
    SM_CXSMICON = 49, // 0x31
    SM_CYSMICON = 50, // 0x32
    SM_CYSMCAPTION = 51, // 0x33
    SM_CXSMSIZE = 52, // 0x34
    SM_CYSMSIZE = 53, // 0x35
    SM_CXMENUSIZE = 54, // 0x36
    SM_CYMENUSIZE = 55, // 0x37
    SM_ARRANGE = 56, // 0x38
    SM_CXMINIMIZED = 57, // 0x39
    SM_CYMINIMIZED = 58, // 0x3A
    SM_CXMAXTRACK = 59, // 0x3B
    SM_CYMAXTRACK = 60, // 0x3C
    SM_CXMAXIMIZED = 61, // 0x3D
    SM_CYMAXIMIZED = 62, // 0x3E
    SM_NETWORK = 63, // 0x3F
    SM_CLEANBOOT = 67, // 0x43
    SM_CXDRAG = 68, // 0x44
    SM_CYDRAG = 69, // 0x45
    SM_SHOWSOUNDS = 70, // 0x46
    SM_CXMENUCHECK = 71, // 0x47
    SM_CYMENUCHECK = 72, // 0x48
    SM_SLOWMACHINE = 73, // 0x49
    SM_MIDEASTENABLED = 74, // 0x4A
    SM_MOUSEWHEELPRESENT = 75, // 0x4B
    SM_XVIRTUALSCREEN = 76, // 0x4C
    SM_YVIRTUALSCREEN = 77, // 0x4D
    SM_CXVIRTUALSCREEN = 78, // 0x4E
    SM_CYVIRTUALSCREEN = 79, // 0x4F
    SM_CMONITORS = 80, // 0x50
    SM_SAMEDISPLAYFORMAT = 81, // 0x51
    SM_IMMENABLED = 82, // 0x52
    SM_CXFOCUSBORDER = 83, // 0x53
    SM_CYFOCUSBORDER = 84, // 0x54
    SM_TABLETPC = 86, // 0x56
    SM_MEDIACENTER = 87, // 0x57
    SM_STARTER = 88, // 0x58
    SM_SERVERR2 = 89, // 0x59
    SM_MOUSEHORIZONTALWHEELPRESENT = 91, // 0x5B
    SM_CXPADDEDBORDER = 92, // 0x5C
    SM_DIGITIZER = 94, // 0x5E
    SM_MAXIMUMTOUCHES = 95, // 0x5F

    SM_REMOTESESSION = 0x1000, // 0x1000
    SM_SHUTTINGDOWN = 0x2000, // 0x2000
    SM_REMOTECONTROL = 0x2001, // 0x2001

    SM_CONVERTABLESLATEMODE = 0x2003,
    SM_SYSTEMDOCKED = 0x2004,
}

[StructLayout(LayoutKind.Sequential)]
struct SYSTEMTIME
{
    [MarshalAs(UnmanagedType.U2)] public short Year;
    [MarshalAs(UnmanagedType.U2)] public short Month;
    [MarshalAs(UnmanagedType.U2)] public short DayOfWeek;
    [MarshalAs(UnmanagedType.U2)] public short Day;
    [MarshalAs(UnmanagedType.U2)] public short Hour;
    [MarshalAs(UnmanagedType.U2)] public short Minute;
    [MarshalAs(UnmanagedType.U2)] public short Second;
    [MarshalAs(UnmanagedType.U2)] public short Milliseconds;

    public SYSTEMTIME(DateTime dt)
    {
        dt = dt.ToUniversalTime();  // SetSystemTime expects the SYSTEMTIME in UTC
        Year = (short)dt.Year;
        Month = (short)dt.Month;
        DayOfWeek = (short)dt.DayOfWeek;
        Day = (short)dt.Day;
        Hour = (short)dt.Hour;
        Minute = (short)dt.Minute;
        Second = (short)dt.Second;
        Milliseconds = (short)dt.Millisecond;
    }
}

[StructLayout(LayoutKind.Sequential)]
struct POINT
{
    public int X;
    public int Y;
}

public class Lab1
{
    [DllImport("kernel32.dll")]
    static extern int GetComputerNameEx(int NameType, StringBuilder buffer, ref Int32 size); 

    [DllImport("advapi32.dll")]
    static extern int GetUserName(StringBuilder buffer, ref Int32 size);

    [DllImport("kernel32.dll")]
    static extern int GetSystemDirectory(StringBuilder buffer, ref Int32 size);

    [DllImport("kernel32.dll")]
    static extern int GetVersionEx(ref OSVERSIONINFO osvi);

    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(SystemMetric smIndex);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SystemParametersInfo(uint uiAction, uint uiParam, StringBuilder pvParam, uint fWinIni);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref uint pvParam, uint fWinIni);

    [DllImport("user32.dll")]
    static extern uint GetSysColor(int nIndex);

    [DllImport("user32.dll")]
    static extern bool SetSysColors(int cElements, int[] lpaElements, uint[] lpaRgbValues);

    [DllImport("kernel32.dll")]
    static extern void GetSystemTime(out SYSTEMTIME t);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCursorPos(out POINT lpPoint);

    public static void Lab_1()
    {
        Int32 size = 256;
        StringBuilder buffer = new StringBuilder(size);
        GetComputerNameEx(4, buffer, ref size);     // NameType = ComputerNameDnsFullyQualified
        Console.WriteLine("computer name: {0}", buffer);

        size = 256;
        StringBuilder buffer_2 = new StringBuilder(size);
        GetUserName(buffer_2, ref size);
        Console.WriteLine("user name: {0}", buffer_2);

        size = 256;
        StringBuilder buffer_3 = new StringBuilder(size);
        GetSystemDirectory(buffer_3, ref size);
        Console.WriteLine("system directory: {0}", buffer_3);

        OSVERSIONINFO osvi = new OSVERSIONINFO();
        osvi.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFO));
        GetVersionEx(ref osvi);
        Console.WriteLine("os version: {0}", osvi.dwBuildNumber);

        SystemMetric smIndex = SystemMetric.SM_CXSCREEN;
        int screen_width = GetSystemMetrics(smIndex);
        Console.WriteLine("screen width: {0}", screen_width);

        SystemMetric smIndex_2 = SystemMetric.SM_CYSCREEN;
        int screen_height = GetSystemMetrics(smIndex_2);
        Console.WriteLine("screen height: {0}", screen_height);

        SystemMetric smIndex_3 = SystemMetric.SM_MOUSEPRESENT;
        int mouse_availability = GetSystemMetrics(smIndex_3);
        Console.WriteLine("mouse availability : {0}", mouse_availability);

        StringBuilder buffer_4 = new StringBuilder(1024);
        const uint SPI_SETDESKWALLPAPER = 0x0073;
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 1024, buffer_4, 0);
        Console.WriteLine("wallpaper path: {0}", buffer_4);

        const uint SPI_GETFONTSMOOTHINGCONTRAST = 0x200C;
        uint contrast = 0;
        SystemParametersInfo(SPI_GETFONTSMOOTHINGCONTRAST, 0, ref contrast, 0);
        Console.WriteLine("contrast: {0}", contrast);

        const uint SPI_GETMOUSESPEED = 0x0070;
        uint mouse_speed = 0;
        SystemParametersInfo(SPI_GETMOUSESPEED, 0, ref mouse_speed, 0);
        Console.WriteLine("mouse speed: {0}", mouse_speed);

        const int COLOUR_BACKGROUND = 1;
        int colour = Convert.ToInt32(GetSysColor(COLOUR_BACKGROUND));
        Color colour_1 = Color.FromArgb(colour & 0xFF, (colour & 0xFF00) >> 8, (colour & 0xFF0000) >> 16);
        Console.WriteLine("background colour: {0}", colour_1);

        int[] elements = {COLOUR_BACKGROUND};
        uint[] colours = {0x0000FF};
        SetSysColors(elements.Length, elements, colours);
        Console.WriteLine("background colour changed to blue");

        const int COLOUR_BACKGROUND_1 = 1;
        int colour_2 = Convert.ToInt32(GetSysColor(COLOUR_BACKGROUND_1));
        Color colour_3 = Color.FromArgb(colour_2 & 0xFF, (colour_2 & 0xFF00) >> 8, (colour_2 & 0xFF0000) >> 16);
        Console.WriteLine("changed background colour: {0}", colour_3);

        SYSTEMTIME date;
        GetSystemTime(out date);
        Console.WriteLine("system date: {0}.{1}.{2}", date.Day, date.Month, date.Year);

        POINT cursor_pos;
        GetCursorPos(out cursor_pos);
        Console.WriteLine("cursor position: {0}, {1}", cursor_pos.X, cursor_pos.Y);
    }
}

