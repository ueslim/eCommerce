using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Features.Orders.CancelOrder;
using OrderManagement.Application.Features.Orders.CreateOrder;
using OrderManagement.Application.Features.Orders.GetOrder;
using OrderManagement.Application.Features.Orders.ListOrders;

namespace OrderManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/orders")] // Alinhado perfeitamente com a rota exigida [3]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 1. Criar pedido [3]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
        {
            var orderId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = orderId }, new { id = orderId });
        }

        // 2. Obter pedido por ID [3]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetOrderQuery(id));
            return result is not null ? Ok(result) : NotFound();
        }

        // 3. Listar pedidos com paginação [3]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new ListOrdersQuery(page, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // 4. Cancelar pedido (PATCH conforme o teste exige [3])
        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                await _mediator.Send(new CancelOrderCommand(id));
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
