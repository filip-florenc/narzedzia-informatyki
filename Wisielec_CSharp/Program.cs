// Gra „Wisielec” – C#
// Autor: Filip Florenc

using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

class Program
{

    static string nazwaPlikuBazy = "slowa.db";
    // Zmienne do obsługi passy zwycięstw (branch dev)
    static int aktualnaPassa = 0;
    static int najlepszaPassa = 0;

    static void Main()
    {

        PrzygotujBaze();

        while (true) // glowna petla gry, bedzie sie powtarzac dopoki nie zamkniemy programu
        {
            Console.Clear();

            // naglowek gry
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== WISIELEC ===");
            Console.ResetColor();

            Console.WriteLine("Wybierz poziom trudnosci:");
            Console.WriteLine("1 - latwy (1-5 liter)");
            Console.WriteLine("2 - sredni (6-8 liter)");
            Console.WriteLine("3 - trudny (9+ liter)");
            int poziom = WczytajPoziom(); // funkcja wczytuje liczbe od gracza, sprawdza czy jest 1,2 lub 3

            // losowanie slowa z bazy
            string slowo = WylosujSlowo(poziom).ToUpper(); // Upper bo bedziemy porownywac litery duze

            // przygotowanie gry
            char[] ukryteLitery = new char[slowo.Length];
            for (int i = 0; i < slowo.Length; i++)
                ukryteLitery[i] = '_'; // na poczatku wszystkie litery ukryte, pokazuje jako _

            List<char> uzyteLitery = new List<char>(); // lista liter, ktore juz podalismy
            int proby = 7; // ilosc prob

            // petla jednej gry
            while (proby > 0 && new string(ukryteLitery) != slowo) // dopoki proby>0 i nie odgadles slowa
            {
                Console.Clear();

                // naglowek
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== WISIELEC ===");
                Console.ResetColor();
                Console.WriteLine("----------------------");

                // rysowanie wisielca
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                RysujWisielca(proby); // funkcja rysuje wisielca wg ilosci pozostalych prob
                Console.ResetColor();

                // pokazanie stanu gry
                Console.WriteLine();
                Console.WriteLine("Slowo: " + string.Join(" ", ukryteLitery)); // join zamienia char[] na string z odstępami
                Console.WriteLine("Uzyte litery: " + (uzyteLitery.Count == 0 ? "-" : string.Join(", ", uzyteLitery))); // pokazuje co juz podalismy
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Pozostale proby: " + proby);
                Console.ResetColor();
                Console.WriteLine("Aktualna passa: " + aktualnaPassa);
                Console.WriteLine("Najlepsza passa: " + najlepszaPassa);

                // pobranie litery od gracza
                Console.Write("Podaj litere: ");
                char litera = char.ToUpper(Console.ReadKey(true).KeyChar); // od razu bez enter, zamiana na duza litere

                // sprawdz czy litera i czy sie nie powtarza
                if (!char.IsLetter(litera) || uzyteLitery.Contains(litera))
                    continue; // jesli nie litera albo juz byla, ignorujemy

                uzyteLitery.Add(litera); // dodajemy do listy uzytych liter

                // sprawdzenie czy litera w slowie
                bool trafienie = false;
                for (int i = 0; i < slowo.Length; i++)
                {
                    if (slowo[i] == litera)
                    {
                        ukryteLitery[i] = litera; // zamiana _ na litere
                        trafienie = true;
                    }
                }

                // komunikat
                if (trafienie)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Trafione: " + litera);
                    Console.ResetColor();
                }
                else
                {
                    proby--;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Pudlo: " + litera);
                    Console.ResetColor();
                }
            }

            // wynik gry
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== WISIELEC ===");
            Console.ResetColor();
            Console.WriteLine("----------------------");

            if (new string(ukryteLitery) == slowo)
            {
                // wygrana – zwiekszamy passe
                aktualnaPassa++;

                if (aktualnaPassa > najlepszaPassa)
                    najlepszaPassa = aktualnaPassa;

                Console.ForegroundColor = ConsoleColor.Green;
                RysujWisielca(proby);
                Console.WriteLine();
                Console.WriteLine("BRAWO! Odgadles slowo: " + slowo);
                Console.WriteLine("Aktualna passa zwyciestw: " + aktualnaPassa);
                Console.WriteLine("Najlepsza passa: " + najlepszaPassa);
                Console.ResetColor();
            }
            else
            {
                // przegrana – reset passy
                aktualnaPassa = 0;

                Console.ForegroundColor = ConsoleColor.Red;
                RysujWisielca(0);
                Console.WriteLine();
                Console.WriteLine("KONIEC. Slowo to: " + slowo);
                Console.WriteLine("Passa zostala zresetowana.");
                Console.ResetColor();
            }


            Console.WriteLine();
            Console.WriteLine("Nacisnij Enter, aby zagrac jeszcze raz...");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { } // czekamy az wcisnie enter, wszystko inne ignorujemy
        }
    }

    // wczytanie poziomu
    static int WczytajPoziom()
    {
        while (true)
        {
            string wejscie = Console.ReadLine().Trim(); // sciagnij tekst i obetnij spacje
            if (int.TryParse(wejscie, out int wynik) && wynik >= 1 && wynik <= 3)
                return wynik; // zwracamy numer poziomu
            Console.WriteLine("Podaj 1,2 lub 3:");
        }
    }

    // rysowanie wisielca
    static void RysujWisielca(int proby)
    {
        string[] etapy = new string[8]; // @ jest po to, zeby tekst w konsoli wyswietlil sie tak jak tutaj wpisany, linia po linii
        etapy[7] = @"
  +---+
  |   |
      |
      |
      |
      |
=========";
        etapy[6] = @"
  +---+
  |   |
  O   |
      |
      |
      |
=========";
        etapy[5] = @"
  +---+
  |   |
  O   |
  |   |
      |
      |
=========";
        etapy[4] = @"
  +---+
  |   |
  O   |
 /|   |
      |
      |
=========";
        etapy[3] = @"
  +---+
  |   |
  O   |
 /|\  |
      |
      |
=========";
        etapy[2] = @"
  +---+
  |   |
  O   |
 /|\  |
 /    |
      |
=========";
        etapy[1] = @"
  +---+
  |   |
  O   |
 /|\  |
 / \  |
      |
=========";
        etapy[0] = @"
  +---+
  |   |
 [X]  |
 /|\  |
 / \  |
      |
=========";

        if (proby < 0) proby = 0; // zabezpieczenie na minus
        if (proby > 7) proby = 7; // zabezpieczenie na wiecej niz 7

        Console.WriteLine(etapy[proby]);
    }

    // przygotowanie bazy SQLite i dodanie slow
    static void PrzygotujBaze()
    {
        string ciagPolaczenia = new SqliteConnectionStringBuilder { DataSource = nazwaPlikuBazy }.ToString();
        SqliteConnection pol = new SqliteConnection(ciagPolaczenia);
        pol.Open();

        // tworzymy tabelke jesli nie istnieje
        SqliteCommand komenda = pol.CreateCommand();
        komenda.CommandText = "CREATE TABLE IF NOT EXISTS Slowa (Id INTEGER PRIMARY KEY AUTOINCREMENT, Slowo TEXT NOT NULL);";
        komenda.ExecuteNonQuery(); // wykonanie komendy sql

        // lista slow do dodania
        string[] listaSlow = new string[]
            {
                "kot","pies","dom","auto","las","mysz","okno","klucz","rower","samolot",
                "telefon","aparat","ekran","monitor","myszka","klawiatura","kabel","zasilacz",
                "komputer","laptop","projekt","program","programowanie","algorytm","dane","baza",
                "serwer","aplikacja","interfejs","grafika","dokument","notebook",
                "studia","uczelia","matematyka","fizyka","chemia","biologia","historia","geografia",
                "elektronika","synchronizacja","przetwarzanie","optymalizacja","wydajnosc","debugowanie",
                "sieci","router","switch","modem","internet","przegladarka","strona","plik","folder",
                "system","windows","linux","macos","konto","haslo","bezpieczenstwo","szyfrowanie",
                "aplikacje","gry","rozrywka","muzyka","film","video","zdjecie","kamera","mikrofon",
                "drukarka","skaner","notatnik","kalendarium","zadanie","lista","tablica","wyklad",
                "laboratorium","eksperyment","wzor","reakcja","uklad","model","symulacja","projektor",
                "kabelhdmi","procesor","pamiec","dysk","grafik","edytor","programista","inzynier",
                "architekt","mechanik","matematyk","fizyk","chemik","biolog","geograf","historyk",
                "literatura","sztuka","muzeum","galeria","teatr","kino","piosenka","melodia","ruch",
                "taniec","sport","pilka","siatkowka","koszykowka","bieganie","rowerowanie","plywanie",
                "podroz","samochod","pociag","samolot","statki","jezioro","morze","gory","las",
                "ogrod","kwiat","drzewo","roslina","zwierze","ptak","ryba","ssak","owad","dom","mieszkanie",
                "pokoj","kuchnia","lazienka","przedpokoj","sypialnia","garaz","balkon","taras","ogród",
                "meble","stol","krzeslo","lozko","szafa","regał","lampka","komputer","telefon","tablet",
                "monitor","klawiatura","mysz","sluchawki","kamera","drukarka","projektor","ksiazka",
                "zeszyt","dziennik","notatnik","formularz","zadanie","test","egzamin","korepetycje",
                "wyklad","laboratorium","przyklad","wzor","obliczenie","rozklad","statystyka","probka",
                "algorytm","programowanie","kod","funkcja","zmienna","petla","instrukcja","warunek",
                "procedura","biblioteka","plik","folder","dysk","pamiec","procesor","monitor","klawiatura",
                "mysz","programista","projektant","inżynier","architekt","mechanik","elektryk","chemik",
                "fizyk","matematyk","biolog","geograf","historyk","filozof","psycholog","socjolog","prawnik",
                "lekarz","pielęgniarka","nauczyciel","student","uczen","dziecko","rodzic","dziadek","babcia",
                "przyjaciel","znajomy","kolega","sasiad","wspolpracownik","szef","prezes","kierownik","asystent"
            };


        // dodanie slow do tabeli
        foreach (string s in listaSlow)
        {
            SqliteCommand insert = pol.CreateCommand();
            insert.CommandText = "INSERT INTO Slowa (Slowo) VALUES ($slownie);"; // $slownie parametr
            insert.Parameters.AddWithValue("$slownie", s);
            insert.ExecuteNonQuery(); // wykonanie insertu
        }
    }

    // losowanie slowa z bazy wedlug poziomu
    static string WylosujSlowo(int poziom)
    {
        string ciagPolaczenia = new SqliteConnectionStringBuilder { DataSource = nazwaPlikuBazy }.ToString();
        SqliteConnection pol = new SqliteConnection(ciagPolaczenia);
        pol.Open();

        string warunek = "1=1"; // default
        if (poziom == 1) warunek = "LENGTH(Slowo) BETWEEN 1 AND 5";
        else if (poziom == 2) warunek = "LENGTH(Slowo) BETWEEN 6 AND 8";
        else warunek = "LENGTH(Slowo) >= 9";

        SqliteCommand komenda = pol.CreateCommand();
        komenda.CommandText = "SELECT Slowo FROM Slowa WHERE " + warunek + " ORDER BY RANDOM() LIMIT 1;"; // losowe slowo

        SqliteDataReader reader = komenda.ExecuteReader(); // pobranie wynikow
        if (reader.Read()) // przesun na pierwszy wiersz
            return reader.GetString(0); // pobranie slowa

        return "0"; // domyslne w razie bledu
    }
}

