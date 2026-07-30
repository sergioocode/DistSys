using System.Net;
using DistSys.Services.Products.BusinessLogic.UseCases;
using DistSys.Services.Products.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DistSys.Services.Products.Api.Write.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController
{
    private readonly IUpdateProductDetails _updateProductDetails;
    private readonly ICreateProductDetails _createProductDetails;

    public ProductController(
        IUpdateProductDetails updateProductDetails,
        ICreateProductDetails createProductDetails
    )
    {
        _updateProductDetails = updateProductDetails;
        _createProductDetails = createProductDetails;
    }

    [HttpPost(Name = "addproduct")]
    [ProducesResponseType(typeof(ResultDto<CreateProductResponse>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> AddProduct(CreateProductRequest createProductRequest)
    {
        CreateProductResponse result = await _createProductDetails.Execute(createProductRequest);

        return result.Success().UseSuccessHttpStatusCode(HttpStatusCode.Created).ToActionResult();
    }

    [HttpPut("updateproductdetails/{id}")]
    [ProducesResponseType(typeof(ResultDto<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateProductDetails(int id, ProductDetails productDetails)
    {
        bool result = await _updateProductDetails.Execute(id, productDetails);

        return result.Success().UseSuccessHttpStatusCode(HttpStatusCode.OK).ToActionResult();
    }
}
