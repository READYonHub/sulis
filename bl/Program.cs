using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;//adatbáziskezeléshez
using ConsoleTableExt;//táblázatos megjelenítéshez
using System.Data;


namespace adatbaziskezelo
{
    internal class Program
    {
        //kapcsolódás az AB-hoz
        private static string constr = "Server=localhost;Database=vilweb;Uid=root;Pwd=;";
        static void Main(string[] args)
        {
            Console.WriteLine("Adatbáziskezelő program");
            Lekerdez("SELECT tnev,SUM(db) AS osszesdb " +
                "FROM termekek INNER JOIN rendelesek ON tazon=rendelesektazon " +
                "GROUP BY tnev ORDER BY SUM(db) DESC LIMIT 10", "legtöbb megrendelés volt a következő 10 termékből ");
            Lekerdez("SELECT knev,tnev,rdatum,ar*db " +
                "FROM kategoriak INNER JOIN termekek ON kazon=termekkazon INNER JOIN rendelesek ON tazon=rendelesektazon " +
                "WHERE telj=25 AND rdatum BETWEEN '2015.03.01' AND '2015.03.05' ", "2015. március első 5 napján megrendelt, „25 W” teljesítményű termékek ");
            Lekerdez("SELECT knev,COUNT(tazon) FROM kategoriak " +
                "LEFT JOIN termekek ON kazon=termekkazon " +
                "GROUP BY knev", "a webáruházban árult termékkategóriák hány terméket tartalmaznak ");
            Console.Write("Add meg a kategória nevét:");
            string keresettkategoria=Console.ReadLine();
            Lekerdez($"SELECT * FROM kategoriak INNER JOIN termekek ON kazon=termekkazon WHERE knev LIKE '%{keresettkategoria}%'", $"Termékek a '{keresettkategoria}' kategóriában");

            Console.ReadKey();
        }
        private static void Lekerdez(string lekerdezes, string fejlec)
        {           
            Console.WriteLine(fejlec);            
            using (var con = new MySqlConnection(constr))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(lekerdezes, con);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    ConsoleTableBuilder.From(dataTable).WithFormat(ConsoleTableBuilderFormat.MarkDown).ExportAndWriteLine();
                    //ConsoleTableBuilder.From(dataTable).WithFormat(ConsoleTableBuilderFormat.Minimal).ExportAndWriteLine();
                    //ConsoleTableBuilder.From(dataTable).WithFormat(ConsoleTableBuilderFormat.Alternative).ExportAndWriteLine();

                }
            }            
        }
    }
}
