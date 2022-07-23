namespace Kysect.RemoteFileSystem.Core;

public interface IFileSystemAccessor
{
    IReadOnlyCollection<Group> GetGroups();
    IReadOnlyCollection<StudentSubmit> GetStudentSubmits(string selectedGroup);
    StudentSubmitContent GetSubmitContent(StudentSubmit studentSubmit);
}

public class FileSystemAccessor : IFileSystemAccessor
{
    private readonly string _rootPath;

    public FileSystemAccessor(string rootPath)
    {
        _rootPath = rootPath;
    }

    public IReadOnlyCollection<Group> GetGroups()
    {
        return Directory
            .EnumerateDirectories(_rootPath)
            .Select(p => new DirectoryInfo(p))
            .Select(d => new Group(d.Name, GetChildDirectoryNames(d.FullName)))
            .ToList();
    }

    public IReadOnlyCollection<StudentSubmit> GetStudentSubmits(string selectedGroup)
    {
        List<StudentSubmit> result = new List<StudentSubmit>();

        string groupDirectory = Path.Combine(_rootPath, selectedGroup);
        string groupName = new DirectoryInfo(groupDirectory).Name;
        foreach (string studentDirectory in Directory.EnumerateDirectories(groupDirectory))
        {
            string studentName = new DirectoryInfo(studentDirectory).Name;
            foreach (var assignmentDirectory in Directory.EnumerateDirectories(studentDirectory))
            {
                string assignmentName = new DirectoryInfo(assignmentDirectory).Name;
                List<StudentSubmit> submitDirectories = GetChildDirectoryNames(assignmentDirectory)
                    .Select(s => new StudentSubmit(groupName, studentName, assignmentName, s))
                    .ToList();

                result.AddRange(submitDirectories);
            }
        }

        return result;
    }

    public StudentSubmitContent GetSubmitContent(StudentSubmit studentSubmit)
    {
        string pathToContent = Path.Combine(
            _rootPath,
            studentSubmit.Group,
            studentSubmit.StudentName,
            studentSubmit.AssignmentTitle,
            studentSubmit.SubmitDate);

        List<StudentSubmitFileInfo> submitFiles = Directory
            .EnumerateFiles(pathToContent, "*", SearchOption.AllDirectories)
            .Select(f => new StudentSubmitFileInfo(Path.GetRelativePath(pathToContent, f), File.ReadAllLines(f)))
            .ToList();

        return new StudentSubmitContent(submitFiles);
    }

    private IReadOnlyCollection<string> GetChildDirectoryNames(string directoryPath)
    {
        return Directory
            .EnumerateDirectories(directoryPath)
            .Select(p => new DirectoryInfo(p))
            .Select(d => d.Name)
            .ToList();
    }
}

public record struct Group(string Name, IReadOnlyCollection<string> Students);
public record struct StudentSubmit(string Group, string StudentName, string AssignmentTitle, string SubmitDate);
public record struct StudentSubmitContent(IReadOnlyCollection<StudentSubmitFileInfo> Files);
public record struct StudentSubmitFileInfo(string RelativePath, string[] Content);