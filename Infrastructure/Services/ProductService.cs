using Application.Interfaces;
using Domain.Entities;

namespace InfraStructure.Services;

public class ProductService : IProductService
{
    public IEnumerable<Product> GetAll()
    {
        return new List<Product>
        {
            new Product { Id = 1, Name = "Phone" },
            new Product { Id = 2, Name = "Laptop" }
        };
    }
}