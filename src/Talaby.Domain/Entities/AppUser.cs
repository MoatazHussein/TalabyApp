using Microsoft.AspNetCore.Identity;
using Talaby.Domain.Enums;

namespace Talaby.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = default!;
    public string? LastName { get; set; }
    public string? Mobile { get; set; } 
    public string? Location { get; set; } 
    public UserType UserType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    //store related fields
    public string? CommercialRegisterNumber { get; set; }
    public string? CommercialRegisterImageUrl { get; set; }
    public int? StoreCategoryId { get; set; }
    public StoreCategory? StoreCategory { get; set; }



}
