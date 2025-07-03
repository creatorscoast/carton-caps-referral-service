using System.Text;

namespace Referral.Api.Core;

/// <summary>
/// Simple referral code generation algorithm. It could be put behind an interface for easier swapping
/// but left as an example of different approaches.
/// </summary>
public static class ReferralCodeGenerator
{
    private const string Base26Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string EncodeGuidToBase26Unique(Guid guid, int length = 6)
    {
        if (length > 26) throw new ArgumentException("Cannot generate more than 26 unique uppercase letters.");

        byte[] bytes = guid.ToByteArray();
        byte[] shortBytes = new byte[8];
        Array.Copy(bytes, shortBytes, 6);

        ulong value = BitConverter.ToUInt64(shortBytes, 0);

        var sb = new StringBuilder();
        var used = new HashSet<char>();

        while (value > 0 && sb.Length < length)
        {
            int remainder = (int)(value % 26);
            char nextChar = Base26Chars[remainder];

            if (!used.Contains(nextChar))
            {
                sb.Insert(0, nextChar);
                used.Add(nextChar);
            }

            value /= 26;
        }

        // Fill in remaining spots with unused letters
        foreach (char c in Base26Chars)
        {
            if (sb.Length >= length) break;
            if (!used.Contains(c))
            {
                sb.Insert(0, c);
                used.Add(c);
            }
        }

        return sb.ToString();
    }
}
