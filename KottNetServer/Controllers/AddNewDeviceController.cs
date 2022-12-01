using KottNetServer.Core;
using KottNetServer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KottNetServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddNewDeviceController : ControllerBase
    {
        /*[HttpGet]
        public string Get()
        {
            return "hnirtsulhrt";
        }*/

        [HttpGet]
        public string Get(string device)
        {
            string json = "";
            DeviceModel model = JsonConvert.DeserializeObject<DeviceModel>(device);
            model.status = "online";
            if (DBHandler.Select(0, model.ip).uid == 0)
            {
                Random rnd = new Random();
                model.uid = rnd.Next(1, 257164);
                DBHandler.Insert(model);
                json = JsonConvert.SerializeObject(model);
                Console.WriteLine("Adding new device: " + model.uid);
                return json;
            }
            else
                model = DBHandler.Select(0, model.ip);
            model.status = "online";

            DBHandler.Update(model);

            bool deviceIsRegistred = false;
            for (int i = 0; i < DeviceManager.Devices.Count; i++)
                if (DeviceManager.Devices[i].uid.Equals(model.uid))
                {
                    deviceIsRegistred = true;
                    DeviceManager.Devices[i].status = "online";
                }

            if (!deviceIsRegistred)
                DeviceManager.Devices.Add(model);

            Console.WriteLine("Device: " + model.uid + " had been successfully registred");

            json = JsonConvert.SerializeObject(model);
            return json;
        }
    }
}
