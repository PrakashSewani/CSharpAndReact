namespace Application.Core
{
    public class Results<T>
    {
        public bool IsSuccess {get;set;}
        public T Value { get; set; }
        public string Error { get; set; }

        public static Results<T> Sucess(T Value)=> new Results<T> {IsSuccess=true,Value=Value};
        public static Results<T> Failure(string error)=> new Results<T> {IsSuccess=false,Error=error};
        
    }
}