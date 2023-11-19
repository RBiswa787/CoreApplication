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
    public class CheckOutController : Controller
    {
        private readonly ICheckOutRepository _checkOutRepository;

        public CheckOutController(ICheckOutRepository checkOutRepository)
        {
            _checkOutRepository = checkOutRepository;
        }

        public class CheckOutDto
        {
            public string SKU { get; set; }
            public int Qty { get; set; }
            public string Location { get; set; }

            public string Title { get; set; }

            public string OrderId {  get; set; }

            public bool IsCheckedOut { get; set; } = false;
        }

        [HttpPost]
        public async Task<IActionResult> AddCheckOut([FromBody] CheckOutDto checkOutDto)
        {

            await _checkOutRepository.AddCheckOut(checkOutDto.SKU, checkOutDto.Qty,checkOutDto.Title, checkOutDto.Location, checkOutDto.OrderId, checkOutDto.IsCheckedOut);

            return Ok();
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAllCheckOuts()
        {
            try
            {
                var checkOuts = await _checkOutRepository.GetAllCheckOuts();
                return Ok(checkOuts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("update")]
        public async Task<IActionResult> UpdateIsCheckedOut([FromBody] CheckOutRequest checkOutRequest)
        {
            var result = await _checkOutRepository.UpdateIsCheckedOut(checkOutRequest.sku, checkOutRequest.orderId);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteCheckOutsByOrderId(string orderId)
        {
            
                await _checkOutRepository.DeleteCheckOutsByOrderId(orderId);
                return Ok();
           
        }


    }

    public class CheckOutRequest
    {
        public string sku { get; set; }
        public string orderId { get; set; }
    }

}
