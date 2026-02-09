using Product.Application.DTOs;
using Product.Domain.Entities;

namespace Product.Application.Common.Mappings;

/// <summary>
/// Extension methods for mapping domain entities to DTOs.
/// Centralizes mapping logic to eliminate duplication across handlers.
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// Maps a Dish entity to DishResponseDto.
    /// </summary>
    public static DishResponseDto ToDto(this Dish dish)
    {
        return new DishResponseDto
        {
            Id = dish.Id,
            NameKa = dish.NameKa,
            NameEn = dish.NameEn,
            DescriptionKa = dish.DescriptionKa,
            DescriptionEn = dish.DescriptionEn,
            Price = dish.Price,
            DishCategoryId = dish.DishCategoryId,
            PreparationTimeMinutes = dish.PreparationTimeMinutes,
            Calories = dish.Calories,
            SpicyLevel = dish.SpicyLevel,
            Ingredients = dish.Ingredients,
            IngredientsEn = dish.IngredientsEn,
            Volume = dish.Volume,
            AlcoholContent = dish.AlcoholContent,
            IsVeganDish = dish.IsVeganDish,
            Comment = dish.Comment,
            ImageUrl = dish.ImageUrl,
            VideoUrl = dish.VideoUrl,
            CreatedAt = dish.CreatedAt,
            UpdatedAt = dish.UpdatedAt
        };
    }

    /// <summary>
    /// Maps a collection of Dish entities to a list of DishResponseDto.
    /// </summary>
    public static List<DishResponseDto> ToDto(this IEnumerable<Dish> dishes)
    {
        return dishes.Select(d => d.ToDto()).ToList();
    }

    /// <summary>
    /// Maps a DishCategory entity to DishCategoryResponseDto.
    /// </summary>
    public static DishCategoryResponseDto ToDto(this DishCategory category)
    {
        return new DishCategoryResponseDto
        {
            Id = category.Id,
            NameKa = category.NameKa,
            NameEn = category.NameEn,
            DescriptionKa = category.DescriptionKa,
            DescriptionEn = category.DescriptionEn,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            ImageUrl = category.ImageUrl,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    /// <summary>
    /// Maps a collection of DishCategory entities to a list of DishCategoryResponseDto.
    /// </summary>
    public static List<DishCategoryResponseDto> ToDto(this IEnumerable<DishCategory> categories)
    {
        return categories.Select(c => c.ToDto()).ToList();
    }

    /// <summary>
    /// Maps an Order entity to OrderResponseDto.
    /// </summary>
    public static OrderResponseDto ToDto(this Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            CustomerAddress = order.CustomerAddress,
            OrderType = order.OrderType.ToString(),
            Status = order.Status.ToString(),
            TableNumber = order.TableNumber,
            Notes = order.Notes,
            TotalAmount = order.TotalAmount,
            TableSessionId = order.TableSessionId,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.OrderItems.Select(i => i.ToDto()).ToList()
        };
    }

    /// <summary>
    /// Maps a collection of Order entities to a list of OrderResponseDto.
    /// </summary>
    public static List<OrderResponseDto> ToDto(this IEnumerable<Order> orders)
    {
        return orders.Select(o => o.ToDto()).ToList();
    }

    /// <summary>
    /// Maps a TableSession entity to TableSessionResponseDto.
    /// </summary>
    public static TableSessionResponseDto ToDto(this TableSession session)
    {
        return new TableSessionResponseDto
        {
            Id = session.Id,
            SessionNumber = session.SessionNumber,
            TableNumber = session.TableNumber,
            CustomerName = session.CustomerName,
            CustomerPhone = session.CustomerPhone,
            Status = session.Status.ToString(),
            TotalAmount = session.TotalAmount,
            CreatedAt = session.CreatedAt,
            UpdatedAt = session.UpdatedAt,
            Orders = session.Orders.Select(o => o.ToDto()).ToList()
        };
    }

    /// <summary>
    /// Maps a collection of TableSession entities to a list of TableSessionResponseDto.
    /// </summary>
    public static List<TableSessionResponseDto> ToDto(this IEnumerable<TableSession> sessions)
    {
        return sessions.Select(s => s.ToDto()).ToList();
    }

    /// <summary>
    /// Maps an OrderItem entity to OrderItemResponseDto.
    /// </summary>
    public static OrderItemResponseDto ToDto(this OrderItem item)
    {
        return new OrderItemResponseDto
        {
            Id = item.Id,
            DishId = item.DishId,
            DishNameKa = item.DishNameKa,
            DishNameEn = item.DishNameEn,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice,
            SpecialInstructions = item.SpecialInstructions
        };
    }

    /// <summary>
    /// Maps a Reservation entity to ReservationResponseDto.
    /// </summary>
    public static ReservationResponseDto ToDto(this Reservation reservation)
    {
        return new ReservationResponseDto
        {
            Id = reservation.Id,
            ReservationNumber = reservation.ReservationNumber,
            CustomerName = reservation.CustomerName,
            CustomerPhone = reservation.CustomerPhone,
            ReservationDate = reservation.ReservationDate,
            ReservationTime = reservation.ReservationTime.ToString(@"hh\:mm"),
            GuestCount = reservation.GuestCount,
            TableNumber = reservation.TableNumber,
            Notes = reservation.Notes,
            Status = reservation.Status.ToString(),
            TotalAmount = reservation.TotalAmount,
            CreatedAt = reservation.CreatedAt,
            UpdatedAt = reservation.UpdatedAt,
            Items = reservation.ReservationItems.Select(i => i.ToDto()).ToList()
        };
    }

    /// <summary>
    /// Maps a collection of Reservation entities to a list of ReservationResponseDto.
    /// </summary>
    public static List<ReservationResponseDto> ToDto(this IEnumerable<Reservation> reservations)
    {
        return reservations.Select(r => r.ToDto()).ToList();
    }

    /// <summary>
    /// Maps a ReservationItem entity to ReservationItemResponseDto.
    /// </summary>
    public static ReservationItemResponseDto ToDto(this ReservationItem item)
    {
        return new ReservationItemResponseDto
        {
            Id = item.Id,
            DishId = item.DishId,
            DishNameKa = item.DishNameKa,
            DishNameEn = item.DishNameEn,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice,
            SpecialInstructions = item.SpecialInstructions
        };
    }
}
