using System.Diagnostics;
using System.Reflection;
using System.Text;
using BenchmarkDotNet.Running;

namespace SystemFundamentals
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FileCreation.TestFileShareMods();
        }

        private static void CreateFile()
        {
            // Read more about options.
            File.Create("", 123, FileOptions.Encrypted | FileOptions.Asynchronous);
            using var fileStream = File.Open("file.txt", FileMode.OpenOrCreate); // FileMode.Truncate
            fileStream.SetLength(0);
            fileStream.Flush();
            // File.WriteAllText("file.txt", string.Empty);

            using var writer = new StreamWriter(fileStream, Encoding.UTF8);

            Task.Delay(1_000).GetAwaiter().GetResult();

            for (int i = 0; i < 1_000_000; i++)
            {
                writer.WriteLine(Random.Shared.Next().ToString());
            }

            writer.Flush();

            // File.WriteAllLines("file.txt", new[] { "content" });
        }

        private static void ParallelWrite()
        {
            var writer = new ParallelWrite();
            writer.Clear();

            var sw = Stopwatch.StartNew();
            writer.Write();
            sw.Stop();

            var seconds = sw.ElapsedMilliseconds / 1000d;
        }

        private static void LoadDLL()
        {
            // Try to import C++ DLL and call some methods (Windows API).

            var jsonDLL = Assembly.LoadFrom("Newtonsoft.Json.dll");
            var exportedTypes = jsonDLL.GetExportedTypes();
            var jsonConvertType = jsonDLL.GetType("Newtonsoft.Json.JsonConvert", throwOnError: false, ignoreCase: true)!;

            var serialize = jsonConvertType.GetMethod("SerializeObject", new[] { typeof(object) })!;
            var objToSerialize = new JohnDoe { FirstName = "John", LastName = "Doe" };
            var json = serialize.Invoke(null, new[] { objToSerialize });

            var deserialize = jsonConvertType.GetMethod("DeserializeObject", 1, new[] { typeof(string) })!
                .MakeGenericMethod(typeof(JohnDoe));

            var objDeserialized = deserialize.Invoke(null, new[] { json });
        }

        class JohnDoe
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }
        }
    }
}