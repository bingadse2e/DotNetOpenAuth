﻿//-----------------------------------------------------------------------
// <copyright file="EndUserAuthorizationSuccessAuthCodeResponse.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.OAuth2.Messages {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Text;

	using DotNetOpenAuth.Messaging;
	using DotNetOpenAuth.OAuth2.ChannelElements;

	/// <summary>
	/// The message sent by the Authorization Server to the Client via the user agent
	/// to indicate that user authorization was granted, carrying an authorization code and possibly an access token,
	/// and to return the user to the Client where they started their experience.
	/// </summary>
	internal class EndUserAuthorizationSuccessAuthCodeResponse : EndUserAuthorizationSuccessResponseBase, ITokenCarryingRequest {
		/// <summary>
		/// Initializes a new instance of the <see cref="EndUserAuthorizationSuccessAuthCodeResponse"/> class.
		/// </summary>
		/// <param name="clientCallback">The URL to redirect to so the client receives the message. This may not be built into the request message if the client pre-registered the URL with the authorization server.</param>
		/// <param name="version">The protocol version.</param>
		internal EndUserAuthorizationSuccessAuthCodeResponse(Uri clientCallback, Version version)
			: base(clientCallback, version) {
			Contract.Requires<ArgumentNullException>(version != null);
			Contract.Requires<ArgumentNullException>(clientCallback != null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EndUserAuthorizationSuccessAuthCodeResponse"/> class.
		/// </summary>
		/// <param name="clientCallback">The URL to redirect to so the client receives the message. This may not be built into the request message if the client pre-registered the URL with the authorization server.</param>
		/// <param name="request">The authorization request from the user agent on behalf of the client.</param>
		internal EndUserAuthorizationSuccessAuthCodeResponse(Uri clientCallback, EndUserAuthorizationRequest request)
			: base(clientCallback, request) {
			Contract.Requires<ArgumentNullException>(clientCallback != null, "clientCallback");
			Contract.Requires<ArgumentNullException>(request != null, "request");
			((IMessageWithClientState)this).ClientState = request.ClientState;
		}

		[MessagePart(Protocol.code, AllowEmpty = false, IsRequired = true)]
		internal string AuthorizationCode { get; set; }

		[MessagePart(Protocol.access_token, AllowEmpty = false, IsRequired = false)]
		internal string AccessToken { get; set; }

		#region ITokenCarryingRequest Members

		string ITokenCarryingRequest.CodeOrToken {
			get { return this.AuthorizationCode; }
			set { this.AuthorizationCode = value; }
		}

		CodeOrTokenType ITokenCarryingRequest.CodeOrTokenType {
			get { return CodeOrTokenType.AuthorizationCode; }
		}

		IAuthorizationDescription ITokenCarryingRequest.AuthorizationDescription { get; set; }

		#endregion
	}
}