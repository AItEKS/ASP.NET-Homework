using PersonalAccount.Data.Extensions;
using PersonalAccount.Web.Extensions;
using PersonalAccount.Web.Logics;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Подключаем конфигурацию и логирование
var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    path: "PersonalAccountApi_.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30
                )
                .CreateLogger();       

// Регистрируем сервиса
builder.Services
    .RegistryPersonalAccountData(configuration)
    .RegistryPersonalAccountWeb(configuration);

// Регистрируем контроллеры    
builder.Services.AddControllersWithViews(
    options =>
    {
        options.Filters.Add<CustomExceptionFilter>();
    });

var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
