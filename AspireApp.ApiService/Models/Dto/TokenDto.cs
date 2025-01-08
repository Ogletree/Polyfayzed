// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
namespace AspireApp.ApiService.Models.Dto;

public class TokenDto
{
    public string token_id { get; set; }
    public string outcome { get; set; }
    public decimal price { get; set; }
    public bool winner { get; set; }
}