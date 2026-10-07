using System;

namespace Application.DTOs.Response
{
    public class CompanyResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Industry { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }
    }
}
