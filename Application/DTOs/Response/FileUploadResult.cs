using System;

namespace Application.DTOs.Response
{
    public class FileUploadResult
    {
        public Guid FileId { get; set; }

        public string Url { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long Size { get; set; }
    }
}
