using Kysect.RemoteFileSystem.Core;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.RemoteFileSystem.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileSystemController : ControllerBase
    {
        private readonly IFileSystemAccessor _fileSystemAccessor;

        public FileSystemController(IFileSystemAccessor fileSystemAccessor)
        {
            _fileSystemAccessor = fileSystemAccessor;
        }

        [HttpGet("groups")]
        public ActionResult<IReadOnlyCollection<Group>> GetGroups()
        {
            return Ok(_fileSystemAccessor.GetGroups());
        }

        [HttpGet("groups/{selectedGroup}/submits")]
        public ActionResult<IReadOnlyCollection<StudentSubmit>> GetStudentSubmits(string selectedGroup)
        {
            return Ok(_fileSystemAccessor.GetStudentSubmits(selectedGroup));
        }

        [HttpGet("submits")]
        public ActionResult<StudentSubmitContent> GetSubmitContent(string group, string studentName, string assignmentTitle, string? submitDate)
        {
            return Ok(_fileSystemAccessor.GetSubmitContent(new StudentSubmit(group, studentName, assignmentTitle, submitDate)));
        }
    }
}