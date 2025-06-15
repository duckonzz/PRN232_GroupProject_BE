namespace GenderHealthCare.Core.Attributes
{
    public class CustomName : Attribute
    {
        public string Name { get; set; }

        public CustomName(string name)
        {
            Name = name;
        }
    }
}
