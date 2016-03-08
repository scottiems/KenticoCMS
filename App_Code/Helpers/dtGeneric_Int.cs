using System;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// Summary description for dtGeneric_Int
/// </summary>
public class dtGeneric_Int : dtBase
{
    private const string _sqlName = "dbo.Generic_Int";

    public enum Fields { Value = 0 }
    public static dtGeneric_Int Empty = new dtGeneric_Int();

    public dtGeneric_Int()
    {
        DataColumn col1 = new DataColumn("Value");
        col1.DataType = Type.GetType("System.Int32");

        Columns.Add(col1);
    }


    public override string SQLName
    {
        get { return _sqlName; }
    }

    public DataRow Add(int? value)
    {
        DataRow row = NewRow();

        row[(int)Fields.Value] = value;
        Rows.Add(row);
        return row;
    }

    public void AddRange(List<int> values)
    {
        foreach (int value in values)
            Add(value);
    }
}