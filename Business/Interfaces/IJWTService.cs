using Common.Models;
using System;
using System.Collections.Generic;

namespace Business.Interfaces
{
	public interface IJWTService
	{
		string CreateToken(Guid clientId, string login, List<System.Security.Claims.Claim> claims);

		string GenerateSecret();

		Envelope<Dictionary<string, object>> VerifyToken(string token);
	}
}