using API.Authorization;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [MustHavePermission]
    [Route("api/[controller]")]
    public class ProductController(IProductService service) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(service.GetAll());
    }

}
