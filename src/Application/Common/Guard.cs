using Domain.Constants;
using Microsoft.Extensions.Localization;

namespace Application.Common;

public static class Guard
{
    private static IStringLocalizer<SharedResources>? _localizer;

    public static void Initialize(IStringLocalizer<SharedResources> localizer)
    {
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public static void AgainstNull<T>(T value, string paramName, string resourceKey, params object[] args)
    {
        if (value == null) {
            throw new ApplicationException(string.Format(_localizer![resourceKey], args));
        }
    }

    public static void AgainstEmptyString(string value, string paramName, string resourceKey, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ApplicationException(string.Format(_localizer![resourceKey], args));
        }
    }

    public static void AgainstEmptyGuid(Guid value, string paramName, string resourceKey, params object[] args)
    {
        if (value == Guid.Empty) {
            throw new ApplicationException(string.Format(_localizer![resourceKey], args));
        }
    }

    public static void AgainstInvalidPageNumber(int pageNumber, string paramName, string resourceKey, params object[] args)
    {
        if (pageNumber < 1)
        {
            throw new ApplicationException(string.Format(_localizer![resourceKey], args));
        }
    }

    public static void AgainstInvalidPageSize(int pageSize, string paramName, string resourceKey, params object[] args)
    {
        if (pageSize < 1 || pageSize > DomainConstants.Book.MaxPageSize) {
            throw new ApplicationException(string.Format(_localizer![resourceKey], args));
        }
    }

    public static void AgainstFalse(bool condition, string paramName, string resourceKey, params object[] args)
    {
        if (!condition) {
            throw new ApplicationException(string.Format(_localizer![resourceKey], args));
        }
    }

    public static void AgainstUnauthorized(bool condition, string resourceKey)
    {
        if (!condition) {
            throw new ApplicationException(_localizer![resourceKey]);
        }
    }

    public static void AgainstNonAdmin(bool isAdmin, string resourceKey)
    {
        if (!isAdmin) {
            throw new ApplicationException(_localizer![resourceKey]);
        }
    }
}