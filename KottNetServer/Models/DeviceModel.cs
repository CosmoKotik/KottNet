namespace KottNetServer.Models
{
    public class DeviceModel
    {
        public int uid { get; set; }
        public string? deviceType { get; set; }
        public string? state { get; set; }
        public string? ip { get; set; }
        public string? room { get; set; }
        public string? device_group { get; set; }
        public string? status { get; set; }
    }
}
