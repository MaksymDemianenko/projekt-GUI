using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ProductService : IProductService
{
    private readonly string _sciezkaCSV = "produkty.csv";
    private List<Produkt> _produkty = new List<Produkt>();

    public List<Produkt> GetAllProducts() => _produkty;

    public Produkt GetProductById(int id) => _produkty.FirstOrDefault(p => p.Id == id);

    public void AddProduct(Produkt produkt)
    {
        // Znajdź największe ID
        int maxId = _produkty.Count > 0 ? _produkty.Max(p => p.Id) : 0;
        produkt.Id = maxId + 1;
        produkt.Utworzono = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _produkty.Add(produkt);
    }

    public void UpdateProduct(Produkt produkt)
    {
        var existing = GetProductById(produkt.Id);
        if (existing != null)
        {
            existing.Nazwa = produkt.Nazwa;
            existing.Cena = produkt.Cena;
            existing.Kategoria = produkt.Kategoria;
        }
    }

    public void DeleteProduct(int id)
    {
        var produkt = GetProductById(id);
        if (produkt != null)
            _produkty.Remove(produkt);
    }

    public void SaveToCsv()
    {
        try
        {
            var linie = _produkty.Select(p => $"{p.Id},{p.Nazwa},{p.Cena},{p.Kategoria},{p.Utworzono}");
            File.WriteAllLines(_sciezkaCSV, linie);
        }
        catch (Exception e)
        {
            throw new Exception($"Błąd zapisu: {e.Message}");
        }
    }

    public void LoadFromCsv()
    {
        _produkty.Clear();
        
        if (!File.Exists(_sciezkaCSV)) return;

        try
        {
            var linie = File.ReadAllLines(_sciezkaCSV);
            foreach (var linia in linie)
            {
                var czesci = linia.Split(',');
                if (czesci.Length >= 5)
                {
                    _produkty.Add(new Produkt
                    {
                        Id = int.Parse(czesci[0]),
                        Nazwa = czesci[1],
                        Cena = double.Parse(czesci[2]),
                        Kategoria = czesci[3],
                        Utworzono = czesci[4]
                    });
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Błąd odczytu: {e.Message}");
        }
    }
}