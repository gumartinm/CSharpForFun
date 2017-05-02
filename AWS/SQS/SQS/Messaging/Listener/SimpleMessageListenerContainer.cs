namespace Example.AWS.SQS.Messaging.Listener
{
	using Amazon.SQS;
	using Amazon.SQS.Model;

	public class SimpleMessageListenerContainer
	{
		private readonly IAmazonSQS _amazonSQS;

		public SimpleMessageListenerContainer(IAmazonSQS amazonSQS)
		{
			_amazonSQS = amazonSQS;
		}

		public IAmazonSQS GetAmazonSQS()
		{
			return _amazonSQS;
		}

		private class AsynchronousMessageListener
		{
			private readonly QueueAttributes _queueAttributes;
			private readonly SimpleMessageListenerContainer _simpleMessageListenerContainer;

			public AsynchronousMessageListener(SimpleMessageListenerContainer simpleMessageListenerContainer,
			                                   QueueAttributes queueAttributes)
			{
				this._simpleMessageListenerContainer = simpleMessageListenerContainer;
				this._queueAttributes = queueAttributes;
			}

			public void DoWork()
			{
				ReceiveMessageResponse receiveMessageResponse = this._simpleMessageListenerContainer
				                                                    .GetAmazonSQS().ReceiveMessage(this._queueAttributes.ReceiveMessageRequest());

				receiveMessageResponse.Messages.ForEach((Message message) => message);
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

			private MessageExecutor(SimpleMessageListenerContainer simpleMessageListenerContainer,
									string logicalQueueName, Message message, QueueAttributes queueAttributes)
			{
				this._simpleMessageListenerContainer = simpleMessageListenerContainer;
				this._logicalQueueName = logicalQueueName;
				this._message = message;
				this._queueUrl = queueAttributes.ReceiveMessageRequest().QueueUrl;
				this._hasRedrivePolicy = queueAttributes.HasRedrivePolicy();
				this._deletionPolicy = queueAttributes.DeletionPolicy();
			}

		public void run()
		{
			string receiptHandle = this._message.ReceiptHandle;
			org.springframework.messaging.Message<String> queueMessage = getMessageForExecution();
			try
			{
				executeMessage(queueMessage);
				ApplyDeletionPolicyOnSuccess(receiptHandle);
			}
			catch (MessagingException messagingException)
			{
				applyDeletionPolicyOnError(receiptHandle, messagingException);
			}
		}

		private void ApplyDeletionPolicyOnSuccess(string receiptHandle)
		{
			if (this._deletionPolicy == SqsMessageDeletionPolicy.ON_SUCCESS ||
					this._deletionPolicy == SqsMessageDeletionPolicy.ALWAYS ||
					this._deletionPolicy == SqsMessageDeletionPolicy.NO_REDRIVE)
			{
				DeleteMessage(receiptHandle);
			}
		}

		private void ApplyDeletionPolicyOnError(string receiptHandle, MessagingException messagingException)
		{
			if (this._deletionPolicy == SqsMessageDeletionPolicy.ALWAYS ||
					(this._deletionPolicy == SqsMessageDeletionPolicy.NO_REDRIVE && !this._hasRedrivePolicy))
			{
				DeleteMessage(receiptHandle);
			}
			else if (this._deletionPolicy == SqsMessageDeletionPolicy.ON_SUCCESS)
			{
				getLogger().error("Exception encountered while processing message.", messagingException);
			}
		}

		private void DeleteMessage(string receiptHandle)
		{
				this._simpleMessageListenerContainer.GetAmazonSQS().DeleteMessage(new DeleteMessageRequest(this._queueUrl, receiptHandle));
		}

		private org.springframework.messaging.Message<String> getMessageForExecution()
		{
			HashMap<String, Object> additionalHeaders = new HashMap<>();
			additionalHeaders.put(QueueMessageHandler.LOGICAL_RESOURCE_ID, this._logicalQueueName);
			if (this.deletionPolicy == SqsMessageDeletionPolicy.NEVER)
			{
				String receiptHandle = this.message.getReceiptHandle();
				QueueMessageAcknowledgment acknowledgment = new QueueMessageAcknowledgment(SimpleMessageListenerContainer.this.getAmazonSqs(), this.queueUrl, receiptHandle);
				additionalHeaders.put(QueueMessageHandler.ACKNOWLEDGMENT, acknowledgment);
			}

			return createMessage(this.message, additionalHeaders);
		}
	}
	}
}
