using System;
using UnityEngine;

namespace Xsolla.Core
{
	internal class AndroidHelper
	{
		private AndroidJavaClass _unityPlayer;
		private AndroidJavaObject _activity;
		private AndroidJavaObject _context;
		private static MainThreadExecutor _mainThreadExecutorInstance;
		private static AndroidJavaClass _xlogin;

		private AndroidJavaClass UnityPlayer
		{
			get
			{
				if (_unityPlayer == null)
					_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

				return _unityPlayer;
			}
		}

		public AndroidJavaObject CurrentActivity
		{
			get
			{
				if (_activity == null)
					_activity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

				return _activity;
			}
		}

		private AndroidJavaObject ApplicationContext
		{
			get
			{
				if (_context == null)
					_context = CurrentActivity.Call<AndroidJavaObject>("getApplicationContext");

				return _context;
			}
		}

		public AndroidJavaClass Xlogin => _xlogin;

		public MainThreadExecutor MainThreadExecutor => _mainThreadExecutorInstance;

		public AndroidHelper()
		{
			CreateMainThreadExecutor();
			InitLogin();
		}

		private static void CreateMainThreadExecutor()
		{
			if (!_mainThreadExecutorInstance) 
				_mainThreadExecutorInstance = new GameObject("Android MainThreadExecutor").AddComponent<MainThreadExecutor>();
		}

		private void InitLogin()
		{
			if (_xlogin != null)
				return;

			try
			{
				_xlogin = new AndroidJavaClass("com.xsolla.android.login.XLogin");
				var socialConfig = new AndroidJavaObject(
					"com.xsolla.android.login.XLogin$SocialConfig",
					XsollaSettings.FacebookAppId,
					XsollaSettings.GoogleServerId,
					XsollaSettings.WeChatAppId,
					XsollaSettings.QQAppId);

				var loginConfigBuilder = new AndroidJavaObject("com.xsolla.android.login.LoginConfig$OauthBuilder");
				loginConfigBuilder.Call<AndroidJavaObject>("setSocialConfig", socialConfig);
				loginConfigBuilder.Call<AndroidJavaObject>("setProjectId", XsollaSettings.LoginId);
				loginConfigBuilder.Call<AndroidJavaObject>("setOauthClientId", XsollaSettings.OAuthClientId);

				var loginConfig = loginConfigBuilder.Call<AndroidJavaObject>("build");

				_xlogin.CallStatic("init", ApplicationContext, loginConfig);
			}
			catch (Exception e)
			{
				throw new AggregateException($"AndroidSDKSocialAuthHelper.Ctor: {e.Message}", e);
			}
		}
	}
}