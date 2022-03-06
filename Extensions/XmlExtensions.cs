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
            service.AddSingleton(item);
            service.AddSingleton<IDbExecute, XmlCommand>();
            service.AddSingleton<IXmlCommand, XmlCommand>();
            XmlCommandManager.LoadCommnads(dirPath);
        }
    }
}
