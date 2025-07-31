using System.Runtime.Serialization;

namespace Application.Common.Enums;

public enum BookSortBy
{
    [EnumMember(Value = "createdAt")]
    CreatedAt = 0,
    [EnumMember(Value = "title")]
    Title = 1
}