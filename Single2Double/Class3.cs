

public class resultobject
{
    public Results Results { get; set; }
}

public class Results
{
    public Output1 output1 { get; set; }
}

public class Output1
{
    public string type { get; set; }
    public Value value { get; set; }
}

public class Value
{
    public string[] ColumnNames { get; set; }
    public string[] ColumnTypes { get; set; }
    public string[][] Values { get; set; }
}
