using System.Text.Json.Serialization;

namespace Inventory.Application.DTOs;

public record UserDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("role")] string Role);
