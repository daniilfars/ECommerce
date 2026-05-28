using MediatR;
using Shared.Domain;

namespace Modules.Catalog.Application.Queries.GetProducts;

public sealed record GetProductsQuery(int Page = 1, int PageSize = 10) : IRequest<Result<GetProductsResponse>>;