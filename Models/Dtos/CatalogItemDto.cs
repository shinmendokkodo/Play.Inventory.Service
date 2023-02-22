using System;

namespace Play.Catalog.Service.Models.Dtos
{
    public record CatalogItemDto
    (
        Guid Id,
        string Name,
        string Description
    );
}