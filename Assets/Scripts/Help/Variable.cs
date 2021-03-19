using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

public class Variable<TYPE> where TYPE : struct
{
	private TYPE value;
    public TYPE Value {
		get => value;
		set
		{
			this.value = value;
			onValueChanged?.Invoke();
		}
	}
	public UnityAction onValueChanged;
}
public class VariableFloat : Variable<float>
{
    public VariableFloat(float initValue = 0)
	{
		Value = initValue;
	}
}