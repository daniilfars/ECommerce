using MediatR;
using Modules.Ordering.Application.Interfaces;
using Modules.Ordering.Domain;
using Shared.Domain;

namespace Modules.Ordering.Application.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
{
    private readonly IOrderingDbContext _context;

    public CreateOrderHandler(IOrderingDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CreateOrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderResult = Order.Create(request.UserId, request.ShippingAddress);
        if (orderResult.IsFailure)
            return Result<CreateOrderResponse>.Failure(orderResult.Error!);

        var order = orderResult.Value;

        foreach (var itemDto in request.Items)
        {
            var itemResult = OrderItem.Create(itemDto.ProductId, itemDto.ProductName, itemDto.PriceAmount, itemDto.PriceCurrency, itemDto.Quantity);
            if (itemResult.IsFailure)
                return Result<CreateOrderResponse>.Failure(itemResult.Error!);

            order.AddItem(itemResult.Value!);
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CreateOrderResponse>.Success(new CreateOrderResponse(order.Id, order.UserId, order.ShippingAddress, order.Status.ToString(), order.TotalAmount));
    }
}
