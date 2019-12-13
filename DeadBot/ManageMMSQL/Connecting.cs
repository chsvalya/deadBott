using DeadBot.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadBot.ManageMMSQL
{
    public class Connecting
    {
        /*я подумала,что вместо отдельного класса проще использовать словарь, мб тогда удалим класс с юзерами? */
        public static Dictionary<long, List<DeadLine>> Connect()
        {
            Dictionary<long, List<DeadLine>> users = new Dictionary<long, List<DeadLine>>();
            SqlConnection conn = new SqlConnection("Data Source=(local);Initial Catalog=dead_bot;Integrated Security=SSPI");
            conn.Open();
            Console.WriteLine("Подключение открыто");
            SqlCommand SelectAll = new SqlCommand("SELECT * FROM data",conn);
            var reader = SelectAll.ExecuteReader();
            while (reader.Read())
            {
                var cid = (long)reader.GetValue(1);
                DeadLine deadLine = new DeadLine()
                {
                    Name = (string)reader.GetValue(0),
                    DateTime = (DateTime)reader.GetValue(2),
                    Priority = (int)reader.GetValue(3),
                    NotificationFrequency = (string)reader.GetValue(4),
                    StartDate = (DateTime)reader.GetValue(5)
                };
                if (!users.ContainsKey(cid))
                    users.Add(cid, new List<DeadLine>() { deadLine });
                else
                    users[cid].Add(deadLine);
            }
            conn.Close();
            return users;
        }
    }
}
