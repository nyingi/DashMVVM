/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 12-Jun-16
 * Time: 2:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace DashMvvm.Messaging
{
	/// <summary>
	/// Description of Messager.
	/// </summary>
	public sealed class Messager : IMessageBus
	{
		private static Messager instance = new Messager();
		
		public static Messager Instance {
			get {
				return instance;
			}
		}
		
		private Messager()
		{
		}
		
		public event EventHandler<MessageEventArgs> MessagePassed;
		public void SendMessage(string messageTag, object message = null)
		{
			if(MessagePassed != null)
			{
				MessagePassed(this, new MessageEventArgs(messageTag, message));
			}
		}
	}
}
