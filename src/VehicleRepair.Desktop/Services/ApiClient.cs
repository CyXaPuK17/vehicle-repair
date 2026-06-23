using Newtonsoft.Json;

namespace VehicleRepair.Desktop.Services;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }
}

public class ApiError
{
    public string Code { get; set; } = "";
    public string Message { get; set; } = "";
}

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly AuthTokenService _auth;

    private const string BaseUrl = "http://localhost:5000/api/v1";

    public ApiClient(HttpClient http, AuthTokenService auth)
    {
        _http = http;
        _auth = auth;
    }

    private void AddAuth(HttpRequestMessage req)
    {
        if (_auth.AccessToken != null)
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth.AccessToken);
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string path)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}{path}");
        AddAuth(req);
        var res = await _http.SendAsync(req);
        var body = await res.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ApiResponse<T>>(body)
               ?? throw new Exception("Пустой ответ от сервера");
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string path, object payload)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}{path}");
        AddAuth(req);
        req.Content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");
        var res = await _http.SendAsync(req);
        var body = await res.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ApiResponse<T>>(body)
               ?? throw new Exception("Пустой ответ от сервера");
    }

    public async Task<ApiResponse<T>> PatchAsync<T>(string path, object payload)
    {
        using var req = new HttpRequestMessage(HttpMethod.Patch, $"{BaseUrl}{path}");
        AddAuth(req);
        req.Content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");
        var res = await _http.SendAsync(req);
        var body = await res.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ApiResponse<T>>(body)
               ?? throw new Exception("Пустой ответ от сервера");
    }

    public async Task LoginAsync(string login, string password)
    {
        var payload = new { login, password };
        using var req = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/auth/login");
        req.Content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");

        // Use a separate handler that stores cookies for refresh token
        var res = await _http.SendAsync(req);
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<ApiResponse<TokenResponse>>(body);
        if (result?.Success != true || result.Data == null)
            throw new Exception(result?.Error?.Message ?? "Ошибка авторизации");

        _auth.SetToken(result.Data.AccessToken, result.Data.Role, result.Data.UserId, null);
    }
}

public class TokenResponse
{
    public string AccessToken { get; set; } = "";
    public string Role { get; set; } = "";
    public string UserId { get; set; } = "";
}
