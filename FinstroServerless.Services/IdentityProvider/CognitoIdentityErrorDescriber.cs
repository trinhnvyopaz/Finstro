using Amazon.CognitoIdentityProvider;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstroServerless.Services.IdentityProvider
{
    /// <summary>
    /// Service to enable Cognito specific errors for application facing identity errors.
    /// </summary>
    /// <remarks>
    /// These errors are returned to controllers and are generally used as display messages to end users.
    /// </remarks>
    public class CognitoIdentityErrorDescriber : IdentityErrorDescriber
    {
        /// <summary>
        /// Returns the <see cref="IdentityError"/> indicating a CognitoServiceError.
        /// </summary>
        /// <param name="failingOperationMessage">The message related to the operation that failed</param>
        /// <param name="exception">The exception</param>
        /// <returns>The default <see cref="IdentityError"/>.</returns>
        public IdentityError CognitoServiceError(string failingOperationMessage, AmazonCognitoIdentityProviderException exception)
        {
            return new IdentityError
            {
                Code = nameof(CognitoServiceError),
                Description = String.Format("{0} : {1}", failingOperationMessage, exception.Message)
            };
        }
    }
}
