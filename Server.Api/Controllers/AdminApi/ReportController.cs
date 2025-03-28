using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Server.Application.Common.Interfaces.Services.Report;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.AdminApi;

[Tags("Admin Reports")]
public class ReportController : AdminApiController
{
    private readonly IContributionReportService _contributionReportService;

    public ReportController(
        ISender mediatorSender,
        IContributionReportService contributionReportService) : base(mediatorSender)
    {
        _contributionReportService = contributionReportService;
    }

    [HttpGet("get-total-contributions-in-each-faculty-in-each-academic-year")]
    [Authorize(Permissions.Dashboards.View)]
    public async Task<IActionResult> GetTotalContributionsInEachFacultyInEachAcademicYear()
    {

    }
}
