using Business.Interfaces;
using Common.BaseClasses;
using Common.Models;
using Common.Randomization;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Security
{
	public class JWTService : LoggingWorker, IJWTService
	{
		public IOptions<AppSettings> AppSettings { get; }
		private const int _expirationInMinutes = 15;

		public JWTService(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(loggerFactory)
		{
			AppSettings = appSettings;
		}

		public string CreateToken(Guid clientId, string login, List<System.Security.Claims.Claim> claims)
		{
			var expiration = DateTime.UtcNow.AddMinutes(_expirationInMinutes);
			var payload = new Dictionary<string, object>  {
				{ TokenKeys.ClientId, clientId },
				{ TokenKeys.Expiration, expiration },
				{ TokenKeys.Login, login }
			};
			if (claims != null && claims.Any()) {
				var _claims = claims.Select(a => new { Type = a.Type, Value = a.Value });
				//var jsonClaims = JsonConvert.SerializeObject(_claims);
				payload.Add(TokenKeys.Claims, _claims);
			}
			var tokenCreationResult = this.makeToken(payload);
			if (tokenCreationResult.Failure) {
				return string.Empty;
			}
			return tokenCreationResult.Result;
		}

		public string GenerateSecret()
		{
			var text = RandomStrings.GetRandomText(100);
			var bytes = Encoding.UTF8.GetBytes(text);
			return Convert.ToBase64String(bytes);
		}

		public Envelope<Dictionary<string, object>> VerifyToken(string token)
		{
			try {
				var decodeResult = this.decode(token);
				if (decodeResult.Failure) {
					return Envelope<Dictionary<string, object>>.Fail(decodeResult.Message);
				}
				object expirationObj;
				var found = decodeResult.Result.TryGetValue(TokenKeys.Expiration, out expirationObj);
				if (!found) {
					return Envelope<Dictionary<string, object>>.Fail("Expiration date not found.");
				}
				var expiration = (DateTime)expirationObj;
				if (expiration < DateTime.UtcNow) {
					return Envelope<Dictionary<string, object>>.Fail("Token has expired.");
				}
				return decodeResult;
			} catch (Exception ex) {
				this.Logger.LogError(ex, "VerifyToken");
				return Envelope<Dictionary<string, object>>.Fail("Token was invalid.");
			}
		}

		private Envelope<Dictionary<string, object>> decode(string token)
		{
			try {
				var serializer = new JsonNetSerializer();
				var provider = new UtcDateTimeProvider();
				var validator = new JwtValidator(serializer, provider);
				var urlEncoder = new JwtBase64UrlEncoder();
				var decoder = new JwtDecoder(serializer, validator, urlEncoder);
				var payload = decoder.DecodeToObject<Dictionary<string, object>>(token, this.AppSettings.Value.JWTSecret, true);
				return Envelope<Dictionary<string, object>>.Ok(payload);
			} catch (TokenExpiredException) {
				return Envelope<Dictionary<string, object>>.Fail("Token has expired.");
			} catch (SignatureVerificationException) {
				return Envelope<Dictionary<string, object>>.Fail("Token has invalid signature.");
			}
		}

		private Envelope<string> makeToken(Dictionary<string, object> payload)
		{
			if (payload == null || !payload.Any()) {
				return Envelope<string>.Fail("Token could not be created. Payload was empty.");
			}
			try {
				var algorithm = new HMACSHA256Algorithm();
				var serializer = new JsonNetSerializer();
				var urlEncoder = new JwtBase64UrlEncoder();
				var encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
				var token = encoder.Encode(payload, this.AppSettings.Value.JWTSecret);
				return Envelope<string>.Ok(token);
			} catch {
				return Envelope<string>.Fail("Token could not be created.");
			}
		}
	}
}