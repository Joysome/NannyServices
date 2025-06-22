using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Nanny.Admin.Application.DTOs;
using Nanny.Admin.Application.Services;

namespace Nanny.Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(
    IProductService service,
    IValidator<CreateProductDto> validator,
    IValidator<PaginationDto> paginationValidator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<ProductDto>>> GetAll([FromQuery] PaginationDto pagination)
    {
        var result = await paginationValidator.ValidateAsync(pagination);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }

        var products = await service.GetAllAsync(pagination.Page, pagination.PageSize);
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await service.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
    {
        var result = await validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }

        var id = await service.CreateAsync(dto);
        var product = await service.GetByIdAsync(id);
        return CreatedAtAction(nameof(GetById), new { id }, product);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, CreateProductDto dto)
    {
        var result = await validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }

        await service.UpdateAsync(id, dto);
        var product = await service.GetByIdAsync(id);
        return Ok(product);
    }
}
