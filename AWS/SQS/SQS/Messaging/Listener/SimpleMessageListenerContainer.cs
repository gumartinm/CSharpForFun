﻿namespace Example.AWS.SQS.Messaging.Listener
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Amazon.SQS;
	using Amazon.SQS.Model;
	using NLog;

	public class SimpleMessageListenerContainer
	{
		private const string SimpleMessageListenerThreadName = "SimpleMessageListenerContainer-Thread";

		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly string _destinationUrl;
		private readonly IAmazonSQS _amazonSQS;
		private readonly IMessageListener _messageListener;
		private readonly string _logicalQueueName;

		public int? MaxNumberOfMessages { get; set; }
		public int? VisibilityTimeout { get; set; }
		public int? WaitTimeOut { get; set; }
		public int BackOffTime { get; set; }
		public SqsMessageDeletionPolicy DeletionPolicy { get; set; }

		public SimpleMessageListenerContainer(IAmazonSQS amazonSQS,
		                                      IMessageListener messageListener,
		                                      string logicalQueueName,
		                                      string destinationUrl)
		{
			_amazonSQS = amazonSQS;
			_messageListener = messageListener;
			_logicalQueueName = logicalQueueName;
			_destinationUrl = destinationUrl;

			BackOffTime = 10000;
			DeletionPolicy = SqsMessageDeletionPolicy.ALWAYS;
		}

		public void DoInit()
		{
			bool hasRedrivePolicy = HasRedrivePolicy();

			var queueAttributes = new QueueAttributes(hasRedrivePolicy, DeletionPolicy, _destinationUrl, MaxNumberOfMessages, VisibilityTimeout, WaitTimeOut);

			var worker = new AsynchronousMessageListener(this, queueAttributes, _logicalQueueName);
			var thread = new Thread(worker.DoRun);
			thread.Name = SimpleMessageListenerThreadName;
			thread.Start();
		}

		private bool HasRedrivePolicy()
		{
			var attributeNames = new List<string>();
			attributeNames.Add(QueueAttributeName.RedrivePolicy);

			var attributesRequest = new GetQueueAttributesRequest();
			attributesRequest.QueueUrl = _destinationUrl;
			attributesRequest.AttributeNames = attributeNames;

			var attributesResponse = _amazonSQS.GetQueueAttributes(attributesRequest);

			return attributesResponse.Attributes.ContainsKey(QueueAttributeName.RedrivePolicy);
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
							._logger.Warn(exception, "An Exception occurred while polling queue '{0}'. The failing operation will be " +
						                                 "retried in {1} milliseconds", _logicalQueueName, _simpleMessageListenerContainer.BackOffTime);
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
					var messageExecutor = new MessageExecutor(_simpleMessageListenerContainer,
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
