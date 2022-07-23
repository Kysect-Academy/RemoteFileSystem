using Kysect.RemoteFileSystem.Core;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
string pathToLocalFileSystem = builder.Configuration["FileSystemPath"] ?? throw new ArgumentException("FileSystemPath is not provided.");
builder.Services.AddSingleton<IFileSystemAccessor>(new FileSystemAccessor(pathToLocalFileSystem));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
