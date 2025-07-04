using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Talaby.Domain.Constants;
using Talaby.Domain.Entities;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Seeders;

internal class TalabySeeder(TalabyDbContext dbContext) : ITalabySeeder
{
    public async Task Seed()
    {

        if (dbContext.Database.GetPendingMigrations().Any())
        {
            await dbContext.Database.MigrateAsync();
        }

        if (await dbContext.Database.CanConnectAsync())
        {
            if (!dbContext.StoreCategories.Any())
            {
                var StoreCategories = GetStoreCategories();
                dbContext.StoreCategories.AddRange(StoreCategories);
                await dbContext.SaveChangesAsync();
            }

            if (!dbContext.Roles.Any())
            {
                var roles = GetRoles();
                dbContext.Roles.AddRange(roles);
                await dbContext.SaveChangesAsync();
            }

        }
    }

    private IEnumerable<StoreCategory> GetStoreCategories()
    {
        List<StoreCategory> StoreCategories = [
            new()
            {
                NameAr = "اعمال المقاولات",
                NameEn = "Contracting works",
                Description = "Contracting works....",

            },
            new ()
            {
                NameAr = "الكترونيات",
                NameEn = "Electronics",
                Description = "Electronics....",
            },

        ];

        return StoreCategories;
    }

    private IEnumerable<AppRole> GetRoles()
    {
        List<AppRole> roles =
            [

            new AppRole
            {
                Id = Guid.NewGuid(),
                Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(UserRoles.Admin.ToLower()),
                NormalizedName  = UserRoles.Admin.ToUpper(),
            },
            new AppRole
            {
                Id = Guid.NewGuid(),
                Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(UserRoles.Client.ToLower()),
                NormalizedName = UserRoles.Client.ToUpper(),
            },
            new AppRole
            {
                Id = Guid.NewGuid(),
                Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(UserRoles.Store.ToLower()),
                NormalizedName =  UserRoles.Store.ToUpper()
            }

            ];

        return roles;
    }
}
