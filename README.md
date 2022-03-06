# 这是一个可以在XML里面写SQL并执行的工具，如果您有更好的想法可以随时提建议，并联系我【Dear_Leexu@163.com】
> XML如下格式
```
<?xml version="1.0" encoding="utf-8"?>
<ArrayOfXmlCommand>
	<XmlCommand Name="Lee_Test">
		<Parameters>
			<Parameter Name="@MoId" Type="Int32"/>
		</Parameters>
		<CommandText>
			<![CDATA[SELECT * FROM dbo.mom_order where MoId = @MoId]]>
		</CommandText>
	</XmlCommand>
</ArrayOfXmlCommand>
```
## 试一下
-----------------
###  支持的参数数据类型 

> 【String】、【Int32】、【Guid】

```
 services.AddXmlProvider(@"E:\Code\XmlToSQL\XmlToSQL.Test\", new TransactionStackItem
            {
                Info = new ConnectionInfo("Data Source=.;Initial Catalog=UFDATA_010_2020;Integrated Security=True", "System.Data.SqlClient"),
                Mode = TransactionMode.Required

            });//先注入


var row = XmlCommand.From("Lee_Test", new { MoId = 1000000040 }).ToDataTable().Rows;//使用方法

```
> 后续会持续更新。。。
