namespace UserQuizApp.Utility
{
    //Utility class that wraps an array of objects with a string that can indicate some additional info to later convert it into a JSON result
    public class CombinedWrapper<T>
    {
        public CombinedWrapper()
        {

        }
        public T[] List { get; set; }
        public string WrapName { get; set; }
    }
}
