﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Xsolla.Core;

public class SimpleButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IDragHandler
{
	Image _image;
	
	[SerializeField]
	Sprite normalStateSprite;
	[SerializeField]
	Sprite hoverStateSprite;
	[SerializeField]
	Sprite pressedStateSprite;

	bool _isClickInProgress;

	public Action onClick;
	private DateTime lastClick;
	private float rateLimitMs = Constants.DefaultButtonRateLimitMs;

	void Awake()
	{
		_image = GetComponent<Image>();
		lastClick = DateTime.MinValue;
	}
	
	public void OnDrag(PointerEventData eventData)
	{
	}
	
	public void OnPointerEnter(PointerEventData eventData)
	{
		OnHover();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OnNormal();

		_isClickInProgress = false;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		_isClickInProgress = true;

		OnPressed();
	}

	private void PerformClickEvent()
	{
		TimeSpan ts = DateTime.Now - lastClick;
		if (ts.TotalMilliseconds > rateLimitMs) {
			lastClick += ts;
			onClick?.Invoke();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (_isClickInProgress)
		{
			OnHover();
			PerformClickEvent();
		}
		
		_isClickInProgress = false;
	}	
	
	protected virtual void OnNormal()
	{
		_image.sprite = normalStateSprite;
	}
	
	protected virtual void OnHover()
	{
		_image.sprite = hoverStateSprite;
	}
	
	protected virtual void OnPressed()
	{
		_image.sprite = pressedStateSprite;
	}
}