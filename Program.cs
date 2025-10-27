using System;
using System.IO;
using System.Collections.Generic;
using Terminal.Gui;

class Produkt
{
    public int Id;
    public string? Nazwa;
    public double Cena;
    public string? Kategoria;
    public string? Utworzono;
}

class Program
{
    static string sciezkaCSV = "produkty.csv";
    static List<Produkt> produkty = new List<Produkt>();
    static ListView? listaProduktow;
    static List<string> listaWyswietlana = new List<string>();

    static void Main()
    {
        Application.Init();
        WczytajCSV();
        AktualizujListeWyswietlana();

        // Główne okno
        var okno = new Window("Menadżer Produktów");
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

        listaProduktow = new ListView(listaWyswietlana);
        listaProduktow.X = 0;
        listaProduktow.Y = 0;
        listaProduktow.Width = Dim.Fill();
        listaProduktow.Height = Dim.Fill();
        
        ramkaListy.Add(listaProduktow);

        // Przyciski na dole
        var ramkaPrzyciskow = new FrameView("Akcje");
        ramkaPrzyciskow.X = 0;
        ramkaPrzyciskow.Y = Pos.Bottom(ramkaListy);
        ramkaPrzyciskow.Width = Dim.Fill();
        ramkaPrzyciskow.Height = 3;

        var przyciskDodaj = new Button("Dodaj");
        przyciskDodaj.X = 0;
        przyciskDodaj.Y = 0;
        przyciskDodaj.Clicked += () => { DodajProdukt(); };

        var przyciskEdytuj = new Button("Edytuj");
        przyciskEdytuj.X = Pos.Right(przyciskDodaj) + 1;
        przyciskEdytuj.Y = 0;
        przyciskEdytuj.Clicked += () => { EdytujProdukt(); };

        var przyciskUsun = new Button("Usuń");
        przyciskUsun.X = Pos.Right(przyciskEdytuj) + 1;
        przyciskUsun.Y = 0;
        przyciskUsun.Clicked += () => { UsunProdukt(); };

        var przyciskZapisz = new Button("Zapisz CSV");
        przyciskZapisz.X = Pos.Right(przyciskUsun) + 1;
        przyciskZapisz.Y = 0;
        przyciskZapisz.Clicked += () => { 
            ZapiszCSV(); 
            MessageBox.Query("Sukces", "Zapisano do CSV!", "OK"); 
        };

        var przyciskWyjscie = new Button("Wyjście");
        przyciskWyjscie.X = Pos.Right(przyciskZapisz) + 1;
        przyciskWyjscie.Y = 0;
        przyciskWyjscie.Clicked += () => { 
            if (MessageBox.Query("Wyjście", "Na pewno wyjść?", "Tak", "Nie") == 0)
                Application.RequestStop(); 
        };

        ramkaPrzyciskow.Add(przyciskDodaj, przyciskEdytuj, przyciskUsun, przyciskZapisz, przyciskWyjscie);
        okno.Add(ramkaListy, ramkaPrzyciskow);

        Application.Top.Add(okno);
        Application.Run();
        Application.Shutdown();
    }

    static void AktualizujListeWyswietlana()
    {
        listaWyswietlana.Clear();
        foreach (var p in produkty)
        {
            string linia = $"{p.Id} - {p.Nazwa} - {p.Cena} zł - {p.Kategoria}";
            listaWyswietlana.Add(linia);
        }
        if (listaProduktow != null)
            listaProduktow.SetSource(listaWyswietlana);
    }

    static void DodajProdukt()
    {
        var oknoDodaj = new Window("Dodaj produkt");
        oknoDodaj.Width = 50;
        oknoDodaj.Height = 12;

        var labelNazwa = new Label("Nazwa:");
        labelNazwa.X = 1;
        labelNazwa.Y = 1;

        var poleNazwa = new TextField("");
        poleNazwa.X = 1;
        poleNazwa.Y = 2;
        poleNazwa.Width = 40;

        var labelCena = new Label("Cena:");
        labelCena.X = 1;
        labelCena.Y = 4;

        var poleCena = new TextField("");
        poleCena.X = 1;
        poleCena.Y = 5;
        poleCena.Width = 15;

        var labelKategoria = new Label("Kategoria:");
        labelKategoria.X = 1;
        labelKategoria.Y = 7;

        var poleKategoria = new TextField("");
        poleKategoria.X = 1;
        poleKategoria.Y = 8;
        poleKategoria.Width = 40;

        var przyciskOK = new Button("OK");
        przyciskOK.X = 10;
        przyciskOK.Y = 10;
        przyciskOK.Clicked += () =>
        {
            if (string.IsNullOrWhiteSpace(poleNazwa.Text.ToString()))
            {
                MessageBox.ErrorQuery("Błąd", "Podaj nazwę!", "OK");
                return;
            }

            Produkt nowy = new Produkt();
            
            // Znajdź największe ID
            int maxId = 0;
            foreach (var p in produkty)
            {
                if (p.Id > maxId) maxId = p.Id;
            }
            nowy.Id = maxId + 1;
            
            nowy.Nazwa = poleNazwa.Text.ToString() ?? "";
            
            if (double.TryParse(poleCena.Text.ToString(), out double cena))
                nowy.Cena = cena;
            else
                nowy.Cena = 0;
            
            nowy.Kategoria = poleKategoria.Text.ToString() ?? "";
            nowy.Utworzono = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            produkty.Add(nowy);
            AktualizujListeWyswietlana();
            Application.RequestStop();
        };

        var przyciskAnuluj = new Button("Anuluj");
        przyciskAnuluj.X = 20;
        przyciskAnuluj.Y = 10;
        przyciskAnuluj.Clicked += () => { Application.RequestStop(); };

        oknoDodaj.Add(labelNazwa, poleNazwa, labelCena, poleCena, labelKategoria, poleKategoria, przyciskOK, przyciskAnuluj);
        Application.Run(oknoDodaj);
    }

