using Microsoft.Extensions.DependencyInjection;
using VehicleRepair.Desktop.Forms;
using VehicleRepair.Desktop.Services;

namespace VehicleRepair.Desktop;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var services = new ServiceCollection();

        services.AddSingleton<AuthTokenService>();
        services.AddSingleton<CookieContainer>();
        services.AddHttpClient<ApiClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            });
        services.AddSingleton<ApiClient>();
        services.AddTransient<LoginForm>();
        services.AddTransient<MainForm>();

        var provider = services.BuildServiceProvider();

        var auth = provider.GetRequiredService<AuthTokenService>();
        var api = provider.GetRequiredService<ApiClient>();
        var loginForm = new LoginForm(api);

        if (loginForm.ShowDialog() != DialogResult.OK)
            return;

        var mainForm = new MainForm(api, auth);
        Application.Run(mainForm);
    }
}
