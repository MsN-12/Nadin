using System.Runtime.InteropServices.JavaScript;
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
        [HttpGet]
        [Route("test")]
        [Authorize]
        public IActionResult Test()
        {
            var user = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
            var user2 = User.Identity.Name;
            var user3 = User.FindFirstValue(ClaimTypes.Email);
            Console.WriteLine("user : " + user);
            Console.WriteLine("user2 : " + user2);
            Console.WriteLine("user3 : " + user3);
            return Ok(user3);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProductDto createProductDto)
        {
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
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound();

            if (existingProduct.ManufactureEmail != _userManager.GetUserName(User))
                return Forbid();

            _mapper.Map(updateProductDto, existingProduct);
            await _productRepository.UpdateAsync(existingProduct);
    
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound();
            if (existingProduct.ManufactureEmail != _userManager.GetUserName(User))
                return Forbid();
            await _productRepository.DeleteAsync(existingProduct);
            return NoContent();
        }}
}
