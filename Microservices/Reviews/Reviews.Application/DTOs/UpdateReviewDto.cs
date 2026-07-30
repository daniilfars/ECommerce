namespace Reviews.Application.DTOs;

public sealed record UpdateReviewDto(int Id, string? Text = null, int? Stars = null); // Dto для параметра контроллера