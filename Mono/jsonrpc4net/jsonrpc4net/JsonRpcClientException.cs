using System;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace GumartinM.JsonRPC4Mono
{
  public class JsonRpcClientException : System.Exception, ISerializable
  {
    /// <summary>
    /// The _code.
    /// </summary>
    private readonly int _code;

    /// <summary>
    /// The _data.
    /// </summary>
    private readonly JToken _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="Example.RemoteAgents.GTKLinux.Model.JsonRpcClientException"/> class.
    /// </summary>
    /// <param name="code">Code.</param>
    /// <param name="message">Message.</param>
    /// <param name="data">Data.</param>
    public JsonRpcClientException(int code, String message, JToken data) : base(message)
    {
      _code = code;
      _data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Example.RemoteAgents.GTKLinux.Model.JsonRpcClientException"/> class.
    /// </summary>
    /// <param name="info">Info.</param>
    /// <param name="context">Context.</param>
    protected JsonRpcClientException(SerializationInfo info, StreamingContext context)
    {
      // TODO
    }

    /// <summary>
    /// Gets the code.
    /// </summary>
    /// <returns>The code.</returns>
    public int getCode() {
      return _code;
    }

    /// <summary>
    /// Gets the data.
    /// </summary>
    /// <returns>The data.</returns>
    public JToken getData() {
      return _data;
    }
  }
}

