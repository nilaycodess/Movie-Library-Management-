using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmKutuphaneYonetimi
{
    public class Yonetici : Film
    {
        public override DataTable FilmleriGetir()
        {
            DataTable dataTable = new DataTable();

            using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234"))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM yonetici", connection))
                {
                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }
        public override void FilmEkle(int id, string ad, string yonetmen, string oyuncular, string tur, string yayin_yili, double degerlendirme)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234"))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO yonetici (id, ad, yonetmen, oyuncular, tur, yayin_yili, degerlendirme) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7)", connection))
                {
                    command.Parameters.AddWithValue("@p1", id);
                    command.Parameters.AddWithValue("@p2", ad);
                    command.Parameters.AddWithValue("@p3", yonetmen);
                    command.Parameters.AddWithValue("@p4", oyuncular);
                    command.Parameters.AddWithValue("@p5", tur);
                    command.Parameters.AddWithValue("@p6", yayin_yili);
                    command.Parameters.AddWithValue("@p7", degerlendirme);

                    command.ExecuteNonQuery();
                }
            }
        }

        public override void FilmGuncelle(int id, string ad, string yonetmen, string oyuncular, string tur, string yayin_yili, double? degerlendirme)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234"))
            {
                connection.Open();

                var query = new StringBuilder("UPDATE yonetici SET ");
                var parameters = new List<NpgsqlParameter>();

                if (!string.IsNullOrEmpty(ad))
                {
                    query.Append("ad = @p2, ");
                    parameters.Add(new NpgsqlParameter("@p2", ad));
                }

                if (!string.IsNullOrEmpty(yonetmen))
                {
                    query.Append("yonetmen = @p3, ");
                    parameters.Add(new NpgsqlParameter("@p3", yonetmen));
                }

                if (!string.IsNullOrEmpty(oyuncular))
                {
                    query.Append("oyuncular = @p4, ");
                    parameters.Add(new NpgsqlParameter("@p4", oyuncular));
                }

                if (!string.IsNullOrEmpty(tur))
                {
                    query.Append("tur = @p5, ");
                    parameters.Add(new NpgsqlParameter("@p5", tur));
                }

                if (!string.IsNullOrEmpty(yayin_yili))
                {
                    query.Append("yayin_yili = @p6, ");
                    parameters.Add(new NpgsqlParameter("@p6", yayin_yili));
                }

                if (degerlendirme.HasValue) // degerlendirmenin sadece bir değeri varsa
                {
                    query.Append("degerlendirme = @p7, ");
                    parameters.Add(new NpgsqlParameter("@p7", degerlendirme.Value)); // degerlendirme'nin değerini alıyoruz
                }

                query.Length -= 2; // son boşluğu sil
                query.Append(" WHERE id = @p1");
                parameters.Add(new NpgsqlParameter("@p1", id));

                using (NpgsqlCommand command = new NpgsqlCommand(query.ToString(), connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    command.ExecuteNonQuery();
                }
            }
        }

        private string connectionString = "server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234";

        public override void Degerlendir(string filmAdi, double puan)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // "yonetici" tablosundan film bilgilerini alıyoruz
                string tur = "";
                int filmId;
                double mevcutDegerlendirme = 0;

                string sql = "SELECT id, tur, degerlendirme FROM yonetici WHERE ad = @filmAdi";
                using (NpgsqlCommand findFilmCommand = new NpgsqlCommand(sql, connection))
                {
                    findFilmCommand.Parameters.AddWithValue("@filmAdi", filmAdi);

                    using (NpgsqlDataReader reader = findFilmCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            filmId = reader.GetInt32(reader.GetOrdinal("id"));
                            tur = reader.GetString(reader.GetOrdinal("tur"));
                            if (!reader.IsDBNull(reader.GetOrdinal("degerlendirme")))
                            {
                                mevcutDegerlendirme = reader.GetDouble(reader.GetOrdinal("degerlendirme"));
                            }
                        }
                        else
                        {
                            throw new Exception("Film bulunamadı.");
                        }
                    }
                }

                // "rapor" tablosuna yeni değerlendirme ekle veya en sonki değerlendirmeyi güncelle
                sql = @"
            INSERT INTO rapor (ad, sayisi, puan, tur)
            VALUES (@filmAdi, 1, @puan, @tur)
            ON CONFLICT (ad)
            DO UPDATE SET sayisi = rapor.sayisi + 1, puan = rapor.puan + EXCLUDED.puan";

                using (NpgsqlCommand insertCommand = new NpgsqlCommand(sql, connection))
                {
                    insertCommand.Parameters.AddWithValue("@filmAdi", filmAdi);
                    insertCommand.Parameters.AddWithValue("@puan", puan);
                    insertCommand.Parameters.AddWithValue("@tur", tur);
                    insertCommand.ExecuteNonQuery();
                }

                // "rapor" tablosundan toplam puanı ve değerlendirme sayısını alma
                double sayisi = 0;
                double toplamPuan = 0;

                sql = "SELECT sayisi, puan FROM rapor WHERE ad = @filmAdi";
                using (NpgsqlCommand getRaporCommand = new NpgsqlCommand(sql, connection))
                {
                    getRaporCommand.Parameters.AddWithValue("@filmAdi", filmAdi);

                    using (NpgsqlDataReader reader = getRaporCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            sayisi = reader.GetDouble(reader.GetOrdinal("sayisi"));
                            toplamPuan = reader.GetDouble(reader.GetOrdinal("puan"));
                        }
                        else
                        {
                            throw new Exception("Rapor bilgileri alınamadı.");
                        }
                    }
                }

                // "yonetici" tablosunun degerlendirme sütununu güncelleme
                double yeniDegerlendirme = toplamPuan / sayisi;

                sql = "UPDATE yonetici SET degerlendirme = @yeniDegerlendirme WHERE id = @filmId";
                using (NpgsqlCommand updateCommand = new NpgsqlCommand(sql, connection))
                {
                    updateCommand.Parameters.AddWithValue("@yeniDegerlendirme", yeniDegerlendirme);
                    updateCommand.Parameters.AddWithValue("@filmId", filmId);
                    updateCommand.ExecuteNonQuery();
                }
            }
        }

        public override DataTable EnYuksekPuanliFilmleriGetir()
        {
            DataTable dt = new DataTable();

            
            using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234"))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM yonetici ORDER BY degerlendirme DESC LIMIT 5", connection))
                {
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
                    da.Fill(dt);
                }
                connection.Close();
            }

            return dt;
        }


    }
}
