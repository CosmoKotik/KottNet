using KottNetServer.Models;

namespace KottNetServer.Core
{
    public class DeviceManager
    {
        public static List<DeviceModel> Devices { get; set; } = new List<DeviceModel>();

        public static void UpdateDeviceList()
        {
            Devices = DBHandler.SelectAll().ToList();
        }

        public static async Task BroadcastAsync()
        {
            for (int i = 0; i < Devices.Count; i++)
            {
                if (Devices[i].status.Equals("online"))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        //Console.WriteLine("http://" + Devices[i].ip + "/checkStatus");
                        try
                        {
                            var result = await client.GetAsync("http://" + Devices[i].ip + "/checkStatus");
                            if (result.IsSuccessStatusCode)
                                Devices[i].status = "online";
                            else
                            {
                                if (Devices[i].status.Equals("online"))
                                {
                                    Devices[i].status = "offline";
                                    Console.WriteLine("Device: " + Devices[i].uid + " is offline.");
                                }
                            }
                        }
                        catch
                        {
                            if (Devices[i].status.Equals("online"))
                            {
                                Devices[i].status = "offline";
                                Console.WriteLine("Device: " + Devices[i].uid + " is offline.");
                            }
                        }

                        DBHandler.Update(Devices[i]);
                    }
                }
            }
        }
    }
}
