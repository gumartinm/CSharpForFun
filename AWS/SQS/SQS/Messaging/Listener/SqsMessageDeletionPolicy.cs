namespace Example.AWS.SQS.Messaging.Listener
{
	public enum SqsMessageDeletionPolicy
	{
		ALWAYS,
		NEVER,
		NO_REDRIVE,
		ON_SUCCESS
	}
}
