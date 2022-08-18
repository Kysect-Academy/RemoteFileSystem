using System.Globalization;

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
    private readonly string _submitDateFormat;

    public FileSystemAccessor(string rootPath, string submitDateFormat)
    {
        _rootPath = rootPath;
        _submitDateFormat = submitDateFormat;
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
        var result = new List<StudentSubmit>();

        string groupDirectory = Path.Combine(_rootPath, selectedGroup);
        string groupName = new DirectoryInfo(groupDirectory).Name;
        foreach (string studentDirectory in Directory.EnumerateDirectories(groupDirectory))
        {
            string studentName = new DirectoryInfo(studentDirectory).Name;
            foreach (string assignmentDirectory in Directory.EnumerateDirectories(studentDirectory))
            {
                string assignmentName = new DirectoryInfo(assignmentDirectory).Name;

                IReadOnlyCollection<string> submitDates = GetChildDirectoryNames(assignmentDirectory);

                IEnumerable<StudentSubmit> submitDirectories =
                    GetSubmitDirectories(submitDates, groupName, studentName, assignmentName);
                result.AddRange(submitDirectories);
            }
        }

        return result;
    }

    private static IEnumerable<StudentSubmit> GetSubmitDirectories(IReadOnlyCollection<string> submitDates,
        string groupName, string studentName, string assignmentName)
    {
        if (submitDates.Count > 0)
        {
            return submitDates
                .Select(submitDate => new StudentSubmit(groupName, studentName, assignmentName, submitDate))
                .ToList();
        }

        return new List<StudentSubmit>
        {
            new StudentSubmit(groupName, studentName, assignmentName, null)
        };
    }


    public StudentSubmitContent GetSubmitContent(StudentSubmit studentSubmit)
    {
        string pathToContent = GetPath(studentSubmit);

        var submitFiles = Directory
            .EnumerateFiles(pathToContent, "*", SearchOption.AllDirectories)
            .Select(f => new StudentSubmitFileInfo(Path.GetRelativePath(pathToContent, f), File.ReadAllLines(f)))
            .ToList();

        return new StudentSubmitContent(submitFiles);
    }

    private string GetPath(StudentSubmit studentSubmit)
    {
        string pathWithoutDate = Path.Combine(
            _rootPath,
            studentSubmit.Group,
            studentSubmit.StudentName,
            studentSubmit.AssignmentTitle);

        ValidateDateFolderPresence(pathWithoutDate);

        return studentSubmit.SubmitDate is null
            ? pathWithoutDate
            : Path.Combine(pathWithoutDate, studentSubmit.SubmitDate);
    }

    private void ValidateDateFolderPresence(string pathWithoutDate)
    {
        if (GetChildDirectoryNames(pathWithoutDate)
            .Any(folder => DateTime.TryParseExact(folder, _submitDateFormat, new CultureInfo(""), DateTimeStyles.None,
                out DateTime _)))
        {
            throw new ArgumentException("Passed incomplete submit info: directory contains submit date");
        }
    }

    private static IReadOnlyCollection<string> GetChildDirectoryNames(string directoryPath)
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