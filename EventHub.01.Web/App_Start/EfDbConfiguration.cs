using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;

namespace EventHub._01.Web
{
    public class EfDbConfiguration : DbConfiguration
    {
        public EfDbConfiguration()
        {
            SetProviderServices("System.Data.SqlClient", SqlProviderServices.Instance);
            SetDefaultConnectionFactory(new SqlConnectionFactory());
        }
    }
}
