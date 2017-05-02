namespace Example.AWS.SQS.Messaging
{
	public interface IMessageListener
	{	
		void Run(string message);
	}
}
