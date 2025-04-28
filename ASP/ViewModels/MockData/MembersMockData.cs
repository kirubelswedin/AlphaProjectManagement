using ASP.ViewModels.Components;

namespace ASP.ViewModels.MockData;

public static class MembersMockData
{
    private static readonly List<MemberCardViewModel> _members = new()
    {
        new MemberCardViewModel
        {
            Id = "tm1",
            ImageUrl = "/images/avatars/Avatar-1.svg",
            FirstName = "Anna",
            LastName = "Johansson",
            Email = "anna.johansson@example.com",
            PhoneNumber = "070-123 45 67",
            JobTitle = "Senior Developer",
            StreetAddress = "Storgatan 1",
            City = "Stockholm",
            PostalCode = "11457",
        },
        new MemberCardViewModel
        {
            Id = "tm2",
            ImageUrl = "/images/avatars/Avatar-2.svg",
            FirstName = "Erik",
            LastName = "Lindberg",
            Email = "erik.lindberg@example.com",
            PhoneNumber = "070-234 56 78",
            JobTitle = "UX Designer",
            StreetAddress = "Kungsgatan 5",
            City = "Göteborg",
            PostalCode = "41119",
        },
        new MemberCardViewModel
        {
            Id = "tm3",
            ImageUrl = "/images/avatars/Avatar-3.svg",
            FirstName = "Maria",
            LastName = "Svensson",
            Email = "maria.svensson@example.com",
            PhoneNumber = "070-345 67 89",
            JobTitle = "Product Manager",
            StreetAddress = "Drottninggatan 12",
            City = "Malmö",
            PostalCode = "21421",
        },
        new MemberCardViewModel
        {
            Id = "tm4",
            ImageUrl = "/images/avatars/Avatar-4.svg",
            FirstName = "Johan",
            LastName = "Andersson",
            Email = "johan.andersson@example.com",
            PhoneNumber = "070-456 78 90",
            JobTitle = "Backend Developer",
            StreetAddress = "Vasagatan 8",
            City = "Uppsala",
            PostalCode = "75320",
        },
        new MemberCardViewModel
        {
            Id = "tm5",
            ImageUrl = "/images/avatars/Avatar-5.svg",
            FirstName = "Lena",
            LastName = "Björk",
            Email = "lena.bjork@example.com",
            PhoneNumber = "070-567 89 01",
            JobTitle = "Frontend Developer",
            StreetAddress = "Sveavägen 22",
            City = "Stockholm",
            PostalCode = "11459",
        },
        new MemberCardViewModel
        {
            Id = "tm6",
            ImageUrl = "/images/avatars/Avatar-6.svg",
            FirstName = "Martin",
            LastName = "Ekström",
            Email = "martin.ekstrom@example.com",
            PhoneNumber = "070-678 90 12",
            JobTitle = "ProjectEntity Lead",
            StreetAddress = "Järntorgsgatan 3",
            City = "Göteborg",
            PostalCode = "41304",
        },
        new MemberCardViewModel
        {
            Id = "tm7",
            ImageUrl = "/images/avatars/Avatar-7.svg",
            FirstName = "Karin",
            LastName = "Nilsson",
            Email = "karin.nilsson@example.com",
            PhoneNumber = "070-789 01 23",
            JobTitle = "CTO",
            StreetAddress = "Triangeln 2",
            City = "Malmö",
            PostalCode = "21143",
        },
        new MemberCardViewModel
        {
            Id = "tm8",
            ImageUrl = "/images/avatars/Avatar-8.svg",
            FirstName = "Anders",
            LastName = "Larsson",
            Email = "anders.larsson@example.com",
            PhoneNumber = "070-890 12 34",
            JobTitle = "DevOps Engineer",
            StreetAddress = "Kyrkogatan 4",
            City = "Uppsala",
            PostalCode = "75231",
        }
    };

    public static IEnumerable<MemberCardViewModel> GetMembers()
    {
        return _members;
    }

    public static List<ProjectMemberViewModel> GetRandomMembers(int count)
    {
        return _members
            .OrderBy(x => Guid.NewGuid())
            .Take(count)
            .Select(m => new ProjectMemberViewModel
            {
                Id = m.Id,
                ImageUrl = m.ImageUrl,
            })
            .ToList();
    }
}