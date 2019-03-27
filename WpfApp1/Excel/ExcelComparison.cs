using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Models {
	public class ExcelComparison {
		public object Actual { get; set; }
		public int Column { get; set; }
		public object Expected { get; set; }
		//this is a bad pattern, but I'm leaving it
		public bool ExpectedAndActual_AreEqual {
			get {
				if (this._expectedAndActual_AreCalculated) {
					return this._expectedAndActual_AreEqual;
				}
				if (this.Actual == null && this.Expected == null) {
					this._expectedAndActual_AreEqual = true;
					return this._expectedAndActual_AreEqual;
				}
				if (this.Actual == null && this.Expected != null) {
					this._expectedAndActual_AreEqual = false;
					return this._expectedAndActual_AreEqual;
				}
				if (this.Actual != null && this.Expected == null) {
					this._expectedAndActual_AreEqual = false;
					return this._expectedAndActual_AreEqual;
				}
				this._expectedAndActual_AreEqual = this.Actual.ToString() == this.Expected.ToString();
				return this._expectedAndActual_AreEqual;
			}
		}
		public string Message { get; set; }
		public int Row { get; set; }
		private bool _expectedAndActual_AreCalculated = false;
		private bool _expectedAndActual_AreEqual = false;
	}
}