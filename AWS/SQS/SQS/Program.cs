namespace Example.AWS.SQS
{
	using System;
	using System.Threading;
	using Service;

	class MainClass
	{
		public static void Main(string[] args)
		{
			var SQSMessageListenerService = new SQSMessageListenerServiceImpl();
			var aWSConfiguration = new AWSConfiguration();
			aWSConfiguration.AccessKey = Environment.GetEnvironmentVariable("AWS_ACCESSKEY");
			aWSConfiguration.DestinationUrl = Environment.GetEnvironmentVariable("AWS_SQS_URL");;
			aWSConfiguration.LogicalQueueName = Environment.GetEnvironmentVariable("AWS_LOGICAL_QUEUE_NAME");
			aWSConfiguration.Region = Environment.GetEnvironmentVariable("AWS_REGION");
			aWSConfiguration.SecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY");

			aWSConfiguration.Configure(SQSMessageListenerService);

			Thread.Sleep(1000000);
		}
	}
}
