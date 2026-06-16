using MediatR;
using Shared.Domain;

namespace Catalog.Application.Commands.UploadProductImage;

public sealed record UploadProductImageCommand(int ProductId, Stream FileStream, string ContentType) : IRequest<Result<UploadProductImageResponse>>;