/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 6/1/2016
 * Time: 6:07 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace FeatherMvvm.Messaging
{
	/// <summary>
	/// Description of IMessageBus.
	/// </summary>
	public interface IMessageBus
	{
		event EventHandler<MessageEventArgs> MessagePassed;
		
		void SendMessage(string messageTag,object message = null);
	}
}
