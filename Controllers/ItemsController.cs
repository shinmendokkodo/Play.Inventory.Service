using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common.Repositories.Interfaces;
using Play.Inventory.Service.Extensions;
using Play.Inventory.Service.Models.Dtos;
using Play.Inventory.Service.Models.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> inventoryRepository;
        private readonly IRepository<CatalogItem> catalogRepository;

        public ItemsController(IRepository<InventoryItem> inventoryRepository, IRepository<CatalogItem> catalogRepository)
        {
            this.inventoryRepository = inventoryRepository;
            this.catalogRepository = catalogRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var inventoryItems = await inventoryRepository.GetAllAsync(item => item.UserId == userId);
            var catalogItemIds = inventoryItems.Select(item => item.CatalogItemId);

            var catalogItems = await catalogRepository.GetAllAsync(item => catalogItemIds.Contains(item.Id));

            var inventoryItemDtos = inventoryItems.Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await inventoryRepository.GetAsync(item => item.UserId == grantItemsDto.UserId
                && item.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await inventoryRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await inventoryRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}
