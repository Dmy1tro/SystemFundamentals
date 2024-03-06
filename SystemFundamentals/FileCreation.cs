using System.Text;

namespace SystemFundamentals
{
    internal class FileCreation
    {
        public static void StreamDocumentation()
        {
            // https://learn.microsoft.com/en-us/dotnet/api/system.io.file.create?view=net-7.0
            // https://learn.microsoft.com/en-us/dotnet/api/system.io.fileoptions?view=net-7.0

            // 220
            // https://github.com/microsoft/referencesource/blob/master/mscorlib/system/io/file.cs
            using var fs = File.Create("file.txt", 4096, FileOptions.SequentialScan | FileOptions.DeleteOnClose);

            // Create benchmarks with different buffer size.
            // Look at experience points.

            // https://devblogs.microsoft.com/oldnewthing/20120120-00/?p=8493
            // https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilea
            // FileOptions.SequentialScan;
            // FileOptions.RandomAccess;

            // 45
            // https://github.com/microsoft/referencesource/blob/master/mscorlib/system/io/file.cs
            using var reader = File.OpenText("file.txt");

            // 227
            // https://github.com/microsoft/referencesource/blob/master/mscorlib/system/io/streamreader.cs
            using var reader2 = new StreamReader("file.txt");

            // 775
            // https://github.com/microsoft/referencesource/blob/master/mscorlib/system/io/file.cs
            using var writer = File.OpenWrite("file.txt");

            // 180
            // 224
            // https://github.com/microsoft/referencesource/blob/master/mscorlib/system/io/streamwriter.cs
            using var writer2 = new StreamWriter("file.txt");

            // Test with different FileAccess (Write and ReadWrite) and try to open for read/write from another proccess
            //  FileStream f = new FileStream(path, mode, FileAccess.Write, FileShare.Read, DefaultFileStreamBufferSize, FileOptions.SequentialScan, Path.GetFileName(path), false, false, checkHost);
        }

        public static void TestFileShareMods()
        {
            var path = "TestFileShare.txt";
            var defaultFileStreamBufferSize = 4096;
            var tcs = new TaskCompletionSource();

            File.WriteAllText(path, string.Empty);

            // ReadWrite
            var t1 = Task.Run(() =>
            {
                using var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, defaultFileStreamBufferSize, FileOptions.SequentialScan);

                for (int i = 0; i < 100; i++)
                {
                    Task.Delay(50).GetAwaiter().GetResult();
                    fs.Write(Encoding.UTF8.GetBytes("1 "));
                }

                tcs.Task.GetAwaiter().GetResult();
            });

            var t2 = Task.Run(() =>
            {
                using var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, defaultFileStreamBufferSize, FileOptions.SequentialScan);

                for (int i = 0; i < 100; i++)
                {
                    Task.Delay(50).GetAwaiter().GetResult();
                    fs.Write(Encoding.UTF8.GetBytes("2 "));
                }

                tcs.Task.GetAwaiter().GetResult();
            });

            // Write
            var t3 = Task.Run(() =>
            {
                using var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, defaultFileStreamBufferSize, FileOptions.SequentialScan);

                for (int i = 0; i < 100; i++)
                {
                    Task.Delay(50).GetAwaiter().GetResult();
                    fs.Write(Encoding.UTF8.GetBytes("3 "));
                }

                tcs.Task.GetAwaiter().GetResult();
            });

            var t4 = Task.Run(() =>
            {
                using var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, defaultFileStreamBufferSize, FileOptions.SequentialScan);

                for (int i = 0; i < 100; i++)
                {
                    Task.Delay(50).GetAwaiter().GetResult();
                    fs.Write(Encoding.UTF8.GetBytes("4 "));
                }

                tcs.Task.GetAwaiter().GetResult();
            });

            // Read
            var t5 = Task.Run(() =>
            {
                using var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite, defaultFileStreamBufferSize, FileOptions.SequentialScan);

                for (int i = 0; i < 100; i++)
                {
                    Task.Delay(50).GetAwaiter().GetResult();

                    // Throws 'Stream does not support writing'
                    // fs.Write(Encoding.UTF8.GetBytes("5 "));
                }

                tcs.Task.GetAwaiter().GetResult();
            });

            tcs.SetResult();
            Task.WhenAll(t1, t2, t3, t4, t5).GetAwaiter().GetResult();

            return;
        }
    }
}
