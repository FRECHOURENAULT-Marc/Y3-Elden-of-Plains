using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable] public class StateMachineElement
{
    public IState state;
    
    public List<ITransition> transitions = new List<ITransition>();
}

public abstract class IStateMachine : MonoBehaviour
{
    protected List<StateMachineElement> _elements;

    protected StateMachineElement _activeElement;

    protected virtual void Start()
    {
        _activeElement = _elements[0];
        _activeElement.state.Enter();
    }

    protected void AddState(IState state, params ITransition[] transitions)
    {
        StateMachineElement nElement = new StateMachineElement();
        nElement.state = state;
        foreach (ITransition transition in transitions)
            nElement.transitions.Add(transition);
        _elements.Add(nElement);
    }
    
    void SetState(IState newState)
    {
        _activeElement.state.Exit();
        _activeElement = _elements.Find(x => x.state == newState);
        _activeElement.state.Enter();
    }

    void TestConditions()
    {
        foreach (ITransition transition in _activeElement.transitions)
        {
            if (transition.Test() == false)
                continue;
            
            SetState(transition.nextState);
            TestConditions();
            return;
        }
    }
    
    void Update()
    {
        TestConditions();
        _activeElement.state.Execute();
    }
}
