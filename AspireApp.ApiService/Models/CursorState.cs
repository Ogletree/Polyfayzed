namespace AspireApp.ApiService.Models;

public class CursorState
{
    public int Id { get; set; }
    public string NextCursor { get; set; }
    public DateTime LastUpdated { get; set; }
}