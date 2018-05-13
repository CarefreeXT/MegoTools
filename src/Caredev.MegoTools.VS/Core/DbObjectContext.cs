using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caredev.Mego;
using Caredev.Mego.Resolve.Providers;

namespace Caredev.MegoTools.Core
{
    public class DbObjectContext : DbContext
    {
        static DbObjectContext()
        {
            var dictionary = DatabaseBase.Databases.ToDictionary(a => a.ProviderName, a => a.Factory);
            dictionary.Add("System.Data.OleDb", System.Data.OleDb.OleDbFactory.Instance);
            DbAccessProvider.SetGetFactory(delegate (string providerName)
            {
                return dictionary[providerName];
            });
        }
        public DbObjectContext(IConnectionInformation connection)
            : base(connection.ConnectionString, connection.ProviderName)
        {
        }
    }
}
