using Microsoft.AspNetCore.Mvc;

namespace Kysect.RemoteFileSystem.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileSystemController : ControllerBase
    {
        private readonly ILogger<FileSystemController> _logger;

        public FileSystemController(ILogger<FileSystemController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        public IActionResult GetOk()
        {
            return Ok();
        }
    }
}