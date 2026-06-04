using MediatR;
using Shared.Domain;

namespace Modules.Catalog.Application.Commands.UploadProductImage;

public sealed record UploadProductImageCommand(int ProductId, Stream FileStream, string ContentType) : IRequest<Result<UploadProductImageResponse>>;