using MediatR;
using Microsoft.AspNetCore.Http;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.DishCategories.Commands.AddDishCategory;

/// <summary>
/// ახალი კატეგორიის დამატების Command
/// </summary>
public record AddDishCategoryCommand : IRequest<Result<DishCategoryResponseDto>>
{
    public string NameKa { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string? DescriptionKa { get; init; }
    public string? DescriptionEn { get; init; }
    public int DisplayOrder { get; init; }

    /// <summary>
    /// სურათის ფაილი (multipart/form-data)
    /// </summary>
    public IFormFile? ImageFile { get; init; }
}
