using Subs.Domain.Enums;
using Subs.Domain.Models.SubscriptionComponents;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Subs.Domain.Models.Messages;

/// <summary>
/// Represents a message entity that tracks the details of an operation performed on a client
/// </summary>
/// <remarks>
/// A message contains metadata about an operation, including its unique identifier, the associated
/// client, the type of operation performed, timestamps for creation and processing, and the current status of the
/// message.
/// </remarks>
public class ClientMessage : Message
{
    public Guid ClientId { get; set; }
    public string? ClientJson { get; set; }

    [NotMapped]
    public Client? Client
    {
        get => string.IsNullOrEmpty(ClientJson) ? null : JsonSerializer.Deserialize<Client>(ClientJson);
        set
        {
            ClientId = value?.Id ?? Guid.Empty;
            ClientJson = value != null ? SerializeClient(value) : null;
        }
    }

    public ClientMessage() { }

    public ClientMessage(Client client, EOperation operation) : base(operation)
    {
        Client = client;
    }

    private string SerializeClient(Client client)
    {
        if (client.Id != Guid.Empty)
            ClientId = client.Id;

        return JsonSerializer.Serialize(client);
    }
}