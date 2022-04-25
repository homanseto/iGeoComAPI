namespace iGeoComAPI.Models
{
    public class CatholicOrgModel
    {
        public string Id { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public string Website { get; set; } = String.Empty;
        public string Phone { get; set; } = String.Empty;
        public string Fax { get; set; } = String.Empty;
    }

    public class CatholicOrgRegion
    {
        public string Region { get; set; } = String.Empty;
    }
}
