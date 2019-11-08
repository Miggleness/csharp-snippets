using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileOperationsAsync
{
    public static class WriteFileOperation
    {
        public static async Task WriteFileAsync(string filePath, Stream content)
        {
            using (var file = new FileStream(filePath,
                FileMode.OpenOrCreate, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                content.Seek(0, SeekOrigin.Begin);
                await content.CopyToAsync(file);
            };

        }

        public static async Task WriteFileAsync(string filePath, string content)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(content);

            using (var file = new FileStream(filePath,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await file.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }
    }
}