    static void EdytujProdukt()
    {
        if (produkty.Count == 0)
        {
            MessageBox.ErrorQuery("Błąd", "Brak produktów!", "OK");
            return;
        }

        if (listaProduktow == null) return;

        int wybraneId = listaProduktow.SelectedItem;
        if (wybraneId < 0 || wybraneId >= produkty.Count)
        {
            MessageBox.ErrorQuery("Błąd", "Wybierz produkt!", "OK");
            return;
        }

        var produkt = produkty[wybraneId];
        
        var oknoEdytuj = new Window("Edytuj produkt");
        oknoEdytuj.Width = 50;
        oknoEdytuj.Height = 12;

        var labelNazwa = new Label("Nazwa:");
        labelNazwa.X = 1;
        labelNazwa.Y = 1;

        var poleNazwa = new TextField(produkt.Nazwa ?? "");
        poleNazwa.X = 1;
        poleNazwa.Y = 2;
        poleNazwa.Width = 40;

        var labelCena = new Label("Cena:");
        labelCena.X = 1;
        labelCena.Y = 4;

        var poleCena = new TextField(produkt.Cena.ToString());
        poleCena.X = 1;
        poleCena.Y = 5;
        poleCena.Width = 15;

        var labelKategoria = new Label("Kategoria:");
        labelKategoria.X = 1;
        labelKategoria.Y = 7;

        var poleKategoria = new TextField(produkt.Kategoria ?? "");
        poleKategoria.X = 1;
        poleKategoria.Y = 8;
        poleKategoria.Width = 40;

        var przyciskOK = new Button("Zapisz");
        przyciskOK.X = 10;
        przyciskOK.Y = 10;
        przyciskOK.Clicked += () =>
        {
            if (!string.IsNullOrWhiteSpace(poleNazwa.Text.ToString()))
                produkt.Nazwa = poleNazwa.Text.ToString() ?? "";

            if (double.TryParse(poleCena.Text.ToString(), out double cena))
                produkt.Cena = cena;

            produkt.Kategoria = poleKategoria.Text.ToString() ?? "";
            
            AktualizujListeWyswietlana();
            Application.RequestStop();
        };

        var przyciskAnuluj = new Button("Anuluj");
        przyciskAnuluj.X = 20;
        przyciskAnuluj.Y = 10;
        przyciskAnuluj.Clicked += () => { Application.RequestStop(); };

        oknoEdytuj.Add(labelNazwa, poleNazwa, labelCena, poleCena, labelKategoria, poleKategoria, przyciskOK, przyciskAnuluj);
        Application.Run(oknoEdytuj);
    }

    static void UsunProdukt()
    {
        if (produkty.Count == 0)
        {
            MessageBox.ErrorQuery("Błąd", "Brak produktów!", "OK");
            return;
        }

        if (listaProduktow == null) return;

        int wybraneId = listaProduktow.SelectedItem;
        if (wybraneId < 0 || wybraneId >= produkty.Count)
        {
            MessageBox.ErrorQuery("Błąd", "Wybierz produkt!", "OK");
            return;
        }

        var produkt = produkty[wybraneId];
        
        if (MessageBox.Query("Usuń", $"Usunąć {produkt.Nazwa}?", "Tak", "Nie") == 0)
        {
            produkty.RemoveAt(wybraneId);
            AktualizujListeWyswietlana();
            MessageBox.Query("Sukces", "Usunięto!", "OK");
        }
    }

    static void WczytajCSV()
    {
        if (File.Exists(sciezkaCSV))
        {
            try
            {
                string[] linie = File.ReadAllLines(sciezkaCSV);
                foreach (string linia in linie)
                {
                    string[] czesci = linia.Split(',');
                    if (czesci.Length >= 5)
                    {
                        Produkt p = new Produkt();
                        p.Id = int.Parse(czesci[0]);
                        p.Nazwa = czesci[1];
                        p.Cena = double.Parse(czesci[2]);
                        p.Kategoria = czesci[3];
                        p.Utworzono = czesci[4];
                        produkty.Add(p);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.ErrorQuery("Błąd", "Nie udało się wczytać: " + e.Message, "OK");
            }
        }
    }

    static void ZapiszCSV()
    {
        try
        {
            List<string> linie = new List<string>();
            foreach (var p in produkty)
            {
                linie.Add($"{p.Id},{p.Nazwa},{p.Cena},{p.Kategoria},{p.Utworzono}");
            }
            File.WriteAllLines(sciezkaCSV, linie);
        }
        catch (Exception e)
        {
            MessageBox.ErrorQuery("Błąd", "Nie udało się zapisać: " + e.Message, "OK");
        }
    }
}