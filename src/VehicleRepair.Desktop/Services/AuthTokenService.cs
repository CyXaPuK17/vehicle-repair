namespace VehicleRepair.Desktop.Services;

public class AuthTokenService
{
    public string? AccessToken { get; private set; }
    public string? Role { get; private set; }
    public string? UserId { get; private set; }
    public string? LinkedEntityId { get; private set; }

    public bool IsAuthenticated => AccessToken != null;

    public void SetToken(string accessToken, string role, string userId, string? linkedEntityId)
    {
        AccessToken = accessToken;
        Role = role;
        UserId = userId;
        LinkedEntityId = linkedEntityId;
    }

    public void Clear()
    {
        AccessToken = null;
        Role = null;
        UserId = null;
        LinkedEntityId = null;
    }
}
