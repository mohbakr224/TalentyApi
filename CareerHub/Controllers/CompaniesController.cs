using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CareerHub.Controllers
{
    [ApiController]
    [Route("api/v1/companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request)
        {
            var company = await _companyService.CreateAsync(request);
            return Ok(new { success = true, data = company });
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var company = await _companyService.GetByIdAsync(id);
            if (company == null) return NotFound();
            return Ok(new { success = true, data = company });
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
        {
            var companies = await _companyService.SearchAsync(name, limit, offset, HttpContext.RequestAborted);
            return Ok(new { success = true, data = companies });
        }
    }
}
