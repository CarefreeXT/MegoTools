using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caredev.MegoTools.Core
{
    /// <summary>
    /// 数据库种类。
    /// </summary>
    public enum EDatabaseKind
    {
        /// <summary>
        /// Microsoft SQL Server
        /// </summary>
        SqlServer,
        /// <summary>
        /// Oracle
        /// </summary>
        Oracle,
        /// <summary>
        /// MySQL
        /// </summary>
        MySQL,
        /// <summary>
        /// PostgreSQL
        /// </summary>
        PostgreSQL,
        /// <summary>
        /// SQLite
        /// </summary>
        SQLite,
        /// <summary>
        /// Microsoft SQL Server Compact
        /// </summary>
        SqlServerCe,
        /// <summary>
        /// Microsoft Access (OleDB)
        /// </summary>
        Access,
        /// <summary>
        /// Microsoft Excel (OleDB)
        /// </summary>
        Excel
    }
}
