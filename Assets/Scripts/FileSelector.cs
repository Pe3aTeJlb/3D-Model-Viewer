#region UsingStatements

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#endregion


/// 	- Allows the user to select a file with the option to only allow certain file types.
/// 	- The window will be drawn with designated styles, or use defaults stored in GUI.skin.
/// 	- Use static 'GetFile' method to open self-drawing FileSelector window.
/// 	- Instances created this way will destroy on close by default. This behaviour can be changed using the 'destroyOneClose' variable.

public class FileSelector : MonoBehaviour 
{
    public  GameObject a;
    public float mnoshitel = 0.15f;
    public static int text_scale = 1;
  
    private void Start()
    {
        a = GameObject.Find("Main Camera");
        if (Screen.width < 1920) {
            text_scale = 14;
      
        }
        if (Screen.width>=1920) {
            text_scale = 36;
           
        }
        if (Screen.width >= 2560) {
            text_scale = 60;
           
        }
     

    }

    #region PublicEnumerations

    /// <summary>
    /// 	- Describes whether or not the file selection was successful.
    /// </summary>
    public enum Status
	{
		Successful, //Used if we successfully got a file
		Cancelled, //Used if the 'Cancel' button is clicked
		Failed, //Used if the Close() method is called while the window is open
		Destroyed, //Used if the instance is destroyed while the window is open
	}
	
	#endregion
	
	#region PublicFields
	

	/// 	- What file type we are searching for.
	public string extension = ".*";
	

	/// 	- The function to be called when the window closes.
	public SelectFileFunction Callback;
	

	/// 	- Gets or sets a value indicating whether the window is open.
	public bool open { get; private set; }
	

	/// 	- If set to true, the window will be centered every OnGUI call.
	public bool center = true;
	

	/// 	- If set to true, this instance will destroy itself when the window closes.
	public bool destroyOnClose = false;
	

	/// 	- The window dimensions.
	public Rect windowDimensions = new Rect(0,0, Screen.width,Screen.height);
	
	#endregion
	
	#region PublicStaticParameters
	

	/// 	- The window style.
	private static GUIStyle _windowStyle;
	public static GUIStyle windowStyle
	{
		get
		{
			if(_windowStyle == null) _windowStyle = GUI.skin.window; _windowStyle.fontSize = text_scale;
			return _windowStyle;
		}
		set { _windowStyle = value ?? GUI.skin.window; }
	}

	/// 	- The style for buttons in the window.
	private static GUIStyle _buttonStyle;
	public static GUIStyle buttonStyle
	{
		get
		{
			if(_buttonStyle == null) _buttonStyle = GUI.skin.button; _buttonStyle.fontSize = text_scale;
            return _buttonStyle;
		}
		set { _buttonStyle = value ?? GUI.skin.button; }
	}
	

	/// 	- The style for labels in the window.
	private static GUIStyle _labelStyle;
	public static GUIStyle labelStyle
	{
		get 
		{ 
			if(_labelStyle == null) _labelStyle = GUI.skin.label; _labelStyle.fontSize = text_scale;

            return _labelStyle;
		}
		set { _labelStyle = value ?? GUI.skin.label; }
	}
	

	/// 	- The style for titles in the window.
	private static GUIStyle _titleStyle;
	public static GUIStyle titleStyle
	{
		get 
		{ 
			if(_titleStyle == null) _titleStyle = GUI.skin.label; _titleStyle.fontSize = text_scale;
			return _titleStyle;
		}
		set { _titleStyle = value ?? GUI.skin.label; }
	}


	/// 	- The style for text fields in the window.
	private static GUIStyle _textFieldStyle;
	public static GUIStyle textFieldStyle
	{
		get 
		{ 
			if(_textFieldStyle == null) _textFieldStyle = GUI.skin.textField; _textFieldStyle.fontSize = text_scale;
			return _textFieldStyle;
		}
		set { _textFieldStyle = value ?? GUI.skin.textField; }
	}

	#endregion
	
	#region PublicDelegates
	

