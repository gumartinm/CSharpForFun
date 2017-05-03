namespace Example.AWS.SQS.Messaging.Config
{
	using System;
	using Amazon.SQS;
	using Listener;


	public class SimpleMessageListenerContainerFactory
	{
		public int? MaxNumberOfMessages { get; set; }
		public int? VisibilityTimeout { get; set; }
		public int? WaitTimeOut { get; set; }
		public int? BackOffTime { get; set; }
		public IAmazonSQS AmazonSqs { get; set; }
		public string LogicalQueueName { get; set; }
		public IMessageListener MessageListener { get; set; }
		public QueueAttributes QueueAttributes { get; set; }
		public string DestinationUrl { get; set; }


		public SimpleMessageListenerContainer CreateSimpleMessageListenerContainer()
		{
			if (AmazonSqs == null)
			{
				throw new ArgumentException("AmazonSqs value is required");
			}
			if (MessageListener == null)
			{
				throw new ArgumentException("MessageListener value is required");
			}
			if (LogicalQueueName == null)
			{
				throw new ArgumentException("LogicalQueueName value is required");
			}
			if (DestinationUrl == null)
			{
				throw new ArgumentException("DestinationUrl value is required");
			}
			
			var simpleMessageListenerContainer = new SimpleMessageListenerContainer(AmazonSqs,
											  										MessageListener,
											  										LogicalQueueName,
			                                                                        DestinationUrl);

			if (MaxNumberOfMessages != null)
			{
				simpleMessageListenerContainer.MaxNumberOfMessages = MaxNumberOfMessages;
			}
			if (VisibilityTimeout != null)
			{
				simpleMessageListenerContainer.VisibilityTimeout = VisibilityTimeout;
			}
			if (WaitTimeOut != null)
			{
				simpleMessageListenerContainer.WaitTimeOut = WaitTimeOut;
			}
			if (BackOffTime != null)
			{
				simpleMessageListenerContainer.BackOffTime = BackOffTime.Value;
			}

			return simpleMessageListenerContainer;
		}
	}
}
