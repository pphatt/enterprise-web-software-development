﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ErrorOr;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Server.Application.Common.Dtos.Content.Media;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Common.Interfaces.Services.Media;
using Server.Domain.Common.Constants.Content;
using System.IO.Compression;

namespace Server.Infrastructure.Services.Media;

public class MediaService : IMediaService
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly MediaSettings _mediaSettings;
    private readonly IDateTimeProvider _dateTimeProvider;
    private CloudinarySettings _cloudinarySettings;
    private Cloudinary _cloudinary;

    public MediaService(IWebHostEnvironment environment, IOptions<MediaSettings> mediaSettings, IDateTimeProvider dateTimeProvider, IOptions<CloudinarySettings> cloudinarySettings)
    {
        _hostEnvironment = environment;
        _mediaSettings = mediaSettings.Value;
        _dateTimeProvider = dateTimeProvider;
        _cloudinarySettings = cloudinarySettings.Value;

        Account account = new Account(_cloudinarySettings.CloudName, _cloudinarySettings.ApiKey, _cloudinarySettings.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task SaveFilesAsync(List<IFormFile> files, string type)
    {
        if (files is null || files.Count == 0)
        {
            throw new ArgumentException("Files are empty or null", nameof(files));
        }

        var now = _dateTimeProvider.UtcNow;
        var wwwRootPath = _hostEnvironment.WebRootPath;

        if (string.IsNullOrEmpty(wwwRootPath))
        {
            throw new InvalidOperationException("WebRootPath is not configured");
        }

        foreach (var file in files)
        {
            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";

            var relativeFolder = $@"{_mediaSettings.MediaFolder}\{type}\{now:MM-yyyy}\{now:D}";

            var absoluteFolderPath = Path.Combine(wwwRootPath, relativeFolder);

            // Create directory if it doesn't exist (no need to check if exist.)
            Directory.CreateDirectory(absoluteFolderPath);

            var absoluteFilePath = Path.Combine(absoluteFolderPath, fileName);
            var relativeFilePath = Path.Combine(relativeFolder, fileName).Replace("\\", "/");

            try
            {
                await using var stream = new FileStream(absoluteFilePath, FileMode.Create);
                await file.CopyToAsync(stream);

                Console.WriteLine(relativeFilePath);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to save file: {ex.Message}", ex);
            }
        }
    }

    public Task RemoveFiles(List<string> paths)
    {
        if (paths.Count == 0)
        {
            throw new ArgumentException("Files path cannot be empty", nameof(paths));
        }

        foreach (var path in paths)
        {
            var absolutePath = Path.Combine(_hostEnvironment.WebRootPath, path.Replace("/", "\\"));

            if (!File.Exists(absolutePath))
            {
                throw new ArgumentException("File path is not exist", nameof(absolutePath));
            }

            File.Delete(absolutePath);
        }

        return Task.CompletedTask;
    }

    public async Task<(Stream FileStream, string ContentType, string FileName)> DownloadFiles(List<string> paths)
    {
        if (paths is null || paths.Count == 0)
        {
            throw new ArgumentException("No files was provided");
        }

        var zipName = $"zip_{_dateTimeProvider.UtcNow:dd-MM-yyyy}";

        using var memoryStream = new MemoryStream();

        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var path in paths)
            {
                var absolutePath = _hostEnvironment.WebRootPath + path.Replace("/", "\\");

                if (!File.Exists(absolutePath))
                {
                    continue;
                }

                var fileName = Path.GetFileName(absolutePath);

                var entry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var entryStream = entry.Open();

                using (var fileStream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read))
                {
                    await fileStream.CopyToAsync(entryStream);
                }
            }
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        return (memoryStream, "application/zip", zipName);
    }

    public async Task<List<FileDto>> UploadFilesToCloudinary(List<IFormFile> files, FileRequiredParamsDto dto)
    {
        var filesDetailsResult = new List<FileDto>();

        if (files is null)
        {
            return filesDetailsResult;
        }

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            var folderPath = dto.type == FileType.Avatar ? $"user-{dto.userId}/avatar" : $"user-{dto.userId}/contributions/contribution-{dto.contributionId}";

            UploadResult uploadResult;

            if (file.Length < 0)
            {
                continue;
            }

            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Folder = folderPath,
                    };

                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
            }
            else
            {
                using (var stream = file.OpenReadStream())
                {
                    var rawParams = new RawUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Folder = folderPath,
                    };

                    uploadResult = await _cloudinary.UploadAsync(rawParams);
                }
            }

            if (uploadResult.Error is null)
            {
                var fileDetails = new FileDto
                {
                    Path = uploadResult.Url.ToString(),
                    Name = uploadResult.DisplayName.ToString(),
                    Extension = extension,
                    PublicId = uploadResult.PublicId,
                    Type = dto.type
                };

                filesDetailsResult.Add(fileDetails);
            }
        }

        return filesDetailsResult;
    }
}
