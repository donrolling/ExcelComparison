using System.Net.Http;

namespace Common.Web.Interfaces {
	public interface IServerResponse {
		bool Cancelled { get; set; }
		string Content { get; }
		HttpResponseMessage Response { get; }
		ResponseType ServerResponseType { get; }
		string Url { get; }
	}
}