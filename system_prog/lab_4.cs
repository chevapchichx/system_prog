using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;

using SystemProg;

public class Lab4 : Form
{
    private DataGridView processGrid = new();
    private DataGridView threadsGrid = new();
    private DataGridView modulesGrid = new();
    private Label systemMemoryLabel = new();
    private Label processMemoryLabel = new();
    private Label currentProcessLabel = new();
    private Button refreshButton;

    [StructLayout(LayoutKind.Sequential)]
    public struct THREADENTRY32
    {
        public uint dwSize;
        public uint cntUsage;
        public uint th32ThreadID;
        public uint th32OwnerProcessID;
        public int tpBasePri;
        public int tpDeltaPri;
        public uint dwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MODULEENTRY32
    {
        public uint dwSize;
        public uint th32ModuleID;
        public uint th32ProcessID;
        public uint GlblcntUsage;
        public uint ProccntUsage;
        public IntPtr modBaseAddr;
        public uint modBaseSize;
        public IntPtr hModule;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szModule;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExePath;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PERFORMANCE_INFORMATION
    {
        public uint cb;
        public UIntPtr CommitTotal;
        public UIntPtr CommitLimit;
        public UIntPtr CommitPeak;
        public UIntPtr PhysicalTotal;
        public UIntPtr PhysicalAvailable;
        public UIntPtr SystemCache;
        public UIntPtr KernelTotal;
        public UIntPtr KernelPaged;
        public UIntPtr KernelNonpaged;
        public UIntPtr PageSize;
        public uint HandleCount;
        public uint ProcessCount;
        public uint ThreadCount;
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

    private const uint TH32CS_SNAPTHREAD = 0x00000004;
    private const uint TH32CS_SNAPMODULE = 0x00000008;
    private const uint TH32CS_SNAPPROCESS = 0x00000002;
    private const uint PROCESS_QUERY_INFORMATION = 0x0400;
    private const uint PROCESS_VM_READ = 0x0010;

    [DllImport("kernel32.dll")]
    static extern bool Thread32First(IntPtr hSnapshot, ref THREADENTRY32 lpte);

    [DllImport("kernel32.dll")]
    static extern bool Thread32Next(IntPtr hSnapshot, ref THREADENTRY32 lpte);

    [DllImport("kernel32.dll")]
    static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

    [DllImport("kernel32.dll")]
    static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

    [DllImport("kernel32.dll")]
    static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

    [DllImport("kernel32.dll")]
    static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

    [DllImport("kernel32.dll")]
    static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

    [DllImport("kernel32.dll")]
    static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll")]
    static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    [DllImport("psapi.dll")]
    static extern bool GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS counters, uint size);

    [DllImport("kernel32.dll")]
    static extern uint GetCurrentProcessId();

    [DllImport("kernel32.dll")]
    static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll")]
    static extern bool DuplicateHandle(
        IntPtr hSourceProcessHandle,
        IntPtr hSourceHandle,
        IntPtr hTargetProcessHandle,
        out IntPtr lpTargetHandle,
        uint dwDesiredAccess,
        bool bInheritHandle,
        uint dwOptions);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    static extern uint GetModuleFileName(IntPtr hModule, [Out] char[] lpFilename, uint nSize);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("psapi.dll")]
    static extern bool GetPerformanceInfo([Out] out PERFORMANCE_INFORMATION PerformanceInformation, uint Size);

    public class ModuleInfo
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public IntPtr Handle { get; private set; }

        public static ModuleInfo GetInfo(string moduleNameOrPath = null, IntPtr moduleHandle = default)
        {
            var info = new ModuleInfo() { Name = "", Path = "" };

            if (moduleHandle != IntPtr.Zero)
            {
                info.Handle = moduleHandle;
                char[] moduleFileName = new char[260];
                if (GetModuleFileName(moduleHandle, moduleFileName, (uint)moduleFileName.Length) > 0)
                {
                    info.Path = new string(moduleFileName).TrimEnd('\0');
                    info.Name = System.IO.Path.GetFileName(info.Path);
                    return info;
                }
            }
            else if (!string.IsNullOrEmpty(moduleNameOrPath))
            {
                info.Handle = GetModuleHandle(moduleNameOrPath);
                if (info.Handle != IntPtr.Zero)
                {
                    char[] moduleFileName = new char[260];
                    if (GetModuleFileName(info.Handle, moduleFileName, (uint)moduleFileName.Length) > 0)
                    {
                        info.Path = new string(moduleFileName).TrimEnd('\0');
                        info.Name = System.IO.Path.GetFileName(info.Path);
                        return info;
                    }
                }
            }

            return null;
        }
    }

