using Foundation;

namespace Keeper.iOS.Extensions;

public static class StringExtensions
{
    public static NSString ToNSString(this string value)
    {
        return new NSString(value);
    }
}

