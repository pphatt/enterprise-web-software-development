﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.Identity.Commands.ForgotPassword;
using Server.Application.Features.Identity.Commands.ResetPassword;
using Server.Contracts.Identity.ForgotPassword;
using Server.Contracts.Identity.ResetPassword;

namespace Server.Api.Controllers.ClientApi;

public class UsersController : ClientApiController
{
    private readonly IMapper _mapper;

    public UsersController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var mapper = _mapper.Map<ForgotPasswordCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            forgotPasswordResult => Ok(forgotPasswordResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var mapper = _mapper.Map<ResetPasswordCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            resetPasswordResult => Ok(resetPasswordResult),
            errors => Problem(errors)
        );
    }
}
