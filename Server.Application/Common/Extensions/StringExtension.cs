﻿using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Enums;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Server.Application.Common.Extensions;

public static class StringExtension
{
    // remove accents from 'é' to 'e' or 'crème brûlée' to 'creme brulee'
    public static string RemoveAccents(this string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        for (int i = 0; i < normalizedString.Length; i++)
        {
            char c = normalizedString[i];
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }

    public static string Slugify(this string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
            return string.Empty;

        string output = phrase.RemoveAccents().ToLower();
        output = Regex.Replace(output, @"[^A-Za-z0-9\s-]", "");
        output = Regex.Replace(output, @"\s+", " ").Trim();
        output = Regex.Replace(output, @"\s", "-");

        return output;
    }

    public static string ToStringValue(this ContributionStatus status)
    {
        switch (status)
        {
            case ContributionStatus.Pending:
                return "PENDING";
            case ContributionStatus.Approve:
                return "APPROVED";
            case ContributionStatus.Reject:
                return "REJECTED";
            default:
                return "UNKNOWN";
        }
    }

    public static string ToStringValue(this ContributionSortBy sortBy)
    {
        switch (sortBy)
        {
            case ContributionSortBy.PublicDate:
                return "DATE";
            case ContributionSortBy.Like:
                return "LIKE";
            case ContributionSortBy.View:
                return "VIEW";
            default:
                return "UNKNOWN";
        }
    }

    public static string ToStringValue(this OrderByEnum orderBy)
    {
        switch (orderBy)
        {
            case OrderByEnum.Descending:
                return "DESCENDING";
            case OrderByEnum.Ascending:
                return "ASCENDING";
            default:
                return "UNKNOWN";
        }
    }
}
