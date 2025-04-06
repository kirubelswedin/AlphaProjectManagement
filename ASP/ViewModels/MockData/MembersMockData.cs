using ASP.ViewModels.Components;

namespace ASP.ViewModels.MockData;

public static class MembersMockData
{
    private static readonly List<MemberCardViewModel> _members = new()
    {
        new MemberCardViewModel
        {
            Id = "tm1",
            Avatar = "/images/avatars/Avatar-1.svg",
            FullName = "Anna Johansson",
            Email = "anna.johansson@example.com",
            PhoneNumber = "070-123 45 67",
            JobTitle = "Senior Developer",
            Address = "Storgatan 1",
            City = "Stockholm",
            PostalCode = "11457",
            StreetName = "Storgatan 1"
        },
        new MemberCardViewModel
        {
            Id = "tm2",
            Avatar = "/images/avatars/Avatar-2.svg",
            FullName = "Erik Lindberg",
            Email = "erik.lindberg@example.com",
            PhoneNumber = "070-234 56 78",
            JobTitle = "UX Designer",
            Address = "Kungsgatan 5",
            City = "Göteborg",
            PostalCode = "41119",
            StreetName = "Kungsgatan 5"
        },
        new MemberCardViewModel
        {
            Id = "tm3",
            Avatar = "/images/avatars/Avatar-3.svg",
            FullName = "Maria Svensson",
            Email = "maria.svensson@example.com",
            PhoneNumber = "070-345 67 89",
            JobTitle = "Product Manager",
            Address = "Drottninggatan 12",
            City = "Malmö",
            PostalCode = "21421",
            StreetName = "Drottninggatan 12"
        },
        new MemberCardViewModel
        {
            Id = "tm4",
            Avatar = "/images/avatars/Avatar-4.svg",
            FullName = "Johan Andersson",
            Email = "johan.andersson@example.com",
            PhoneNumber = "070-456 78 90",
            JobTitle = "Backend Developer",
            Address = "Vasagatan 8",
            City = "Uppsala",
            PostalCode = "75320",
            StreetName = "Vasagatan 8"
        },
        new MemberCardViewModel
        {
            Id = "tm5",
            Avatar = "/images/avatars/Avatar-5.svg",
            FullName = "Lena Björk",
            Email = "lena.bjork@example.com",
            PhoneNumber = "070-567 89 01",
            JobTitle = "Frontend Developer",
            Address = "Sveavägen 22",
            City = "Stockholm",
            PostalCode = "11459",
            StreetName = "Sveavägen 22"
        },
        new MemberCardViewModel
        {
            Id = "tm6",
            Avatar = "/images/avatars/Avatar-6.svg",
            FullName = "Martin Ekström",
            Email = "martin.ekstrom@example.com",
            PhoneNumber = "070-678 90 12",
            JobTitle = "ProjectEntity Lead",
            Address = "Järntorgsgatan 3",
            City = "Göteborg",
            PostalCode = "41304",
            StreetName = "Järntorgsgatan 3"
        },
        new MemberCardViewModel
        {
            Id = "tm7",
            Avatar = "/images/avatars/Avatar-7.svg",
            FullName = "Karin Nilsson",
            Email = "karin.nilsson@example.com",
            PhoneNumber = "070-789 01 23",
            JobTitle = "CTO",
            Address = "Triangeln 2",
            City = "Malmö",
            PostalCode = "21143",
            StreetName = "Triangeln 2"
        },
        new MemberCardViewModel
        {
            Id = "tm8",
            Avatar = "/images/avatars/Avatar-8.svg",
            FullName = "Anders Larsson",
            Email = "anders.larsson@example.com",
            PhoneNumber = "070-890 12 34",
            JobTitle = "DevOps Engineer",
            Address = "Kyrkogatan 4",
            City = "Uppsala",
            PostalCode = "75231",
            StreetName = "Kyrkogatan 4"
        }
    };

    public static IEnumerable<MemberCardViewModel> GetMembers()
    {
        return _members;
    }

    public static List<MemberViewModel> GetRandomMembers(int count)
    {
        return _members
            .OrderBy(x => Guid.NewGuid())
            .Take(count)
            .Select(m => new MemberViewModel
            {
                Id = m.Id,
                Avatar = m.Avatar,
            })
            .ToList();
    }
}