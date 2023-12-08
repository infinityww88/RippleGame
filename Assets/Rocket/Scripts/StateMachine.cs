using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace StateMachine
{
	public class State<T> {
		private string name;
		private State<T> parentState;
		private Dictionary<T, State<T>> nextStates;
		
		public State(State<T> parentState, string name) {
			this.parentState = parentState;
			this.name = name;
			this.nextStates = new Dictionary<T, State<T>>();
		}
		
		public override string ToString() {
			return $"state {name}";
		}
		
		public void AddTransition(T transition, State<T> nextState) {
			nextStates.Add(transition, nextState);
		}
		
		public virtual void OnEnter() {
		}
		
		public virtual void OnExit() {
		}
		
		public virtual State<T> Trigger(T transition) {
			Debug.Log($"trigger {this} {transition}");
			if (nextStates.ContainsKey(transition)) {
				var nextState = nextStates[transition];
				this.OnExit();
				if (nextState.parentState != this.parentState) {
					this.parentState?.OnExit();
					nextState.parentState?.OnEnter();
				}
				nextState.OnEnter();
				return nextState;
			}
			return this;
		}
	}
}
