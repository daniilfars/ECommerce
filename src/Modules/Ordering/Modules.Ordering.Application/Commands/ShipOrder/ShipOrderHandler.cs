using MediatR;
using Modules.Ordering.Application.Interfaces;
using Shared.Domain;

namespace Modules.Ordering.Application.Commands.ShipOrder;

public class ShipOrderHandler : IRequestHandler<ShipOrderCommand, Result>
{
    private readonly IOrderingDbContext _context;

    public ShipOrderHandler(IOrderingDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure("Заказ не найден");

        var result = order.Ship();
        if (result.IsFailure)
            return Result.Failure(result.Error!);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
