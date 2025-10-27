using System;
using System.Collections.Generic;
using Terminal.Gui;

class Program
{
    private static IProductService _productService;
    private static ListView _listaProduktow;
    private static List<string> _listaWyswietlana = new List<string>();

    static void Main()
    {
        // Inicjalizacja SLM
        ServiceLocator.Initialize();
        _productService = ServiceLocator.GetService<IProductService>();
        
        // Wczytaj dane
        try
        {
            _productService.LoadFromCsv();
        }
        catch (Exception e)
        {
            MessageBox.ErrorQuery("Błąd", e.Message, "OK");
        }

        Application.Init();
        AktualizujListeWyswietlana();

        // Główne okno
        var okno = new Window("Menadżer Produktów (SLM Architecture)");
        okno.X = 0;
        okno.Y = 1;
        okno.Width = Dim.Fill();
        okno.Height = Dim.Fill();

        // Ramka z listą produktów
        var ramkaListy = new FrameView("Lista produktów");
        ramkaListy.X = 0;
        ramkaListy.Y = 0;
        ramkaListy.Width = Dim.Fill();
        ramkaListy.Height = Dim.Fill() - 3;

        _listaProduktow = new ListView(_listaWyswietlana);
        _listaProduktow.X = 0;
        _listaProduktow.Y = 0;
        _listaProduktow.Width = Dim.Fill();
        _listaProduktow.Height = Dim.Fill();
        
        ramkaListy.Add(_listaProduktow);

        // Przyciski na dole
        var ramkaPrzyciskow = new FrameView("Akcje");
        ramkaPrzyciskow.X = 0;
        ramkaPrzyciskow.Y = Pos.Bottom(ramkaListy);
        ramkaPrzyciskow.Width = Dim.Fill();
        ramkaPrzyciskow.Height = 3;

        var przyciskDodaj = new Button("Dodaj");
        przyciskDodaj.X = 0;
        przyciskDodaj.Y = 0;
        przyciskDodaj.Clicked += DodajProdukt;

        var przyciskEdytuj = new Button("Edytuj");
        przyciskEdytuj.X = Pos.Right(przyciskDodaj) + 1;
        przyciskEdytuj.Y = 0;
        przyciskEdytuj.Clicked += EdytujProdukt;

        var przyciskUsun = new Button("Usuń");
        przyciskUsun.X = Pos.Right(przyciskEdytuj) + 1;
        przyciskUsun.Y = 0;
        przyciskUsun.Clicked += UsunProdukt;

        var przyciskZapisz = new Button("Zapisz CSV");
        przyciskZapisz.X = Pos.Right(przyciskUsun) + 1;
        przyciskZapisz.Y = 0;
        przyciskZapisz.Clicked += () => 
        { 
            try
            {
                _productService.SaveToCsv();
                MessageBox.Query("Sukces", "Zapisano do CSV!", "OK");
            }
            catch (Exception e)
            {
                MessageBox.ErrorQuery("Błąd", e.Message, "OK");
            }
        };

        var przyciskWyjscie = new Button("Wyjście");
        przyciskWyjscie.X = Pos.Right(przyciskZapisz) + 1;
        przyciskWyjscie.Y = 0;
        przyciskWyjscie.Clicked += () => 
        { 
            if (MessageBox.Query("Wyjście", "Na pewno wyjść?", "Tak", "Nie") == 0)
            {
                ServiceLocator.Dispose();
                Application.RequestStop();
            }
        };

        ramkaPrzyciskow.Add(przyciskDodaj, przyciskEdytuj, przyciskUsun, przyciskZapisz, przyciskWyjscie);
        okno.Add(ramkaListy, ramkaPrzyciskow);

        Application.Top.Add(okno);
        Application.Run();
        Application.Shutdown();
    }

    static void AktualizujListeWyswietlana()
    {
        _listaWyswietlana.Clear();
        foreach (var p in _productService.GetAllProducts())
        {
            string linia = $"{p.Id} - {p.Nazwa} - {p.Cena} zł - {p.Kategoria}";
            _listaWyswietlana.Add(linia);
        }
        _listaProduktow?.SetSource(_listaWyswietlana);
    }

