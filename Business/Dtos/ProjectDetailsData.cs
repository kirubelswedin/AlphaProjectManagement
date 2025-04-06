namespace Business.Dtos;

public class ProjectDetailsData
{
    public string Id { get; set; } = null!;
    public string? Image { get; set; }
    public string ProjectName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal? Budget { get; set; }
    

    public string ClientName { get; set; } = null!;
    public string StatusName { get; set; } = null!;
    public string CreatedByName { get; set; } = null!;
    public ICollection<ProjectMemberData> Members { get; set; } = [];
}

public class ProjectMemberData
{
    public string UserId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Image { get; set; }
    public string RoleName { get; set; } = null!;
}