using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;


namespace adatbaziskezeloWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string constr = "Server=localhost;Database=vilweb;Uid=root;Pwd=;";
        public MainWindow()
        {
            InitializeComponent();
            adatokbetoltese();
        }

        private void adatokbetoltese()
        {
            cbkategoria.Items.Clear();//lista eleminek törlése
            cbkat.Items.Clear();//lista eleminek törlése
            cbfeszultseg.Items.Clear();//lista eleminek törlése
            cbfesz.Items.Clear();//lista eleminek törlése
            cbfoglalat.Items.Clear();//lista eleminek törlése
            cbfogl.Items.Clear();//lista eleminek törlése

            MySqlConnection con = new MySqlConnection(constr);
            MySqlCommand cmd = con.CreateCommand();
            MySqlDataReader reader;
            cmd.CommandText = "select * from kategoriak";
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbkategoria.Items.Add(reader.GetValue(1));
                cbkat.Items.Add(reader.GetValue(1));

            }
            cbkat.SelectedIndex = 0;
            con.Close();
            con = new MySqlConnection(constr);
            cmd = con.CreateCommand();
            cmd.CommandText = "select DISTINCT fesz from termekek";
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbfeszultseg.Items.Add(reader.GetValue(0));
                cbfesz.Items.Add(reader.GetValue(0));

            }
            cbfesz.SelectedIndex = 0;
            con.Close();
            con = new MySqlConnection(constr);
            cmd = con.CreateCommand();
            cmd.CommandText = "select DISTINCT foglalat from termekek";
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbfoglalat.Items.Add(reader.GetValue(0));
                cbfogl.Items.Add(reader.GetValue(0));

            }
            cbfogl.SelectedIndex = 0;
            con.Close();
            List<termekadatok> adatok = new List<termekadatok>();
            con = new MySqlConnection(constr);
            cmd = con.CreateCommand();
            cmd.CommandText = "select knev,tnev,fesz,telj,foglalat,elettartam,ar from kategoriak INNER JOIN termekek ON kazon=termekkazon";
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                termekadatok termek = new termekadatok();
                termek.kategorianev = reader.GetString(0);
                termek.termeknev = reader.GetString(1);
                termek.feszultseg = reader.GetString(2);
                termek.teljesitmeny = reader.GetInt32(3);
                termek.foglalat = reader.GetString(4);
                termek.elettartam = reader.GetInt32(5);
                termek.ar = reader.GetInt32(6);
                adatok.Add(termek);
            }
            dataGrid.ItemsSource = adatok;
            con.Close();

        }

        private void cbkategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbfeszultseg.SelectedItem = null;
            cbfoglalat.SelectedItem = null;
            if (cbkategoria.SelectedItem != null)
            {
                string kivalasztottkategoria = cbkategoria.SelectedItem.ToString();
                kivalasztottadatokszurese("knev", kivalasztottkategoria);
            }
        }

        private void kivalasztottadatokszurese(string oszlopnev, string kivalasztott_ertek)
        {
            kivalasztasoktorlese();
            MySqlConnection con = new MySqlConnection(constr);
            MySqlCommand cmd = con.CreateCommand();
            MySqlDataReader reader;
            cmd.CommandText = $"select knev,tnev,fesz,telj,foglalat,elettartam,ar from kategoriak INNER JOIN termekek ON kazon=termekkazon WHERE {oszlopnev} LIKE '%{kivalasztott_ertek}%'";
            con.Open();
            List<termekadatok> adatok = new List<termekadatok>();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                termekadatok termek = new termekadatok();
                termek.kategorianev = reader.GetString(0);
                termek.termeknev = reader.GetString(1);
                termek.feszultseg = reader.GetString(2);
                termek.teljesitmeny = reader.GetInt32(3);
                termek.foglalat = reader.GetString(4);
                termek.elettartam = reader.GetInt32(5);
                termek.ar = reader.GetInt32(6);
                adatok.Add(termek);
            }
            dataGrid.ItemsSource = adatok;
            con.Close();
        }

        private void kivalasztasoktorlese()
        {
            cbfeszultseg.SelectedItem = null;
            cbfoglalat.SelectedItem = null;
            cbkategoria.SelectedItem = null;
        }

        private void cbfeszultseg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (cbfeszultseg.SelectedItem != null)
            {
                string kivalasztottfeszultseg = cbfeszultseg.SelectedItem.ToString();
                kivalasztottadatokszurese("fesz", kivalasztottfeszultseg);
            }
        }

        private void cbfoglalat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cbfoglalat.SelectedItem != null)
            {
                string kivalasztottfoglalat = cbfoglalat.SelectedItem.ToString();
                kivalasztottadatokszurese("foglalat", kivalasztottfoglalat);
            }
        }

        private void btujtermek_Click(object sender, RoutedEventArgs e)
        {
            //nem lehetnek üres beviteli mezők
            if (tbteljesitmeny.Text != ""
                && cbkat.SelectedItem != null
                && cbfesz.SelectedItem != null
                && cbfogl.SelectedItem != null
                && tbelettartam.Text != ""
                && tbtermeknev.Text != ""
                && tbar.Text != "")
            {
                //tbteljesitmeny csak egész szám lehet
                //tbelettartam csak egész szám lehet
                //tbar csak egész szám lehet
                bool teljesitmenyok =ellenorzes(tbteljesitmeny.Text, "Teljesítmény");
                bool elettartamok =ellenorzes(tbelettartam.Text, "Élettartam");
                bool arok =ellenorzes(tbar.Text, "Ár");
                if (teljesitmenyok && elettartamok && arok)
                {
                    int termekkazon = cbkat.SelectedIndex + 1;
                    int telj = int.Parse(tbteljesitmeny.Text);
                    int elettartam = int.Parse(tbelettartam.Text);
                    int ar = int.Parse(tbar.Text);
                    string fesz = cbfesz.SelectedValue.ToString();
                    string foglalat = cbfogl.SelectedValue.ToString();
                    string tnev = tbtermeknev.Text;
                    using (var con = new MySqlConnection(constr))
                    {
                        con.Open();
                        string sql = "INSERT INTO termekek(termekkazon,tnev,fesz,telj,foglalat,elettartam,ar)" +
                            "VALUES(@termekkazon,@tnev,@fesz,@telj,@foglalat,@elettartam,@ar)";
                        using (var cmd = new MySqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@termekkazon", termekkazon);
                            cmd.Parameters.AddWithValue("@tnev", tnev);
                            cmd.Parameters.AddWithValue("@fesz", fesz);
                            cmd.Parameters.AddWithValue("@telj", telj);
                            cmd.Parameters.AddWithValue("@foglalat", foglalat);
                            cmd.Parameters.AddWithValue("@elettartam", elettartam);
                            cmd.Parameters.AddWithValue("@ar", ar);
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    MessageBox.Show("Adatrögzítés sikerült");
                    tbtermeknev.Text = "";
                    tbteljesitmeny.Text = "";
                    tbelettartam.Text = "";
                    tbar.Text = "";
                    adatokbetoltese();
                }
            }
            else MessageBox.Show("Minden adatot kötelező megadni!");
        }

        private bool ellenorzes(string bevitelimezo, string uzenet)
        {
            bool ok = false;
            try
            {
                int adat = Convert.ToInt32(bevitelimezo);
                ok = true;
            }
            catch
            {
                MessageBox.Show(uzenet+" nem szám!");
                ok = false;
            }
            return ok;
        }

        private void btujkategoria_Click(object sender, RoutedEventArgs e)
        {
            if (tbujkategoria.Text != "")
            {
                using (var con = new MySqlConnection(constr))
                {
                    con.Open();
                    string sql = "INSERT INTO kategoriak(knev)" +
                        "VALUES(@knev)";
                    using (var cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@knev", tbujkategoria.Text);

                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                MessageBox.Show("Adatrögzítés sikerült");

                tbujkategoria.Text = "";
                adatokbetoltese();
            }
            else MessageBox.Show("Meg kell adni az új kategóriát!");
        }
    }
}
