using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Nanny.Admin.Application.DTOs;
using Nanny.Admin.Application.Services;

namespace Nanny.Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(
    ICustomerService service,
    IValidator<CreateCustomerDto> validator,
    IValidator<PaginationDto> paginationValidator)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<CustomerDto>>> GetAll([FromQuery] PaginationDto pagination)
    {
        var result = await paginationValidator.ValidateAsync(pagination);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }

        var items = await service.GetAllAsync(pagination.Page, pagination.PageSize);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Get(Guid id)
    {
        var customer = await service.GetByIdAsync(id);
        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerDto dto)
    {
        var result = await validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }

        var id = await service.CreateAsync(dto);
        var customer = await service.GetByIdAsync(id);
        return CreatedAtAction(nameof(Get), new { id }, customer);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Update(Guid id, [FromBody] CreateCustomerDto dto)
    {
        var result = await validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }

        await service.UpdateAsync(id, dto);
        var customer = await service.GetByIdAsync(id);
        return Ok(customer);
    }
}
