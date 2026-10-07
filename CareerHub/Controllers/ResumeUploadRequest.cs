using Microsoft.AspNetCore.Http;

namespace CareerHub.Controllers
{
    public class ResumeUploadRequest
    {
        public IFormFile? File { get; set; }

        public string? Name { get; set; }
    }
}
