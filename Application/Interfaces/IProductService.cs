using Domain.Entities;

namespace Application.Interfaces;

public interface IProductService
{
    IEnumerable<Product> GetAll();
}