    private void ProcessHandles()
    {
        uint currentPID = GetCurrentProcessId();
        currentProcessLabel.Text = $"Текущий процесс (PID: {currentPID})\n";

        IntPtr pseudoHandle = GetCurrentProcess();
            
        IntPtr duplicatedHandle;
        if (DuplicateHandle(
            pseudoHandle,
            pseudoHandle,
            pseudoHandle,
            out duplicatedHandle,
            0,
            false,
            2)) 
        {
            currentProcessLabel.Text += $"Дескриптор DuplicateHandle: {duplicatedHandle}\n";
        }

        IntPtr openedHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, currentPID);
        if (openedHandle != IntPtr.Zero)
        {
            currentProcessLabel.Text += $"Дескриптор OpenProcess: {openedHandle}\n";
        }

        var moduleInfo = ModuleInfo.GetInfo(moduleHandle: IntPtr.Zero);
        if (moduleInfo != null)
        {
            currentProcessLabel.Text += $"Модуль: {moduleInfo.Name}\n" +
                                        $"Путь: {moduleInfo.Path}\n" +
                                        $"Дескриптор модуля: {moduleInfo.Handle}\n";
        }

        LoadModules(currentPID);

        if (duplicatedHandle != IntPtr.Zero) CloseHandle(duplicatedHandle);
        if (openedHandle != IntPtr.Zero) CloseHandle(openedHandle);
    }

    private void UpdateSystemMemoryInfo()
    {
        PERFORMANCE_INFORMATION performanceInfo = new();
        performanceInfo.cb = (uint)Marshal.SizeOf(typeof(PERFORMANCE_INFORMATION));

        if (GetPerformanceInfo(out performanceInfo, performanceInfo.cb))
        {
            ulong pageSize = (ulong)performanceInfo.PageSize;
            ulong totalPhys = (ulong)performanceInfo.PhysicalTotal * pageSize;
            ulong availPhys = (ulong)performanceInfo.PhysicalAvailable * pageSize;
            ulong totalVirtual = (ulong)performanceInfo.CommitLimit * pageSize;
            ulong availVirtual = totalVirtual - ((ulong)performanceInfo.CommitTotal * pageSize);

            systemMemoryLabel.Text = 
                $"Физическая память:\n" +
                $"Всего: {totalPhys / (1024 * 1024):N0} МБ\n" +
                $"Доступно: {availPhys / (1024 * 1024):N0} МБ\n" +
                $"Использовано: {(totalPhys - availPhys) / (1024 * 1024):N0} МБ\n\n" +
                $"Виртуальная память:\n" +
                $"Всего: {totalVirtual / (1024 * 1024):N0} МБ\n" +
                $"Доступно: {availVirtual / (1024 * 1024):N0} МБ\n" +
                $"Использовано: {((ulong)performanceInfo.CommitTotal * pageSize) / (1024 * 1024):N0} МБ";
        }
    }

    private void UpdateProcessList()
    {
        processGrid.Rows.Clear();
        IntPtr snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
        if (snapshot == IntPtr.Zero) return;

        try
        {
            var processEntry = new PROCESSENTRY32 
            { 
                dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32)) 
            };

            if (Process32First(snapshot, ref processEntry))
            {
                do
                {
                    processGrid.Rows.Add(
                        processEntry.szExeFile,
                        processEntry.th32ProcessID,
                        processEntry.cntThreads
                    );
                } while (Process32Next(snapshot, ref processEntry));
            }
        }
        finally
        {
            CloseHandle(snapshot);
        }
    }

    private void LoadThreads(uint processId)
    {
        threadsGrid.Rows.Clear();
        IntPtr snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, processId);
        if (snapshot == IntPtr.Zero) return;

        try
        {
            var threadEntry = new THREADENTRY32 
            { 
                dwSize = (uint)Marshal.SizeOf(typeof(THREADENTRY32)) 
            };

            if (Thread32First(snapshot, ref threadEntry))
            {
                do
                {
                    if (threadEntry.th32OwnerProcessID == processId)
                    {
                        threadsGrid.Rows.Add(
                            threadEntry.th32ThreadID,
                            threadEntry.tpBasePri
                        );
                    }
                } while (Thread32Next(snapshot, ref threadEntry));
            }
        }
        finally
        {
            CloseHandle(snapshot);
        }
    }

    private void LoadModules(uint processId)
    {
        modulesGrid.Rows.Clear();
        IntPtr snapshot = IntPtr.Zero;
        
        try
        {
            snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, processId);
            if (snapshot == IntPtr.Zero)
            {
                MessageBox.Show($"Не удалось получить доступ к модулям процесса {processId}",
                              "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var moduleEntry = new MODULEENTRY32 
            { 
                dwSize = (uint)Marshal.SizeOf(typeof(MODULEENTRY32)) 
            };

            if (Module32First(snapshot, ref moduleEntry))
            {
                do
                {
                    var moduleInfo = ModuleInfo.GetInfo(moduleEntry.szModule);
                    modulesGrid.Rows.Add(
                        moduleEntry.szModule,
                        moduleEntry.szExePath,
                        moduleInfo?.Handle.ToString() ?? "N/A"
                    );
                } while (Module32Next(snapshot, ref moduleEntry));
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке модулей: {ex.Message}",
                          "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (snapshot != IntPtr.Zero)
                CloseHandle(snapshot);
        }
    }

    private void ProcessGrid_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        uint processId = uint.Parse(processGrid.Rows[e.RowIndex].Cells[1].Value.ToString());
        LoadThreads(processId);

        IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, processId);
        if (processHandle == IntPtr.Zero) return;

        try
        {
            if (GetProcessMemoryInfo(processHandle, out PROCESS_MEMORY_COUNTERS memCounters,
                (uint)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS))))
            {
                processMemoryLabel.Text = 
                    $"Память процесса (PID {processId}):\n" +
                    $"Рабочий набор: {memCounters.WorkingSetSize / 1024:N0} КБ\n" +
                    $"Пиковый рабочий набор: {memCounters.PeakWorkingSetSize / 1024:N0} КБ\n" +
                    $"Файл подкачки: {memCounters.PagefileUsage / 1024:N0} КБ\n" +
                    $"Пиковый размер файла подкачки: {memCounters.PeakPagefileUsage / 1024:N0} КБ\n" +
                    $"Количество ошибок страниц: {memCounters.PageFaultCount}";
            }
        }
        finally
        {
            CloseHandle(processHandle);
        }
    }

    private void InitializeComponents()
    {
        this.Size = new Size(800, 640); 
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.Text = "Информация о процессах";

        systemMemoryLabel = new Label
        {
            Location = new Point(10, 10),
            AutoSize = true,
            MaximumSize = new Size(400, 0) 
        };

        processMemoryLabel = new Label
        {
            Location = new Point(10, 80), 
            AutoSize = true,
            MaximumSize = new Size(400, 0)
        };

        currentProcessLabel = new Label
        {
            Location = new Point(420, 10), 
            AutoSize = true,
            MaximumSize = new Size(360, 0) 
        };

        processGrid = new DataGridView
        {
            Location = new Point(10, 160), 
            Size = new Size(770, 120), 
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToOrderColumns = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
        processGrid.Columns.Add("ProcessName", "Имя процесса");
        processGrid.Columns.Add("PID", "ID процесса");
        processGrid.Columns.Add("ThreadCount", "Количество потоков");
        processGrid.CellClick += ProcessGrid_CellClick;

        threadsGrid = new DataGridView
        {
            Location = new Point(10, 300), 
            Size = new Size(770, 120), 
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToOrderColumns = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
        threadsGrid.Columns.Add("ThreadId", "ID потока");
        threadsGrid.Columns.Add("BasePriority", "Базовый приоритет");

        modulesGrid = new DataGridView
        {
            Location = new Point(10, 440), 
            Size = new Size(770, 150),
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToOrderColumns = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
        modulesGrid.Columns.Add("ModuleName", "Имя модуля");
        modulesGrid.Columns.Add("ModulePath", "Путь к модулю");
        modulesGrid.Columns.Add("ModuleHandle", "Дескриптор модуля");


        this.Controls.AddRange(new Control[] {
        systemMemoryLabel,
        processMemoryLabel,
        currentProcessLabel,
        refreshButton,
        processGrid,
        threadsGrid,
        modulesGrid
    });
    }


    public Lab4()
    {
        InitializeComponents();
        UpdateSystemMemoryInfo();
        UpdateProcessList();
        ProcessHandles();
    }

    public static void Lab_4()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Lab4());
    }
}