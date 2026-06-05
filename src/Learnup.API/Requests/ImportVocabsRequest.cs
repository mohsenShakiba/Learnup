namespace Learnup.API.Requests;

public class ImportVocabsRequest
{
    public IFormFile File { get; set; } = null!;
    public int LevelId { get; set; }
    public int LanguageId { get; set; }
}
