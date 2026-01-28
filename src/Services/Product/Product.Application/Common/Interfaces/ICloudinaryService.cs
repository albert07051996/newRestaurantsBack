using Microsoft.AspNetCore.Http;

namespace Product.Application.Common.Interfaces;

/// <summary>
/// Cloudinary სერვისის ინტერფეისი
/// </summary>
public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);
    Task<bool> DeleteImageAsync(string publicId, CancellationToken cancellationToken = default);
}
