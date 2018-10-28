namespace AppName.Web.DataStructures
{
    public abstract class CapacityDefinition
    {
        public readonly int Amount;

        protected CapacityDefinition(int amount)
        {
            Amount = amount;
        }
    }
}