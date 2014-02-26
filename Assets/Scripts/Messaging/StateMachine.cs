using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StateMachine<State,Noty>
{
	static bool setInitialStateCalled = false;
	
	private struct StateData
	{
		public State state;
		public Noty enter;
		public Noty exit;
		public StateData(State _state, Noty _enter, Noty _exit)
		{
			state = _state;
			enter = _enter;
			exit = _exit;
		}
	}
	
	public class StateChangeData
	{
		public State changingFrom;
		public State changingTo;
		public object optionalData;
		
		public StateChangeData(State changingFrom, State changingTo, object optionalData)
		{
			this.changingFrom = changingFrom;
			this.changingTo = changingTo;
			if(optionalData != null)
				this.optionalData = optionalData;
		}
	}

	private static Dictionary<State, StateData> states = new Dictionary<State, StateData>();
	
	private static State currentState;
	
	public static void SetInitialState(State stateToChangeTo)
	{
		setInitialStateCalled = true;
		
		currentState = stateToChangeTo;
		
		SendEnterStateNotification(currentState);
	}
	
	public static void ChangeState(State stateToChangeTo)
	{
		if(setInitialStateCalled == false)
		{
			Debug.LogError ("-+ Told to change to a state without a SetInitialState being called at some point, this is likely to be wrong!");
		}
		if(currentState.Equals(stateToChangeTo))
			return;
		
		Debug.Log("Changing from state: " + currentState + " To state: " + stateToChangeTo);

		SendExitStateNotification(stateToChangeTo);
		
		var previousState = currentState;
		currentState = stateToChangeTo;
		
		SendEnterStateNotification(previousState);
	}
	
	public static void RegisterState(State state, Noty enterNotification, Noty exitNotification)
	{
		if(!states.ContainsKey(state))
			states.Add(state, new StateData(state, enterNotification, exitNotification));
	}
	
	private static void SendExitStateNotification(State changingTo)
	{
		var stateData = new StateChangeData(currentState, changingTo, null);

		Messenger<StateChangeData>.Invoke(states[currentState].exit.ToString(), stateData);
	}
	
	private static void SendEnterStateNotification(State changingFrom)
	{
		var stateData = new StateChangeData(changingFrom, currentState, null);

		Messenger<StateChangeData>.Invoke(states[currentState].enter.ToString(), stateData);
	}
	
	//Overloads for sending a data payload with the state change
	
	public static void ChangeState(State stateToChangeTo, object notiData)
	{
		if(currentState.Equals(stateToChangeTo))
			return;
		
		Debug.Log("Changing from state: " + currentState + " To state: " + stateToChangeTo);
		
		SendExitStateNotification(stateToChangeTo, notiData);
		
		var previousState = currentState;
		currentState = stateToChangeTo;
		
		SendEnterStateNotification(previousState, notiData);
	}
	
	
	private static void SendExitStateNotification(State changingTo, object notiData)
	{
		var stateData = new StateChangeData(currentState, changingTo, notiData);

		Messenger<StateChangeData>.Invoke(states[currentState].exit.ToString(), stateData);
	}
	
	
	private static void SendEnterStateNotification(State changingFrom, object notiData)
	{
		var stateData = new StateChangeData(changingFrom, currentState, notiData);

		Messenger<StateChangeData>.Invoke(states[currentState].enter.ToString(), stateData);
	}
}