using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace GumartinM.JsonRPC4NET
{
    public class ExceptionResolver
    {
        /// <summary>
        /// Resolves the exception.
        /// </summary>
        /// <returns>The exception.</returns>
        /// <param name="errorToken">Error token.</param>
        public Exception ResolveException(JToken errorToken)
        {
            JObject errorData = (JObject)errorToken;
            IDictionary<string, JToken> errorTokens = errorData;

            if (!errorTokens.ContainsKey("data"))
            {
                return CreateJsonRpcClientException(errorToken);
            }

            JToken dataToken = errorToken["data"];
            JObject data = (JObject)dataToken;
            errorTokens = data;

            if (!errorTokens.ContainsKey("exceptionTypeName"))
            {
                return CreateJsonRpcClientException(errorToken);
            }

            string exceptionTypeName = data["exceptionTypeName"].Value<string>();
            string message = data.Value<string>("message");

            Exception endException = CreateException(exceptionTypeName, message);

            return endException;
        }

        /// <summary>
        /// Creates the json rpc client exception.
        /// </summary>
        /// <returns>The json rpc client exception.</returns>
        /// <param name="errorToken">Error token.</param>
        private JsonRpcClientException CreateJsonRpcClientException(JToken errorToken)
        {
            return new JsonRpcClientException(
              errorToken.Value<int?>("code") ?? 0,
              errorToken.Value<string>("message"),
              errorToken.SelectToken("data"));
        }

        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <returns>The exception.</returns>
        /// <param name="exceptionTypeName">Exception type name.</param>
        /// <param name="message">Message.</param>
        private Exception CreateException(string exceptionTypeName, string message)
        {
            return new Exception("Remote exception: " + exceptionTypeName + "Message: " + message);
        }
    }
}
