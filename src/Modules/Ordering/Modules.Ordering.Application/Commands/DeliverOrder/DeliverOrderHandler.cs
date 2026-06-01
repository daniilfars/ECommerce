using MediatR;
using Modules.Ordering.Application.Interfaces;
using Shared.Domain;

namespace Modules.Ordering.Application.Commands.DeliverOrder;

public class DeliverOrderHandler : IRequestHandler<DeliverOrderCommand, Result>
{
    private readonly IOrderingDbContext _context;

    public DeliverOrderHandler(IOrderingDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeliverOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure("Заказ не найден");

        var result = order.Deliver();
        if (result.IsFailure)
            return Result.Failure(result.Error!);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
