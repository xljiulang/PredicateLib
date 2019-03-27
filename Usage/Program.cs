using System;
using System.Linq.Expressions;

namespace Usage
{
    class User
    {
        public int? Age { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var uri = new Uri("http://www.xx.com?age=1&name=laojiu&birthday=2010-01-01&id=001");

            var predicate = uri.AsCondition<User>()
                .OperatorFor(item => item.Age, Operator.GreaterThan)                
                .ToAndPredicate();

            Console.WriteLine(predicate);
            Console.ReadLine();
        }
    }
}
