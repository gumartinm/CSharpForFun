namespace Example.AWS.SQS
{
	using Amazon;
	using Amazon.Runtime;
	using Amazon.SQS;
	using Messaging.Listener;
	using Messaging.Config;
	using System;

	public class AWSConfiguration
	{
		public string Region { get; set; }
		public string AccessKey { get; set; }
		public string SecretKey { get; set; }
		public string DestinationUrl { get; set; }
		public string LogicalQueueName { get; set; }

		public void Configure(IMessageListener messageListener)
		{
			if (Region == null)
			{
				throw new ArgumentException("Region value is required");
			}
			if (AccessKey == null)
			{
				throw new ArgumentException("AccessKey value is required");
			}
			if (SecretKey == null)
			{
				throw new ArgumentException("SecretKey value is required");
			}

			var region = RegionEndpoint.GetBySystemName(Region);
			var awsCredentials = new BasicAWSCredentials(AccessKey, SecretKey);
			var amazonSQS = new AmazonSQSClient(awsCredentials, region);
			var factory = new SimpleMessageListenerContainerFactory();
			factory.AmazonSqs = amazonSQS;
			factory.DestinationUrl = DestinationUrl;
			factory.LogicalQueueName = LogicalQueueName;
			factory.MessageListener = messageListener;

			var simpleMessageListenerContainer = factory.CreateSimpleMessageListenerContainer();
			simpleMessageListenerContainer.DoInit();
		}
	}
}
