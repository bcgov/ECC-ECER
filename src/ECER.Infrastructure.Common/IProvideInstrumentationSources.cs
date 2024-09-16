namespace ECER.Infrastructure.Common;

public interface IProvideInstrumentationSources
{
  InstrumentationSources GetInstrumentationSources();
}

public record InstrumentationSources
{
  public IEnumerable<string> TraceSources { get; set; } = [];
}
