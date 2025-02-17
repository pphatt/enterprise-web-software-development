﻿namespace Server.Contracts.Faculties.GetAllFacultiesPagination;

public class GetAllFacultiesPaginationRequest
{
    public string? Keyword { get; set; }

    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
