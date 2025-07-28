using System.Runtime.Serialization;

namespace Domain.Enums;

public enum UserRole
{
    [EnumMember(Value = "User")]
    User = 0,
    [EnumMember(Value = "Admin")]
    Admin = 1
}