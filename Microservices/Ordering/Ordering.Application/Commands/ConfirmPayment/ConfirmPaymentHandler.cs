using MediatR;
using Ordering.Application.Interfaces;
using Shared.Domain;

public class ConfirmPaymentHandler : IRequestHandler<ConfirmPaymentCommand, Result>
{
    private readonly IOrderingDbContext _context;

    public ConfirmPaymentHandler(IOrderingDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure("Заказ не найден");

        var result = order.Pay();
        if (result.IsFailure)
            return Result.Failure(result.Error!);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}