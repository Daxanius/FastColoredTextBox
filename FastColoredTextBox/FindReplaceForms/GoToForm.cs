using System;
using System.Windows.Forms;
using FastColoredTextBoxNS.FindReplaceForms;

namespace FastColoredTextBoxNS {
	public partial class GoToForm : Form, IGotoForm {
		private readonly Gotoer gotoer = new();

		public GoToForm() => InitializeComponent();

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);

			label.Text = String.Format("Line number (1 - {0}):", gotoer.MaxLineCount);
			nuLineNumber.Maximum = gotoer.MaxLineCount;
			nuLineNumber.Value = gotoer.SelectedLine;
		}

		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			nuLineNumber.Focus();
		}

		private void BtnOk_Click(object sender, EventArgs e) => Goto();
		public Gotoer GetGotoer() => gotoer;
		void IGotoForm.Focus() => Focus();

		public void Goto() {
			gotoer.Goto((int)nuLineNumber.Value);
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
