﻿using FastColoredTextBoxNS.Types;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Tester {
	public partial class HyperlinkSample : Form {
		readonly TextStyle blueStyle = new(Brushes.Blue, null, FontStyle.Underline);

		public HyperlinkSample() => InitializeComponent();

		private void Fctb_TextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
			e.ChangedRange.ClearStyle(blueStyle);
			e.ChangedRange.SetStyle(blueStyle, @"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");
		}

		bool CharIsHyperlink(Place place) {
			var mask = fctb.GetStyleIndexMask(new Style[] { blueStyle });
			if (place.iChar < fctb.GetLineLength(place.iLine))
				if ((fctb[place].style & mask) != 0)
					return true;

			return false;
		}

		private void Fctb_MouseMove(object sender, MouseEventArgs e) {
			var p = fctb.PointToPlace(e.Location);
			if (CharIsHyperlink(p))
				fctb.Cursor = Cursors.Hand;
			else
				fctb.Cursor = Cursors.IBeam;
		}

		private void Fctb_MouseDown(object sender, MouseEventArgs e) {
			var p = fctb.PointToPlace(e.Location);
			if (CharIsHyperlink(p)) {
				var url = fctb.GetRange(p, p).GetFragment(@"[\S]").Text;
				Process.Start(url);
			}
		}
	}
}
