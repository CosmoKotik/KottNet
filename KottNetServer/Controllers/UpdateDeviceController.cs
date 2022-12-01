using KottNetServer.Core;
using KottNetServer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KottNetServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateDeviceController : ControllerBase
    {
        [HttpGet]
        public async Task<string> GetAsync(string device, bool isEsp = false)
        {
            DeviceModel model = JsonConvert.DeserializeObject<DeviceModel>(device);
            model.status = "online";
            if (!isEsp)
                for (int i = 0; i < DeviceManager.Devices.Count; i++)
                {
                    if (DeviceManager.Devices[i].uid == model.uid)
                        using (HttpClient client = new HttpClient())
                        {
                            try
                            {
                                switch (model.deviceType)
                                {
                                    case "light":
                                        int state = bool.Parse(model.state) ? 1 : 0;
                                        var result = await client.GetAsync("http://" + model.ip + "/setValues?state=" + state);
                                        Console.WriteLine("Client set the light from device: " + model.uid + " to: " + model.state);
                                        if (!result.IsSuccessStatusCode)
                                        {
                                            model.status = "offline";
                                            Console.WriteLine("Device: " + model.uid + " is offline.");
                                        }
                                        break;
                                }
                            }
                            catch
                            {
                                model.status = "offline";
                                Console.WriteLine("Device: " + model.uid + " is offline.");
                            }

                            DBHandler.Update(model);
                        }
                }
            else
            {
                switch (model.deviceType)
                {
                    case "light":
                        /*if (int.Parse(model.state) == 0)
                            model.state = "false";
                        else
                            model.state = "true";*/
                        Console.WriteLine("Device: " + model.uid + " set the light to: " + model.state);
                        DBHandler.Update(model);
                        break;
                }
            }

            return JsonConvert.SerializeObject(model);
        }
    }
}
