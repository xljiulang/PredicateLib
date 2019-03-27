using System;
using System.Linq.Expressions;

namespace Usage
{
    class User
    {
        public string Id { get; set; }
        public int? Age { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var uri = new Uri("http://www.xx.com/?age=1&name=%E8%80%81%E4%B9%9D&id=001");

            var predicate = uri.AsCondition<User>()
                .OperatorFor(item => item.Age, Operator.GreaterThan)
                .IgnoreFor(item => item.Id)
                .ToAndPredicate();

            Console.WriteLine(predicate);
            Console.ReadLine();
        }
    }
}
