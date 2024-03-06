using System.Runtime.InteropServices;
using System.Text.Json;

namespace SystemFundamentals
{

    internal class PInvoke
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr CreateWindow(
           string lpClassName,
           string lpWindowName,
           uint dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        [DllImport("user32.dll")]
        static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool Beep(int freq, int duration);

        [DllImport("User32.dll", ExactSpelling = true)]
        static extern bool MessageBeep(uint type);
        public enum BeepType : uint
        {
            /// <summary>
            /// A simple windows beep
            /// </summary>            
            SimpleBeep = 0xFFFFFFFF,
            /// <summary>
            /// A standard windows OK beep
            /// </summary>
            OK = 0x00,
            /// <summary>
            /// A standard windows Question beep
            /// </summary>
            Question = 0x20,
            /// <summary>
            /// A standard windows Exclamation beep
            /// </summary>
            Exclamation = 0x30,
            /// <summary>
            /// A standard windows Asterisk beep
            /// </summary>
            Asterisk = 0x40,
        }

        [DllImport("User32.dll")]
        public static extern int MessageBox(int h, string m, string c, int type);

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public uint dwOemId;
            public uint dwPageSize;
            public uint lpMinimumApplicationAddress;
            public uint lpMaximumApplicationAddress;
            public uint dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public uint dwProcessorLevel;
            public uint dwProcessorRevision;

            public string ToJson()
            {
                return JsonSerializer.Serialize(new Dictionary<string, uint>
                {
                    ["dwOemId"] = dwOemId,
                    ["dwPageSize"] = dwPageSize,
                    ["lpMinimumApplicationAddress"] = lpMinimumApplicationAddress,
                    ["lpMaximumApplicationAddress"] = lpMaximumApplicationAddress,
                    ["dwActiveProcessorMask"] = dwActiveProcessorMask,
                    ["dwNumberOfProcessors"] = dwNumberOfProcessors,
                    ["dwProcessorType"] = dwProcessorType,
                    ["dwAllocationGranularity"] = dwAllocationGranularity,
                    ["dwProcessorLevel"] = dwProcessorLevel,
                    ["dwProcessorRevision"] = dwProcessorRevision,
                });
            }
        }

        [DllImport("kernel32")]
        static extern void GetSystemInfo(ref SYSTEM_INFO pSI);

        [DllImport("user32.dll")]
        static extern int ShowCursor(bool bShow);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CopyFileEx(
            string lpExistingFileName,
            string lpNewFileName,
            CopyProgressRoutine lpProgressRoutine,
            IntPtr lpData,
            ref Int32 pbCancel,
            CopyFileFlags dwCopyFlags);

        delegate CopyProgressResult CopyProgressRoutine(
            long TotalFileSize,
            long TotalBytesTransferred,
            long StreamSize,
            long StreamBytesTransferred,
            uint dwStreamNumber,
            CopyProgressCallbackReason dwCallbackReason,
            IntPtr hSourceFile,
            IntPtr hDestinationFile,
            IntPtr lpData);

        enum CopyProgressResult : uint
        {
            PROGRESS_CONTINUE = 0,
            PROGRESS_CANCEL = 1,
            PROGRESS_STOP = 2,
            PROGRESS_QUIET = 3
        }

        enum CopyProgressCallbackReason : uint
        {
            CALLBACK_CHUNK_FINISHED = 0x00000000,
            CALLBACK_STREAM_SWITCH = 0x00000001
        }

        [Flags]
        enum CopyFileFlags : uint
        {
            COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
            COPY_FILE_RESTARTABLE = 0x00000002,
            COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
            COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008
        }

        public static void Example()
        {
            // http://pinvoke.net/default.aspx/user32.CreateWindow
            // https://www.mono-project.com/docs/advanced/pinvoke/
            // https://www.mono-project.com/docs/advanced/com-interop/

            // https://learn.microsoft.com/en-us/windows/win32/learnwin32/creating-a-window
            // https://riptutorial.com/winapi/example/9373/creating-a-window
            //CreateWindow();

            var sysInfo = new SYSTEM_INFO();
            GetSystemInfo(ref sysInfo);

            MessageBeep((uint)BeepType.OK);
            MessageBeep((uint)BeepType.Question);
            MessageBeep((uint)BeepType.SimpleBeep);
            MessageBeep((uint)BeepType.Asterisk);
            MessageBeep((uint)BeepType.Exclamation);

            Beep(1000, 1000);

            MessageBox(0, sysInfo.ToJson(), "System info", 0);

            ShowCursor(false);
            ShowCursor(true);

            var oldFile = @"C:\Projects\Education\PlayWithCopy\CopyMe.txt";
            var newFile = @"C:\Projects\Education\PlayWithCopy\Here\CopyMe.txt";
            int pbCancel = 0;

            CopyFileEx(
                oldFile,
                newFile,
                (_, _, _, _, _, _, _, _, _) => CopyProgressResult.PROGRESS_CONTINUE,
                IntPtr.Zero,
                ref pbCancel,
                CopyFileFlags.COPY_FILE_RESTARTABLE);

        }
    }
}
