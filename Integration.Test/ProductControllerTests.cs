public class ProductsControllerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockMapper = new Mock<IMapper>();
        _controller = new ProductsController(_mockProductRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsNoContent_WhenNoProducts()
    {
        // Arrange
        _mockProductRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Product>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenProductsExist()
    {
        // Arrange
        var products = new List<Product> { new Product { Id = 1, Name = "Product1" } };
        _mockProductRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnProducts = Assert.IsType<List<Product>>(okResult.Value);
        Assert.Single(returnProducts);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenProductExists()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Product1" };
        _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnProduct = Assert.IsType<Product>(okResult.Value);
        Assert.Equal(product.Id, returnProduct.Id);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _controller.Create(new CreateProductDto());

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenProductIsCreated()
    {
        // Arrange
        var createProductDto = new CreateProductDto { Name = "New Product" };
        var product = new Product { Id = 1, Name = "New Product" };
        var productDto = new ProductDto { Id = 1, Name = "New Product" };

        _mockMapper.Setup(m => m.Map<Product>(It.IsAny<CreateProductDto>())).Returns(product);
        _mockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(productDto);
        _mockProductRepository.Setup(repo => repo.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Email, "test@example.com") }, "mock"));
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

        // Act
        var result = await _controller.Create(createProductDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnProduct = Assert.IsType<ProductDto>(createdAtActionResult.Value);
        Assert.Equal(productDto.Id, returnProduct.Id);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _controller.Update(1, new UpdateProductDto());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        int productId = 1;
        _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync((Product)null);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "test@example.com")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act
        var result = await _controller.Update(productId, new UpdateProductDto());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
 

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "test@example.com")
        }, "mock"));
    
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
 

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenProductIsDeleted()
    {
        // Arrange
        var product = new Product { Id = 1, ManufactureEmail = "test@example.com" };
        _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
        _mockProductRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Email, "test@example.com") }, "mock"));
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
