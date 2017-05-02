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
			_hasRedrivePolicy = hasRedrivePolicy;
			_deletionPolicy = deletionPolicy;
			_destinationUrl = destinationUrl;
			_maxNumberOfMessages = maxNumberOfMessages;
			_visibilityTimeout = visibilityTimeout;
			_waitTimeOut = waitTimeOut;
		}

		public bool HasRedrivePolicy()
		{
			return _hasRedrivePolicy;
		}

		public ReceiveMessageRequest ReceiveMessageRequest()
		{
			ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest(this._destinationUrl);
			receiveMessageRequest.AttributeNames.Add(ReceivingAttributes);
			receiveMessageRequest.MessageAttributeNames.Add(ReceivingMessageAttributes);

			if (_maxNumberOfMessages != null)
			{
				receiveMessageRequest.MaxNumberOfMessages = _maxNumberOfMessages.Value;
			}
			else
			{
				receiveMessageRequest.MaxNumberOfMessages = DefaultMaxNumOfMessages;
			}

			if (_visibilityTimeout != null)
			{
				receiveMessageRequest.VisibilityTimeout = _visibilityTimeout.Value;
			}

			if (_waitTimeOut != null)
			{
				receiveMessageRequest.WaitTimeSeconds = _waitTimeOut.Value;
			}

			return receiveMessageRequest;
		}

		public SqsMessageDeletionPolicy DeletionPolicy()
		{
			return _deletionPolicy;
		}
	}
}
