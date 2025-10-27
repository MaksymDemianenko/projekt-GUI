using System.Collections.Generic;

public interface IProductService
{
    List<Produkt> GetAllProducts();
    Produkt GetProductById(int id);
    void AddProduct(Produkt produkt);
    void UpdateProduct(Produkt produkt);
    void DeleteProduct(int id);
    void SaveToCsv();
    void LoadFromCsv();
}