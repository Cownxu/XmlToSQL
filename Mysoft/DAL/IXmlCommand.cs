using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XmlToSQL.Mysoft.DAL
{
    public interface IXmlCommand: IDbExecute
    {
        Task<XmlCommand> From(string name, object argsObject);
        Task<XmlCommand> From(string name);
    }
}
