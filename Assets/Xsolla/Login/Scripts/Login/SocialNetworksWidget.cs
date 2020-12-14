﻿using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SocialNetworksWidget : MonoBehaviour
	{
		[SerializeField] private InputField FilterInput = default;

		[SerializeField] private SimpleButton ReturnButton = default;

		[SerializeField] private SocialProviderButton[] SocialNetworkButtons = default;

		public Action<SocialProvider> OnSocialButtonClick { get; set; }

		private void Awake()
		{
			ReturnButton.onClick += () => gameObject.SetActive(false);
			FilterInput.onValueChanged.AddListener(OnFilterChanged);
		}

		private void OnEnable()
		{
			FilterInput.onValueChanged.AddListener(OnFilterChanged);

			foreach (var button in SocialNetworkButtons)
			{
				button.OnClick = () => { OnSocialButtonClick?.Invoke(button.SocialProvider); };
			}
		}

		private void OnDisable()
		{
			FilterInput.onValueChanged.RemoveListener(OnFilterChanged);
			FilterInput.text = string.Empty;
		}

		private void OnFilterChanged(string filterText)
		{
			filterText = filterText.ToLower();

			foreach (var button in SocialNetworkButtons)
			{
				button.gameObject.SetActive(button.SocialProvider.GetParameter().StartsWith(filterText));
			}
		}
	}
}