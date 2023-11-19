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
    public class SuppliesController : Controller
    {
        private readonly ISuppliesRepository _suppliesRepository;

        public SuppliesController(ISuppliesRepository suppliesRepository)
        {
            _suppliesRepository = suppliesRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplies>>> GetSupplies()
        {
            var supplies = await _suppliesRepository.GetAllSupplies();

            if (supplies == null)
            {
                return NotFound();
            }

            return Ok(supplies);
        }

        [HttpGet("{sku}")]
        public async Task<ActionResult<Supplies>> GetSupplyById(string sku)
        {
            var supply = await _suppliesRepository.GetSupplyByIdAsync(sku);

            if (supply == null)
            {
                return NotFound();
            }

            return Ok(supply);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSupply(Supplies supply)
        {
            await _suppliesRepository.CreateSupplyAsync(supply);

            return CreatedAtAction(nameof(GetSupplyById), new { sku = supply.SKU }, supply);
        }


        [HttpDelete("{sku}")]
        public async Task<ActionResult> DeleteSupply(string sku)
        {
            var result = await _suppliesRepository.DeleteSupplyAsync(sku);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet]
        [Route("api/supplies/{supplier}")]
        public async Task<ActionResult<List<Supplies>>> GetSuppliesBySupplier(string supplier)
        {
            try
            {
                var supplies = await _suppliesRepository.GetSuppliesBySupplier(supplier);

                if (supplies == null || supplies.Count() == 0)
                {
                    return NotFound();
                }

                return Ok(supplies);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

    }
}
