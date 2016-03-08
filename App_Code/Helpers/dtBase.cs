using System;
using System.Data;
using System.Runtime.Serialization;
using Standpoint.Core.Data;

/// <summary>
/// Summary description for dtBase
/// </summary>
/// 
 [Serializable]
public class dtBase : DataTable 
{
    public dtBase()
    {
    }

    public virtual string SQLName
    {
        get { throw new NotImplementedException(); }
    }

    public virtual int Count
    {
        get { return Rows.Count; }
    }
    public virtual SQLTableValueInput ToSql()
    {
        if (Rows.Count == 0) return new SQLTableValueInput(null, SQLName);
        return new SQLTableValueInput(this, SQLName);
    }
    public virtual DataRow AddBlankRow()
    {
        DataRow row = NewRow();
        Rows.Add(row);
        return row;
    }

    protected dtBase(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {

    }
}