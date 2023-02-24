﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly Lazy<T> LazyInstance = new Lazy<T>(CreateSingleton);

    public static T Instance => LazyInstance.Value;

    private static T CreateSingleton()
    {
        var ownerObject = new GameObject($"{typeof(T).Name} (singleton)");
        var instance = ownerObject.AddComponent<T>();
        DontDestroyOnLoad(ownerObject);
        return instance;
    }
}

public class GameManager : Singleton<GameManager>
{
    public int playingPersonnage;

    PlayerState actualPlayerState;
    public PlayerState ActualPlayerState
    {
        get
        {
            return actualPlayerState;
        }
        set
        {
            actualPlayerState = value;
            if (value == PlayerState.isTargeting)
            {
                RaiseStartTargetingEvent();
            }
        }
    }


    public delegate void StartTargetingEventHandler();
    public static event StartTargetingEventHandler StartTargetingEvent;

    protected virtual void RaiseStartTargetingEvent()
    {
        StartTargetingEvent?.Invoke();
    }


    public SkillBase actualPlayerAttack;

    public List<GameObject> playerOrder = new List<GameObject>();
    public int turnCount;
    int actualPlayerIndex;

    //get the playing character
    public GameObject ActualPlayer
    {
        get {if (playerOrder.Count != 0){ return playerOrder[actualPlayerIndex];}
             else { return null; }
            }
    }


    //get the playing character script
    public PersonnageScript ActualPlayerScript
    {
        get { if (ActualPlayer != null) { return ActualPlayer.GetComponent<PersonnageScript>(); }
              else { return null; } }
    }

    public enum PlayerState
    {
        idle,
        isMoving,
        isTargeting,
        isAttacking,
        isDying,
    }

    public delegate void StartTurnEventHandler();
    public static event StartTurnEventHandler StartTurnEvent;

    //when called skip to the next character turn
    public void NextPlayerTurn()
    {
        ActualPlayerScript.EndTurn();
        if (actualPlayerIndex < playerOrder.Count -1)
        {
            actualPlayerIndex++;
        }
        else
        {
            turnCount++;
            actualPlayerIndex = 0;
        }
        ActualPlayerScript.StartTurn();
        RaiseStartTurnEvent();
    }

    public void RemoveFromIndex(GameObject objToRemove)
    {
        GameObject actualPlayer = ActualPlayer;
        playerOrder.Remove(objToRemove);
        for (int i = 0; i < playerOrder.Count; i++)
        {
            if (playerOrder[i] == actualPlayer)
            {
                actualPlayerIndex = i;
                break;
            }
        }
    }

    protected virtual void RaiseStartTurnEvent()
    {
        StartTurnEvent?.Invoke();
    }
}
