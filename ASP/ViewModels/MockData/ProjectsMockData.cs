using ASP.ViewModels.Components;
using Domain.Models;

namespace ASP.ViewModels.MockData;

public static class ProjectsMockData
{

    private static readonly string[] ProjectImages = {
        "/images/project/Image-1.svg",
        "/images/project/Image-2.svg",
        "/images/project/Image-3.svg",
        "/images/project/Image-4.svg",
        "/images/project/Image-5.svg",
        "/images/project/Image-6.svg",
        "/images/project/Image-7.svg",
        "/images/project/Image-8.svg"
    };

    private static string GetRandomProjectImage()
    {
        var random = new Random();
        int index = random.Next(0, ProjectImages.Length);
        return ProjectImages[index];
    }

    public static List<ProjectCardViewModel> GetProjects() => new()
    {
        new()
        {
            Id = "1",
            ProjectName = "E-commerce Platform Redesign",
            ClientName = "TechCorp Solutions",
            Description = "Complete redesign of the e-commerce platform with modern UI/UX and improved checkout flow",
            StartDate = "2024-01-15",
            EndDate = "2024-06-30",
            TimeLeft = "3 months left",
            Budget = 150000,
            Status = new Status { StatusName = "In Progress" },
            ProjectImage = GetRandomProjectImage(),
            IsUrgent = false,
            Members = MembersMockData.GetRandomMembers(3)
        },
        new()
        {
            Id = "2",
            ProjectName = "Mobile App Development",
            ClientName = "HealthCare Plus",
            Description = "Development of a healthcare mobile app for patient appointment management and medical records",
            StartDate = "2024-02-01",
            EndDate = "2024-04-15",
            TimeLeft = "2 weeks left",
            Budget = 85000,
            Status = new Status { StatusName = "Cancelled" },
            ProjectImage = GetRandomProjectImage(),
            IsUrgent = true,
            Members = MembersMockData.GetRandomMembers(4)
        },
        new()
        {
            Id = "3",
            ProjectName = "CRM System Integration",
            ClientName = "Global Sales Inc",
            Description = "Integration of new CRM system with existing business processes and data migration",
            StartDate = "2024-03-01",
            EndDate = "2024-08-30",
            TimeLeft = "5 months left",
            Budget = 200000,
            Status = new Status { StatusName = "Not Started" },
            ProjectImage = GetRandomProjectImage(),
            IsUrgent = false,
            Members = MembersMockData.GetRandomMembers(2)
        },
        new()
        {
            Id = "4",
            ProjectName = "Website Optimization",
            ClientName = "Marketing Pro",
            Description = "Performance optimization and SEO improvements for company website",
            StartDate = "2024-01-01",
            EndDate = "2024-03-15",
            TimeLeft = "2 days overdue",
            Budget = 45000,
            Status = new Status { StatusName = "Completed" },
            ProjectImage = GetRandomProjectImage(),
            IsUrgent = false,
            CompletedOnTime = false,
            Members = MembersMockData.GetRandomMembers(2)
        },
        new()
        {
            Id = "5",
            ProjectName = "Data Analytics Dashboard",
            ClientName = "DataViz Corp",
            Description = "Development of real-time analytics dashboard with interactive visualizations",
            StartDate = "2024-02-15",
            EndDate = "2024-07-30",
            TimeLeft = "4 months left",
            Budget = 175000,
            Status = new Status { StatusName = "In Progress" },
            ProjectImage = GetRandomProjectImage(),
            IsUrgent = false,
            Members = MembersMockData.GetRandomMembers(3)
        },
        new()
        {
            Id = "6",
            ProjectName = "Security Audit System",
            ClientName = "SecureNet",
            Description = "Development of automated security audit system with vulnerability scanning",
            StartDate = "2024-01-10",
            EndDate = "2024-03-10",
            TimeLeft = "5 days overdue",
            Budget = 95000,
            Status = new Status { StatusName = "Paused" },
            ProjectImage = GetRandomProjectImage(),
            IsUrgent = true,
            Members = MembersMockData.GetRandomMembers(3)
        },
        new()
        {
            Id = "7",
            ProjectName = "AI Chatbot Implementation",
            ClientName = "ServiceBot AI",
            Description = "Implementation of AI-powered customer service chatbot with natural language processing",
            StartDate = "2024-04-01",
            EndDate = "2024-09-30",
            TimeLeft = "6 months left",
            Budget = 250000,
            Status = new Status { StatusName = "Not Started" },
            ProjectImage = GetRandomProjectImage(),
            IsUrgent = false,
            Members = MembersMockData.GetRandomMembers(4)
        },
        new()
        {
            Id = "8",
            ProjectName = "Legacy System Migration",
            ClientName = "OldTech Solutions",
            Description = "Migration of legacy systems to modern cloud-based infrastructure",
            StartDate = "2024-01-01",
            EndDate = "2024-02-28",
            TimeLeft = "Completed on time",
            Budget = 120000,
            Status = new Status { StatusName = "Completed" },
            ProjectImage = GetRandomProjectImage(),
            CompletedOnTime = true,
            Members = MembersMockData.GetRandomMembers(4)
        }
    };
}