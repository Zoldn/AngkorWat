namespace AngkorWebService.Models
{
    public class TestInputObject
    {
        public TestInputObject() { }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public double Size { get; set; }
    }
}
