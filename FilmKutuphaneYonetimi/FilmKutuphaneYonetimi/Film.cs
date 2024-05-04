using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmKutuphaneYonetimi
{
    public abstract class Film
    {
        public int id { get; set; }
        public string ad { get; set; }
        public string yonetmen { get; set; }
        public string oyuncular { get; set; }
        public string tur { get; set; }

        public string yayin_yili { get; set; }

        public double degerlendirme { get; set; }

        public abstract DataTable FilmleriGetir();
        public abstract void FilmEkle(int id, string ad, string yonetmen, string oyuncular, string tur, string yayin_yili, double degerlendirme);
        public abstract void FilmGuncelle(int id, string ad, string yonetmen, string oyuncular, string tur, string yayin_yili, double? degerlendirme);

        public abstract void Degerlendir(string filmAdi, double puan);
        public abstract DataTable EnYuksekPuanliFilmleriGetir();
    }
}
