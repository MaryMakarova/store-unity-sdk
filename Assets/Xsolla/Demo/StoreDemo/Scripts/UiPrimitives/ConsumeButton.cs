﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeButton : MonoBehaviour
{
	public SimpleTextButton consumeButton;
	public Action onClick { get; set; }
	public ValueCounter counter;

	void Start()
    {
		consumeButton.onClick = () => {
			if(counter > 0) {
				onClick?.Invoke();
			} else {
				Debug.Log("You try consume item with quantity = 0!");
			}
		};
	}
}
