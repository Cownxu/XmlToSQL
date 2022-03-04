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
> 后续会持续更新
