using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmKutuphaneYonetimi
{
    public class Standart : Kullanici
    {
        public override DataTable BilgileriGetir(int id) 
        {
            DataTable dataTable = new DataTable();

            using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234"))
            {
                connection.Open();

                // seçilen id ye göre
                using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM standartbilgi WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id); 

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        public override void BilgiEkle(int id, string adsoyad, double tc, string dogum, string cinsiyet, string tur)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234"))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO standartbilgi (id, adsoyad, tc, dogum, cinsiyet, tur) VALUES (@id, @adsoyad, @tc, @dogum, @cinsiyet, @tur)", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@adsoyad", adsoyad);
                    command.Parameters.AddWithValue("@tc", tc);
                    command.Parameters.AddWithValue("@dogum", dogum);
                    command.Parameters.AddWithValue("@cinsiyet", cinsiyet);
                    command.Parameters.AddWithValue("@tur", tur);

                    command.ExecuteNonQuery();
                }
            }
        }

        public override void BilgiGuncelle(int id, string adsoyad, double? tc, string dogum, string cinsiyet, string tur)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234"))
            {
                connection.Open();

                var query = new StringBuilder("UPDATE standartbilgi SET ");
                var parameters = new List<NpgsqlParameter>();

                if (!string.IsNullOrEmpty(adsoyad))
                {
                    query.Append("adsoyad = @adsoyad, ");
                    parameters.Add(new NpgsqlParameter("@adsoyad", adsoyad));
                }

                if (tc.HasValue)
                {
                    query.Append("tc = @tc, ");
                    parameters.Add(new NpgsqlParameter("@tc", tc.Value));
                }

                if (!string.IsNullOrEmpty(dogum))
                {
                    query.Append("dogum = @dogum, ");
                    parameters.Add(new NpgsqlParameter("@dogum", dogum));
                }

                if (!string.IsNullOrEmpty(cinsiyet))
                {
                    query.Append("cinsiyet = @cinsiyet, ");
                    parameters.Add(new NpgsqlParameter("@cinsiyet", cinsiyet));
                }

                if (!string.IsNullOrEmpty(tur))
                {
                    query.Append("tur = @tur, ");
                    parameters.Add(new NpgsqlParameter("@tur", tur));
                }

                query.Length -= 2; // son boşluk sil
                query.Append(" WHERE id = @id;");
                parameters.Add(new NpgsqlParameter("@id", id));

                using (NpgsqlCommand command = new NpgsqlCommand(query.ToString(), connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    command.ExecuteNonQuery();
                }
            }
        }
        public override void BilgiSil(string adsoyad)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234"))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM standartbilgi WHERE adsoyad = @adsoyad", connection))
                {
                    command.Parameters.AddWithValue("@adsoyad", adsoyad);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override void SifreSil(string adsoyad)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234"))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM standartsifre WHERE adsoyad = @adsoyad", connection))
                {
                    command.Parameters.AddWithValue("@adsoyad", adsoyad);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override void UcretHesapla()
        {
            ucret = 100;


        }

        public override void Kaydet(string ad, NpgsqlConnection baglanti)
        {
            try
            {
                // veritabanı aç
                baglanti.Open();

                // film bilgilerini al
                string selectQuery = "SELECT * FROM yonetici WHERE ad = @ad";

                // film bilgilerini çek
                using (NpgsqlCommand selectCommand = new NpgsqlCommand(selectQuery, baglanti))
                {
                    selectCommand.Parameters.AddWithValue("@ad", ad);

                    using (NpgsqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // yeni bir kayıt yap
                            string insertQuery = @"INSERT INTO standartliste (id, ad, yonetmen, oyuncular, tur, yayin_yili, degerlendirme) 
                                               VALUES (@id, @ad, @yonetmen, @oyuncular, @tur, @yayin_yili, @degerlendirme)";

                            using (NpgsqlCommand insertCommand = new NpgsqlCommand(insertQuery, baglanti))
                            {
                                // Parametreler
                                insertCommand.Parameters.AddWithValue("@id", reader["id"]);
                                insertCommand.Parameters.AddWithValue("@ad", reader["ad"]);
                                insertCommand.Parameters.AddWithValue("@yonetmen", reader["yonetmen"]);
                                insertCommand.Parameters.AddWithValue("@oyuncular", reader["oyuncular"]);
                                insertCommand.Parameters.AddWithValue("@tur", reader["tur"]);
                                insertCommand.Parameters.AddWithValue("@yayin_yili", reader["yayin_yili"]);
                                insertCommand.Parameters.AddWithValue("@degerlendirme", reader["degerlendirme"]);

                                
                                reader.Close();

                                // Kayıt ekleme sorgusunu çalıştır
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // film bulunamazsa kullanıcıya mesaj
                            throw new Exception("Film bulunamadı.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // hata yönetimi
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                
                if (baglanti.State == System.Data.ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }
        }

        public override DataTable FilmListesi()
        {
            DataTable dataTable = new DataTable();

            using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234"))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM standartliste", connection))
                {
                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        public override void FilmSil(string ad)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234"))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM standartliste WHERE ad = @ad", connection))
                {
                    command.Parameters.AddWithValue("@ad", ad);
                    command.ExecuteNonQuery();
                }
            }

        }
        public override void YorumYap(string filmAdi, string yorum, string adSoyad)
        {
            string connString = "server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234";
            using (NpgsqlConnection connection = new NpgsqlConnection(connString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO yorum (ad, yorum, adsoyad) VALUES (@ad, @yorum, @adsoyad)", connection))
                {
                    command.Parameters.AddWithValue("@ad", filmAdi);
                    command.Parameters.AddWithValue("@yorum", yorum);
                    command.Parameters.AddWithValue("@adsoyad", adSoyad);
                    command.ExecuteNonQuery();
                }
            }
        }
        public override DataTable YorumlariGetir()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("FilmAdi");
            dataTable.Columns.Add("Yorumlar", typeof(List<string>));

            using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234"))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand("SELECT ad, adsoyad, yorum FROM yorum ORDER BY ad", connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        var currentFilm = "";
                        List<string> yorumlar = null;

                        while (reader.Read())
                        {
                            var filmAdi = reader.GetString(0);
                            var adSoyad = reader.GetString(1);
                            var yorum = reader.GetString(2);

                            if (currentFilm != filmAdi)
                            {
                                if (yorumlar != null)
                                {
                                    dataTable.Rows.Add(currentFilm, yorumlar);
                                }

                                currentFilm = filmAdi;
                                yorumlar = new List<string>();
                            }

                            yorumlar.Add($"{adSoyad} : {yorum}");
                        }

                        // son film için
                        if (yorumlar != null)
                        {
                            dataTable.Rows.Add(currentFilm, yorumlar);
                        }
                    }
                }
            }

            return dataTable;
        }
    }
}
