using MediatR;
using Microsoft.AspNetCore.Http;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.DishCategories.Commands.UpdateDishCategory;

/// <summary>
/// კატეგორიის განახლების Command
/// </summary>
public record UpdateDishCategoryCommand : IRequest<Result<DishCategoryResponseDto>>
{
    public Guid Id { get; init; }
    public string NameKa { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string? DescriptionKa { get; init; }
    public string? DescriptionEn { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; } = true;

    /// <summary>
    /// ახალი სურათის ფაილი (optional)
    /// </summary>
    public IFormFile? ImageFile { get; init; }
}
