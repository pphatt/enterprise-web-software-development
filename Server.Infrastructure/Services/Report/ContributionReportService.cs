﻿using Dapper;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Common.Interfaces.Services.Report;
using Server.Application.Wrapper.Report;
using Server.Domain.Entity.Content;
using Server.Infrastructure.Migrations;
using Server.Infrastructure.Persistence.AppDbConnection;

using static Server.Domain.Common.Errors.Errors;

namespace Server.Infrastructure.Services.Report;

public class ContributionReportService : IContributionReportService
{
    private readonly IAppDbConnectionFactory _connectionFactory;
    private readonly IFacultyRepository _facultyRepository;

    public ContributionReportService(IAppDbConnectionFactory connectionFactory, IFacultyRepository facultyRepository)
    {
        _connectionFactory = connectionFactory;
        _facultyRepository = facultyRepository;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<TotalContributionsInEachFacultyInEachAcademicYearReportDto>>> GetTotalContributionsInEachFacultyInEachAcademicYearReport()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql =
            """
            SELECT ay.Name AS AcademicYear,
                f.Name AS Faculty,
                COALESCE(count(c.Id), 0) AS TotalContributions
            FROM AcademicYears ay
            CROSS JOIN Faculties f
            LEFT JOIN Contributions c ON c.AcademicYearId = ay.Id AND c.FacultyId = f.Id
            WHERE c.DateDeleted is null
            GROUP BY ay.Name, f.Name
            ORDER BY ay.Name, f.Name;
            """;

        var query = (await connection.QueryAsync<GetTotalContributionsInEachFacultyInEachAcademicYearDto>(sql: sql)).AsList();

        var count = query.Count();
        var facultyCount = await _facultyRepository.CountAsync();

        var result = new ReportResponseWrapper<AcademicYearReportResponseWrapper<TotalContributionsInEachFacultyInEachAcademicYearReportDto>>();

        for (var i = 0; i < count; i += facultyCount)
        {
            var academicYearResult = new AcademicYearReportResponseWrapper<TotalContributionsInEachFacultyInEachAcademicYearReportDto>();

            academicYearResult.AcademicYear = query[i].AcademicYear;

            for (var j = i; j < i + facultyCount; j++)
            {
                var facultyResult = new TotalContributionsInEachFacultyInEachAcademicYearReportDto();

                facultyResult.Faculty = query[j].Faculty;
                facultyResult.TotalContributions = query[j].TotalContributions;

                academicYearResult.DataSets.Add(facultyResult);
            }

            result.Response.Add(academicYearResult);
        }

        result.Response = result.Response
            .OrderByDescending(x => x.AcademicYear)
            .ToList();

        return result;
    }
}
