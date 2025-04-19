using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace SystemProg
{
    public class Lab3 : Form
    {
        private DataGridView processGrid = new();
        private Label memoryLoadLabel = new();
        private Label totalPhysLabel = new();
        private Label availPhysLabel = new();
        private Label totalVirtualLabel = new();
        private Label availVirtualLabel = new();

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUS
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_MEMORY_COUNTERS
        {
            public uint cb;
            public uint PageFaultCount;
            public ulong PeakWorkingSetSize;
            public ulong WorkingSetSize;
            public ulong QuotaPeakPagedPoolUsage;
            public ulong QuotaPagedPoolUsage;
            public ulong QuotaPeakNonPagedPoolUsage;
            public ulong QuotaNonPagedPoolUsage;
            public ulong PagefileUsage;
            public ulong PeakPagefileUsage;
        }

        [DllImport("kernel32.dll")]
        static extern void GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll")]
        static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("psapi.dll")]
        static extern bool GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS counters, uint size);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        const uint TH32CS_SNAPPROCESS = 0x00000002;
        const uint PROCESS_QUERY_INFORMATION = 0x0400;

        public Lab3()
        {
            InitializeComponents();
            UpdateMemoryInfo();
            UpdateProcessList();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(540, 470);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Memory Monitor";

            // Memory info labels
            // �������� �������� AutoSize ��� Label, ����� ����� �� ��������.  
            memoryLoadLabel = new Label() { Location = new Point(10, 10), AutoSize = true };
            totalPhysLabel = new Label() { Location = new Point(10, 30), AutoSize = true };
            availPhysLabel = new Label() { Location = new Point(10, 50), AutoSize = true };
            totalVirtualLabel = new Label() { Location = new Point(10, 70), AutoSize = true };
            availVirtualLabel = new Label() { Location = new Point(10, 90), AutoSize = true };

            // Process grid
            processGrid = new DataGridView
            {
                Location = new Point(10, 120),
                Size = new Size(500, 300),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            processGrid.Columns.Add("ProcessName", "Process Name");
            processGrid.Columns.Add("PID", "Process ID");
            processGrid.Columns.Add("ThreadCount", "Thread Count");
            processGrid.Columns.Add("WorkingSet", "Working Set (KB)");
            processGrid.Columns.Add("PageFile", "Page File (KB)");

            this.Controls.AddRange(new Control[] {
                memoryLoadLabel, totalPhysLabel, availPhysLabel,
                totalVirtualLabel, availVirtualLabel, processGrid
            });
        }

        private void UpdateMemoryInfo()
        {
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            memStatus.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));

            if (GlobalMemoryStatusEx(ref memStatus))
            {
                const double MB = 1024.0 * 1024.0;
                memoryLoadLabel.Text = $"Memory Load: {memStatus.dwMemoryLoad}%";
                totalPhysLabel.Text = $"Total Physical Memory: {memStatus.ullTotalPhys / MB:N0} MB";
                availPhysLabel.Text = $"Available Physical Memory: {memStatus.ullAvailPhys / MB:N0} MB";
                totalVirtualLabel.Text = $"Total Virtual Memory: {memStatus.ullTotalVirtual / MB:N0} MB";
                availVirtualLabel.Text = $"Available Virtual Memory: {memStatus.ullAvailVirtual / MB:N0} MB";
            }
        }

        private void UpdateProcessList()
        {
            processGrid.Rows.Clear();
            IntPtr snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

            if (snapshot != IntPtr.Zero)
            {
                PROCESSENTRY32 processEntry = new PROCESSENTRY32();
                processEntry.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));

                if (Process32First(snapshot, ref processEntry))
                {
                    do
                    {
                        IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION, false, processEntry.th32ProcessID);
                        string workingSet = "N/A";
                        string pageFile = "N/A";

                        if (processHandle != IntPtr.Zero)
                        {
                            if (GetProcessMemoryInfo(processHandle, out PROCESS_MEMORY_COUNTERS memCounter,
                                (uint)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS))))
                            {
                                workingSet = (memCounter.WorkingSetSize / 1024).ToString();
                                pageFile = (memCounter.PagefileUsage / 1024).ToString();
                            }
                            CloseHandle(processHandle);
                        }

                        processGrid.Rows.Add(
                            processEntry.szExeFile,
                            processEntry.th32ProcessID,
                            processEntry.cntThreads,
                            workingSet,
                            pageFile
                        );
                    } while (Process32Next(snapshot, ref processEntry));
                }
                CloseHandle(snapshot);
            }
        }

        public static void Lab_3()
        {
            Application.EnableVisualStyles();
            Application.Run(new Lab3());
        }
    }
}