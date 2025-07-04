using Talaby.Domain.Entities;

namespace Talaby.Application.Users.Dtos;

public class UserDto
{
    public string Id { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Location { get; set; }
    public int UserTypeValue { get; set; }
    public string UserTypeName { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool EmailConfirmed { get; set; }
    public List<string> Roles { get; set; }



    //store related fields
    public string? CommercialRegisterImageUrl { get; set; }
    public int? StoreCategoryId { get; set; }
    public StoreCategory? StoreCategory { get; set; }
    //public string? StoreCategoryName { get; set; }



}

