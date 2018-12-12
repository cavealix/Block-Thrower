using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class Recognition : MonoBehaviour {

	KeywordRecognizer keywordRecognizer;

	Dictionary<string,System.Action> keywords = new Dictionary<string, System.Action>();

	void Start()
	{
		keywords.Add("go", ()=>
		{
			GoCalled();
		});

		keywords.Add("start", ()=>
		{
			StartCalled();
		});

		keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
		//
		keywordRecognizer.OnPhraseRecognized += KeywordRecognizerOnPhraseRecognized;
		//initiate listening service
		keywordRecognizer.Start();

		print(keywords);
	}

	void KeywordRecognizerOnPhraseRecognized(PhraseRecognizedEventArgs args)
	{
		System.Action keywordAction;

		if(keywords.TryGetValue(args.text, out keywordAction))
		{
			//call relevant function
			keywordAction.Invoke();
		}
	}

	void GoCalled()
	{
		print("Go Called");
	}

	void StartCalled()
	{
		print("Start Called");
	}

}
