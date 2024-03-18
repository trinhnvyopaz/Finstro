﻿using System;
using Microsoft.AspNetCore.Authorization;

namespace Finstro.Serverless.API.Common
{
	public class CognitoGroupAuthorizationRequirement : IAuthorizationRequirement
	{
		public string CognitoGroup { get; private set; }

		public CognitoGroupAuthorizationRequirement(string cognitoGroup)
		{
			CognitoGroup = cognitoGroup;
		}
	}
}
