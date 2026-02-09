using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.TableSessions.Queries;

public sealed class GetTableSessionByIdQueryHandler : IRequestHandler<GetTableSessionByIdQuery, Result<TableSessionResponseDto>>
{
    private readonly ITableSessionRepository _repository;

    public GetTableSessionByIdQueryHandler(ITableSessionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TableSessionResponseDto>> Handle(GetTableSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var session = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (session is null)
            return Result<TableSessionResponseDto>.Failure(new Error("TableSession.NotFound", $"Table session with ID {request.Id} was not found."));

        return Result<TableSessionResponseDto>.Success(session.ToDto());
    }
}

public sealed class GetActiveTableSessionsQueryHandler : IRequestHandler<GetActiveTableSessionsQuery, Result<List<TableSessionResponseDto>>>
{
    private readonly ITableSessionRepository _repository;

    public GetActiveTableSessionsQueryHandler(ITableSessionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<TableSessionResponseDto>>> Handle(GetActiveTableSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _repository.GetAllActiveAsync(cancellationToken);
        return Result<List<TableSessionResponseDto>>.Success(sessions.ToDto());
    }
}

public sealed class GetAllTableSessionsQueryHandler : IRequestHandler<GetAllTableSessionsQuery, Result<List<TableSessionResponseDto>>>
{
    private readonly ITableSessionRepository _repository;

    public GetAllTableSessionsQueryHandler(ITableSessionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<TableSessionResponseDto>>> Handle(GetAllTableSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _repository.GetAllAsync(cancellationToken);
        return Result<List<TableSessionResponseDto>>.Success(sessions.ToDto());
    }
}

public sealed class GetActiveSessionForTableQueryHandler : IRequestHandler<GetActiveSessionForTableQuery, Result<TableSessionResponseDto>>
{
    private readonly ITableSessionRepository _repository;

    public GetActiveSessionForTableQueryHandler(ITableSessionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TableSessionResponseDto>> Handle(GetActiveSessionForTableQuery request, CancellationToken cancellationToken)
    {
        var session = await _repository.GetActiveSessionForTableAsync(request.TableNumber, cancellationToken);
        if (session is null)
            return Result<TableSessionResponseDto>.Failure(new Error("TableSession.NotFound", $"No active session found for table {request.TableNumber}."));

        return Result<TableSessionResponseDto>.Success(session.ToDto());
    }
}
