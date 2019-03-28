# PredicateLib
[PredicateLib](https://github.com/xljiulang/PredicateLib)是谓词筛选表达式Expression&lt;Func&lt;T, bool>>的一个扩展库，它可以帮你创建一个复杂且灵活的Expression&lt;Func&lt;T, bool>>，以作为EF、MongoDB Driver等ORM框架的查询条件。

### 1 Predicate的创建
#### 1.1 true或false Predicate
```c#
var predicate = Predicate.True<User>();
```
> 表达式输出

```c#
item => true
```

#### 1.2 通过属性创建Predicate
```c#
var predicate = Predicate.Create<User>("age", 2, Operator.GreaterThan);
```

> 表达式输出

```c#
item => (item.Age > 2)
```


### 2 Predicate的逻辑扩展
```c#
var predicate = Predicate
    .True<User>()
    .And(item => item.Name == "laojiu");

if (true)
{
    predicate = predicate.And(item => item.Age > 10 && item.Age < 20);
}
```
> 表达式输出

```c#
item => ((True AndAlso (item.Name == "laojiu")) AndAlso ((item.Age > 10) AndAlso (item.Age < 20)))
```


### 3 Condition转换为Predicate
Condition对象支持传入`IEnumerable<KeyValuePair<,>>`，`IEnumerable<ConditionItem>`等类型作为条件项，然后转换为Predicate，适用于前端传入查询不确定的字段与值，后端不需要修改代码的需求。

```c#
var uri = new Uri("http://www.xx.com?age=1&name=laojiu&id=001");
var predicate = uri.AsCondition<User>()
    .OperatorFor(item => item.Age, Operator.GreaterThan)
    .IgnoreFor(item => item.Id)
    .ToAndPredicate();  
```
> 表达式输出

```c#
item => ((item.Age > 1) AndAlso item.Name.Contains("laojiu"))
```

