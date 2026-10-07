using Application.DTOs.Response;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IFileService
    {
        Task<FileUploadResult> UploadAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken);

        Task DeleteAsync(Guid fileId, CancellationToken cancellationToken);

        Task<string> GetUrlAsync(Guid fileId, CancellationToken cancellationToken);

        // Retrieve the stored file as a read-only stream. Returns null if file not found.
        Task<System.IO.Stream?> GetAsync(Guid fileId, CancellationToken cancellationToken);
    }
}
