using OrderingGrpc;
using Grpc.Core;
using Ordering.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Ordering.Domain;

namespace Ordering.Application.Services;

public class OrderingGrpcService : OrderingService.OrderingServiceBase
{
    private readonly IOrderingDbContext _context;

    public OrderingGrpcService(IOrderingDbContext context)
    {
        _context = context;
    }

    public override async Task<CheckPurchaseReply> CheckPurchase(CheckPurchaseRequest request, ServerCallContext context)
    {
        var userId = Guid.Parse(context.GetHttpContext().User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var hasPurchased = await _context.Orders.AnyAsync(o => o.UserId == userId && o.Status == OrderStatus.Delivered && o.Items.Any(i => i.ProductId == request.ProductId));

        return new CheckPurchaseReply { HasPurchased = hasPurchased };
    }
}