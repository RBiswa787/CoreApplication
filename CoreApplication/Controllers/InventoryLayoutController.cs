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
    public class InventoryLayoutController : Controller
    {
        private readonly IInventoryLayoutRepository _inventoryLayoutRepository;

        public InventoryLayoutController(IInventoryLayoutRepository inventoryLayoutRepository)
        {
            _inventoryLayoutRepository = inventoryLayoutRepository;
        }

        [HttpGet]
        public async Task<ActionResult<InventoryLayout>> Get()
        {
            var inventoryLayout = await _inventoryLayoutRepository.GetInventoryLayout();
            if (inventoryLayout == null)
            {
                return NotFound(inventoryLayout);
            }
            return inventoryLayout;
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] InventoryLayout layout)
        {
            if (layout == null)
            {
                return BadRequest();
            }

            var isSuccess = await _inventoryLayoutRepository.UpdateInventoryLayout(layout);
            if (!isSuccess)
            {
                return StatusCode(500);
            }

            return NoContent();
        }

    }
}
