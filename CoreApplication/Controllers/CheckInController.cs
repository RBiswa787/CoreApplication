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
    public class CheckInController : Controller
    {
        private readonly ICheckInRepository _checkInRepository;

        public CheckInController(ICheckInRepository checkInRepository)
        {
            _checkInRepository = checkInRepository;
        }

        public class CheckInDto
        {
            public string SKU { get; set; }
            public int Qty { get; set; }
            public string Location { get; set; }

            public string Title {  get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> AddCheckIn([FromBody] CheckInDto checkInDto)
        {

            await _checkInRepository.AddCheckIn(checkInDto.SKU, checkInDto.Qty, checkInDto.Location,checkInDto.Location);

            return Ok();
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAllCheckIns()
        {
            try
            {
                var checkIns = await _checkInRepository.GetAllCheckIns();
                return Ok(checkIns);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{location}")]
        public async Task<IActionResult> DeleteCheckIns(string location)
        {
            await _checkInRepository.DeleteCheckInsByLocation(location);
            return Ok();
        }



    }

   
}
