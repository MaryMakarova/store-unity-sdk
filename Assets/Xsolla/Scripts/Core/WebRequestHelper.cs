﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public class WebRequestHelper : MonoBehaviour
	{
		static WebRequestHelper _instance;
		
		public static WebRequestHelper Instance
		{
			get
			{
				if (_instance == null)
				{
					Init();
				}

				return _instance;
			}
		}
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		internal static void Init()
		{
			if (ReferenceEquals(_instance, null))
			{
				var instances = FindObjectsOfType<WebRequestHelper>();

				if (instances.Length > 1)
				{
					Debug.LogError(typeof(WebRequestHelper) + " Something went really wrong " +
									" - there should never be more than 1 " + typeof(WebRequestHelper) +
									" Reopening the scene might fix it.");
				}
				else if (instances.Length == 0)
				{
					var singleton = new GameObject {hideFlags = HideFlags.HideAndDontSave};
					_instance = singleton.AddComponent<WebRequestHelper>();
					singleton.name = typeof(WebRequestHelper).ToString();

					DontDestroyOnLoad(singleton);

					Debug.Log("[Singleton] An _instance of " + typeof(WebRequestHelper) +
								" is needed in the scene, so '" + singleton.name +
								"' was created with DontDestroyOnLoad.");
				}
				else
				{
					Debug.Log("[Singleton] Using _instance already created: " + _instance.gameObject.name);
				}
			}
		}

		// Prevent accidental WebRequestHelper instantiation
		WebRequestHelper()
		{
		}
		
		public void PostRequest<T>(string url, WWWForm form, WebRequestHeader requestHeader, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			StartCoroutine(PostRequestCor<T>(url, form, requestHeader, onComplete, onError, errorsToCheck));
		}
		
		public void PostRequest<T>(string url, string jsonData, WWWForm form, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			StartCoroutine(PostRequestCor<T>(url, jsonData, form, requestHeaders, onComplete, onError, errorsToCheck));
		}

		public void GetRequest<T>(string url, WebRequestHeader requestHeader = null, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			StartCoroutine(GetRequestCor<T>(url, requestHeader, onComplete, onError, errorsToCheck));
		}

		public void PutRequest(string url, string jsonData, WebRequestHeader requestHeader, WebRequestHeader contentHeader = null, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			StartCoroutine(PutRequestCor(url, jsonData,requestHeader, contentHeader, onComplete, onError, errorsToCheck));
		}

		public void DeleteRequest(string url, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			StartCoroutine(DeleteRequestCor(url, requestHeader, onComplete, onError, errorsToCheck));
		}

		IEnumerator PostRequestCor<T>(string url, WWWForm form, WebRequestHeader requestHeader, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			var webRequest = UnityWebRequest.Post(url, form);
			
			webRequest.SetRequestHeader(requestHeader.Name, requestHeader.Value);

#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif

			ProcessRequest(webRequest, onComplete, onError, errorsToCheck);
		}
		
		IEnumerator PostRequestCor<T>(string url, string jsonData, WWWForm form, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			var webRequest = UnityWebRequest.Post(url, form);

			foreach (var requestHeader in requestHeaders)
			{
				webRequest.SetRequestHeader(requestHeader.Name, requestHeader.Value);
			}
			
			if (!string.IsNullOrEmpty(jsonData))
			{
				webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
			}

#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif

			ProcessRequest(webRequest, onComplete, onError, errorsToCheck);
		}

		IEnumerator GetRequestCor<T>(string url, WebRequestHeader requestHeader = null, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			var webRequest = UnityWebRequest.Get(url);

			if (requestHeader != null)
			{
				webRequest.SetRequestHeader(requestHeader.Name, requestHeader.Value);
			}

#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif

			ProcessRequest(webRequest, onComplete, onError, errorsToCheck);
		}

		IEnumerator PutRequestCor(string url, string jsonData, WebRequestHeader authHeader, WebRequestHeader contentHeader = null, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			var webRequest = new UnityWebRequest(url, "PUT");

			webRequest.downloadHandler = new DownloadHandlerBuffer();
			
			if (!string.IsNullOrEmpty(jsonData))
			{
				webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
			}
			
			webRequest.SetRequestHeader(authHeader.Name, authHeader.Value);

			if (contentHeader != null)
			{
				webRequest.SetRequestHeader(contentHeader.Name, contentHeader.Value);
			}

#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif

			ProcessRequest(webRequest, onComplete, onError, errorsToCheck);
		}
		
		IEnumerator DeleteRequestCor(string url, WebRequestHeader authHeader, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			var webRequest = UnityWebRequest.Delete(url);
			
			webRequest.SetRequestHeader(authHeader.Name, authHeader.Value);
			
			webRequest.downloadHandler = new DownloadHandlerBuffer();
			
#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif

			ProcessRequest(webRequest, onComplete, onError, errorsToCheck);
		}

		void ProcessRequest(UnityWebRequest webRequest, Action onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck)
		{
			if (webRequest.isNetworkError)
			{
				TriggerOnError(onError, Error.NetworkError);
			}
			else
			{
				print(webRequest.downloadHandler.text);
				var error = CheckForErrors(webRequest.downloadHandler.text, errorsToCheck);
				if (error == null)
				{
					if (onComplete != null)
					{
						onComplete();
					}
				}
				else
				{
					TriggerOnError(onError, error);
				}
			}
		}

		void ProcessRequest<T>(UnityWebRequest webRequest, Action<T> onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck) where T : class
		{
			if (webRequest.isNetworkError)
			{
				TriggerOnError(onError, Error.NetworkError);
			}
			else
			{
				var response = webRequest.downloadHandler.text;
				print(response);

				var error = CheckForErrors(response, errorsToCheck);
				if (error == null)
				{
					var data = ParseUtils.FromJson<T>(response);
					if (data != null)
					{
						if (onComplete != null)
						{
							onComplete(data);
						}
					}
					else
					{
						TriggerOnError(onError, Error.UnknownError);
					}
				}
				else
				{
					TriggerOnError(onError, error);
				}
			}
		}
		
		Error CheckForErrors(string json, Dictionary<string, ErrorType> errorsToCheck)
		{
			var error = ParseUtils.ParseError(json);
			if (error != null && !string.IsNullOrEmpty(error.statusCode))
			{
				if (errorsToCheck != null && errorsToCheck.ContainsKey(error.statusCode))
				{
					error.ErrorType = errorsToCheck[error.statusCode];
					return error;
				}

				if (Error.GeneralErrors.ContainsKey(error.statusCode))
				{
					error.ErrorType = Error.GeneralErrors[error.statusCode];
					return error;
				}

				return Error.UnknownError;
			}

			return null;
		}
		void TriggerOnError(Action<Error> onError, Error error)
		{
			if (onError != null)
			{
				onError(error);
			}
		}
		
	}
}

