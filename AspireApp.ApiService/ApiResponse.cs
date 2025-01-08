using AspireApp.ApiService.Models.Dto;

namespace AspireApp.ApiService;

internal class ApiResponse
{
    public List<MarketDto> data { get; set; }
    public string next_cursor { get; set; }
}