using UnityEngine;
using System.Collections;

namespace BoardGameKit
{
	/// <summary>
	/// Base class for a Field 
	/// Take a look at "DefaultField" and "CustomField" for how to use it
	/// </summary>
	public abstract class Field : MonoBehaviour
	{
		public abstract int ID { get; set; }

		public virtual int GoBackNumSteps { get; set; }
		public virtual int GoAheadNumSteps { get; set; }
		public virtual int GotoField { get; set; }
		public virtual ActionType Action { get; set; }
		public virtual FieldType Type { get; set; }

		/// <summary>
		/// override to perform custom Action
		/// </summary>
		/// <param name="player">Player.</param>
		public virtual void DoAction(Player player)
		{
		}

		void Start()
		{
			if(this.GetComponentInChildren<TextMesh>() != null)
			{
				// replace '\n' with an actual linebreak since you can't enter linebreaks in TextMesh's Textfield
				string str = this.GetComponentInChildren<TextMesh>().text;
				str = str.Replace("\\n","\n");
				this.GetComponentInChildren<TextMesh>().text = str;
			}
		}
	}
}

