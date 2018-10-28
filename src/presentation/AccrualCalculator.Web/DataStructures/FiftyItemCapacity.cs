namespace AppName.Web.DataStructures
{
    public class FiftyItemCapacity : CapacityDefinition
    {
        public FiftyItemCapacity() : this(50)
        {
        }

        private FiftyItemCapacity(int amount) : base(amount)
        {
            
        }
    }
}