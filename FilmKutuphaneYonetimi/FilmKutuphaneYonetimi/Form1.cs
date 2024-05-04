using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FilmKutuphaneYonetimi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        NpgsqlConnection baglanti = new NpgsqlConnection("server=localHost; port=5432; Database=nesne; user ID=postgres; password=1234");
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();

                int id = int.Parse(textBox7.Text); // ID değeri
                string ad = string.IsNullOrEmpty(textBox8.Text) ? null : textBox8.Text;
                string yonetmen = string.IsNullOrEmpty(textBox9.Text) ? null : textBox9.Text;
                string oyuncular = string.IsNullOrEmpty(textBox10.Text) ? null : textBox10.Text;
                string tur = string.IsNullOrEmpty(textBox11.Text) ? null : textBox11.Text;
                string yayin_yili = string.IsNullOrEmpty(textBox12.Text) ? null : textBox12.Text;
                double? degerlendirme = null; // Nullable double olarak başlat

                if (!string.IsNullOrEmpty(textBox13.Text))
                {
                    if (double.TryParse(textBox13.Text, out double result))
                    {
                        degerlendirme = result; // parse edilen değri atıyoruz
                    }
                    else
                    {
                        MessageBox.Show("Lütfen geçerli bir sayı giriniz.");
                        return; 
                    }
                }
                else
                {
                    degerlendirme = null;
                    // degerlendirme null
                }


                Yonetici yonetici = new Yonetici();
                yonetici.FilmGuncelle(id, ad, yonetmen, oyuncular, tur, yayin_yili, degerlendirme);

                MessageBox.Show("Film başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox20_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage7_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                baglanti.Open();
                NpgsqlCommand komut2 = new NpgsqlCommand("insert into parola (kullaniciadi, sifre) values (@p1, @p2)", baglanti);
                komut2.Parameters.AddWithValue("@p1", textBox1.Text);
                komut2.Parameters.AddWithValue("@p2", textBox2.Text);
                komut2.ExecuteNonQuery();
                MessageBox.Show("Başarılı");
                baglanti.Close();
                tabControl1.SelectedTab = tabPage3;
            }
            else if (checkBox2.Checked)
            {
                baglanti.Open();
                NpgsqlCommand komut3 = new NpgsqlCommand("insert into standartsifre (kullaniciadi, sifre, adsoyad) values (@p1, @p2, @p3)", baglanti);
                komut3.Parameters.AddWithValue("@p1", textBox1.Text);
                komut3.Parameters.AddWithValue("@p2", textBox2.Text);
                komut3.Parameters.AddWithValue("@p3", textBox3.Text);
                komut3.ExecuteNonQuery();
                MessageBox.Show("Başarılı");
                baglanti.Close();
                tabControl1.SelectedTab = tabPage3;
                // Standart sınıfından örnek nesne oluşturma
                Standart standartKullanici = new Standart();

                // UcretHesapla metodunu çağır
                standartKullanici.UcretHesapla();

                // bir ücret değeri alınmadığı için MessageBox ile msj göster
                MessageBox.Show("Standart Kullanıcı olduğunuz için ücretiniz: " + standartKullanici.ucret.ToString() + "TL'dir.");
            }
            else if (checkBox3.Checked)
            {
                baglanti.Open();
                NpgsqlCommand komut4 = new NpgsqlCommand("insert into premiumsifre (kullaniciadi, sifre, adsoyad) values (@p1, @p2, @p3)", baglanti);
                komut4.Parameters.AddWithValue("@p1", textBox1.Text);
                komut4.Parameters.AddWithValue("@p2", textBox2.Text);
                komut4.Parameters.AddWithValue("@p3", textBox3.Text);
                komut4.ExecuteNonQuery();
                MessageBox.Show("Başarılı");
                baglanti.Close();
                tabControl1.SelectedTab = tabPage3;
                // Premium sınıfından örnek nesne oluştur
                Premium premiumKullanici = new Premium();

                // UcretHesapla metodunu çağır
                premiumKullanici.UcretHesapla();

                // bir ücret değeri alınmadığı için MessageBox ile msj göster
                MessageBox.Show("Premium Kullanıcı olduğunuz için ücretiniz: " + premiumKullanici.ucret.ToString() + "TL'dir.");
            }

            else
            {
                MessageBox.Show("Hata");
                baglanti.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();
                if (checkBox4.Checked)
                {
                    string npgsql = "SELECT * FROM parola WHERE kullaniciadi = @adi AND sifre = @sifresi";
                    NpgsqlParameter prm1 = new NpgsqlParameter("adi", textBox4.Text.Trim());
                    //kullaniciadi 20 karakter olarak sınırladık. daha az karakterli kullaniciadi girilirse hata verir. Trim fonknu boşluklari yok etmesi için kullandık.
                    NpgsqlParameter prm2 = new NpgsqlParameter("sifresi", textBox5.Text.Trim());
                    NpgsqlCommand komut = new NpgsqlCommand(npgsql, baglanti);
                    komut.Parameters.Add(prm1);
                    komut.Parameters.Add(prm2);
                    DataTable dt = new DataTable();
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(komut);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        tabControl1.SelectedTab = tabPage4;
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı adı veya şifre hatalı.");
                    }
                }
                else if (checkBox5.Checked)
                {
                    string npgsql = "SELECT * FROM standartsifre WHERE kullaniciadi = @adi AND sifre = @sifresi AND adsoyad = @adsoyadi";
                    NpgsqlParameter prm1 = new NpgsqlParameter("adi", textBox4.Text.Trim());
                    NpgsqlParameter prm2 = new NpgsqlParameter("sifresi", textBox5.Text.Trim());
                    NpgsqlParameter prm3 = new NpgsqlParameter("adsoyadi", textBox6.Text.Trim()); // textBox35'teki adsoyad için parametre
                    NpgsqlCommand komut = new NpgsqlCommand(npgsql, baglanti);
                    komut.Parameters.Add(prm1);
                    komut.Parameters.Add(prm2);
                    komut.Parameters.Add(prm3); // yeni parametreyi sorguya ekliyoruz
                    DataTable dt = new DataTable();
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(komut);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        tabControl1.SelectedTab = tabPage7;
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı adı, şifre veya ad soyad hatalı.");
                    }
                }
                else if (checkBox6.Checked) // premium kullanıcı girişi için:
                {
                    string npgsql = "SELECT * FROM premiumsifre WHERE kullaniciadi = @adi AND sifre = @sifresi AND adsoyad = @adsoyadi";
                    NpgsqlParameter prm1 = new NpgsqlParameter("adi", textBox4.Text.Trim());
                    NpgsqlParameter prm2 = new NpgsqlParameter("sifresi", textBox5.Text.Trim());
                    NpgsqlParameter prm3 = new NpgsqlParameter("adsoyadi", textBox6.Text.Trim()); // textBox35'teki adsoyad için parametre
                    NpgsqlCommand komut = new NpgsqlCommand(npgsql, baglanti);
                    komut.Parameters.Add(prm1);
                    komut.Parameters.Add(prm2);
                    komut.Parameters.Add(prm3);
                    DataTable dt = new DataTable();
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(komut);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        tabControl1.SelectedTab = tabPage8;
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı adı, şifre veya ad soyad hatalı.");
                    }
                }
                baglanti.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Hatali Giris");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();

                int id = int.Parse(textBox7.Text);
                string ad = textBox8.Text;
                string yonetmen = textBox9.Text;
                string oyuncular = textBox10.Text;
                string tur = textBox11.Text;
                string yayin_yili = textBox12.Text;
                double degerlendirme = double.Parse(textBox13.Text);

                // yonetici sınıfından bi nesne oluşturup FilmEkle metodu ile veritabanına film ekleme
                Yonetici yonetici = new Yonetici();
                yonetici.FilmEkle(id, ad, yonetmen, oyuncular, tur, yayin_yili, degerlendirme);

                
                BildirimGoster($"{ad} filmi eklendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
            // bildirim gösterimini button7_Click içine taşıdık
            void BildirimGoster(string mesaj)
            {
                // notifyIcon nesnesi 
                NotifyIcon notifyIcon = new NotifyIcon();
                notifyIcon.Icon = SystemIcons.Information; // bildrm ikonu

                
                notifyIcon.BalloonTipTitle = "Film Ekleme Bildirimi";
                notifyIcon.BalloonTipText = mesaj;
                notifyIcon.Visible = true;

                
                notifyIcon.ShowBalloonTip(3000);

                // ikonu temizleme
                notifyIcon.Dispose();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Yonetici yonetici = new Yonetici();
            dataGridView1.DataSource = yonetici.FilmleriGetir();
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            string veri_sorgusu = "select * from yonetici where ad like '%" + textBox16.Text + "%'";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(veri_sorgusu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            string veri_sorgusu = "select * from yonetici where yonetmen like '%" + textBox15.Text + "%'";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(veri_sorgusu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            string veri_sorgusu = "select * from yonetici where tur like '%" + textBox14.Text + "%'";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(veri_sorgusu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            string veri_sorgusu = "select * from yonetici where ad like '%" + textBox17.Text + "%'";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(veri_sorgusu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView2.DataSource = ds.Tables[0];
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            string veri_sorgusu = "select * from yonetici where yonetmen like '%" + textBox18.Text + "%'";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(veri_sorgusu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView2.DataSource = ds.Tables[0];
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            string veri_sorgusu = "select * from yonetici where tur like '%" + textBox19.Text + "%'";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(veri_sorgusu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView2.DataSource = ds.Tables[0];
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Yonetici yonetici = new Yonetici();
            dataGridView2.DataSource = yonetici.FilmleriGetir();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string connectionString = "server=localHost; port=5432; Database=nesne; user ID=postgres; password=1234";

            
            using (NpgsqlConnection baglanti = new NpgsqlConnection(connectionString))
            {
                // standart nesnesi oluşturduk
                Standart standart = new Standart();

                // film adını al
                string filmAdi = textBox21.Text;

                // kaydet metodunu çağır
                standart.Kaydet(filmAdi, baglanti);

                // kullanıcıya mesaj göster
                MessageBox.Show(filmAdi + " izleme listenize eklendi.");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage9;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Yonetici yonetici = new Yonetici();
            dataGridView3.DataSource = yonetici.FilmleriGetir();
        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            string veri_sorgusu = "select * from yonetici where ad like '%" + textBox22.Text + "%'";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(veri_sorgusu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView3.DataSource = ds.Tables[0];
        }

        private void textBox23_TextChanged(object sender, EventArgs e)
        {
            string veri_sorgusu = "select * from yonetici where yonetmen like '%" + textBox23.Text + "%'";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(veri_sorgusu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView3.DataSource = ds.Tables[0];
        }

        private void textBox24_TextChanged(object sender, EventArgs e)
        {
            string veri_sorgusu = "select * from yonetici where tur like '%" + textBox24.Text + "%'";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(veri_sorgusu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView3.DataSource = ds.Tables[0];
        }

        private void button14_Click(object sender, EventArgs e)
        {
            string connectionString = "server=localHost; port=5432; Database=nesne; user ID=postgres; password=1234";

            
            using (NpgsqlConnection baglanti = new NpgsqlConnection(connectionString))
            {
                // Standart nesnesi oluştur
                Premium premium = new Premium();

                // Film adını al
                string filmAdi = textBox26.Text;

                // Kaydet metodunu çağır
                premium.Kaydet(filmAdi, baglanti);

                // Kullanıcıya mesaj göster
                MessageBox.Show(filmAdi + " izleme listenize eklendi.");
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string filmAdi = textBox40.Text; // Kullanıcının girdiği film adı
            double puan;
            if (!double.TryParse(textBox25.Text, out puan))
            {
                MessageBox.Show("Lütfen geçerli bir puan giriniz.");
                return;
            }

            try
            {
                Yonetici yonetici = new Yonetici();
                yonetici.Degerlendir(filmAdi, puan);
                MessageBox.Show("Film başarıyla değerlendirildi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage10;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Standart standart = new Standart();
            int id = Convert.ToInt32(int.Parse(textBox20.Text)); ; // Burada kullanıcıdan alınan ya da seçilen id değeri olmalı
            dataGridView4.DataSource = standart.BilgileriGetir(id);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Standart standart = new Standart();
            int id = int.Parse(textBox20.Text);
            string adsoyad = textBox27.Text;
            double tc = double.Parse(textBox28.Text);
            string dogum = textBox29.Text;
            string cinsiyet = textBox30.Text;
            string tur = textBox31.Text;

            standart.BilgiEkle(id, adsoyad, tc, dogum, cinsiyet, tur);
            MessageBox.Show("Bilgi başarıyla eklendi.");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Standart standart = new Standart();
            int id = int.Parse(textBox20.Text);
            string adsoyad = string.IsNullOrEmpty(textBox27.Text) ? null : textBox27.Text;
            double? tc = string.IsNullOrEmpty(textBox28.Text) ? (double?)null : double.Parse(textBox28.Text);
            string dogum = string.IsNullOrEmpty(textBox29.Text) ? null : textBox29.Text;
            string cinsiyet = string.IsNullOrEmpty(textBox30.Text) ? null : textBox30.Text;
            string tur = string.IsNullOrEmpty(textBox31.Text) ? null : textBox31.Text;

            standart.BilgiGuncelle(id, adsoyad, tc, dogum, cinsiyet, tur);
            MessageBox.Show("Bilgi başarıyla güncellendi.");

        }

        private void button18_Click(object sender, EventArgs e)
        {
            Standart standart = new Standart();
            string adsoyad = textBox27.Text; // kullanıcıdan TextBox ile adsoyad alıyoruz

            // bilgileri silecek metodları çağır
            standart.BilgiSil(adsoyad);
            standart.SifreSil(adsoyad);

            
            MessageBox.Show("Bilgiler silindi.");
        }

        private void button19_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage5;
        }

        private void button24_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage6;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(textBox32.Text); // textBox20'dekini int'e çevir
                Premium premium = new Premium();
                dataGridView5.DataSource = premium.BilgileriGetir(id);
            }
            catch (FormatException)
            {
                MessageBox.Show("Lütfen geçerli bir ID giriniz.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            Premium premium = new Premium();
            int id = int.Parse(textBox32.Text);
            string adsoyad = textBox33.Text;
            double tc = double.Parse(textBox34.Text);
            string dogum = textBox35.Text;
            string cinsiyet = textBox36.Text;
            string tur = textBox37.Text;

            premium.BilgiEkle(id, adsoyad, tc, dogum, cinsiyet, tur);
            MessageBox.Show("Bilgi başarıyla eklendi.");
        }

        private void button21_Click(object sender, EventArgs e)
        {
            Premium premium = new Premium();
            int id = int.Parse(textBox32.Text);
            string adsoyad = string.IsNullOrEmpty(textBox33.Text) ? null : textBox33.Text;
            double? tc = string.IsNullOrEmpty(textBox34.Text) ? (double?)null : double.Parse(textBox34.Text);
            string dogum = string.IsNullOrEmpty(textBox35.Text) ? null : textBox35.Text;
            string cinsiyet = string.IsNullOrEmpty(textBox36.Text) ? null : textBox36.Text;
            string tur = string.IsNullOrEmpty(textBox37.Text) ? null : textBox37.Text;

            premium.BilgiGuncelle(id, adsoyad, tc, dogum, cinsiyet, tur);
            MessageBox.Show("Bilgi başarıyla güncellendi.");
        }

        private void button20_Click(object sender, EventArgs e)
        {
            Premium premium = new Premium();
            string adsoyad = textBox33.Text; // kullanıcıdan TextBox ile adsoyad alıyoruz

            // bilgileri silecek metodları çağırın
            premium.BilgiSil(adsoyad);
            premium.SifreSil(adsoyad);

          
            MessageBox.Show("Bilgiler silindi.");
        }

        private void button25_Click(object sender, EventArgs e)
        {
            Standart standart = new Standart();
            dataGridView6.DataSource = standart.FilmListesi();
        }

        private void button28_Click(object sender, EventArgs e)
        {
            Standart standart = new Standart(); // standart nesne oluştur
            string ad = textBox38.Text;

            // film silme metodu
            standart.FilmSil(ad);

            MessageBox.Show("Bilgiler silindi.");
        }

        private void button26_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage7;
        }

        private void button27_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage5;
        }

        private void button32_Click(object sender, EventArgs e)
        {
            Premium premium = new Premium();
            string ad = textBox39.Text;

            premium.FilmSil(ad);

            MessageBox.Show("Bilgiler silindi.");
        }

        private void button29_Click(object sender, EventArgs e)
        {
            Premium premium = new Premium();
            dataGridView7.DataSource = premium.FilmListesi();
        }

        private void button30_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage8;
        }

        private void button31_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage6;
        }

        private void button33_Click(object sender, EventArgs e)
        {
            dataGridView8.DataSource = null;
            dataGridView8.Columns.Clear();
            Yonetici yonetici = new Yonetici();
            DataTable dt = yonetici.EnYuksekPuanliFilmleriGetir();
            dataGridView8.DataSource = dt;
        }


        private string connectionString = "server=localhost; port=5432; Database=nesne; user ID=postgres; password=1234";
        private void button34_Click(object sender, EventArgs e)
        {
            dataGridView8.DataSource = null;
            dataGridView8.Columns.Clear();
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                SELECT tur, SUM(sayisi) as toplam_degerlendirme
                FROM rapor
                GROUP BY tur
                ORDER BY toplam_degerlendirme DESC
                LIMIT 3";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add("tur");
                            dt.Columns.Add("toplam_degerlendirme");

                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["tur"] = reader.GetString(reader.GetOrdinal("tur"));
                                row["toplam_degerlendirme"] = reader.GetDouble(reader.GetOrdinal("toplam_degerlendirme"));
                                dt.Rows.Add(row);
                            }

                            dataGridView8.DataSource = dt;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu: " + ex.Message);
                }
            }
        }

        private void button36_Click(object sender, EventArgs e)
        {
            dataGridView8.DataSource = null;
            dataGridView8.Columns.Clear();
            Yonetici yonetici = new Yonetici();
            dataGridView8.DataSource = yonetici.FilmleriGetir();
        }

        private void button37_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage7;
        }

        private void button38_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage11;
        }

        private void button39_Click(object sender, EventArgs e)
        {
            dataGridView9.DataSource = null;
            dataGridView9.Columns.Clear();
            Yonetici yonetici = new Yonetici();
            dataGridView9.DataSource = yonetici.FilmleriGetir();
        }

        private void button43_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage8;
        }

        private void button46_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage12;
        }

        private void button35_Click(object sender, EventArgs e)
        {
            var filmAdi = textBox42.Text;
            var yorum = textBox41.Text;
            var adSoyad = textBox6.Text;

            Standart kullanici = new Standart();
            kullanici.YorumYap(filmAdi, yorum, adSoyad);
        }

        private void button45_Click(object sender, EventArgs e)
        {
            dataGridView8.DataSource = null;
            dataGridView8.Columns.Clear();
            dataGridView8.Rows.Clear();
            Standart standart = new Standart();
            DataTable yorumlarTablosu = standart.YorumlariGetir();

            

            // sütunları ekledik
            dataGridView8.Columns.Add("FilmAdi", "Film Adı");
            dataGridView8.Columns.Add("Yorumlar", "Yorumlar");

            foreach (DataRow row in yorumlarTablosu.Rows)
            {
                int rowIndex = dataGridView8.Rows.Add();
                var yorumListesi = (List<string>)row["Yorumlar"];

                dataGridView8.Rows[rowIndex].Cells["FilmAdi"].Value = row["FilmAdi"];
                dataGridView8.Rows[rowIndex].Cells["Yorumlar"].Value = string.Join(Environment.NewLine, yorumListesi);
            }

            // otomatik boyutlandırma. yorumların hepsini görebilmk için
            dataGridView8.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView8.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView8.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        private void button44_Click(object sender, EventArgs e)
        {       
            dataGridView9.DataSource = null;
            dataGridView9.Columns.Clear();
            dataGridView9.Rows.Clear();
            Premium premium = new Premium();
            DataTable yorumlarTablosu = premium.YorumlariGetir();



            // sütunları ekle
            dataGridView9.Columns.Add("FilmAdi", "Film Adı");
            dataGridView9.Columns.Add("Yorumlar", "Yorumlar");

            foreach (DataRow row in yorumlarTablosu.Rows)
            {
                int rowIndex = dataGridView9.Rows.Add();
                var yorumListesi = (List<string>)row["Yorumlar"];

                dataGridView9.Rows[rowIndex].Cells["FilmAdi"].Value = row["FilmAdi"];
                dataGridView9.Rows[rowIndex].Cells["Yorumlar"].Value = string.Join(Environment.NewLine, yorumListesi);
            }

            // oto boyut yorumların hepsini görmek için
            dataGridView9.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView9.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView9.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        private void button41_Click(object sender, EventArgs e)
        {
            dataGridView9.DataSource = null;
            dataGridView9.Columns.Clear();
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                SELECT tur, SUM(sayisi) as toplam_degerlendirme
                FROM rapor
                GROUP BY tur
                ORDER BY toplam_degerlendirme DESC
                LIMIT 3";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add("tur");
                            dt.Columns.Add("toplam_degerlendirme");

                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["tur"] = reader.GetString(reader.GetOrdinal("tur"));
                                row["toplam_degerlendirme"] = reader.GetDouble(reader.GetOrdinal("toplam_degerlendirme"));
                                dt.Rows.Add(row);
                            }

                            dataGridView9.DataSource = dt;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu: " + ex.Message);
                }
            }
        }

        private void button40_Click(object sender, EventArgs e)
        {
            dataGridView9.DataSource = null;
            dataGridView9.Columns.Clear();
            Yonetici yonetici = new Yonetici();
            DataTable dt = yonetici.EnYuksekPuanliFilmleriGetir();
            dataGridView9.DataSource = dt;
        }

        private void button42_Click(object sender, EventArgs e)
        {
            var filmAdi = textBox44.Text;
            var yorum = textBox43.Text;
            var adSoyad = textBox6.Text;

            Premium kullanici = new Premium();
            kullanici.YorumYap(filmAdi, yorum, adSoyad);
        }
    }
}
