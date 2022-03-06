# 这是一个可以在XML里面写SQL执行的工具
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
service.AddXmlProvider(dirPath,connectionString,"System.Data.SqlClient");//先注入

XmlCommand command = new XmlCommand("Lee_Test", new { MoId  = 1000000040 });//使用方法1
var data = command.ToDataTable();

var row = XmlCommand.From("Lee_Test", new { MoId = 1000000040 }).ToDataTable().Rows;//使用方法2

```
> 后续会持续更新。。。
