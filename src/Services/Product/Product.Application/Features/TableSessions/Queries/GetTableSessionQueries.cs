using MediatR;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.TableSessions.Queries;

public record GetTableSessionByIdQuery(Guid Id) : IRequest<Result<TableSessionResponseDto>>;

public record GetActiveTableSessionsQuery : IRequest<Result<List<TableSessionResponseDto>>>;

public record GetAllTableSessionsQuery : IRequest<Result<List<TableSessionResponseDto>>>;

public record GetActiveSessionForTableQuery(int TableNumber) : IRequest<Result<TableSessionResponseDto>>;