    static void DodajProdukt()
    {
        var oknoDodaj = new Window("Dodaj produkt");
        oknoDodaj.Width = 50;
        oknoDodaj.Height = 12;

        var labelNazwa = new Label("Nazwa:") { X = 1, Y = 1 };
        var poleNazwa = new TextField("") { X = 1, Y = 2, Width = 40 };

        var labelCena = new Label("Cena:") { X = 1, Y = 4 };
        var poleCena = new TextField("") { X = 1, Y = 5, Width = 15 };

        var labelKategoria = new Label("Kategoria:") { X = 1, Y = 7 };
        var poleKategoria = new TextField("") { X = 1, Y = 8, Width = 40 };

        var przyciskOK = new Button("OK") { X = 10, Y = 10 };
        przyciskOK.Clicked += () =>
        {
            if (string.IsNullOrWhiteSpace(poleNazwa.Text.ToString()))
            {
                MessageBox.ErrorQuery("Błąd", "Podaj nazwę!", "OK");
                return;
            }

            var nowy = new Produkt
            {
                Nazwa = poleNazwa.Text.ToString() ?? "",
                Cena = double.TryParse(poleCena.Text.ToString(), out double cena) ? cena : 0,
                Kategoria = poleKategoria.Text.ToString() ?? ""
            };

            _productService.AddProduct(nowy);
            AktualizujListeWyswietlana();
            Application.RequestStop();
        };

        var przyciskAnuluj = new Button("Anuluj") { X = 20, Y = 10 };
        przyciskAnuluj.Clicked += () => Application.RequestStop();

        oknoDodaj.Add(labelNazwa, poleNazwa, labelCena, poleCena, labelKategoria, poleKategoria, przyciskOK, przyciskAnuluj);
        Application.Run(oknoDodaj);
    }

    static void EdytujProdukt()
    {
        var produkty = _productService.GetAllProducts();
        if (produkty.Count == 0)
        {
            MessageBox.ErrorQuery("Błąd", "Brak produktów!", "OK");
            return;
        }

        int wybraneId = _listaProduktow.SelectedItem;
        if (wybraneId < 0 || wybraneId >= produkty.Count)
        {
            MessageBox.ErrorQuery("Błąd", "Wybierz produkt!", "OK");
            return;
        }

        var produkt = produkty[wybraneId];
        
        var oknoEdytuj = new Window("Edytuj produkt");
        oknoEdytuj.Width = 50;
        oknoEdytuj.Height = 12;

        var labelNazwa = new Label("Nazwa:") { X = 1, Y = 1 };
        var poleNazwa = new TextField(produkt.Nazwa ?? "") { X = 1, Y = 2, Width = 40 };

        var labelCena = new Label("Cena:") { X = 1, Y = 4 };
        var poleCena = new TextField(produkt.Cena.ToString()) { X = 1, Y = 5, Width = 15 };

        var labelKategoria = new Label("Kategoria:") { X = 1, Y = 7 };
        var poleKategoria = new TextField(produkt.Kategoria ?? "") { X = 1, Y = 8, Width = 40 };

        var przyciskOK = new Button("Zapisz") { X = 10, Y = 10 };
        przyciskOK.Clicked += () =>
        {
            if (!string.IsNullOrWhiteSpace(poleNazwa.Text.ToString()))
                produkt.Nazwa = poleNazwa.Text.ToString() ?? "";

            if (double.TryParse(poleCena.Text.ToString(), out double cena))
                produkt.Cena = cena;

            produkt.Kategoria = poleKategoria.Text.ToString() ?? "";
            
            _productService.UpdateProduct(produkt);
            AktualizujListeWyswietlana();
            Application.RequestStop();
        };

        var przyciskAnuluj = new Button("Anuluj") { X = 20, Y = 10 };
        przyciskAnuluj.Clicked += () => Application.RequestStop();

        oknoEdytuj.Add(labelNazwa, poleNazwa, labelCena, poleCena, labelKategoria, poleKategoria, przyciskOK, przyciskAnuluj);
        Application.Run(oknoEdytuj);
    }

    static void UsunProdukt()
    {
        var produkty = _productService.GetAllProducts();
        if (produkty.Count == 0)
        {
            MessageBox.ErrorQuery("Błąd", "Brak produktów!", "OK");
            return;
        }

        int wybraneId = _listaProduktow.SelectedItem;
        if (wybraneId < 0 || wybraneId >= produkty.Count)
        {
            MessageBox.ErrorQuery("Błąd", "Wybierz produkt!", "OK");
            return;
        }

        var produkt = produkty[wybraneId];
        
        if (MessageBox.Query("Usuń", $"Usunąć {produkt.Nazwa}?", "Tak", "Nie") == 0)
        {
            _productService.DeleteProduct(produkt.Id);
            AktualizujListeWyswietlana();
            MessageBox.Query("Sukces", "Usunięto!", "OK");
        }
    }
}