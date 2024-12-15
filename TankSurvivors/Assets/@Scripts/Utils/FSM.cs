using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	public class FSMData<T>
	{
		public T t;
		public System.Action cbIn;
		public System.Action cbLoop;
		public System.Action cbOut;
	}

	public class FSM<T> : MonoBehaviour
	{
		Dictionary<T, FSMData<T>> dicFun = new Dictionary<T, FSMData<T>>();
		protected T preState	= default(T);
		protected T curState	= default(T);
		protected T nextState	= default(T);
		System.Action cbIn;
		System.Action cbLoop;
		System.Action cbOut;

		protected void InitState(T _t)
		{
			curState = _t;
		}

		protected void AddState(T _t, System.Action _cbIn, System.Action _cbLoop, System.Action _cbOut)
		{
			if (dicFun.ContainsKey(_t))
			{
				Debug.LogError("Already added state " + _t);
			}
			else
			{
				FSMData<T> _data = new FSMData<T>();
				_data.t			= _t;
				_data.cbIn		= _cbIn;
				_data.cbLoop	= _cbLoop;
				_data.cbOut		= _cbOut;

				dicFun.Add(_t, _data);
			}
		}

		public void MoveState(T _nextState)
		{
			if (!dicFun.ContainsKey(_nextState))
			{
				Debug.LogError("I don't Know " + _nextState + " You must add State");
				return;
			}


			//else if (curState.Equals(_nextState))
			//{
			//	return;
			//}

			//1. 현재상태 ... 종료작업...
			if(cbOut != null)
			{
				cbOut();
			}

			//2-1. 새로운 상태 들어가....
			preState	= curState;
			curState	= _nextState;
			nextState	= _nextState;

			//2-2. callback setting...
			FSMData<T> _data = dicFun[curState];
			cbIn	= _data.cbIn;
			cbLoop	= _data.cbLoop;
			cbOut	= _data.cbOut;

			//2-3. in
			if(cbIn != null)
			{
				cbIn();
			}

			//2-4. loop -> update...
		}

		protected void Update()
		{
			//Debug.Log(this + " " + curState);
			if(cbLoop != null)
			{
				cbLoop();
			}
		}
	}
}