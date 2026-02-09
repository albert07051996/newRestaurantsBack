using MediatR;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.TableSessions.Commands.CloseTableSession;

public record CloseTableSessionCommand(Guid SessionId) : IRequest<Result<TableSessionResponseDto>>;
