using System.Security.Cryptography;
using System.Text;

namespace Learnup.Application.Helpers;

public static class AuthHelper
{
    public static string NormalizeMobileNumber(string mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            throw new ArgumentException("Mobile number is required.", nameof(mobileNumber));
        }

        return mobileNumber.Trim();
    }
    
    public static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("OTP code is required.", nameof(code));
        }

        return code.Trim();
    }
    
}