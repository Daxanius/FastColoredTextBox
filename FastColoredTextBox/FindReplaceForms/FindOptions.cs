namespace FastColoredTextBoxNS.FindReplaceForms {
	/// <summary>
	///  This is used to specify options to find values in a tb
	/// </summary>
	public struct FindOptions {
		/// <summary>
		///  If set to true, is case sensitive
		/// </summary>
		public bool MatchCase { get; set; }

		/// <summary>
		///  If set to true, uses a regex search pattern
		/// </summary>
		public bool IsRegex { get; set; }

		/// <summary>
		///	 If set to true, match the whole word
		/// </summary>
		public bool WholeWord { get; set; }
	}
}