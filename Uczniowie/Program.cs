using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uczniowie;

class Program
{
    private static readonly string SciezkaPliku = "uczniowie.csv";
    
    static void Main()
    {
        Dictionary<string, Uczen> uczniowie = new Dictionary<string, Uczen>();

        WczytajUczniowZPliku(uczniowie);

        Console.WriteLine("Program zarządzania uczniami");
        Console.WriteLine("Wpisz 'stop' jako imię, aby zakończyć dodawanie uczniów.");

        while (true)
        {
            Console.WriteLine("\nPodaj dane nowego ucznia:");
            Console.Write("Imię: ");
            string imie = Console.ReadLine();

            if (imie.ToLower() == "stop")
                break;

            int wiek = 0;
            bool wiekPoprawny = false;
            while (!wiekPoprawny)
            {
                Console.Write("Wiek: ");
                string wiekStr = Console.ReadLine();
                try
                {
                    wiek = int.Parse(wiekStr);
                    wiekPoprawny = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Błąd: Wprowadź poprawną liczbę całkowitą jako wiek.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                }
            }

            Console.Write("Ulubione przedmioty (oddzielone przecinkami): ");
            string przedmiotyStr = Console.ReadLine();
            List<string> przedmioty = przedmiotyStr.Split(',')
                                                  .Select(p => p.Trim())
                                                  .Where(p => !string.IsNullOrWhiteSpace(p))
                                                  .ToList();

            Uczen uczen = new Uczen(imie, wiek, przedmioty);
            if (uczniowie.ContainsKey(imie))
            {
                Console.WriteLine($"Uwaga: Zastępuję istniejącego ucznia o imieniu {imie}.");
                uczniowie[imie] = uczen;
            }
            else
            {
                uczniowie.Add(imie, uczen);
            }
        }

        ZapiszUczniowDoPliku(uczniowie);

        WyswietlUczniow(uczniowie);
        
        Console.Write("\nPokaż uczniów powyżej wieku (podaj wiek): ");
        if (int.TryParse(Console.ReadLine(), out int progWieku))
        {
            FiltrujUczniowPoWieku(uczniowie, progWieku);
        }
        else
        {
            Console.WriteLine("Nieprawidłowy wiek.");
        }
    }

    private static void WczytajUczniowZPliku(Dictionary<string, Uczen> uczniowie)
    {
        if (!File.Exists(SciezkaPliku))
            return;

        try
        {
            string[] linie = File.ReadAllLines(SciezkaPliku);
            foreach (string linia in linie)
            {
                if (string.IsNullOrWhiteSpace(linia))
                    continue;

                try
                {
                    Uczen uczen = Uczen.ZCSVLine(linia);
                    if (!uczniowie.ContainsKey(uczen.Imie))
                    {
                        uczniowie.Add(uczen.Imie, uczen);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas wczytywania linii: {linia}. Błąd: {ex.Message}");
                }
            }
            Console.WriteLine($"Wczytano {uczniowie.Count} uczniów z pliku.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd podczas wczytywania pliku: {ex.Message}");
        }
    }

    private static void ZapiszUczniowDoPliku(Dictionary<string, Uczen> uczniowie)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(SciezkaPliku))
            {
                foreach (var uczen in uczniowie.Values)
                {
                    sw.WriteLine(uczen.DoCSV());
                }
            }
            Console.WriteLine($"Zapisano {uczniowie.Count} uczniów do pliku {SciezkaPliku}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd podczas zapisywania do pliku: {ex.Message}");
        }
    }

    private static void WyswietlUczniow(Dictionary<string, Uczen> uczniowie)
    {
        if (uczniowie.Count == 0)
        {
            Console.WriteLine("\nBrak uczniów do wyświetlenia.");
            return;
        }

        Console.WriteLine("\nLista wszystkich uczniów:");
        Console.WriteLine(new string('-', 80));
        Console.WriteLine($"{"Imię",-15} | {"Wiek",5} | {"Ulubione przedmioty",-50}");
        Console.WriteLine(new string('-', 80));

        foreach (var uczen in uczniowie.Values)
        {
            string przedmioty = string.Join(", ", uczen.UlubionePrzedmioty);
            if (przedmioty.Length > 47)
            {
                przedmioty = przedmioty.Substring(0, 47) + "...";
            }
            Console.WriteLine($"{uczen.Imie,-15} | {uczen.Wiek,5} | {przedmioty,-50}");
        }
        Console.WriteLine(new string('-', 80));
    }

    private static void FiltrujUczniowPoWieku(Dictionary<string, Uczen> uczniowie, int minWiek)
    {
        var filtrowaniUczniowie = uczniowie.Values.Where(u => u.Wiek > minWiek).ToList();

        if (filtrowaniUczniowie.Count == 0)
        {
            Console.WriteLine($"\nBrak uczniów powyżej {minWiek} lat.");
            return;
        }

        Console.WriteLine($"\nUczniowie powyżej {minWiek} lat:");
        Console.WriteLine(new string('-', 80));
        Console.WriteLine($"{"Imię",-15} | {"Wiek",5} | {"Ulubione przedmioty",-50}");
        Console.WriteLine(new string('-', 80));

        foreach (var uczen in filtrowaniUczniowie)
        {
            string przedmioty = string.Join(", ", uczen.UlubionePrzedmioty);
            if (przedmioty.Length > 47)
            {
                przedmioty = przedmioty.Substring(0, 47) + "...";
            }
            Console.WriteLine($"{uczen.Imie,-15} | {uczen.Wiek,5} | {przedmioty,-50}");
        }
        Console.WriteLine(new string('-', 80));
    }
}