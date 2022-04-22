using System;
using System.Runtime.Serialization;

namespace FastColoredTextBoxNS {
	internal class FastColoredTextBoxException : Exception {
		public FastColoredTextBoxException() { }
		public FastColoredTextBoxException(string message) : base(message) { }
		public FastColoredTextBoxException(string message, Exception innerException) : base(message, innerException) { }
		protected FastColoredTextBoxException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}