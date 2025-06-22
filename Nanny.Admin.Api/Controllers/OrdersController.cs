using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Nanny.Admin.Application.DTOs;
using Nanny.Admin.Application.Services;

namespace Nanny.Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(
    IOrderService service,
    IValidator<CreateOrderDto> createValidator,
    IValidator<CreateOrderLineDto> lineValidator,
    IValidator<ChangeOrderStatusDto> statusValidator,
    IValidator<PaginationDto> paginationValidator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<OrderDto>>> GetAll([FromQuery] PaginationDto pagination)
    {
        var result = await paginationValidator.ValidateAsync(pagination);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }

        return Ok(await service.GetAllAsync(pagination.Page, pagination.PageSize));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        var order = await service.GetByIdAsync(id);
        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto)
    {
        var result = await createValidator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }

        var id = await service.CreateAsync(dto);
        var order = await service.GetByIdAsync(id);
        return CreatedAtAction(nameof(GetById), new { id }, order);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<OrderDto>> ChangeStatus(Guid id, ChangeOrderStatusDto dto)
    {
        var result = await statusValidator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }

        await service.ChangeStatusAsync(id, dto.Status);

        var order = await service.GetByIdAsync(id);
        return Ok(order);
    }

    [HttpPost("{id:guid}/orderlines")]
    public async Task<ActionResult<OrderDto>> AddLine(Guid id, CreateOrderLineDto dto)
    {
        var result = await lineValidator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }

        await service.AddOrderLineAsync(id, dto);
        
        var order = await service.GetByIdAsync(id);
        return Ok(order);
    }

    [HttpDelete("{id:guid}/lines/{lineId:guid}")]
    public async Task<ActionResult> DeleteLine(Guid id, Guid lineId)
    {
        await service.RemoveOrderLineAsync(id, lineId);
        return NoContent();
    }
}
