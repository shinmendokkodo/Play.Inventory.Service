using System;
using Play.Inventory.Service.Models.Dtos;
using Play.Inventory.Service.Models.Entities;

namespace Play.Inventory.Service.Extensions
{
    public static class InventoryItemExtension
    {
        public static InventoryItemDto AsDto(this InventoryItem inventoryItem, string name, string description)
        {
            return new InventoryItemDto(inventoryItem.CatalogItemId, name, description, inventoryItem.Quantity, inventoryItem.AcquiredDate);
        }
    }
}
