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
using CoreApplication.Data;

namespace CoreApplication.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderRepository.GetAllOrders();

                if (orders == null || orders.Count() == 0)
                {
                    return NotFound();
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>> GetOrderById(string orderId)
        {
            try
            {
                var order = await _orderRepository.GetOrderById(orderId);

                if (order == null)
                {
                    return NotFound();
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("partner/{partnerId}")]
        public async Task<ActionResult<List<Order>>> GetOrdersByPartnerId(string partnerId)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByPartnerId(partnerId);

                if (orders == null || orders.Count() == 0)
                {
                    return NotFound();
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> AddOrder(Order order)
        {
            try
            {
                var result = await _orderRepository.AddOrder(order);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{orderId}/{status}")]
        public async Task<ActionResult<bool>> UpdateOrderStatus(string orderId, string status)
        {
            try
            {
                var result = await _orderRepository.UpdateOrderStatus(orderId, status);

                if (!result)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpDelete("{orderId}")]
        public async Task<ActionResult<bool>> RemoveOrder(string orderId)
        {
            try
            {
                var result = await _orderRepository.RemoveOrder(orderId);

                if (!result)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckInToInventory([FromBody] string Location, string SKU, int Qty, string Title)
        {
            await _orderRepository.CheckInToInventory(Location, SKU, Qty, Title);

            return Ok();
        }


    }
    
}
