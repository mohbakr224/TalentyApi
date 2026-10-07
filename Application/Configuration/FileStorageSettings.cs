namespace Application.Configuration
{
    public class FileStorageSettings
    {
        public string StoragePath { get; set; } = "./storage";

        public string BaseUrl { get; set; } = "http://localhost:5000/files";
    }
}
