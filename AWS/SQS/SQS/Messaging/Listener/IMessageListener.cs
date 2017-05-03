namespace Example.AWS.SQS.Messaging.Listener
{
	public interface IMessageListener
	{	
		void Run(string message);
	}
}
