using MediatR;
using Ordering.Application.Interfaces;
using Shared.Domain;

namespace Ordering.Application.Commands.CancelOrder;

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, Result>
{
    private readonly IOrderingDbContext _context;

    public CancelOrderHandler(IOrderingDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure("Заказ не найден");

        if(!request.IsAdmin && order.UserId != request.UserId)
            return Result.Failure("Нет доступа к заказу");

        var result = order.Cancel();
        if (result.IsFailure)
            return Result.Failure(result.Error!);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}