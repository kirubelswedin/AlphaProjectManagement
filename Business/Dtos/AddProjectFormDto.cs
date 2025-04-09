using Microsoft.AspNetCore.Http;

namespace Business.Dtos;

public class AddProjectDto
{
    public IFormFile? ImageUrl { get; set; }
    public string ProjectName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal? Budget { get; set; }
    public string ClientId { get; set; } = null!;
    public int StatusId { get; set; }
    public List<string> MemberIds { get; set; } = [];
}