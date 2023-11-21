namespace UserQuizApp.Utility
{
    //Utility class that encapsulates an array of objects so that they could be turned into a JSON
    public class ListWrapper<T>
    {
        public ListWrapper()
        {

        }

        public T[] List { get; set; }
    }
}
