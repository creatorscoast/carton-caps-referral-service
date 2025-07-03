using Dapper;
using System.Data;

namespace Referral.Api.Core;
public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid guid)
    {
        parameter.Value = guid.ToString();
    }

    public override Guid Parse(object value)
    {
        return Guid.Parse(value.ToString());
    }
}