	/// 	- Delegate for the Callback function.
	public delegate void SelectFileFunction(Status status, string path);
	
	#endregion
	
	#region PrivateFields
	
	private string path = "";
	private string file = "";
	
	private int selection;
	private Vector2 scrollPosition;
	
	#endregion
	
	#region PrivateStaticFields
		
	private static GameObject updater;
	
	#endregion

	#region MonoBehaviourFunctions
	
	private void OnGUI()
	{	
		if(open)
		{
			if(center) windowDimensions.center = new Vector2(Screen.width*0.5f, Screen.height*0.5f);
			GUI.Window(0, windowDimensions, DrawFileSelector, "Select a "+extension+" File");
		}
	}
	
	private void OnDestroy()
	{
		if(open && Callback != null) Callback(Status.Destroyed, "");
	}
	
	#endregion
	
	#region PublicFunctions
	

	/// 	- Opens this instance.
	public void Open()
	{    
		open = true;
	}
	

	/// 	- Opens this instance to the specified startingDirectory.
	/// </summary>
	/// <param name='startingDirectory'>
	/// 	- The directory to start in.
	/// </param>
	public void Open(string startingDirectory)
	{

		path = startingDirectory;
		open = true;
	}
	

	/// 	- Closes this instance.
	public void Close()
	{
		if(open && Callback != null) Callback(Status.Failed, "");
		open = false;
		if(destroyOnClose) Destroy(this);
	}
	
	#endregion
	
	#region PublicStaticFunctions
	

	/// 	- Gets a file with a specified extension.

	/// <param name='startingDirectory'>
	/// 	- The directory to start the search in.
	/// </param>
	/// <param name='Callback'>
	/// 	- The function to call when the file has been selected.
	/// </param>
	/// <param name='extension'>
	/// 	- The extension of the desired file type.
	/// </param>
	public static void GetFile(string startingDirectory, SelectFileFunction Callback = null, string extension = ".*")
	{
		if(updater == null) 
		{
			updater = new GameObject("Select File");
			updater.AddComponent<FS_Cleanup>();
			updater.hideFlags = HideFlags.HideInHierarchy;
		}
		
		FileSelector instance = updater.AddComponent<FileSelector>();
		
		instance.Callback = Callback;
		instance.extension = extension;
		instance.path = startingDirectory;
		instance.destroyOnClose = true;
		instance.open = true;
	}
	
	/// <summary>
	/// 	- Gets a file with a specified extension.
	/// </summary>
	/// <param name='Callback'>
	/// 	- The function to call when the file has been selected.
	/// </param>
	/// <param name='extension'>
	/// 	- The extension of the desired file type.
	/// </param>
	public static void GetFile(SelectFileFunction Callback = null, string extension = ".*")
	{
		GetFile(Application.dataPath, Callback, extension);
		//GetFile("/", Callback, extension);
	}
	
	#endregion
	
	#region PrivateFunctions


	private void DrawFileSelector(int ID)
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		
		//Path Buttons
		GUILayout.Label("Path : ", titleStyle);
		
		GUILayout.BeginHorizontal();
		
			string[] parentDirectories = GetParentDirectories(path);
			
			float maximumWidth = windowDimensions.width - 30;
			float totalWidth = 0;
			float width = 0;
			float spacingWidth = 11; //public variable?
			float arrowWidth = labelStyle.CalcSize(new GUIContent(" > ")).x;
			float arrowSpacing = arrowWidth + spacingWidth;
		
			for(int i = 0; i < parentDirectories.Length; i++){
				width = buttonStyle.CalcSize(new GUIContent(parentDirectories[i])).x;
				
				totalWidth += (width + spacingWidth);
				if(totalWidth > maximumWidth)
				{
					totalWidth = width;
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
				}
            

                if (GUILayout.Button(parentDirectories[i], buttonStyle, GUILayout.Width(width), GUILayout.Height(buttonStyle.CalcSize(new GUIContent(parentDirectories[i])).y*1.35f)))
				{
					path = GetParentDirectories(path, true)[i];
					break;
				}
	
				GUILayout.Label(" > ", labelStyle, GUILayout.Width(arrowWidth));
				totalWidth += (arrowSpacing);
			
				if(totalWidth > maximumWidth)
				{
					totalWidth = 0;
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
				}
			}
			
