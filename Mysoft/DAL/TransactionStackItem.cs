namespace XmlToSQL.Mysoft.DAL
{
	internal class TransactionStackItem
	{
		public ConnectionInfo Info { get; set; }

		public TransactionMode Mode { get; set; }

		public bool EnableTranscation { get; set; }

		public bool CanClose { get; set; }

		public int HitCount { get; set; }
	}
}
