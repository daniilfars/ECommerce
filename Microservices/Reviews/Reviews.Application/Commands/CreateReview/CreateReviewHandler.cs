using Grpc.Core;
using MediatR;
using OrderingGrpc;
using Reviews.Application.Interfaces;
using Reviews.Domain;
using Shared.Domain;

namespace Reviews.Application.Commands.CreateReview;

public sealed class CreateReviewHandler : IRequestHandler<CreateReviewCommand, Result<CreateReviewResponse>>
{
    private readonly IReviewsDbContext _context;
    private readonly OrderingService.OrderingServiceClient _orderingServiceClient;

    public CreateReviewHandler(IReviewsDbContext context, OrderingService.OrderingServiceClient orderingServiceClient)
    {
        _context = context;
        _orderingServiceClient = orderingServiceClient;
    }

    public async Task<Result<CreateReviewResponse>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var checkPurchaseReply = await _orderingServiceClient.CheckPurchaseAsync(new CheckPurchaseRequest { ProductId = request.ProductId }, cancellationToken: cancellationToken);

            if (!checkPurchaseReply.HasPurchased)
                return Result<CreateReviewResponse>.Failure("Нету прав на отзыв");

            var reviewResult = Review.Create(request.UserId, request.ProductId, request.Text, request.Stars);
            if(reviewResult.IsFailure)
                return Result<CreateReviewResponse>.Failure(reviewResult.Error!);

            var review = reviewResult.Value!;

            _context.Reviews.Add(review);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<CreateReviewResponse>.Success(new CreateReviewResponse(request.UserId, review.Id, request.ProductId, request.Text, request.Stars));
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return Result<CreateReviewResponse>.Failure(ex.Message);
        }
        catch (RpcException ex)
        {
            return Result<CreateReviewResponse>.Failure($"Ошибка связи с Ordering: {ex.Status.Detail}");
        }
    }
}