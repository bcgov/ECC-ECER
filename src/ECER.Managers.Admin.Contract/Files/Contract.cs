namespace ECER.Managers.Admin.Contract.Files;
public record SaveFileCommand(IEnumerable<FileData> Items);

public record FileQuery(IEnumerable<FileLocation> FileLocations);

public record FileLocation(string Id, string Folder);

public record FIleQueryResults(IEnumerable<FileData> Items);

public record FileData(FileLocation FileLocation, string FileName, string ContentType, Stream Content);
