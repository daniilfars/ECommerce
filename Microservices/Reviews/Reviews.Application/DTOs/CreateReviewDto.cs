namespace Reviews.Application.DTOs;

public sealed record CreateReviewDto(int ProductId, string Text, int Stars); // Dto для параметра контроллера
