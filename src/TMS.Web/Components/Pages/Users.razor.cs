using Microsoft.AspNetCore.Components;
using TMS.Core.DTOs;
using TMS.Core.Enums;
using TMS.Web.Services;
using AntDesign;

namespace TMS.Web.Components.Pages;

public partial class Users : ComponentBase
{
    [Inject] private IUserService UserService { get; set; } = default!;
    [Inject] private MessageService Message { get; set; } = default!;

    private List<UserDto> users = new();
    private bool loading = true;
    private bool modalVisible = false;
    private bool isEditMode = false;
    private CreateUserDto currentUser = new();
    private string editUserId = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadUsers();
    }

    private async Task LoadUsers()
    {
        loading = true;
        try
        {
            users = await UserService.GetAllUsersAsync();
        }
        catch (Exception ex)
        {
            Message.Error($"Failed to load users: {ex.Message}");
        }
        finally
        {
            loading = false;
        }
    }

    private void ShowAddModal()
    {
        isEditMode = false;
        currentUser = new CreateUserDto { IsActive = true, Role = UserRole.Translator };
        editUserId = string.Empty;
        modalVisible = true;
    }

    private void ShowEditModal(UserDto user)
    {
        isEditMode = true;
        editUserId = user.Id;
        currentUser = new CreateUserDto
        {
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            Password = string.Empty
        };
        modalVisible = true;
    }

    private async Task HandleSubmit()
    {
        try
        {
            if (isEditMode)
            {
                var updateDto = new UpdateUserDto
                {
                    Name = currentUser.Name,
                    Email = currentUser.Email,
                    Role = currentUser.Role,
                    IsActive = currentUser.IsActive
                };
                var success = await UserService.UpdateUserAsync(editUserId, updateDto);
                if (success)
                {
                    Message.Success("User updated successfully!");
                    modalVisible = false;
                    await LoadUsers();
                }
                else
                {
                    Message.Error("Failed to update user");
                }
            }
            else
            {
                var success = await UserService.CreateUserAsync(currentUser);
                if (success)
                {
                    Message.Success("User created successfully!");
                    modalVisible = false;
                    await LoadUsers();
                }
                else
                {
                    Message.Error("Failed to create user");
                }
            }
        }
        catch (Exception ex)
        {
            Message.Error($"Error: {ex.Message}");
        }
    }

    private void HandleCancel()
    {
        modalVisible = false;
    }

    private async Task DeleteUser(string userId)
    {
        try
        {
            var success = await UserService.DeleteUserAsync(userId);
            if (success)
            {
                Message.Success("User deleted successfully!");
                await LoadUsers();
            }
            else
            {
                Message.Error("Failed to delete user");
            }
        }
        catch (Exception ex)
        {
            Message.Error($"Error: {ex.Message}");
        }
    }

    private string GetRoleColor(string role)
    {
        return role switch
        {
            "Admin" => "red",
            "Manager" => "blue",
            "Translator" => "green",
            _ => "default"
        };
    }
}
