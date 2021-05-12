namespace ApiDTC.Utilities
{
    public class GenericResponse<T>
    {

        public string Code { get; set; }
        public T Result { get; set; }
        public string Message { get; set; }
    }
}
