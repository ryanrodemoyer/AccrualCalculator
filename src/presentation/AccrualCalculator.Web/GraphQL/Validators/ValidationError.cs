namespace AppName.Web.GraphQL.Validators
{
    public class ValidationError
    {
        public string ErrorName { get; private set; }

        public string ErrorMessage { get; private set; }

        public ValidationError(string errorName, string errorMessage)
        {
            ErrorName = errorName;
            ErrorMessage = errorMessage;
        }

        public override string ToString()
        {
            return $"Error: Name={ErrorName}, Message={ErrorMessage}";
        }
    }
}