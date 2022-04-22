using FastColoredTextBoxNS.Types;
using FastColoredTextBoxNS.Input;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

namespace FastColoredTextBoxNS.FindReplaceForms {
	/// <summary>
	///  A class that can replace patterns in a FastColoredTextBox
	/// </summary>
	public class Replacer {
		private readonly FastColoredTextBox _textBox;

		public FastColoredTextBox GetTextBox() => _textBox;
		public TextSelectionRange GetSelection() => _textBox.Selection;

		/// <param name="textBox">The textbox to search on</param>
		public Replacer(FastColoredTextBox textBox) => _textBox = textBox;

		private bool ReplaceRange(string pattern, string value, TextSelectionRange range, RegexOptions opt) {
			foreach (var r in range.GetRangesByLines(pattern, opt)) {
				if (r.ReadOnly) { return false; }
				_textBox.Selection = r;
				_textBox.TextSource.Manager.ExecuteCommand(new ReplaceTextCommand(_textBox.TextSource, new() { range }, value));
				_textBox.Selection.SetStartAndEnd(new Place(0, 0));
				_textBox.DoSelectionVisible();
				_textBox.Invalidate();
				return true;
			}

			return false;
		}

		private bool ReplaceRangeReversed(string pattern, string value, TextSelectionRange range, RegexOptions opt) {
			foreach (var r in range.GetRangesByLinesReversed(pattern, opt)) {
				if (r.ReadOnly) { return false; }
				_textBox.Selection = r;
				_textBox.TextSource.Manager.ExecuteCommand(new ReplaceTextCommand(_textBox.TextSource, new() { range }, value));
				_textBox.Selection.SetStartAndEnd(new Place(0, 0));
				_textBox.DoSelectionVisible();
				_textBox.Invalidate();
				return true;
			}

			return false;
		}

		private bool ReplaceRanges(string value, List<TextSelectionRange> ranges) {
			if (ranges.Count > 0) { return false; }

			// Check readonly
			foreach (var r in ranges) {
				if (r.ReadOnly) { return false; }
			}

			// Replace
			_textBox.TextSource.Manager.ExecuteCommand(new ReplaceTextCommand(_textBox.TextSource, ranges, value));
			_textBox.Selection.SetStartAndEnd(new Place(0, 0));
			_textBox.Invalidate();
			return true;
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

		/// <summary>
		///  Replace the value after the selection in the textbox
		/// </summary>
		/// <param name="pattern">The pattern to search for</param>
		/// <param name="value">The value to replace with</param>
		/// <param name="options">The search options to use</param>
		public void ReplaceNext(string pattern, string value, FindOptions options = new()) {
			RegexOptions opt = options.MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
			if (!options.IsRegex)
				pattern = Regex.Escape(pattern);
			if (options.WholeWord)
				pattern = "\\b" + pattern + "\\b";

			TextSelectionRange selectedRange = _textBox.Selection.Clone();
			selectedRange.Normalize();

			// Search range after selection
			TextSelectionRange searchRange = new(_textBox) {
				Start = selectedRange.End,
				End = new Place(_textBox.GetLineLength(_textBox.LinesCount - 1), _textBox.LinesCount - 1)
			};

			if (ReplaceRange(pattern, value, searchRange, opt)) { return; }

			// Search range before selection
			searchRange.Start = new Place(0, 0);
			searchRange.End = selectedRange.Start;
			if (ReplaceRange(pattern, value, searchRange, opt)) { return; }

			throw new FastColoredTextBoxException("No matches found");
		}

		/// <summary>
		///  Replace the value before the selection in the textbox
		/// </summary>
		/// <param name="pattern">The pattern to search for</param>
		/// <param name="value">The value to replace with</param>
		/// <param name="options">The search options to use</param>
		public virtual void ReplacePrev(string pattern, string value, FindOptions options = new()) {
			RegexOptions opt = options.MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
			if (!options.IsRegex)
				pattern = Regex.Escape(pattern);
			if (options.WholeWord)
				pattern = "\\b" + pattern + "\\b";

			TextSelectionRange selectedRange = _textBox.Selection.Clone();
			selectedRange.Normalize();

			// Search range after selection
			TextSelectionRange searchRange = new(_textBox) {
				Start = selectedRange.End,
				End = new Place(_textBox.GetLineLength(_textBox.LinesCount - 1), _textBox.LinesCount - 1)
			};

			if (ReplaceRangeReversed(pattern, value, searchRange, opt)) { return; }

			// Search range before selection
			searchRange.Start = new Place(0, 0);
			searchRange.End = selectedRange.Start;
			if (ReplaceRangeReversed(pattern, value, searchRange, opt)) { return; }

			throw new FastColoredTextBoxException("No matches found");
		}
	}

	public interface IReplaceForm : IDisposable {
		/// <summary>
		///  Returns the finder object used to find values in the textbox
		/// </summary>
		Replacer GetReplacer();

		/// <summary>
		///  A method to build the findoptions to use for searching
		/// </summary>
		FindOptions GetFindOptions();

		/// <summary>
		///  Set the default pattern for the find form
		/// </summary>
		void SetPattern(string pattern);

		/// <summary>
		///  Get the current pattern to search with
		/// </summary>
		string GetPattern();

		/// <summary>
		///  Set the default value for the find form
		/// </summary>
		void SetValue(string value);

		/// <summary>
		///  Get the current value to replace with
		/// </summary>
		string GetValue();

		/// <summary>
		///  Replace all text based on the current form settings
		/// </summary>
		void ReplaceAll();

		/// <summary>
		///  Replace next based on the current form settings
		/// </summary>
		void ReplaceNext();

		/// <summary>
		///	 Replace previous based on the current form settings
		/// </summary>
		void ReplacePrev();

		/// <summary>
		///  A way to interface with the base show method from a form
		/// </summary>
		void Show();

		/// <summary>
		///  A way to interface with the base focus method from a form
		/// </summary>
		void Focus();
	}
}