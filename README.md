# PredicateLib
谓词筛选表达式Expression&lt;Func&lt;T, bool>>的扩展库


### Predicate拼接
```c#
var predicate = Predicate
    .True<User>()
    .And(item => item.Name == "laojiu");

if (true)
{
    predicate = predicate.And(item => item.Age > 10 && item.Age < 20);
}
```


### Uri搜索条件转换为Predicate
```c#
var uri = new Uri("http://www.xx.com?age=1&name=laojiu&id=001");
var predicate = uri.AsCondition<User>()
    .OperatorFor(item => item.Age, Operator.GreaterThan)
    .IgnoreFor(item => item.Id)
    .ToAndPredicate();  
```
> 输出

```
item => ((item.Age > 1) AndAlso item.Name.Contains("laojiu"))
```

