using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ModularApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SecureController : ControllerBase

{
    [Authorize]
    [HttpGet("protected")]
    public IActionResult GetSecureData()
    {
        return Ok(new { Message = "You are authorized!" });
    }
}