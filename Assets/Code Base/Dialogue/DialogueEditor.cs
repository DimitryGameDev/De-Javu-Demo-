using System;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public  class DialogueEditor : EditorWindow
{
	private const string RootFolder = "Assets/Resources/";
	
	public string language;
	public string episodeFolder;
	public string fileName;
	
	public DialogueNode[] node;
	public DialogueNode[] dialogueEdit;
	
	private void CreateDialogue()
	{
		var newDialogue = CreateInstance<DialogueObj>();
		newDialogue.SetDialogue(node);
		
		string path = RootFolder + language +"/" + episodeFolder + "/" + fileName + ".asset";
		if (!Directory.Exists(RootFolder + language + "/" + episodeFolder))
		{
			Directory.CreateDirectory(RootFolder + language + "/" + episodeFolder);
		}
		if (File.Exists(path))
		{
			Debug.LogWarning("The dialogue already exists: " + path);
			return;
		}
		Debug.Log("Dialogue saved: " + path);
		AssetDatabase.CreateAsset(newDialogue, path);
		node = null;
	}
	
	private void EditDialogue(DialogueObj dialogue)
	{
		dialogue.SetDialogue(dialogueEdit);
		Debug.Log("Dialogue edit");
		dialogueEdit = null;
	}
	
	
	
#if UNITY_EDITOR
	
	[MenuItem("R.K.Help/Dialogue")]
	public static void Open()
	{
		// This method is called when the user selects the menu item in the Editor.
		EditorWindow wnd = GetWindow<DialogueEditor>();
		wnd.titleContent = new GUIContent("Dialogue");

		// Limit size of the window.
		wnd.minSize = new Vector2(450, 200);
		wnd.maxSize = new Vector2(1920, 720);
	}
	private VisualElement _root;
	private Box _createBox;
	private Box _editBox;
	private ScrollView _scrollView;
	private TextField _language ;
	private TextField _episode;
	private TextField _name;
	private ObjectField _editObject;
    private DialogueObj _currentDialogue;
    private Button _saveButton;
    
	public void CreateGUI()
	{
		_root = rootVisualElement;
		_createBox = new Box();
		_editBox = new Box();
		
	
		_root.Add(new Label());
		//Creation
		Label labelCreate = new Label("Dialogue Creator");
		_root.Add(labelCreate);
		_root.Add(_createBox);
		PathView();
		DialogueView();
		CreateButton(_createBox);
		

		_root.Add(new Label());
		//Editing
		Label editLabel = new Label("Dialogue Editor");
		_root.Add(editLabel);
		_root.Add(_editBox);
		
		_editObject = new ObjectField("Chose Dialogue");
		_editObject.objectType = typeof(DialogueObj);
		_editBox.Add(_editObject);
		
		Box buttonBox = new Box();
		_editBox.Add(buttonBox);
		buttonBox.style.flexDirection = FlexDirection.Row;
		EditButtons(buttonBox);
		EditDialogueView();
		
	}
	
	private void DialogueView()
	{
		var scrollView = new ScrollView(ScrollViewMode.Vertical);
		scrollView.Add(new Label());
		_createBox.Add(scrollView);
		var nodes = new IMGUIContainer(() =>
		{
			var serializedObject = new SerializedObject(this);
			var property = serializedObject.FindProperty(nameof(node));
			serializedObject.Update();
			EditorGUILayout.PropertyField(property);
			serializedObject.ApplyModifiedProperties();
		});
		scrollView.Add(nodes);
	}
	
	
	private void ClearEditBox()
	{
		_scrollView.Clear();
	}
	private void PathView()
	{ 
		Box pathBox = new Box();
		_createBox.Add(pathBox);
		_language = new TextField("Language");
		_episode = new TextField("Episode");
		_name = new TextField("Name");
		
		_language.SetValueWithoutNotify("Russian");
		_episode.SetValueWithoutNotify("Chapter_1");
		_name.SetValueWithoutNotify("Name");
		pathBox.Add(_language);
		pathBox.Add(_episode);
		pathBox.Add(_name);
	}
	
	private void CreateButton(VisualElement root)
	{
		Button button = new Button();
		button.name = "Create";
		button.text = "Create";
		button.clicked += Create;
		root.Add(button);
	}
	
	private void EditButtons(VisualElement root)
	{
		Button editButton = new Button();
		editButton.name = "Edit";
		editButton.text = "Edit";
		editButton.style.width = 100;
		editButton.clicked += Editor;
		root.Add(editButton);
		
		_saveButton = new Button();
		_saveButton.name = "Save";
		_saveButton.text = "Save";
		_saveButton.style.width = 100;
		_saveButton.clicked += SaveDialogue;
		_saveButton.visible = false;
		root.Add(_saveButton);
		
	}

	
	private void SaveDialogue()
	{
		EditDialogue(_currentDialogue);
		ClearEditBox();
		_saveButton.visible = false;
		_editObject.value = null;
	}

	private void EditDialogueView()
	{
		_scrollView = new ScrollView(ScrollViewMode.Vertical);
		_editBox.Add(new Label());
		_editBox.Add(_scrollView);
	}
	private void Editor()
	{
		_currentDialogue = _editObject.value as DialogueObj;
		if (_currentDialogue == null)
		{
			Debug.Log("No dialogue selected");
			return;
		}
		ClearEditBox();
		_saveButton.visible = true;
		
		dialogueEdit = _currentDialogue.Dialogue;
		var editNodes = new IMGUIContainer(() =>
		{
			var serializedObject = new SerializedObject(this);
			var property = serializedObject.FindProperty(nameof(dialogueEdit));
			serializedObject.Update();
			EditorGUILayout.PropertyField(property);
			serializedObject.ApplyModifiedProperties();
		});
		_scrollView.Add(new Label(_editObject.value.name));
		_scrollView.Add(editNodes);
		
	}
	
	private void Create()
	{
		if (_language.value == "" ) Debug.LogWarning("Please select a folder");
		if ( _episode.value == "")  Debug.LogWarning("Please select a episode folder");
		if (_name.value == "") Debug.LogWarning("Please select a file name");
		
		if(_language.value == "" || _episode.value == "" || _name.value == "") return;
		language = _language.value;
		episodeFolder = _episode.value;
		fileName = _name.value;
		CreateDialogue();
	}
	
#endif
}

[Serializable]
public class DialogueNode
{
	public NpcText[] npcText;
	public PlayerAnswer[] playerAnswer;
}
	
[Serializable]
public class PlayerAnswer
{
	[TextArea] public string text;
	public int toNode;
	public int value;
	public bool exit;
}

[Serializable]
public class NpcText
{
	[TextArea] public string text;
	public NpcEmotion emotion = NpcEmotion.Neutral;
	public Npc name = Npc.Hero;
}

public enum Npc
{
	Hero = 0,
	April = 1,
	Patient = 2
}

public enum NpcEmotion
{
	Neutral = 0,
	Happy = 1,
	Angry = 2,
	Sad = 3,
	Fear = 4
}