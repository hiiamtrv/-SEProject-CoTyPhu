using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

/// <summary>
/// THE BANK CONTROLS ALL MONEYS FLOW IN GAME
/// </summary>
public class Bank: MonoBehaviour
{
	//  Events ----------------------------------------

	//	Singleton -------------------------------------
	private static Bank _ins;
	public static Bank Ins
    {
		get { return _ins; }
    }

	//  Properties ------------------------------------
	public int MoneyBank { get => _moneyBank; }
	public Dictionary<Player,int> AllMoneyPlayers { get => _moneyPlayer; }
	
	[System.Serializable]
	public class PairPlayer
    {
		public Player player;
		public int money;
    }
	public List<PairPlayer> _moneyPlayers = new List<PairPlayer>();

	//  Fields ----------------------------------------
	private int _moneyBank;
	[SerializeField] private Dictionary<Player, int> _moneyPlayer = new Dictionary<Player, int>();


	//  Initialization --------------------------------
	public Bank()
    {

    }

	public Bank(int moneyBank, Player[] arrPlayers)
	{
		_moneyBank = moneyBank;
		foreach (Player player in arrPlayers)
        {
			AddPlayer(player);
        }
	}

    //  Unity Methods ---------------------------------
    private void Start()
    {
		_ins = this;
    }


    //  Methods ---------------------------------------
    public int MoneyPlayer(Player player)
    {
		if (_moneyPlayer.ContainsKey(player))
			return _moneyPlayer[player];
		else 
			return -65536;
    }

	public void AddPlayer(Player player)
    {
		if(_moneyPlayer.ContainsKey(player))
        {
			Debug.LogError("Player added duplicated in Bank.AddPlayer()");
        }

		_moneyPlayer.Add(player, 999999);
		_moneyPlayers.Add(new PairPlayer() { player = player, money = 999999 });
	}

	public void RemovePlayer(Player player)
	{
		//Take all money of the player and remove him
		if (_moneyPlayer.ContainsKey(player))
        {
			TakeMoney(player, MoneyPlayer(player));
			_moneyPlayer.Remove(player);
		}
	}

	public List<TransactionModifier> TakeMoneyModifier = new List<TransactionModifier>();
	public List<TransactionModifier> GiveMoneyModifier = new List<TransactionModifier>();

	public void TakeMoney(Player player, int amount, bool IsBetweenPlayers = false)
	{
		int currentAmount = amount;
		List<TransactionModifier> modifiers = new List<TransactionModifier>(TakeMoneyModifier);
		foreach (var modifier in modifiers)
		{
			if (modifier.IsActive(player, amount, IsBetweenPlayers))
			{
				var result = modifier.ModifyTransaction(player, amount, currentAmount);
				player = result.Item1;
				amount = result.Item2;
				currentAmount = result.Item3;
			}
		}

		if (!_moneyPlayer.ContainsKey(player)) return;

		_moneyPlayer[player] -= amount;
		_moneyPlayers.Find(x => x.player == player).money -= amount;
		if (_moneyPlayer[player] <= 0)
        {
			//TODO: Bankrupt the player
        }
	}

	public void SendMoney(Player player, int amount, bool IsBetweenPlayers = false)
	{
		int currentAmount = amount;
		List<TransactionModifier> modifiers = new List<TransactionModifier>(TakeMoneyModifier);
		foreach (var modifier in modifiers)
		{
			if (modifier.IsActive(player, amount, IsBetweenPlayers))
			{
				var result = modifier.ModifyTransaction(player, amount, currentAmount);
				player			= result.Item1;
				amount			= result.Item2;
				currentAmount	= result.Item3;
			}
		}

		if (!_moneyPlayer.ContainsKey(player)) return;

		_moneyPlayer[player] += amount;
		_moneyPlayers.Find(x => x.player == player).money += amount;
		_moneyBank -= amount;
	}

	public void TransactBetweenPlayers(Player source, Player destination, int amount)
	{
		if (!_moneyPlayer.ContainsKey(source) || !_moneyPlayer.ContainsKey(destination) || source == destination) return;

		TakeMoney(source, amount, true);
		SendMoney(destination, amount, true);
	}

	//  Event Handlers --------------------------------
}

public interface ITransaction
{
	public int MoneyAmount
    {
		get;
    }
	// object Source and Destination may be a List
	public object Source
    {
		get;
    }
	public object Destination
    {
		get;
    }
}

public interface TransactionModifier
{
	public bool IsActive(Player player, int baseAmount, bool IsBetweenPlayers);
	// New player target / amount
	public Tuple<Player, int, int> ModifyTransaction(Player target, int baseAmount, int amount);
}