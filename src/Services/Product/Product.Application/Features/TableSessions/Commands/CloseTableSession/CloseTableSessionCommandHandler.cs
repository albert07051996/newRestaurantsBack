using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.TableSessions.Commands.CloseTableSession;

public sealed class CloseTableSessionCommandHandler : IRequestHandler<CloseTableSessionCommand, Result<TableSessionResponseDto>>
{
    private readonly ITableSessionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CloseTableSessionCommandHandler(ITableSessionRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TableSessionResponseDto>> Handle(CloseTableSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _repository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
            return Result<TableSessionResponseDto>.Failure(new Error("TableSession.NotFound", $"Table session with ID {request.SessionId} was not found."));

        try
        {
            session.Close();
        }
        catch (InvalidOperationException ex)
        {
            return Result<TableSessionResponseDto>.Failure(new Error("TableSession.CloseError", ex.Message));
        }

        // Session is already tracked from the query — change tracking handles the update
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TableSessionResponseDto>.Success(session.ToDto());
    }
}
