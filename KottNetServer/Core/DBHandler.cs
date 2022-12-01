using KottNetServer.Models;
using MySql.Data.MySqlClient;

namespace KottNetServer.Core
{
    public class DBHandler
    {
        private static string _server = "";
        private static string _user = "";
        private static string _password = "";
        private static string _database = "";

        public static DeviceModel Select(int uid = 0, string ip = "")
        {
            string connStr = $"server={_server};user={_user};database={_database};port=3306;password={_password}";
            MySqlConnection conn = new MySqlConnection(connStr);
            DeviceModel model = new DeviceModel();
            try
            {
                conn.Open();

                string sql = "";

                if (uid != 0)
                    sql = $"SELECT * FROM devices WHERE uid='{uid}'";
                else if (!ip.Equals(""))
                    sql = $"SELECT * FROM devices WHERE ip='{ip}'";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    model.uid = rdr.GetInt32("uid");
                    model.deviceType = rdr.GetString("deviceType");
                    model.state = rdr.GetString("state");
                    model.status = rdr.GetString("status");
                    //model.uid = rdr.GetString("uid");
                }
                rdr.Close();

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Device does not exist");
            }

            conn.Close();
            return model;
        }

        public static DeviceModel[] SelectAll()
        {
            string connStr = $"server={_server};user={_user};database={_database};port=3306;password={_password}";
            MySqlConnection conn = new MySqlConnection(connStr);
            List<DeviceModel> models = new List<DeviceModel>();
            try
            {
                conn.Open();

                string sql = "SELECT * FROM devices";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    DeviceModel model = new DeviceModel();
                    model.uid = rdr.GetInt32("uid");
                    model.deviceType = rdr.GetString("deviceType");
                    model.state = rdr.GetString("state");
                    model.status = rdr.GetString("status");
                    model.ip = rdr.GetString("ip");
                    //model.uid = rdr.GetString("uid");
                    models.Add(model);
                }
                rdr.Close();

                return models.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Device does not exist");
            }

            conn.Close();
            return models.ToArray();
        }

        public static void Update(DeviceModel model)
        {
            string connStr = $"server={_server};user={_user};database={_database};port=3306;password={_password}";
            MySqlConnection conn = new MySqlConnection(connStr);
            
            try
            {
                conn.Open();

                string sql = $"UPDATE devices SET uid='{model.uid}',deviceType='{model.deviceType}',state='{model.state}', status='{model.status}'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
        }

        public static void Insert(DeviceModel model)
        {
            string connStr = $"server={_server};user={_user};database={_database};port=3306;password={_password}";
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = $"INSERT INTO devices(uid, deviceType, state, ip, status) VALUES ({model.uid}, '{model.deviceType}', '{model.state}', '{model.ip}', '{model.status}')";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
        }
    }
}