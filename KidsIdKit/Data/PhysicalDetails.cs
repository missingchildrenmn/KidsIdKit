namespace KidsIdKit.Data
{
    public class PhysicalDetails
    {
        public string Height { get; set; }
        public string Weight { get; set; }
        public DateTime MeasurementDate { get; set; } = DateTime.Today;
        public string HairColor { get; set; }
        public string HairStyle { get; set; }
        public string EyeColor { get; set; }
        public bool EyeGlasses { get; set; }
        public bool EyeContacts { get; set; }
        public string SkinTone { get; set; }
        public string RacialEthnicIdentity { get; set; }
        public string Gender { get; set; }
        public string GenderIdentity { get; set; }
    }
}
