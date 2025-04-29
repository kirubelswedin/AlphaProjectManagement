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
      FirstName = "John",
      LastName = "Anderson",
      Email = "john.anderson@techcorp.com",
      PhoneNumber = "+46 70 123 45 67",
      StreetAddress = "456 Tech St",
      PostalCode = "67890",
      City = "Tech City",
      CreatedAt = DateTime.Now.AddMonths(-6)
    },
    new()
    {
      Id = "2",
      ClientName = "HealthCare Plus",
      FirstName = "Sarah",
      LastName = "Johnson",
      Email = "sarah.j@healthcareplus.com",
      PhoneNumber = "+46 73 456 78 90",
      StreetAddress = "123 Main St",
      PostalCode = "12345",
      City = "Anytown",
      CreatedAt = DateTime.Now.AddMonths(-4)
    },
    new()
    {
      Id = "3",
      ClientName = "Global Sales Inc",
      FirstName = "Michael",
      LastName = "Brown",
      Email = "m.brown@globalsales.com",
      PhoneNumber = "+46 76 789 01 23",
      StreetAddress = "789 Oak St",
      PostalCode = "54321",
      City = "Sometown",
      CreatedAt = DateTime.Now.AddMonths(-3)
    },
    new()
    {
      Id = "4",
      ClientName = "Marketing Pro",
      FirstName = "Emma",
      LastName = "Wilson",
      Email = "emma.w@marketingpro.com",
      PhoneNumber = "+46 72 345 67 89",
      StreetAddress = "321 Pine St",
      PostalCode = "98765",
      City = "Yourtown",
      CreatedAt = DateTime.Now.AddMonths(-2)
    },
    new()
    {
      Id = "5",
      ClientName = "DataViz Corp",
      FirstName = "David",
      LastName = "Lee",
      Email = "david.lee@datavizcorp.com",
      PhoneNumber = "+46 70 987 65 43",
      StreetAddress = "456 Elm St",
      PostalCode = "67890",
      City = "Othertown",
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
      FirstName = c.FirstName,
      LastName = c.LastName,
      Email = c.Email,
      PhoneNumber = c.PhoneNumber
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
      FirstName = client.FirstName,
      LastName = client.LastName,
      Email = client.Email,
      PhoneNumber = client.PhoneNumber
    };
  }

  public static void UpdateClient(ClientEntity clientEntity)
  {
    var existingClient = _clients.FirstOrDefault(c => c.Id == clientEntity.Id);
    if (existingClient != null)
    {
      existingClient.ClientName = clientEntity.ClientName;
      existingClient.FirstName = clientEntity.FirstName;
      existingClient.LastName = clientEntity.LastName;
      existingClient.Email = clientEntity.Email;
      existingClient.PhoneNumber = clientEntity.PhoneNumber;
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