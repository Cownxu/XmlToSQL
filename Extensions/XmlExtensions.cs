using Microsoft.Extensions.DependencyInjection;
using XmlToSQL.Mysoft.DAL;
using XmlToSQL.Mysoft.Xml;

namespace XmlToSQL.Extensions
{
    public static class XmlExtensions
    {
        /// <summary>
        /// 添加XmlToSql
        /// </summary>
        /// <param name="service"></param>
        /// <param name="dirPath">存放XML的的目录</param>
        /// <param name="connectionString">数据库链接</param>
        /// <param name="providerName">数据库Provider名称</param>
        public static void AddXmlProvider(this IServiceCollection service, string
            dirPath, string connectionString, string providerName = "System.Data.SqlClient")
        {
            service.AddSingleton<XmlCommand>();
            XmlCommandManager.LoadCommnads(dirPath);
            ConnectionManager manager = new ConnectionManager();
            manager.PushTransactionMode(TransactionMode.Required, connectionString, providerName);
            //System.Data.SqlClient
        }
    }
}
