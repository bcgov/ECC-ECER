using MediatR;

namespace ECER.Managers.Admin.Contract.Files;
public record SaveFileCommand(IEnumerable<FileData> Items) : IRequest;

public record FileQuery(IEnumerable<FileLocation> FileLocations) : IRequest<FileQueryResults>;

public record FileLocation(string Id, string Folder);

public record FileProperties
{
  private readonly Dictionary<string, string> properties = new();
  private Dictionary<string, string>? tags;

  public string Classification
  {
    get => properties["classification"] ?? string.Empty;
    set => properties["classification"] = value ?? string.Empty;
  }

  public string? Tags
  {
    get => tags == null ? null : string.Join(',', tags.Select(p => $"{p.Key}={p.Value}"));
    set => tags = value == null ? null : value.Split(',').ToDictionary(t => t.Split('=')[0], t => t.Split('=')[1]);
  }

  public IEnumerable<KeyValuePair<string, string>>? TagsList
  {
    get => tags;
    set => tags = value == null ? null : new Dictionary<string, string>(value);
  }
}

public record FileQueryResults(IEnumerable<FileData> Items);

public record FileData(FileLocation FileLocation, FileProperties FileProperties, string FileName, string ContentType, Stream Content);
