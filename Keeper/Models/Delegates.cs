using Data.Abstractions;

namespace Keeper.Models
{
    public delegate void DataRowDelegate(DataRow dataRow);
    public delegate bool EditDataRow(DataRow dataRow);
}