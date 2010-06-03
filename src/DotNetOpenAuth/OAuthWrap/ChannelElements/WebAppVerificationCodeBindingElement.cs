﻿//-----------------------------------------------------------------------
// <copyright file="WebAppVerificationCodeBindingElement.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.OAuthWrap.ChannelElements {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Messages;
	using Messaging;
	using Messaging.Bindings;

	/// <summary>
	/// A binding element for OAuth 2.0 authorization servers that create/verify
	/// issued verification codes as part of obtaining access/refresh tokens.
	/// </summary>
	internal class WebAppVerificationCodeBindingElement : AuthServerBindingElementBase {
		/// <summary>
		/// Initializes a new instance of the <see cref="WebAppVerificationCodeBindingElement"/> class.
		/// </summary>
		internal WebAppVerificationCodeBindingElement() {
		}

		/// <summary>
		/// Gets the protection commonly offered (if any) by this binding element.
		/// </summary>
		/// <value>Always <c>MessageProtections.None</c></value>
		/// <remarks>
		/// This value is used to assist in sorting binding elements in the channel stack.
		/// </remarks>
		public override MessageProtections Protection {
			get { return MessageProtections.None; }
		}

		/// <summary>
		/// Prepares a message for sending based on the rules of this channel binding element.
		/// </summary>
		/// <param name="message">The message to prepare for sending.</param>
		/// <returns>
		/// The protections (if any) that this binding element applied to the message.
		/// Null if this binding element did not even apply to this binding element.
		/// </returns>
		/// <remarks>
		/// Implementations that provide message protection must honor the
		/// <see cref="MessagePartAttribute.RequiredProtection"/> properties where applicable.
		/// </remarks>
		public override MessageProtections? ProcessOutgoingMessage(IProtocolMessage message) {
			var response = message as WebAppSuccessResponse;
			if (response != null) {
				var directResponse = (IDirectResponseProtocolMessage)response;
				var request = (WebAppRequest)directResponse.OriginatingRequest;
				ITokenCarryingRequest tokenCarryingResponse = response;
				tokenCarryingResponse.AuthorizationDescription = new VerificationCode(this.AuthorizationServer.Secret, this.AuthorizationServer.VerificationCodeNonceStore, request.ClientIdentifier, request.Callback, request.Scope, response.AuthorizingUsername);

				return MessageProtections.None;
			}

			return null;
		}

		/// <summary>
		/// Performs any transformation on an incoming message that may be necessary and/or
		/// validates an incoming message based on the rules of this channel binding element.
		/// </summary>
		/// <param name="message">The incoming message to process.</param>
		/// <returns>
		/// The protections (if any) that this binding element applied to the message.
		/// Null if this binding element did not even apply to this binding element.
		/// </returns>
		/// <exception cref="ProtocolException">
		/// Thrown when the binding element rules indicate that this message is invalid and should
		/// NOT be processed.
		/// </exception>
		/// <remarks>
		/// Implementations that provide message protection must honor the
		/// <see cref="MessagePartAttribute.RequiredProtection"/> properties where applicable.
		/// </remarks>
		public override MessageProtections? ProcessIncomingMessage(IProtocolMessage message) {
			var request = message as WebAppAccessTokenRequest;
			if (request != null) {
				IAccessTokenRequest accessRequest = request;
				((VerificationCode)accessRequest.AuthorizationDescription).VerifyCallback(request.Callback);

				return MessageProtections.None;
			}

			return null;
		}
	}
}