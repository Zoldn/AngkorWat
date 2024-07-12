namespace AngkorWebService.Models
{
    public class TestOutputObject
    {
        public Dictionary<int, int> ResultMap { get; set; } = new();
        public List<double> ResultList { get; set; } = new();
        public TestOutputObject() { }
    }
}
