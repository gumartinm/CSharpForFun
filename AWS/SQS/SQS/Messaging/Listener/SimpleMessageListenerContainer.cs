namespace Example.AWS.SQS.Messaging.Listener
{
	using System;
	using System.Threading;
	using Amazon.SQS;
	using Amazon.SQS.Model;
	using NLog;

	public class SimpleMessageListenerContainer
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly IAmazonSQS _amazonSQS;
		private readonly IMessageListener _messageListener;
		private readonly QueueAttributes _queueAttributes;
		private readonly string _logicalQueueName;

		public int BackOffTime { get; set; }

		public SimpleMessageListenerContainer(IAmazonSQS amazonSQS,
		                                      IMessageListener messageListener,
		                                      QueueAttributes queueAttributes,
		                                      string logicalQueueName)
		{
			_amazonSQS = amazonSQS;
			_messageListener = messageListener;
			_queueAttributes = queueAttributes;
			_logicalQueueName = logicalQueueName;
			BackOffTime = 10000;
		}

		public void DoInit()
		{
			AsynchronousMessageListener worker = new AsynchronousMessageListener(this, _queueAttributes, _logicalQueueName);
			Thread thread = new Thread(worker.DoRun);
			thread.Start();
		}

		private class AsynchronousMessageListener
		{
			private readonly SimpleMessageListenerContainer _simpleMessageListenerContainer;
			private readonly QueueAttributes _queueAttributes;
			private readonly string _logicalQueueName;

			public AsynchronousMessageListener(SimpleMessageListenerContainer simpleMessageListenerContainer,
			                                   QueueAttributes queueAttributes,
			                                   string logicalQueueName)
			{
				_simpleMessageListenerContainer = simpleMessageListenerContainer;
				_queueAttributes = queueAttributes;
				_logicalQueueName = logicalQueueName;
			}

			public void DoRun()
			{
				while (true)
				{
					try {
						DoRunThrowable();
					}
					catch (Exception exception) {
						_simpleMessageListenerContainer
							._logger.Warn(exception, "An Exception occurred while polling queue '{}'. The failing operation will be " +
						                                 "retried in {} milliseconds", _logicalQueueName, _simpleMessageListenerContainer.BackOffTime);
						Thread.Sleep(_simpleMessageListenerContainer.BackOffTime);
					}
				}
			}

			public void DoRunThrowable()
			{
				ReceiveMessageResponse receiveMessageResponse = 
					_simpleMessageListenerContainer._amazonSQS.ReceiveMessage(_queueAttributes.ReceiveMessageRequest());

				receiveMessageResponse.Messages.ForEach((Message message) =>
				{
					MessageExecutor messageExecutor = new MessageExecutor(_simpleMessageListenerContainer,
																		  _logicalQueueName,
																		  message,
																		  _queueAttributes);
					messageExecutor.DoRun();
				});
			}
		}

		private class MessageExecutor
		{
			private readonly SimpleMessageListenerContainer _simpleMessageListenerContainer;
			private readonly Message _message;
			private readonly string _logicalQueueName;
			private readonly string _queueUrl;
			private readonly bool _hasRedrivePolicy;
			private readonly SqsMessageDeletionPolicy _deletionPolicy;

			public MessageExecutor(SimpleMessageListenerContainer simpleMessageListenerContainer,
									string logicalQueueName, Message message, QueueAttributes queueAttributes)
			{
				_simpleMessageListenerContainer = simpleMessageListenerContainer;
				_logicalQueueName = logicalQueueName;
				_message = message;
				_queueUrl = queueAttributes.ReceiveMessageRequest().QueueUrl;
				_hasRedrivePolicy = queueAttributes.HasRedrivePolicy();
				_deletionPolicy = queueAttributes.DeletionPolicy();
			}

			public void DoRun()
			{
				string receiptHandle = _message.ReceiptHandle;
				string body = MessageForExecution();
				try {
					ExecuteMessage(body);
					ApplyDeletionPolicyOnSuccess(receiptHandle);
				}
				catch (Exception exception) {
					ApplyDeletionPolicyOnError(receiptHandle, exception);
				}
			}

			private void ExecuteMessage(string message)
			{
				_simpleMessageListenerContainer._messageListener.Run(message);
			}

			private void ApplyDeletionPolicyOnSuccess(string receiptHandle)
			{
				if (_deletionPolicy == SqsMessageDeletionPolicy.ON_SUCCESS ||
						_deletionPolicy == SqsMessageDeletionPolicy.ALWAYS ||
						_deletionPolicy == SqsMessageDeletionPolicy.NO_REDRIVE)
				{
					DeleteMessage(receiptHandle);
				}
			}

			private void ApplyDeletionPolicyOnError(string receiptHandle, Exception exception)
			{
				if (_deletionPolicy == SqsMessageDeletionPolicy.ALWAYS ||
						(_deletionPolicy == SqsMessageDeletionPolicy.NO_REDRIVE && !_hasRedrivePolicy))
				{
					DeleteMessage(receiptHandle);
				}
				else if (_deletionPolicy == SqsMessageDeletionPolicy.ON_SUCCESS)
				{
					_simpleMessageListenerContainer._logger.Error(exception, "Exception encountered while processing message.");
				}
			}

			private void DeleteMessage(string receiptHandle)
			{
				_simpleMessageListenerContainer._amazonSQS.DeleteMessage(new DeleteMessageRequest(_queueUrl, receiptHandle));
			}

			private string MessageForExecution()
			{
				// DO I REALLY NEED TO SEND ACKNOWLEDGMENT TO SQS?? :(

				//HashMap<String, Object> additionalHeaders = new HashMap<>();
				//additionalHeaders.put(QueueMessageHandler.LOGICAL_RESOURCE_ID, _logicalQueueName);
				//if (_deletionPolicy == SqsMessageDeletionPolicy.NEVER)
				//{
				//	string receiptHandle = _message.ReceiptHandle;
				//		QueueMessageAcknowledgment acknowledgment = new QueueMessageAcknowledgment(_simpleMessageListenerContainer.GetAmazonSQS(), _queueUrl, receiptHandle);
				//	additionalHeaders.put(QueueMessageHandler.ACKNOWLEDGMENT, acknowledgment);
				//}

				return _message.Body;
			}
		}
	}
}
