using Application.Configuration;
using Application.DTOs.Response;
using Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.Storage
{
    public class LocalFileStorageService : IFileService
    {
        private readonly FileStorageSettings _settings;

        public LocalFileStorageService(IOptions<FileStorageSettings> options)
        {
            _settings = options.Value;

            if (!Directory.Exists(_settings.StoragePath))
                Directory.CreateDirectory(_settings.StoragePath);
        }

        public async Task<FileUploadResult> UploadAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            var safeName = Path.GetFileName(fileName);
            var storedFileName = id + "_" + safeName;
            var path = Path.Combine(_settings.StoragePath, storedFileName);

            using (var fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await content.CopyToAsync(fs, cancellationToken);
            }

            var url = new Uri(new Uri(_settings.BaseUrl.TrimEnd('/')), storedFileName).ToString();

            var info = new FileInfo(path);

            return new FileUploadResult
            {
                FileId = id,
                Url = url,
                FileName = safeName,
                ContentType = contentType,
                Size = info.Length
            };
        }

        public Task DeleteAsync(Guid fileId, CancellationToken cancellationToken)
        {
            var dir = new DirectoryInfo(_settings.StoragePath);
            var file = dir.GetFiles().FirstOrDefault(f => f.Name.StartsWith(fileId.ToString() + "_"));

            if (file != null)
                file.Delete();

            return Task.CompletedTask;
        }

        public Task<string> GetUrlAsync(Guid fileId, CancellationToken cancellationToken)
        {
            var dir = new DirectoryInfo(_settings.StoragePath);
            var file = dir.GetFiles().FirstOrDefault(f => f.Name.StartsWith(fileId.ToString() + "_"));

            if (file == null) return Task.FromResult(string.Empty);

            var url = new Uri(new Uri(_settings.BaseUrl.TrimEnd('/')), file.Name).ToString();
            return Task.FromResult(url);
        }

        public Task<Stream?> GetAsync(Guid fileId, CancellationToken cancellationToken)
        {
            var dir = new DirectoryInfo(_settings.StoragePath);
            var file = dir.GetFiles().FirstOrDefault(f => f.Name.StartsWith(fileId.ToString() + "_"));

            if (file == null) return Task.FromResult<Stream?>(null);

            Stream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult<Stream?>(stream);
        }
    }
}
