# PredicateLib
谓词筛选表达式Expression&lt;Func&lt;T, bool>>的扩展库

### 1 Predicate的创建
### 1.1 true或false Predicate
```c#
var predicate = Predicate.True<User>();
```
> 表达式输出

```c#
item => true
```

### 1.2 通过属性创建Predicate
```c#
var predicate = Predicate.Create<User>("age", 2, Operator.GreaterThan);
```

> 表达式输出

```c#
item => (item.Age > 2)
```


### 2 Predicate的And Or扩展
```c#
var predicate = Predicate
    .True<User>()
    .And(item => item.Name == "laojiu");

if (true)
{
    predicate = predicate.And(item => item.Age > 10 && item.Age < 20);
}
```


### 3 Condition转换为Predicate
```c#
var uri = new Uri("http://www.xx.com?age=1&name=laojiu&id=001");
var predicate = uri.AsCondition<User>()
    .OperatorFor(item => item.Age, Operator.GreaterThan)
    .IgnoreFor(item => item.Id)
    .ToAndPredicate();  
```
> 输出

```c#
item => ((item.Age > 1) AndAlso item.Name.Contains("laojiu"))
```

