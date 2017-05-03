namespace Example.AWS.SQS.Service
{
	using Messaging.Listener;
	using NLog;

	public class SQSMessageListenerServiceImpl : IMessageListener
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public void Run(string message)
		{
			_logger.Info("Message: {0}", message);
		}
	}
}
