using System;
using System.Windows.Forms;

namespace FastColoredTextBoxNS.FindReplaceForms {
	public class Gotoer {
		public int MaxLineCount { get; set; }
		public int SelectedLine { get; private set; }

		public void Goto(int line) => SelectedLine = Math.Clamp(line, 0, MaxLineCount);
	}

	public interface IGotoForm {
		/// <summary>
		///  Gets the gotoer
		/// </summary>
		Gotoer GetGotoer();

		/// <summary>
		///  A way to go to the line set in the form
		/// </summary>
		void Goto();

		/// <summary>
		///  A way to interface with the base showdialog method from a form
		/// </summary>
		DialogResult ShowDialog();

		/// <summary>
		///  A way to interface with the base focus method from a form
		/// </summary>
		void Focus();
	}
}