using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmKutuphaneYonetimi
{
    using Npgsql;
    using System;
    using System.Data;

    public abstract class Kullanici
    {

        public int id { get; set; }
        public string adsoyad { get; set; }
        public double tc { get; set; }
        public string dogum { get; set; }

        public string cinsiyet { get; set; }

        public string tur { get; set; }

        public int ucret { get; set; }

        public abstract DataTable BilgileriGetir(int id);
        public abstract void BilgiEkle(int id, string adsoyad, double tc, string dogum, string cinsiyet, string tur);
        public abstract void BilgiGuncelle(int id, string adsoyad, double? tc, string dogum, string cinsiyet, string tur);

        public abstract void BilgiSil(string adsoyad);
        public abstract void SifreSil(string adsoyad);
        public abstract void UcretHesapla();

        public abstract void Kaydet(string ad, NpgsqlConnection baglanti);

        public abstract DataTable FilmListesi();

        public abstract void FilmSil(string ad);

        public abstract void YorumYap(string filmAdi, string yorum, string adSoyad);

        public abstract DataTable YorumlariGetir();
    }
}
