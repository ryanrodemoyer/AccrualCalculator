namespace AppName.Web.Models
{
    public class AirportInput
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public AirportInput()
        {
            
        }

        public AirportInput(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}