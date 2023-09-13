using System.Data.SqlClient;

internal class Program
{
    private static SqlConnection connection;
    private static SqlCommand command;
    private static string connectionString = "Server=localhost;Database=Words;User Id=SA;Password=reallyStrongPwd123;MultipleActiveResultSets=true;TrustServerCertificate=true;";
    public static string[] trwords = { "kitap", "ev", "kalem", "sandalye", "elma", "köpek", "çanta", "top", "araba", "ağaç", "masa", "telefon", "gözlük", "saat", "anahtar", "kapı", "kedi", "ayakkabı", "çiçek", "televizyon" };
    public static string[] enwords = { "book", "house", "pen", "chair", "apple", "dog", "bag", "ball", "car", "tree", "table", "phone", "glasses", "watch", "key", "door", "cat", "shoe", "flower", "television" };

    private static void Main(string[] args)
    {
        connection = new SqlConnection(connectionString);
        connection.Open();
        HomeMenu();
    }
    public static void HomeMenu()
    {
        bool ab = true;
        while (ab)
        {
            Console.WriteLine("Hoşgeldin");
            Console.WriteLine("Yapmak istediğin işlemi seç");
            Console.WriteLine("1-Kelime Ekleme");
            Console.WriteLine("2-Kelime Güncelleme");
            Console.WriteLine("3-Kelime Silme");
            Console.WriteLine("4-Kelimeleri Listele");
            Console.WriteLine("5-Dizide Arama");
            Console.WriteLine("6-Dizideki Kelimeleri Listele");
            Console.WriteLine("7-Programdan Çıkış");
            int choose;
            if (int.TryParse(Console.ReadLine(), out choose))
            {
                switch (choose)
                {
                    case 1:
                        Console.Clear();
                        AddWord();
                        Menu();
                        break;
                    case 2:
                        Console.Clear();
                        UpdateWord();
                        break;
                    case 3:
                        Console.Clear();
                        DeleteWord();
                        break;
                    case 4:
                        Console.Clear();
                        WordsList();
                        break;
                    case 5:
                        Console.Clear();
                        SearchWords();
                        break;
                    case 6:
                        Console.Clear();
                        ListWords();
                        break;
                    case 7:
                        ab = false;
                        Environment.Exit(0);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Geçersiz bir seçenek girdiniz. Lütfen 1-7 arası bir seçenek girin.\n");
                        break;
                }
            }
            else Console.Clear(); Console.WriteLine("Geçersiz bir karakter girdin. Lütfen 1-7 arası bir seçenek girin. \n");
        }
    }
    public static void AddWord()
    {
        bool ab = true;
        Console.WriteLine("Türkçe kelime Gir: ");
        string tr = Console.ReadLine();
        Console.WriteLine("İngilizce karşılığını gir: ");
        string en = Console.ReadLine();


        //Array.Resize(ref trwords, trwords.Length + 1);
        //Array.Resize(ref enwords, enwords.Length + 1);

        //trwords[trwords.Length - 1] = tr;
        //enwords[enwords.Length - 1] = en;


        string query = "INSERT INTO Words (TrWords,EnWords) VALUES (@TrWords,@EnWords)";

        using (command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@TrWords", tr);
            command.Parameters.AddWithValue("@EnWords", en);
            int count = command.ExecuteNonQuery();
            if (count > 0) Console.WriteLine("Başarılı");
            else Console.WriteLine("Başarısız Kayıt");
        }
        Console.WriteLine("Kayıt Eklenmiştir " + tr + "--" + en);

        //string[] t_Words = { "Kitap", "Ev", "Kalem", "Sandalye", "Elma", "Köpek", "Çanta", "Top", "Araba", "Ağaç" };
        //string[] e_Words = { "Book", "House", "Pen", "Chair", "Apple", "Dog", "Bag", "Ball", "Car", "Tree" };
        Menu();
    }
    public static void UpdateWord()
    {
        bool ab = true;
        Console.WriteLine("Güncellemek istediğin Kelimeyi gir: ");
        string update = Console.ReadLine();
        string query = "SELECT Id FROM Words WHERE TrWords=@Update OR EnWords=@Update";
        using (command = new SqlCommand(query, connection)) ;
        command.Parameters.AddWithValue("@Update", update);
        object result = command.ExecuteScalar();
        if (result != null)
        {
            int id = Convert.ToInt32(result);

            Console.WriteLine("Güncellenecek Tr Kelime: ");
            string tr = Console.ReadLine();
            Console.WriteLine("Güncellenecek En Kelime: ");
            string en = Console.ReadLine();

            string query1 = "UPDATE Words SET TrWords=@TrWords, EnWords=@EnWords WHERE Id=@Id";
            using (command = new SqlCommand(query1, connection))
            {
                command.Parameters.AddWithValue("@TrWords", tr);
                command.Parameters.AddWithValue("@EnWords", en);
                command.Parameters.AddWithValue("@Id", id);
                int count = command.ExecuteNonQuery();
                if (count > 0) Console.WriteLine("Güncelleme başarılı");
                else Console.WriteLine("Güncelleme başarısız");
            }
        }
        else Console.WriteLine("Güncelleme Başarısız");
        Menu();
    }
    public static void DeleteWord()
    {
        bool ab = true;
        Console.WriteLine("Silmek istediğin Kelimeyi gir: ");
        string update = Console.ReadLine();
        string query = "SELECT Id FROM Words WHERE TrWords=@Update OR EnWords=@Update";
        using (command = new SqlCommand(query, connection)) ;
        command.Parameters.AddWithValue("@Update", update);
        object result = command.ExecuteScalar();
        if (result != null)
        {
            int id = Convert.ToInt32(result);

            string query1 = "DELETE FROM Words WHERE @Id=Id";
            command = new SqlCommand(query1, connection);
            command.Parameters.AddWithValue("@Id", id);
            int count = command.ExecuteNonQuery();
            if (count > 0) Console.WriteLine("Silme işlemi başarılı");
        }
        else Console.WriteLine("Silme Başarısız");
        Menu();
    }
    static void ListWords()
    {
        Console.WriteLine("Dizinin İçindeki Elemanları Alfabetik sıraya göre listeleme\n");
        List<Tuple<string, string>> kelimeCiftleri = new List<Tuple<string, string>>();

        for (int i = 0; i < trwords.Length; i++)
        {
            kelimeCiftleri.Add(new Tuple<string, string>(trwords[i], enwords[i]));
        }

        // Kelime çiftlerini alfabetik sıraya göre sıralayın
        kelimeCiftleri.Sort((x, y) => x.Item1.CompareTo(y.Item1));

        // Sıralanmış kelime çiftlerini ekrana yazdırın
        Console.WriteLine("Türkçe Kelimeler - İngilizce Karşılıklar (Alfabetik Sıraya Göre)");
        Console.WriteLine("-------------------------------------------------------------");

        foreach (var kelimeCifti in kelimeCiftleri)
        {
            Console.WriteLine(kelimeCifti.Item1 + " - " + kelimeCifti.Item2);
        }
        Menu();
    }
    static void SearchWords()
    {


        bool aa = true;
        while (aa)
        {
            Console.WriteLine("Arama Yapmak İstediğiniz Dili Seçiniz.");
            Console.WriteLine("1-Türkçe");
            Console.WriteLine("2-English");
            Console.WriteLine("3-Anasayfa");
            Console.WriteLine("4-Çıkış");
            int choose;
            if (int.TryParse(Console.ReadLine(), out choose))
            {
                switch (choose)
                {
                    case 1:
                        Console.Write("Aradığınız Türkçe kelime: ");
                        string aranan1 = Console.ReadLine();
                        int trIndex = Array.IndexOf(trwords, aranan1);
                        if (trIndex != -1)
                        {
                            Console.WriteLine("========================");
                            Console.WriteLine("Aranan Kelime : {0}", enwords[trIndex]);
                        }
                        else Console.WriteLine("Aranan kelime bulunamadı.");
                        break;
                    case 2:
                        Console.Write("Aradığınız İngilizce kelime: ");
                        string aranan2 = Console.ReadLine();
                        int enIndex = Array.IndexOf(enwords, aranan2);
                        if (enIndex != -1)
                        {
                            Console.WriteLine("========================");
                            Console.WriteLine("Aranan Kelime : {0}", trwords[enIndex]);
                        }
                        else Console.WriteLine("Aranan kelime bulunamadı.");
                        break;
                    case 3:
                        Console.Clear();
                        HomeMenu();
                        break;
                    case 4:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçenek.");
                        break;
                }
            }
            else Console.WriteLine("Yanlış girdiniz.");
            Console.WriteLine("Tekrar Arama yapmak isterseniz 'evet' E/H 'hayır'");
            string tekrar = Console.ReadLine();
            Console.Clear();
            if (tekrar.ToLower() == "E")
            {
                aa = false;
            }
        }

        Menu();
    }
    static void WordsList()
    {
        bool ab = true;
        string query = "SELECT * FROM Words ORDER BY TrWords ASC";
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                Console.WriteLine("Türkçe Kelimeler - İngilizce Karşılıklar");
                Console.WriteLine("--------------------------------------");
                int a = 0;
                while (reader.Read())
                {
                    string trWord = reader["TrWords"].ToString();
                    string enWord = reader["EnWords"].ToString();
                    Console.WriteLine(trWord + " - " + enWord);


                    //Array.Resize(ref trwords, trwords.Length + 1);
                    //Array.Resize(ref enwords, enwords.Length + 1);
                    //trwords[a] = trWord;
                    //enwords[a] = enWord;
                    //a++;
                    //Console.WriteLine("dizler" + a);
                }
            }
        }
        Menu();
    }
    public static void Menu()
    {
        bool ab = true;

        while (ab)
        {
            Console.WriteLine("----------\n1-Ana Sayfa \n2-Programı Kapat");
            int choose;

            if (int.TryParse(Console.ReadLine(), out choose))
            {
                switch (choose)
                {
                    case 1:
                        HomeMenu();
                        break;
                    case 2:
                        ab = false;
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Geçersiz bir seçenek girdiniz. Lütfen 1 veya 2 girin.\n");
                        break;
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Geçersiz bir seçenek girdiniz. Lütfen 1 veya 2 girin.\n");
            }
        }
    }
}