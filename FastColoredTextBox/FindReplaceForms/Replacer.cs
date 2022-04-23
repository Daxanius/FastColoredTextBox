using FastColoredTextBoxNS.Types;
using FastColoredTextBoxNS.Input;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

namespace FastColoredTextBoxNS.FindReplaceForms {
	/// <summary>
	///  A class that can replace patterns in a FastColoredTextBox
	/// </summary>
	public class Replacer : Finder {
		/// <param name="textBox">The textbox to search on</param>
		public Replacer(FastColoredTextBox textBox) : base(textBox) { }

		private bool ReplaceRanges(string value, List<TextSelectionRange> ranges) {
			if (ranges.Count == 0) { return false; }

			// Check readonly
			foreach (var r in ranges) { if (r.ReadOnly) { return false; } }

			// Replace
			_textBox.BeginUpdate();
			_textBox.TextSource.Manager.ExecuteCommand(new ReplaceTextCommand(_textBox.TextSource, ranges, value));
			_textBox.Selection.SetStartAndEnd(new Place(0, 0));
			_textBox.Invalidate();
			_textBox.EndUpdate();
			return true;
		}

		/// <summary>
		///  Replace the current selection with a value
		/// </summary>
		/// <param name="value">The value to replace the selection with</param>
		public void ReplaceSelection(string value) {
			if (_textBox.SelectionLength == 0) { throw new FastColoredTextBoxException("Selection is empty"); }
			if (_textBox.Selection.ReadOnly) { throw new FastColoredTextBoxException("Selection is readonly"); }
			_textBox.InsertText(value);
		}

		/// <summary>
		///  Replace all values matching the pattern in the textbox
		/// </summary>
		/// <param name="pattern">The pattern to search for</param>
		/// <param name="value">The value to replace with</param>
		/// <param name="options">The search options to use</param>
		public int ReplaceAll(string pattern, string value, FindOptions options = new()) {
			RegexOptions opt = options.MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
			if (!options.IsRegex)
				pattern = Regex.Escape(pattern);
			if (options.WholeWord)
				pattern = "\\b" + pattern + "\\b";

			var range = _textBox.Selection.IsEmpty ? _textBox.Range.Clone() : _textBox.Selection.Clone();
			var list = new List<TextSelectionRange>();
			foreach (var r in range.GetRangesByLines(pattern, opt)) { list.Add(r); }

			ReplaceRanges(value, list);
			return list.Count;
		}
	}

	public interface IReplaceForm : IFindForm, IDisposable {
		/// <summary>
		///  Returns the finder object used to find values in the textbox
		/// </summary>
		Replacer GetReplacer();

		/// <summary>
		///  Set the default value for the find form
		/// </summary>
		void SetValue(string value);

		/// <summary>
		///  Get the current value to replace with
		/// </summary>
		string GetValue();

		/// <summary>
		///  Replace all based on form settings
		/// </summary>
		void ReplaceAll();

		/// <summary>
		///  Replace selection based on form settings
		/// </summary>
		void ReplaceSelection();
	}
}