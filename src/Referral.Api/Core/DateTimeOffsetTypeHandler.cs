using Dapper;
using System.Data;

namespace Referral.Api.Core;
public class DateTimeOffsetTypeHandler : SqlMapper.TypeHandler<DateTimeOffset>
{
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset dt)
    {
        parameter.Value = dt.ToString();
    }

    public override DateTimeOffset Parse(object value)
    {
        return DateTimeOffset.Parse(value.ToString());
    }
}
