﻿using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYear;

public class GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearQuery : IRequest<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearReportDto>>>
{
}
