﻿using Microsoft.AspNetCore.Http;
using Server.Application.Common.Dtos.Media;
using Server.Contracts.Common.Media;

namespace Server.Application.Common.Interfaces.Services.Media;

public interface IMediaService
{
    Task SaveFilesAsync(List<IFormFile> files, string type);

    Task RemoveFiles(List<string> paths);

    Task<(Stream FileStream, string ContentType, string FileName)> DownloadFiles(List<string> path);

    Task<List<FileDto>> UploadFilesToCloudinary(List<IFormFile> files, FileRequiredParamsDto dto);

    Task RemoveFilesFromCloudinary(List<DeleteFilesRequest> files);

    string? GenerateDownloadUrl(List<string> publicIds);
}
