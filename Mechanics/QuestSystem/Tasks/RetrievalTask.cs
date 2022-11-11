﻿using SpiritMod.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Mechanics.QuestSystem.Tasks
{
	public class RetrievalTask : QuestTask
	{
		public override string ModCallName => "Retrieve";

		private int _itemID;
		private int _itemsNeeded;
		private string _wording;
		private int _lastCount;
		private string _objective;

		public RetrievalTask() { }

		public RetrievalTask(int itemID, int amount, string wordChoice = "Retrieve", string objective = null)
		{
			_itemID = itemID;
			_itemsNeeded = amount;
			_wording = wordChoice;
			_objective = objective;
		}

		public override QuestTask Parse(object[] args)
		{
			// get the item ID
			if (!QuestUtils.TryUnbox(args[1], out int itemID))
			{
				if (QuestUtils.TryUnbox(args[1], out short idShort, "Item ID"))
					itemID = idShort;
				else
					return null;
			}

			// get the amount
			if (!QuestUtils.TryUnbox(args[2], out int amount, "Item amount"))
				return null;

			// get the word choice, if there is one
			string wordChoice = "Retrieve";
			if (args.Length > 3)
			{
				if (!QuestUtils.TryUnbox(args[3], out wordChoice, "Word choice"))
					return null;
			}

			// get the name override, if there is one
			string objective = null;
			if (args.Length > 4)
			{
				if (!QuestUtils.TryUnbox(args[4], out objective, "Approach NPC Objective"))
					return null;
			}

			return new RetrievalTask(itemID, amount, wordChoice, objective);
		}

		public override void AutogeneratedBookText(List<string> lines) => lines.Add(GetObjectives(false));
		public override void AutogeneratedHUDText(List<string> lines) => lines.Add(GetObjectives(true));

		public string GetObjectives(bool showProgress)
		{
			var builder = new StringBuilder();

			if (_objective != null)
			{
				builder.Append(_objective);
				return builder.ToString();
			}

			string itemName = Lang.GetItemNameValue(_itemID);
			string count = _itemsNeeded > 1 ? _itemsNeeded.ToString() : "a";
			builder.Append(_wording).Append(" ").Append(count).Append(" ").Append(itemName);

			// pluralness
			builder.Append(QuestUtils.GetPluralEnding(_itemsNeeded, itemName));

			// add a progress bracket at the end like: (x/y)
			if (showProgress)
			{
				int showAmount = System.Math.Min(_itemsNeeded, _lastCount);
				builder.Append(" [c/97E2E2:(").Append(showAmount).Append("/").Append(_itemsNeeded).Append(")]");
			}

			return builder.ToString();
		}

		public override bool CheckCompletion()
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				_lastCount = Main.LocalPlayer.CountItem(_itemID, _itemsNeeded);
				return _lastCount >= _itemsNeeded;
			}
			else if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				_lastCount = 0;
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					Player p = Main.player[i];
					if (p.active)
						_lastCount += p.CountItem(_itemID, _itemsNeeded);
				}
			}
			return _lastCount >= _itemsNeeded;
		}
	}
}
