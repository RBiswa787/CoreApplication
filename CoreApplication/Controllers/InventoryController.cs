using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreApplication.Interfaces;
using CoreApplication.Model;
using CoreApplication.Infrastructure;
using CoreApplication.Helpers;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace CoreApplication.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class InventoryController : Controller
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryController(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        [HttpDelete("reset")]
        public async Task<ActionResult> ResetInventoryCollection()
        {
            try
            {
                await _inventoryRepository.Reset();
                return Ok("Inventory collection has been reset.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while resetting the inventory collection: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("current")]
        public async Task<IActionResult> GetCurrentInventory()
        {
            List<Inventory> inventory = await _inventoryRepository.GetCurrentInventory();

            return Ok(inventory);
        }

        [HttpGet("{sku}")]
        public async Task<IActionResult> AllocateLocation(string sku, int qty, string title)
        {
            var location = await _inventoryRepository.AllocateLocation(sku,qty,title);

            if (string.IsNullOrEmpty(location))
            {
                return NotFound(); // SKU not found or no locations available
            }

            return Ok(location);
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateInventory([FromBody] SKUObject skuObject)
        {
            try
            {
                var result = await _inventoryRepository.UpdateInventory(skuObject.location,skuObject.sku,skuObject.qty, skuObject.title);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deduct")]
        public async Task<IActionResult> DeductQty([FromBody] DeductDTO deductDTO)
        {
            var result = await _inventoryRepository.DeductQty(deductDTO.location, deductDTO.qty);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

    }

    public  class SKUObject
    {
        public string location { get; set; } = String.Empty;
        public string sku { get; set; } = String.Empty;
        public int qty { get; set; } = 0;

        public string title {  get; set; } = String.Empty;
    }

    public class DeductDTO
    {
        public string location { get; set; } = String.Empty;
        public int qty { get; set; } = 0;
    }

}
