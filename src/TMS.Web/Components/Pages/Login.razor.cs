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

    private LoginModels.LoginModel loginModel = new()
    {
        Email = "admin@translationgate.com",
        Password = "password123"
    };
    private bool loading = false;
    private bool rememberMe = false;

    private async Task HandleLogin()
    {
        loading = true;
        
        try
        {
            // Call backend API
            var response = await Http.PostAsJsonAsync("/api/auth/login", loginModel);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginModels.LoginResponse>();
                
                if (result != null && result.Success)
                {
                    Message.Success("Login successful! Welcome back.");
                    
                    // Store token and user info in localStorage
                    if (!string.IsNullOrEmpty(result.Token))
                    {
                        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "token", result.Token);
                    }
                    
                    if (result.User != null)
                    {
                        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "user", 
                            System.Text.Json.JsonSerializer.Serialize(result.User));
                    }
                    
                    // Small delay to ensure localStorage is persisted
                    await Task.Delay(100);
                    
                    // Navigate to Dashboard with forceLoad to ensure full page reload
                    Navigation.NavigateTo("/dashboard", forceLoad: true);
                }
                else
                {
                    Message.Error(result?.Message ?? "Login failed");
                }
            }
            else
            {
                var errorResult = await response.Content.ReadFromJsonAsync<LoginModels.LoginResponse>();
                Message.Error(errorResult?.Message ?? "Invalid email or password");
            }
        }
        catch (HttpRequestException)
        {
            Message.Error("Unable to3 connect to server. Please try again.");
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
