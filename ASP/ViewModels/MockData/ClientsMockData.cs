using Data.Entities;
using Domain.Models;

namespace ASP.ViewModels.MockData;

public static class ClientsMockData
{
  private static readonly List<ClientEntity> _clients = new()
  {
    new()
    {
      Id = "1",
      ClientName = "TechCorp Solutions",
      ContactPerson = "John Anderson",
      Email = "john.anderson@techcorp.com",
      Phone = "+46 70 123 45 67",
      CreatedAt = DateTime.Now.AddMonths(-6)
    },
    new()
    {
      Id = "2",
      ClientName = "HealthCare Plus",
      ContactPerson = "Sarah Johnson",
      Email = "sarah.j@healthcareplus.com",
      Phone = "+46 73 456 78 90",
      CreatedAt = DateTime.Now.AddMonths(-4)
    },
    new()
    {
      Id = "3",
      ClientName = "Global Sales Inc",
      ContactPerson = "Michael Brown",
      Email = "m.brown@globalsales.com",
      Phone = "+46 76 789 01 23",
      CreatedAt = DateTime.Now.AddMonths(-3)
    },
    new()
    {
      Id = "4",
      ClientName = "Marketing Pro",
      ContactPerson = "Emma Wilson",
      Email = "emma.w@marketingpro.com",
      Phone = "+46 72 345 67 89",
      CreatedAt = DateTime.Now.AddMonths(-2)
    },
    new()
    {
      Id = "5",
      ClientName = "DataViz Corp",
      ContactPerson = "David Lee",
      Email = "david.lee@datavizcorp.com",
      Phone = "+46 70 987 65 43",
      CreatedAt = DateTime.Now.AddMonths(-1)
    }
  };

  public static void AddClient(ClientEntity clientEntity)
  {
    // adds unique ID if it doesn't exist
    if (string.IsNullOrEmpty(clientEntity.Id))
    {
      clientEntity.Id = Guid.NewGuid().ToString();
    }

    // sets CreatedAt if it doesn't exist
    if (clientEntity.CreatedAt == default)
    {
      clientEntity.CreatedAt = DateTime.Now;
    }

    _clients.Add(clientEntity);
  }
  
  public static List<Client> GetClients()
  {
    return _clients.Select(c => new Client
    {
      Id = c.Id,
      ClientName = c.ClientName,
      ContactPerson = c.ContactPerson,
      Email = c.Email,
      Phone = c.Phone
    }).ToList();
  }

  public static Client? GetClientById(string id)
  {
    var client = _clients.FirstOrDefault(c => c.Id == id);
    if (client == null) return null;

    return new Client
    {
      Id = client.Id,
      ClientName = client.ClientName,
      ContactPerson = client.ContactPerson,
      Email = client.Email,
      Phone = client.Phone
    };
  }

  public static void UpdateClient(ClientEntity clientEntity)
  {
    var existingClient = _clients.FirstOrDefault(c => c.Id == clientEntity.Id);
    if (existingClient != null)
    {
      existingClient.ClientName = clientEntity.ClientName;
      existingClient.ContactPerson = clientEntity.ContactPerson;
      existingClient.Email = clientEntity.Email;
      existingClient.Phone = clientEntity.Phone;
    }
  }

  public static void DeleteClient(string id)
  {
    var client = _clients.FirstOrDefault(c => c.Id == id);
    if (client != null)
    {
      _clients.Remove(client);
    }
  }
}