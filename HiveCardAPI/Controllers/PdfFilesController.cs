using HiveCardAPI.Data;
using HiveCardAPI.Dtos;
using HiveCardAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiveCardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfFilesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PdfFilesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> UploadPdf([FromBody] PdfFileUploadDto dto)
        {
            var pdfFile = new PdfFile
            {
                UserId = dto.UserId,
                FileName = dto.FileName,
                FileUrl = dto.FileUrl,
                UploadedAt = DateTime.UtcNow,
                StatementId = 0 // Set later after parsing
            };

            _context.PdfFiles.Add(pdfFile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = pdfFile.Id }, pdfFile);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var file = await _context.PdfFiles
                .Include(f => f.User)
                .Include(f => f.Statement)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (file == null) return NotFound();
            return Ok(file);
        }
    }


}
