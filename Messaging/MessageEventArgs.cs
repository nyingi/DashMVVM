/*
 * Created by SharpDevelop.
 * User: Nyingi
 * Date: 6/1/2016
 * Time: 6:09 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace FeatherMvvm.Messaging
{
	/// <summary>
	/// Description of MessageEventArgs.
	/// </summary>
	public class MessageEventArgs : EventArgs
	{
		public object Message { get; set; }
		public string MessageTag { get; set; }
		
		
		public MessageEventArgs(string messageTag,object message = null)
		{
			Message = message;
			MessageTag = messageTag;
		}
	}
}
