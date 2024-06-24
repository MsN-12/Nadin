using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nadin.Application.DTOs;
using Nadin.Application.Interfaces;
using Nadin.Core.Entities;

namespace Nadin.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            _productRepository = productRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productRepository.GetAllAsync();
            if (!products.Any())
            {
                return NoContent();
            }
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return BadRequest();
            }
            var product = _mapper.Map<Product>(createProductDto);
            product.ManufactureEmail = email;

            await _productRepository.AddAsync(product);
    
            var productDto = _mapper.Map<ProductDto>(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, productDto);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
                
            }
            var emailClaim = User.FindFirstValue(ClaimTypes.Email); 
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound();
            
            if (existingProduct.ManufactureEmail != emailClaim)
                return Forbid();

            _mapper.Map(updateProductDto, existingProduct);
            await _productRepository.UpdateAsync(existingProduct);
    
            return Ok("Product Updated");
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var emailClaim = User.FindFirstValue(ClaimTypes.Email); 
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound();
            
            if (existingProduct.ManufactureEmail != emailClaim)
                return Forbid();
            
            await _productRepository.DeleteAsync(existingProduct);
            return NoContent();
        }}
}
