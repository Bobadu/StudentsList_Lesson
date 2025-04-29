namespace Uczniowie;

class Uczen
{
    public string Imie { get; set; }
    public int Wiek { get; set; }
    public List<string> UlubionePrzedmioty { get; set; }

    public Uczen(string imie, int wiek, List<string> ulubionePrzedmioty)
    {
        Imie = imie;
        Wiek = wiek;
        UlubionePrzedmioty = ulubionePrzedmioty;
    }

    public string DoCSV()
    {
        string przedmioty = string.Join("|", UlubionePrzedmioty);
        return $"{Imie},{Wiek},{przedmioty}";
    }

    public static Uczen ZCSVLine(string linia)
    {
        string[] parts = linia.Split(',');
        if (parts.Length < 3)
            throw new FormatException("Nieprawidłowy format linii CSV");

        string imie = parts[0];
        int wiek = int.Parse(parts[1]);
        List<string> przedmioty = parts[2].Split('|').ToList();

        return new Uczen(imie, wiek, przedmioty);
    }
}