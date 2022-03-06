using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using XmlToSQL.Mysoft.DAL;
using XmlToSQL.Mysoft.Xml;

namespace XmlToSQL.Extensions
{
    public static class XmlExtensions
    {

        /// <summary>
        /// 添加XmlToSql、xml所在的目录、基础数据【链接字符串、事务级别、Provider名称】，Provider名称可以默认为【System.Data.SqlClient】
        /// </summary>
        /// <param name="service"></param>
        /// <param name="dirPath">xml所在的目录</param>
        /// <param name="item">基础数据{链接字符串、事务级别、Provider名称}</param>
        public static void AddXmlProvider(this IServiceCollection service,string dirPath, TransactionStackItem item )
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="service"> </param>
            /// <param name="dirPath">xml所在的目录</param>
            /// <param name="item">基础数据{链接字符串、事务级别、Provider名称}</param>
            /// 
            //, string dirPath, string connectionString, TransactionMode mode, string providerName = @"System.Data.SqlClient"
            //TransactionStackItem stackItem = new TransactionStackItem();
            //stackItem.Info = new ConnectionInfo(connectionString, providerName);
            //stackItem.Mode = mode;
            service.AddSingleton(item);
            service.AddSingleton<IDbExecute, XmlCommand>();
            service.AddSingleton<IXmlCommand, XmlCommand>();
            XmlCommandManager.LoadCommnads(dirPath);
            //ConnectionScope scope = new ConnectionScope(mode);
            //ConnectionScope.s_connectionString = connectionString;
            //ConnectionScope.s_providerName = providerName;
            //TransactionStackItem stackItem = new TransactionStackItem();
            //stackItem..Info = new ConnectionInfo(connectionString, providerName);
            //stackItem.Mode = mode;
            //service.AddSingleton(stackItem);
                       // ConnectionScope manager = new ConnectionScope(TransactionMode.Required, connectionString, providerName);
            //service.AddSingleton<ConnectionManager>();
           
           // service.AddSingleton(manager);
            //System.Data.SqlClient
        }
    }
}
