using Common.Web.Interfaces;
using System.Net.Http;

namespace Common.Web.Models {
	public class ServerResponse : IServerResponse {
		public bool Cancelled { get; set; }
		public string Content { get; protected set; }
		public HttpResponseMessage Response { get; protected set; }
		public ResponseType ServerResponseType { get; protected set; }
		public string Url { get; protected set; }

		public ServerResponse(string url, HttpResponseMessage response, string content, ResponseType responseType) {
			this.Url = url;
			this.Response = response;
			this.Content = content;
			this.ServerResponseType = responseType;
		}
	}
}