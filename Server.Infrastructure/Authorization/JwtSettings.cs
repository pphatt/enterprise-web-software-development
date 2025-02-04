﻿namespace Server.Infrastructure.Authorization;

public class JwtSettings
{
    public static string SelectionName { get; set; } = "JwtSettings";

    public string Secret { get; set; } = null!;

    public int ExpiryMinutes { get; set; }

    public string Issuer { get; set; } = null!;

    public string Audience { get; set; } = null!;
}
