using FastColoredTextBoxNS.Types;
using System.Text.RegularExpressions;
using System;

namespace FastColoredTextBoxNS.FindReplaceForms {
	/// <summary>
	///  A class that can find patterns in a FastColoredTextBox
	/// </summary>
	public class Finder {
		protected readonly FastColoredTextBox _textBox;

		public FastColoredTextBox GetTextBox() => _textBox;
		public TextSelectionRange GetSelection() => _textBox.Selection;

		/// <param name="textBox">The textbox to search on</param>
		public Finder(FastColoredTextBox textBox) => _textBox = textBox;

		private bool SearchRange(string pattern, TextSelectionRange range, RegexOptions opt) {
			foreach (var r in range.GetRangesByLines(pattern, opt)) {
				_textBox.Selection = r;
				_textBox.DoSelectionVisible();
				_textBox.Invalidate();
				return true;
			}

			return false;
		}

		private bool SearchRangeReversed(string pattern, TextSelectionRange range, RegexOptions opt) {
			foreach (var r in range.GetRangesByLinesReversed(pattern, opt)) {
				_textBox.Selection = r;
				_textBox.DoSelectionVisible();
				_textBox.Invalidate();
				return true;
			}

			return false;
		}

		/// <summary>
		///  Find the value after the selection in the textbox
		/// </summary>
		/// <param name="pattern">The pattern to search for</param>
		/// <param name="options">The search options to use</param>
		public virtual void FindNext(string pattern, FindOptions options = new()) {
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
			if (SearchRange(pattern, searchRange, opt)) { return; }

			// Search range before selection
			searchRange.Start = new Place(0, 0);
			searchRange.End = selectedRange.Start;
			if (SearchRange(pattern, searchRange, opt)) { return; }

			throw new FastColoredTextBoxException("No matches found");
		}

		/// <summary>
		///  Find the value before the selection in the textbox
		/// </summary>
		/// <param name="pattern">The pattern to search for</param>
		/// <param name="options">The search options to use</param>
		public virtual void FindPrev(string pattern, FindOptions options = new()) {
			RegexOptions opt = options.MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
			if (!options.IsRegex)
				pattern = Regex.Escape(pattern);
			if (options.WholeWord)
				pattern = "\\b" + pattern + "\\b";

			TextSelectionRange selectedRange = _textBox.Selection.Clone();
			selectedRange.Normalize();

			// Search range before selection
			TextSelectionRange searchRange = new(_textBox) {
				Start = new Place(0, 0),
				End = selectedRange.Start
			};
			if (SearchRangeReversed(pattern, searchRange, opt)) { return; }

			// Search range after selection
			searchRange.Start = selectedRange.End;
			searchRange.End = new Place(_textBox.GetLineLength(_textBox.LinesCount - 1), _textBox.LinesCount - 1);
			if (SearchRangeReversed(pattern, searchRange, opt)) { return; }

			throw new FastColoredTextBoxException("No matches found");
		}
	}

	/// <summary>
	///  Use this interface if you want to create your own findform
	/// </summary>
	public interface IFindForm : IDisposable {
		/// <summary>
		///  Returns the finder object used to find values in the textbox
		/// </summary>
		Finder GetFinder();

		/// <summary>
		///  A method to build the findoptions to use for searching
		/// </summary>
		FindOptions GetFindOptions();

		/// <summary>
		///  Set the default pattern for the find form
		/// </summary>
		/// <param name="pattern">Pattern to use</param>
		void SetPattern(string pattern);

		/// <summary>
		///  Get the current pattern to search with
		/// </summary>
		string GetPattern();

		/// <summary>
		///  A findnext method that uses the form settings
		/// </summary>
		void FindNext();

		/// <summary>
		///  A findprevious method that uses the form settings
		/// </summary>
		void FindPrev();

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