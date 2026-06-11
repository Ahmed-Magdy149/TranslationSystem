using TMS.Application.Interfaces;
using TMS.Application.Services;
using TMS.Core.Entities;
using TMS.Core.Interfaces;
using TMS.Infrastructure.Data;
using TMS.Infrastructure.Repositories;
using TMS.Infrastructure.Services;
using TMS.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// MongoDB Configuration
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<MongoDbContext>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITranslatorRepository, TranslatorRepository>();
builder.Services.AddScoped<IRepository<TaskHistory>>(sp =>
    new MongoRepository<TaskHistory>(sp.GetRequiredService<MongoDbContext>().TaskHistories));
builder.Services.AddScoped<IRepository<Review>>(sp =>
    new MongoRepository<Review>(sp.GetRequiredService<MongoDbContext>().Reviews));

// Application Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITranslatorService, TranslatorService>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();
builder.Services.AddScoped<IAssistantService, AssistantService>();

// Infrastructure Services
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IWordCounterService, WordCounterService>();

// Controllers (API)
builder.Services.AddControllers();

// Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Ant Design Blazor
builder.Services.AddAntDesign();

// HttpClient for API calls
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(sp => 
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext?.Request;
    
    var baseUrl = request != null 
        ? $"{request.Scheme}://{request.Host}"
        : "https://localhost:7065";
    
    return new HttpClient { BaseAddress = new Uri(baseUrl) };
});

// Web Services
builder.Services.AddScoped<TMS.Web.Services.IUserService, TMS.Web.Services.UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
