namespace Example.AWS.SQS.Messaging.Listener
{
	using Amazon.SQS.Model;

	public class QueueAttributes
	{
		private const string ReceivingAttributes = "All";
		private const string ReceivingMessageAttributes = "All";
		private const int DefaultMaxNumOfMessages = 10;

		private readonly bool _hasRedrivePolicy;
		private readonly SqsMessageDeletionPolicy _deletionPolicy;
		private readonly string _destinationUrl;
		private readonly int? _maxNumberOfMessages;
		private readonly int? _visibilityTimeout;
		private readonly int? _waitTimeOut;

		public QueueAttributes(bool hasRedrivePolicy, SqsMessageDeletionPolicy deletionPolicy, string destinationUrl,
							   int? maxNumberOfMessages, int? visibilityTimeout, int? waitTimeOut)
		{
			this._hasRedrivePolicy = hasRedrivePolicy;
			this._deletionPolicy = deletionPolicy;
			this._destinationUrl = destinationUrl;
			this._maxNumberOfMessages = maxNumberOfMessages;
			this._visibilityTimeout = visibilityTimeout;
			this._waitTimeOut = waitTimeOut;
		}

		public bool HasRedrivePolicy()
		{
			return this._hasRedrivePolicy;
		}

		public ReceiveMessageRequest ReceiveMessageRequest()
		{
			ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest(this._destinationUrl).
					AttributeNames.(ReceivingAttributes).
					withMessageAttributeNames(ReceivingMessageAttributes);

			if (this._maxNumberOfMessages != null)
			{
				receiveMessageRequest.MaxNumberOfMessages = this._maxNumberOfMessages.Value;
			}
			else
			{
				receiveMessageRequest.MaxNumberOfMessages = DefaultMaxNumOfMessages;
			}

			if (this._visibilityTimeout != null)
			{
				receiveMessageRequest.VisibilityTimeout = this._visibilityTimeout.Value;
			}

			if (this._waitTimeOut != null)
			{
				receiveMessageRequest.WaitTimeSeconds = this._waitTimeOut.Value;
			}

			return receiveMessageRequest;
		}

		public SqsMessageDeletionPolicy DeletionPolicy()
		{
			return this._deletionPolicy;
		}
	}
}
