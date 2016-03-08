using System;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// Summary description for dtGeneric_String
/// </summary>
public class dtGeneric_String : dtBase
{
    private const string _sqlName = "dbo.Generic_String";

    public enum Fields { Value = 0 }

    public dtGeneric_String()
    {
        DataColumn dataColumn = new DataColumn("Value");
        dataColumn.DataType = System.Type.GetType("System.String");

        Columns.Add(dataColumn);
    }

    public override string SQLName
    {
        get { return _sqlName; }
    }

    #region Public Methods

    public DataRow Add(string value)
    {
        DataRow dataRow = this.NewRow();
        dataRow[(int)Fields.Value] = value;

        this.Rows.Add(dataRow);
        return dataRow;
    }

    public void AddRange(List<string> values)
    {
        foreach (string value in values)
            this.Add(value);
    }

    #endregion
}