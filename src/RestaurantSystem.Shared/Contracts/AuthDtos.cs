namespace RestaurantSystem.Shared.Contracts;

public record LoginRequest(string Username, string Password);

public record LoginResponse(
    Guid UserId,
    string Username,
    string Role,
    string AccessToken,
    DateTime ExpiresAtUtc
);