namespace Example.AWS.SQS
{
	using Service;
	using Service.Impl;

	class MainClass
	{
		public static void Main(string[] args)
		{
			IMessageListenerService service = new SQSMessageListenerServiceImpl();
		}
	}
}
