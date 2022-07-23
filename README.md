# RemoteFileSystem

RemoteFileSystem - это простой API для доступа к файловой системе на удалённом устройстве.

## How to run

1. Сохранить датасет на файловой системе к которой будет доступ.
2. Указать в appsettings путь к директории, где датасет лежит (ключ FileSystemPath)
3. Запустить веб сервис
4. Написать клиент для обращения к сервису

Пример клиента:

```csharp
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri($"https://localhost:7280");

IReadOnlyCollection<Group>? groups = httpClient.GetFromJsonAsync<IReadOnlyCollection<Group>>("FileSystem/groups").Result;
foreach (Group group in groups)
{
    IReadOnlyCollection<StudentSubmit> submits = httpClient.GetFromJsonAsync<IReadOnlyCollection<StudentSubmit>>($"FileSystem/groups/{group.Name}/submits").Result;
    foreach (StudentSubmit studentSubmit in submits)
    {
        Console.WriteLine(studentSubmit);
        string requestUri = $"FileSystem/submits?group={studentSubmit.Group}" +
                              $"&studentName={studentSubmit.StudentName}" +
                              $"&assignmentTitle={studentSubmit.AssignmentTitle}" +
                              $"&submitDate={studentSubmit.SubmitDate}";
        StudentSubmitContent content = httpClient.GetFromJsonAsync<StudentSubmitContent>(requestUri).Result;
    }
}
```
