using Microsoft.Extensions.Localization;

namespace Application.Common;

public static class Guard
{
    private static IStringLocalizer<SharedResource>? _localizer;

    public static void Initialize(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public static void AgainstNull<T>(T value, string paramName, string resourceKey, params object[] args)
    {
        if (value == null)
        {
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

    public static void AgainstUnauthorized(bool condition, string resourceKey)
    {
        if (!condition)
        {
            throw new ApplicationException(_localizer![resourceKey]);
        }
    }

    public static void AgainstNonAdmin(bool isAdmin, string resourceKey)
    {
        if (!isAdmin)
        {
            throw new ApplicationException(_localizer![resourceKey]);
        }
    }

}
