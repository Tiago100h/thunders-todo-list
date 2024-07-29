using Microsoft.EntityFrameworkCore;
using ThundersTodoList.Infra;

namespace ThundersTodoList.Api.Configuration;

public static class DatabaseManagementService
{
    public static void MigrateDatabase(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        serviceScope!.ServiceProvider!.GetService<AppDbContext>()!.Database.Migrate();
    }
}
