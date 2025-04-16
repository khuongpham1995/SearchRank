using System.ComponentModel.DataAnnotations;

namespace SearchRank.Application.Extensions;

public static class ValidationExtension
{
    public static bool IsEmailValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var emailAttribute = new EmailAddressAttribute();
        return emailAttribute.IsValid(email);
    }

    public static bool IsPaginationValid(int currentPage, int pageSize)
    {
        return currentPage >= 1 && pageSize >= 1;
    }
}