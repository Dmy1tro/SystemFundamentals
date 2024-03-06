using System.Text;
using BenchmarkDotNet.Attributes;

namespace SystemFundamentals
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        private const string ReadFilePath = "JohnDoe.txt";
        private const string WriteFilePath = "WriteBecnh.txt";

        [Params(1024, 2048, 4096, 8192)]
        public int BufferSize { get; set; }

        [Benchmark]
        public void Reader()
        {
            using var reader = new StreamReader(ReadFilePath, Encoding.UTF8, true, BufferSize);

            string line;
            while ((line = reader.ReadLine()) != null)
            {

            }
        }

        [Benchmark]
        public void Writer()
        {
            using var writer = new StreamWriter(WriteFilePath, false, Encoding.UTF8, BufferSize);

            for (int i = 0; i < 1_000_000; i++)
            {
                var text = new string('x', 234);
                writer.WriteLine(text);
            }

            // It is not mandatory to call 'Flush'. Anyway writer call it when disposing.
            // line 250
            // https://github.com/microsoft/referencesource/blob/master/mscorlib/system/io/streamwriter.cs
            writer.Flush();
        }
    }
}
