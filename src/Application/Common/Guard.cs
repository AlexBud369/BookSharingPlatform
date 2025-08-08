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
            var message = _localizer != null ? _localizer[resourceKey, args].Value : string.Format(resourceKey, args);
            throw new ApplicationException(message);
        }
    }

    public static void AgainstEmptyString(string value, string paramName, string resourceKey, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(value)) {
            var message = _localizer != null ? _localizer[resourceKey, args].Value : string.Format(resourceKey, args);
            throw new ApplicationException(message);
        }
    }

    public static void AgainstEmptyGuid(Guid value, string paramName, string resourceKey, params object[] args)
    {
        if (value == Guid.Empty) {
            var message = _localizer != null ? _localizer[resourceKey, args].Value : string.Format(resourceKey, args);
            throw new ApplicationException(message);
        }
    }

    public static void AgainstInvalidPageNumber(int pageNumber, string paramName, string resourceKey, params object[] args)
    {
        if (pageNumber < 1) {
            var message = _localizer != null ? _localizer[resourceKey, args].Value : string.Format(resourceKey, args);
            throw new ApplicationException(message);
        }
    }

    public static void AgainstInvalidPageSize(int pageSize, string paramName, string resourceKey, params object[] args)
    {
        if (pageSize < 1 || pageSize > DomainConstants.Book.MaxPageSize) {
            var message = _localizer != null ? _localizer[resourceKey, args].Value : string.Format(resourceKey, args);
            throw new ApplicationException(message);
        }
    }

    public static void AgainstFalse(bool condition, string paramName, string resourceKey, params object[] args)
    {
        if (!condition) {
            var message = _localizer != null ? _localizer[resourceKey, args].Value : string.Format(resourceKey, args);
            throw new ApplicationException(message);
        }
    }

    public static void AgainstUnauthorized(bool condition, string resourceKey)
    {
        if (!condition) {
            var message = _localizer != null ? _localizer[resourceKey].Value : resourceKey;
            throw new ApplicationException(message);
        }
    }

    public static void AgainstNonAdmin(bool isAdmin, string resourceKey)
    {
        if (!isAdmin) {
            var message = _localizer != null ? _localizer[resourceKey].Value : resourceKey;
            throw new ApplicationException(message);
        }
    }
}