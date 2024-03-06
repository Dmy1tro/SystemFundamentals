using System.Collections.Concurrent;
using System.Text;

namespace SystemFundamentals
{
    internal class ParallelWrite
    {
        private readonly ConcurrentQueue<string> _writeQueue = new ConcurrentQueue<string>();
        private readonly TaskCompletionSource _taskCompletionSource = new TaskCompletionSource();

        public void Clear()
        {
            if (File.Exists("file-parallel.txt"))
                File.WriteAllText("file-parallel.txt", string.Empty);
        }

        public void Write()
        {
            Task.Run(() => CreateText());

            while (!_taskCompletionSource.Task.IsCompleted || !_writeQueue.IsEmpty)
            {
                var file = File.Open("file-parallel.txt", FileMode.Append);
                var writer = new StreamWriter(file, Encoding.UTF8);

                while (_writeQueue.TryDequeue(out var text))
                {
                    writer.WriteLine(text);
                }

                writer.Flush();
                writer.Close();
                file.Close();

                Task.Delay(300).GetAwaiter().GetResult();
            }
        }

        private void CreateText()
        {
            var task1 = Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    _writeQueue.Enqueue(Random.Shared.Next().ToString() + "_task1");
                    Task.Delay(300).GetAwaiter().GetResult();
                }
            });

            var task2 = Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    _writeQueue.Enqueue(Random.Shared.Next().ToString() + "_task2");
                    Task.Delay(300).GetAwaiter().GetResult();
                }
            });

            var task3 = Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    _writeQueue.Enqueue(Random.Shared.Next().ToString() + "_task3");
                    Task.Delay(300).GetAwaiter().GetResult();
                }
            });

            try
            {
                Task.WhenAll(task1, task2, task3).GetAwaiter().GetResult();
            }
            finally
            {
                _taskCompletionSource.SetResult();
            }
        }
    }
}
