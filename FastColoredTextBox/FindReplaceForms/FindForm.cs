using FastColoredTextBoxNS.FindReplaceForms;
using System;
using System.Windows.Forms;

namespace FastColoredTextBoxNS {
	/// <summary>
	/// A standard built-in findform, implements IFindForm
	/// </summary>
	public partial class FindForm : Form, IFindForm {
		private readonly Finder finder;

		public FindForm(FastColoredTextBox tb) {
			InitializeComponent();
			finder = new(tb);
		}

		private void TbFind_KeyPress(object sender, KeyPressEventArgs e) {
			switch (e.KeyChar) {
				case '\r':
					btFindNext.PerformClick();
					e.Handled = true;
					return;
				case '\x1b':
					Hide();
					e.Handled = true;
					return;
			}
		}

		private void FindForm_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing) {
				e.Cancel = true;
				Hide();
			}
			finder.GetTextBox().Focus();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			switch (keyData) {
				case Keys.Escape:
					Close();
					return true;
				default:
					return base.ProcessCmdKey(ref msg, keyData);
			}
		}

		protected override void OnActivated(EventArgs e) => tbFind.Focus();
		private void BtClose_Click(object sender, EventArgs e) => Close();
		private void BtFindNext_Click(object sender, EventArgs e) => FindNext();
		private void BtnFindPrev_Click(object sender, EventArgs e) => FindPrev();

		public Finder GetFinder() => finder;
		public FindOptions GetFindOptions() => new() { IsRegex = cbRegex.Checked, MatchCase = cbMatchCase.Checked, WholeWord = cbWholeWord.Checked };
		public string GetPattern() => tbFind.Text;
		public void SetPattern(string pattern) => tbFind.Text = pattern;
		void IFindForm.Focus() => Focus();

		void IFindForm.Show() {
			tbFind.SelectAll();
			Show();
		}

		public void FindNext() {
			try {
				finder.FindNext(GetPattern(), GetFindOptions());
			} catch (Exception exception) {
				MessageBox.Show(exception.Message);
			}
		}

		public void FindPrev() {
			try {
				finder.FindPrev(GetPattern(), GetFindOptions());
			} catch (Exception exception) {
				MessageBox.Show(exception.Message);
			}
		}
	}
}