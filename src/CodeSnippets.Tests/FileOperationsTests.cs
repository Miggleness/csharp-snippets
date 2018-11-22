using Shouldly;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CodeSnippets.Tests
{
    public class FileOperationsTests : IDisposable
    {
        const string filePath = @".\\test.txt";
        public FileOperationsTests()
        {
            Cleanup();
            Initialize();
        }

        [Fact]
        public async Task Should_write_stream_to_file()
        {   
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write("Hello");
                writer.Flush();

                await FileOperations.WriteFileAsync(filePath, stream);
            }

            File.ReadAllText(filePath).ShouldBe("Hello");
        }

        [Fact]
        public async Task Should_write_string_to_file()
        {
            await FileOperations.WriteFileAsync(filePath, "Hello");

            File.ReadAllText(filePath, Encoding.Unicode).ShouldBe("Hello");
        }

        [Fact]
        public void Should_throw_io_exception_on_file_used_by_other_process()
        {
            var ex = Should.Throw<System.IO.IOException>(() =>
            {
                using (var file = new FileStream(filePath,
                    FileMode.Append, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true))
                {
                    new FileStream(filePath,
                    FileMode.Append, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true);
                };
            });

            ex.Message.ShouldMatch("because it is being used by another process");
        }


        public void Dispose()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            if (File.Exists(filePath)) { File.Delete(filePath); }
        }

        private void Initialize()
        {
            if (File.Exists(filePath)) { File.Delete(filePath); }
        }
    }
}
