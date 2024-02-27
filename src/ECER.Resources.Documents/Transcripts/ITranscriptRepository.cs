namespace ECER.Resources.Documents.Transcripts;

public interface ITranscriptRepository
{
  Task<IEnumerable<Transcript>> Query(TranscriptQuery query);

  Task<string> Save(Transcript transcript);

  Task<string> Update(Transcript transcript);

  Task<bool> Delete(string transcriptId);
}

public record TranscriptQuery
{
  public string? ByApplicationId { get; set; }
}

public record Transcript(string? Id, string ApplicationId, string ApplicantId)
{
  public string EducationalInstitutionName { get; set; } = string.Empty;
  public string ProgramName { get; set; } = string.Empty;
  public string CampusLocation { get; set; } = string.Empty;
  public string StudentName { get; set; } = string.Empty;
  public string StudentNumber { get; set; } = string.Empty;
  public string LanguageofInstruction { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
}
