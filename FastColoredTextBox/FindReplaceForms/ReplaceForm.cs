﻿using FastColoredTextBoxNS.FindReplaceForms;
using System;
using System.Windows.Forms;

namespace FastColoredTextBoxNS {
	public partial class ReplaceForm : Form, IReplaceForm {
		private readonly Replacer replacer;

		public ReplaceForm(FastColoredTextBox tb) {
			InitializeComponent();
			replacer = new(tb);
		}

		private void TbFind_KeyPress(object sender, KeyPressEventArgs e) {
			switch (e.KeyChar) {
				case '\r':
					BtReplaceNext_Click(sender, null);
					break;
				case '\x1b':
					Hide();
					break;
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if (keyData == Keys.Escape) {
				Close();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void ReplaceForm_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing) {
				e.Cancel = true;
				Hide();
			}
			replacer.GetTextBox().Focus();
		}

		protected override void OnActivated(EventArgs e) => tbFind.Focus();
		private void BtClose_Click(object sender, EventArgs e) => Close();
		private void BtReplaceNext_Click(object sender, EventArgs e) => FindNext();
		private void BtPrev_Click(object sender, EventArgs e) => FindPrev();
		private void BtReplace_Click(object sender, EventArgs e) => ReplaceSelection();
		private void BtReplaceAll_Click(object sender, EventArgs e) => ReplaceAll();

		public Replacer GetReplacer() => replacer;
		public FindOptions GetFindOptions() => new() { IsRegex = cbRegex.Checked, MatchCase = cbMatchCase.Checked, WholeWord = cbWholeWord.Checked };
		public void SetPattern(string pattern) => tbFind.Text = pattern;
		public string GetPattern() => tbFind.Text;
		public void SetValue(string value) => tbReplace.Text = value;
		public string GetValue() => tbReplace.Text;
		public Finder GetFinder() => replacer;
		void IFindForm.Focus() => Focus();

		new void Show() {
			tbFind.SelectAll();
			Show();
		}

		public void ReplaceAll() {
			try {
				MessageBox.Show(replacer.ReplaceAll(GetPattern(), GetValue(), GetFindOptions()) + " occurrence(s) replaced");
			} catch (Exception ex) { MessageBox.Show(ex.Message); }
		}

		public void ReplaceSelection() {
			try {
				replacer.ReplaceSelection(GetValue());
				FindNext();
			} catch (Exception ex) { MessageBox.Show(ex.Message); }
		}

		public void FindNext() {
			try {
				replacer.FindNext(GetPattern(), GetFindOptions());
			} catch (Exception ex) { MessageBox.Show(ex.Message); }
		}

		public void FindPrev() {
			try {
				replacer.FindPrev(GetPattern(), GetFindOptions());
			} catch (Exception ex) { MessageBox.Show(ex.Message); }
		}
	}
}