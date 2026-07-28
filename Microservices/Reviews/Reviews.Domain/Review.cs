using Shared.Domain;

namespace Reviews.Domain;

public class Review : AggregateRoot<int>
{
    public Guid UserId { get; private set; }
    public int ProductId { get; private set; }
    public string Text { get; private set; }
    public int Stars { get; private set; }

    private Review() { }

    private Review(Guid userId, int productId, string text, int stars)
    {
        UserId = userId;
        ProductId = productId;
        Text = text;
        Stars = stars;
    }

    public static Result<Review> Create(Guid userId, int productId, string text, int stars)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result<Review>.Failure("Текст отзыва не может быть пустым");

        if (stars <= 0 || stars > 5)
            return Result<Review>.Failure("Количество звезд не может быть равно нулю");

        var review = new Review(userId, productId, text, stars);
        return Result<Review>.Success(review);
    }

    public Result UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result.Failure("Текст отзыва не может быть пустым");

        Text = text;
        return Result.Success();
    }

    public Result UpdateStars(int stars)
    {
        if (stars <= 0 || stars > 5)
            return Result.Failure("Количество звезд не может быть равно нулю");

        Stars = stars;
        return Result.Success();
    }
}