			string currentDirectory = new FileInfo(path).Name;
        //////////////////////////////////////////////////////////////////////
			width = buttonStyle.CalcSize(new GUIContent(currentDirectory)).x;
        ////////////////////////////////////////////////////////////////////////////
			GUILayout.Label(currentDirectory, buttonStyle, GUILayout.Width(width), GUILayout.Height(buttonStyle.CalcSize(new GUIContent(currentDirectory)).y*1.35f));
		
		GUILayout.EndHorizontal();
		
		GUILayout.Space(1);
		
		//Directory Buttons
		GUILayout.Label("Directories : ", titleStyle);
		
		GUILayout.BeginHorizontal();
		
			string[] childDirectories = GetChildDirectories(path);
			float buttonWidth = (windowDimensions.width - 80) / 4f;
			
			for(int i = 0; i < childDirectories.Length; i++){
				if(GUILayout.Button(childDirectories[i], buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth * mnoshitel)))
				{
					path = GetChildDirectories(path, true)[i];
					break;
				}
				
				GUILayout.Space(10);
				
				if((i % 4) == 3)
				{
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
				}
			}
		
		GUILayout.EndHorizontal();
		
		GUILayout.Space(1);
		
		//File Buttons
		GUILayout.Label("Files : ", titleStyle);
		
		GUILayout.BeginHorizontal();
     
            string[] files = GetFiles(path, extension: extension);
            for (int i = 0; i < files.Length; i++)
            {
                if (GUILayout.Button(files[i], buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth * mnoshitel)))
                {
                    file = files[i];
                    break;
                }

                GUILayout.Space(10);

                if ((i % 4) == 3)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
        

     

		GUILayout.EndHorizontal();
		
		GUILayout.Space(1);

        //////////////////////////////////////
        if (extension == ".png")
        {
            GUILayout.Label(" ", titleStyle);

            GUILayout.BeginHorizontal();

            string[] files2 = GetFiles(path, false, ".jpg");
            for (int i = 0; i < files2.Length; i++)
            {
                if (GUILayout.Button(files2[i], buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth * mnoshitel)))
                {
                    file = files2[i];
                    break;
                }

                GUILayout.Space(10);

                if ((i % 4) == 3)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(1);
        }
        /////////////


        //Returning values
     //   GUILayout.BeginHorizontal();
		//	GUILayout.Label("Selected File : ", titleStyle, GUILayout.Width(titleStyle.CalcSize(new GUIContent("Selected File : ")).x));
		//	file = GUILayout.TextField(file);
		//GUILayout.EndHorizontal();

        GUILayout.Space(1);
        //File.Exists(path+@"\"+file) &&
        if (extension == ".obj")
        {
            if (Path.GetExtension(path + @"\" + file) == extension)
            {
                if (GUILayout.Button("Select", GUILayout.Height(buttonWidth * mnoshitel)))
                {
                    a.GetComponent<Menu>().MENU();
                    Menu.destinationFileName = file;
                    Menu.absoluteFilePathInSystem = path + @"/" + file;
                    open = false;
                    Menu.FonOff();
                    if (Callback != null) Callback(Status.Successful, path + @"\" + file);
                    open = false;

                    if (destroyOnClose) Destroy(this);


                }
                GUILayout.Space(5);
            }

            //if (GUILayout.Button("Cancel", GUILayout.Height(buttonWidth * mnoshitel)))
            //{
                //a.GetComponent<Menu>().MENU();
                //Menu.closed = true;
                //if (Callback != null) Callback(Status.Cancelled, "");
                //open = false;
              //  if (destroyOnClose) Destroy(this);


            //}

            GUILayout.EndScrollView();
        }

        if (extension == ".png")
        {
            if (Path.GetExtension(path + @"\" + file) == extension || Path.GetExtension(path + @"\" + file) == ".jpg")
            {
                if (GUILayout.Button("Select", GUILayout.Height(buttonWidth * mnoshitel)))
                {
                    a.GetComponent<Menu>().MENU();
                    Menu.destinationFileName = file;
                    Menu.absoluteFilePathInSystem = path + @"/" + file;
                    open = false;
                    Menu.FonOff();
                    if (Callback != null) Callback(Status.Successful, path + @"\" + file);
                    open = false;

                    if (destroyOnClose) Destroy(this);


                }
                GUILayout.Space(5);
            }

            //if (GUILayout.Button("Cancel", GUILayout.Height(buttonWidth * mnoshitel)))
            //{
               // a.GetComponent<Menu>().MENU();
              //  Menu.closed = true;
             //   if (Callback != null) Callback(Status.Cancelled, "");
            //    open = false;
           //     if (destroyOnClose) Destroy(this);


          //  }

            GUILayout.EndScrollView();
        }


    }
	
	#endregion
	





















	#region PrivateStaticFunctions
	
	private static string[] GetParentDirectories(string filePath, bool includePaths = false)
	{
		List<string> parents = new List<string>();
		FileInfo fileInfo;
		
		while(true){
			try{
				fileInfo = new FileInfo(filePath);
				if(!includePaths) parents.Add(fileInfo.Directory.Name);
				else parents.Add(fileInfo.Directory.FullName);
				
				filePath = fileInfo.Directory.FullName;
			}
			
			catch{ break; }
		}
		
		parents.Reverse();
		return parents.ToArray();		
	}
	
	private static string[] GetChildDirectories(string directoryPath, bool includePaths = false)
	{

       // return Directory.GetDirectories(directoryPath);

        DirectoryInfo directory;
        directory = new FileInfo(directoryPath).Directory;
        Debug.Log(directoryPath);
        if (Directory.Exists(directoryPath))
            directory = new DirectoryInfo(directoryPath);
        else
        {
            try
            {
               
                directory = new FileInfo(directoryPath).Directory;
               
            }
            catch
            {
                return new string[0];
            }
        }

        List<string> children = new List<string>();
    
     try
        {
            DirectoryInfo[] directories = directory.GetDirectories();
    
            foreach (DirectoryInfo childDir in directories)
            {
                if (!includePaths)    children.Add(childDir.Name);
                else children.Add(childDir.FullName);
            }
        }

        catch
        {
            children = new List<string>();
        }
      
        return children.ToArray();

    }
	
	private static string[] GetFiles(string directoryPath, bool includePaths = false, string extension = ".*")
	{

        //return Directory.GetFiles(directoryPath);


        DirectoryInfo directory;

        if(Directory.Exists(directoryPath))
          directory = new DirectoryInfo(directoryPath);
        else
        {
        	try{ directory = new FileInfo(directoryPath).Directory;	}
        	catch{ return new string[0]; }
        }

        List<string> files = new List<string>();

        try{
        FileInfo[] fileInfos = directory.GetFiles("*"+extension);

        foreach(FileInfo fileInfo in fileInfos){
        if(!includePaths) files.Add(fileInfo.Name);

        else files.Add(fileInfo.FullName);	
        	}
        }

        catch{
        files = new List<string>();
        }

        return files.ToArray();
    }
	
	#endregion















	public void Update(){
        
            if (Input.GetKey(KeyCode.Escape))
            {
            Menu.closed = true;
               if (Callback != null) Callback(Status.Cancelled, "");
                open = false;
                 if (destroyOnClose) Destroy(this);
        }

    }
}


/// 	- A small class used to clean up the updater object when we change scenes or the application quits.
public class FS_Cleanup : MonoBehaviour
{
	void OnApplicationQuit()
	{
		Destroy(gameObject);
	}
	
	void OnLevelLoaded()
	{
		Destroy(gameObject);
	}
}


