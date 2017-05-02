namespace Example.AWS.SQS.Service.Impl
{
	using System.Threading;
	using Amazon.SQS;

	public class SQSMessageListenerServiceImpl : IMessageListenerService
	{
		private readonly IAmazonSQS _amazonSQS;

		public SQSMessageListenerServiceImpl(IAmazonSQS amazonSQS)
		{
			_amazonSQS = amazonSQS;
		}

		public IAmazonSQS GetAmazonSQS()
		{
			return _amazonSQS;
		}
		public void Init()
		{
			
		}



	}
}
