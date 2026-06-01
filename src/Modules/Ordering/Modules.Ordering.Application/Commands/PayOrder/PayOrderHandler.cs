using MediatR;
using Modules.Ordering.Application.Interfaces;
using Shared.Domain;

namespace Modules.Ordering.Application.Commands.PayOrder;

public class PayOrderHandler : IRequestHandler<PayOrderCommand, Result>
{
    private readonly IOrderingDbContext _context;

    public PayOrderHandler(IOrderingDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(PayOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure("Заказ не найден");

        if (order.UserId != request.UserId)
            return Result.Failure("Нет доступа к заказу");

        var result = order.Pay();
        if (result.IsFailure)
            return Result.Failure(result.Error!);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
