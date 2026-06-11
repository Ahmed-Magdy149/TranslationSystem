using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using AntDesign;
using TMS.Web.Models.ViewModels;

namespace TMS.Web.Components.Pages;

public partial class Login : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private MessageService Message { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private LoginModels.LoginModel loginModel = new();
    private bool loading = false;
    private bool rememberMe = false;

    private async Task HandleLogin()
    {
        loading = true;
        try
        {
            var response = await Http.PostAsJsonAsync("/api/auth/login", loginModel);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginModels.LoginResponse>();
                
                if (result != null && result.Success)
                {
                    Message.Success("Login successful!");
                    await JSRuntime.InvokeVoidAsync("localStorage.setItem", "user", 
                        System.Text.Json.JsonSerializer.Serialize(result.User));
                    Navigation.NavigateTo("/");
                }
                else
                {
                    Message.Error(result?.Message ?? "Login failed");
                }
            }
            else
            {
                Message.Error("Invalid email or password");
            }
        }
        catch (Exception ex)
        {
            Message.Error($"Login error: {ex.Message}");
        }
        finally
        {
            loading = false;
        }
    }
}
