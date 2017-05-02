namespace Example.AWS.SQS.Messaging.Listener
{
	using System;
	using Amazon.SQS;
	using Amazon.SQS.Model;
	using NLog;

	public class SimpleMessageListenerContainer
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly IAmazonSQS _amazonSQS;
		private readonly IMessageListener _messageListener;

		public SimpleMessageListenerContainer(IAmazonSQS amazonSQS, IMessageListener messageListener)
		{
			_amazonSQS = amazonSQS;
			_messageListener = messageListener;
		}

		public IAmazonSQS GetAmazonSQS()
		{
			return _amazonSQS;
		}

		public Logger GetLogger()
		{
			return _logger;
		}

		public IMessageListener GetMessageListener()
		{
			return _messageListener;
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
				ReceiveMessageResponse receiveMessageResponse = _simpleMessageListenerContainer
				                                               		.GetAmazonSQS().ReceiveMessage(_queueAttributes.ReceiveMessageRequest());

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
				try
				{
					ExecuteMessage(body);
					ApplyDeletionPolicyOnSuccess(receiptHandle);
				}
				catch (Exception exception)
				{
					ApplyDeletionPolicyOnError(receiptHandle, exception);
				}
			}

			private void ExecuteMessage(string message)
			{
				_simpleMessageListenerContainer.GetMessageListener().Run(message);
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
					_simpleMessageListenerContainer.GetLogger().Error(exception, "Exception encountered while processing message.");
				}
			}

			private void DeleteMessage(string receiptHandle)
			{
					_simpleMessageListenerContainer.GetAmazonSQS().DeleteMessage(new DeleteMessageRequest(_queueUrl, receiptHandle));
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
