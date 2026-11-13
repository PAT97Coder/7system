namespace DataAccessLayer
{
    public sealed class dt208_DeviceEdit
    {
        public int Id { get; set; }
        public long ExpectedConfigId { get; set; }
        public string SN { get; set; }
        public string DisplayName { get; set; }
        public string Model { get; set; }
        public string IdDept { get; set; }
        public string Location { get; set; }
        public string IdManager { get; set; }
        public decimal TempMin { get; set; }
        public decimal TempMax { get; set; }
        public decimal HumidityMin { get; set; }
        public decimal HumidityMax { get; set; }
        public int ExpectedMinutes { get; set; }
        public int DeviceState { get; set; }
        public string Reason { get; set; }
    }

}
