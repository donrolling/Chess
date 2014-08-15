using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.Model {
	public class GameMetaData {
		public enum MetaType {
			Event,
			Site,
			Date,
			Round,
			White,
			Black,
			Result,
			WhiteElo,
			BlackElo,
			ECO,
			ID,
			Filename,
			Annotator,
			Source,
			Remark
		}

		public string Event { get; set; }
		public string Site { get; set; }
		public string Date { get; set; }
		public string Round { get; set; }
		public string White { get; set; }
		public string Black { get; set; }
		public string Result { get; set; }
		public string WhiteELO { get; set; }
		public string BlackELO { get; set; }
		public string ECO { get; set; }
		public string ID { get; set; }
		public string Filename { get; set; }
		public string Annotator { get; set; }
		public string Source { get; set; }
		public string Remark { get; set; }

		public SortedList<int, string> Moves { get; set; }

		public void SetValue(GameMetaData.MetaType metaType, string value) {
			switch (metaType) {
				case GameMetaData.MetaType.Event:
					this.Event = value;
					break;
				case GameMetaData.MetaType.Site:
					this.Site = value;
					break;
				case GameMetaData.MetaType.Date:
					this.Date = value;
					break;
				case GameMetaData.MetaType.Round:
					this.Round = value;
					break;
				case GameMetaData.MetaType.White:
					this.White = value;
					break;
				case GameMetaData.MetaType.Black:
					this.Black = value;
					break;
				case GameMetaData.MetaType.Result:
					this.Result = value;
					break;
				case GameMetaData.MetaType.WhiteElo:
					this.WhiteELO = value;
					break;
				case GameMetaData.MetaType.BlackElo:
					this.BlackELO = value;
					break;
				case GameMetaData.MetaType.ECO:
					this.ECO = value;
					break;
				case GameMetaData.MetaType.Annotator:
					this.Annotator = value;
					break;
				case GameMetaData.MetaType.Source:
					this.Source = value;
					break;
				case GameMetaData.MetaType.Remark:
					this.Remark = value;
					break;
				case GameMetaData.MetaType.Filename:
					this.Filename = value;
					break;
				case GameMetaData.MetaType.ID:
					this.ID = value;
					break;
			}
		}
		public string GetValue(MetaType metaType) {
			var value = this.GetType().GetProperty(metaType.ToString()).GetValue(this, null);
			return value.ToString();
		}
		public string GetValue(string propertyName) {
			string retval = string.Empty;
			var propInfo = this.GetType().GetProperty(propertyName);
			try {
				var value = propInfo.GetValue(this, null);
				retval = value.ToString();
			} catch {

			}
			return retval;
		}
	}
}