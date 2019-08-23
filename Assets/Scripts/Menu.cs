using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Android;

public class Menu : MonoBehaviour {
    //Скриншот и создание кнопки
    public Camera camera;
    public GameObject cam;
    Vector3 offset;
    GameObject model_for_screenshot;
    public Material m1;
    public Transform ui_content;
    public GameObject button, button2;
    public List<GameObject> buttons;
    public int resWidth = 1024;
    public int resHeight = 1024;

    private string path = ""; //из fileselector
    public bool windowOpen; //из fileselector
    bool importing_mesh; //если импортируется меш

    public string modelsFolder_path, screenShotsFolder_path, texturesFolder_path, savedModelsFile_path, screenShot_destination_path, savedTexturesFile_path, savedScreenShotFile_path; // сокращённый пути до файла в проекте
    public static string destinationFileName; // Имя выбранного файла. статическая из fileselector
    public string obj_destination_path; //Путь к модели внутри проекта
    public string png_destination_path; //Путь к текстуре внутри проекта
    public static bool closed; //Если проводник был закрыт. статическая из fileselector
    public static string mesh_path, texture_path; // технические для вызова в spawner
    public string mtl_destination_path;
    public string model_mtl_pathInFolder;
    [SerializeField]
    public List<string> models; //Массив с сылками на меши внутри проекта

    [SerializeField]
    public List<string> textures;

    [SerializeField]
    public List<string> screenshots;

    public List<GameObject> someshit, someshit2; //Для удаления кнопок импорта текстуры на материал и кнопок выбора диска

    public Material[] materials;
    public GameObject background2, background3, instruction_text, about_text, material_buttons_content, material_buttons_scroll, disk_directory_content, disk_scroll;
    public string mat_name;
    public GameObject batching_saving1, batching_saving2;
    //Android statement
    private AndroidJavaObject javaClass;
    public string sd_card_path, sd_card_path2;
    public static string absoluteFilePathInSystem;
    public static GameObject background2_2;

    // Use this for initialization
    void Start() {

        background2_2 = background2;
        //параша тута/
#if !UNITY_EDITOR && UNITY_ANDROID
        Screen.orientation = ScreenOrientation.LandscapeLeft;

         if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
        }
        else
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
        }
        else
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }

        javaClass = new AndroidJavaObject("com.pplosstudio.fileworklibrary.FileWork");
        javaClass.Call("CreateData");
        sd_card_path = javaClass.Call<string>("MainFolderPath");
		savedModelsFile_path = sd_card_path + "/models.txt";
		savedTexturesFile_path = sd_card_path + "/textures.txt";
		savedScreenShotFile_path = sd_card_path + "/screenshot.txt";
        sd_card_path2 = sd_card_path.Remove(sd_card_path.Length - 7, 7);

           foreach (string ln in File.ReadAllLines(savedModelsFile_path)) {
             models.Add(ln);
        }
        foreach (string ln in File.ReadAllLines(savedTexturesFile_path))
        {
           textures.Add(ln);
        }
        foreach (string ln2 in File.ReadAllLines(savedScreenShotFile_path))
         {
          screenshots.Add(ln2);
        }

#endif

#if UNITY_EDITOR
        /// СОздаёт папки, если их нет
        if (!File.Exists(Application.dataPath + "/Models")) {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Models");
        }
        if (!File.Exists(Application.dataPath + "/ScreenShots")) {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/ScreenShots");
        }
        if (!File.Exists(Application.dataPath + "/Textures")) {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Textures");
        }
        if (!File.Exists(Application.dataPath + "/models.txt")) {
            System.IO.File.Create(Application.dataPath + "/models.txt");
        }
        if (!File.Exists(Application.dataPath + "/textures.txt")) {
            System.IO.File.Create(Application.dataPath + "/textures.txt");
        }
        if (!File.Exists(Application.dataPath + "/screenshot.txt")) {
            System.IO.File.Create(Application.dataPath + "/screenshot.txt");
        }

        ///Сокращённый путь до папки в формате стринг
        modelsFolder_path = Application.dataPath + "/Models";
        screenShotsFolder_path = Application.dataPath + "/ScreenShots";
        texturesFolder_path = Application.dataPath + "/Textures";
        savedModelsFile_path = Application.dataPath + "/models.txt";
        savedTexturesFile_path = Application.dataPath + "/textures.txt";
        savedScreenShotFile_path = Application.dataPath + "/screenshot.txt";


        ///Чтение файла с сылками
        if (File.Exists(Application.dataPath + "/models.txt")) {
            using (StreamReader sr = new StreamReader(savedModelsFile_path)) {
                while (sr.EndOfStream == false) {
                    models.Add(sr.ReadLine());
                }
            }
        }
        if (File.Exists(Application.dataPath + "/textures.txt")) {
            using (StreamReader sr = new StreamReader(savedTexturesFile_path)) {
                while (sr.EndOfStream == false) {
                    textures.Add(sr.ReadLine());
                }
            }
        }
        if (File.Exists(Application.dataPath + "/screenshot.txt")) {
            using (StreamReader sr = new StreamReader(savedScreenShotFile_path)) {
                while (sr.EndOfStream == false) {
                    screenshots.Add(sr.ReadLine());
                }
            }
        }
#endif
        for (int i = 0; i < models.Count; i++) {
            var j = i;
            //GameObject	newButton = new GameObject() ;
            GameObject newButton = Instantiate(button, ui_content);
            newButton.GetComponent<Button>().onClick.AddListener(delegate { LoadModel(j); });
            newButton.transform.Find("Import").GetComponent<Button>().onClick.AddListener(delegate { TextureMaterial(j); });
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(File.ReadAllBytes(screenshots[i]));
            newButton.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, resWidth, resHeight), Vector2.zero);
            buttons.Add(newButton);
        }
        closed = false;
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKey(KeyCode.Escape))
        {
            // Debug.Log("Work");
            MENU();//метод, который выполняется при нажатии
        }

    }

    //след. диск.
    public void Import_Mesh() {
#if UNITY_EDITOR
        //  Disk(".obj");
        FileSelector.GetFile("D:/", GotFile, ".obj");
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
        FileSelector.GetFile(sd_card_path2, GotFile, ".obj");
        FonOn();
#endif
        windowOpen = true;
        importing_mesh = true;
    }

    //текстурирование материала. след Impotr_Texture
    public void TextureMaterial(int index)
    {

        model_mtl_pathInFolder = models[index].Remove(models[index].Length - 4) + ".mtl";

        if (File.Exists(model_mtl_pathInFolder) == true)
        {
            material_buttons_scroll.SetActive(true);
            materials = OBJLoader.LoadMTLFile(model_mtl_pathInFolder);

            foreach (Material n in materials)
            {
                Material t = n;
                GameObject newButton = Instantiate(button2, material_buttons_content.transform);
                newButton.GetComponentInChildren<Text>().text = n.name;
                newButton.GetComponent<Button>().onClick.AddListener(delegate {
                    Import_Texture(newButton.GetComponentInChildren<Text>().text);
                });
                someshit.Add(newButton);
            }
        }
    }

    //след. диск.
    public void Import_Texture(string nn)
    {
#if UNITY_EDITOR && !UNITY_ANDROID
        Disk(".png");
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
        FileSelector.GetFile(sd_card_path2, GotFile, ".png");
        FonOn();
#endif
        mat_name = nn;
        windowOpen = true;
        importing_mesh = false;
    }

    //Выбор диска см. GotFile
    public void Disk(string extension)
    {

        disk_scroll.SetActive(true);
        string[] drives = System.IO.Directory.GetLogicalDrives();
        foreach (string str in drives)
        {
            GameObject newButton = Instantiate(button2, disk_directory_content.transform);
            newButton.GetComponentInChildren<Text>().text = str;
            newButton.GetComponent<Button>().onClick.AddListener(delegate {
                FileSelector.GetFile(newButton.GetComponentInChildren<Text>().text, GotFile, extension);
               // FileSelector.GetFile(sd_card_path, GotFile, extension);
                FonOn();
            });
            someshit2.Add(newButton);
        }

    }

    // Получаем путь к файлу, записываем в .txt, дальше см. функции
    void GotFile(FileSelector.Status status, string path) {
        this.path = path;
        this.windowOpen = false;


        if (importing_mesh == true && closed == false) {
#if !UNITY_EDITOR && UNITY_ANDROID
            obj_destination_path = sd_card_path +"/"+ destinationFileName;
          //  t1.text = javaClass.Call<bool>("FileExists", obj_destination_path).ToString();
            if (javaClass.Call<bool>("FileExists", obj_destination_path) == false) {

                javaClass.Call("CopyFile", absoluteFilePathInSystem, obj_destination_path);
                models.Add(obj_destination_path);
                //запись в файл

                // foreach (string model in models) {
                //  javaClass.Call("WriteFile", savedModelsFile_path, model);
                //  }
                using (StreamWriter outputFile = new StreamWriter(savedModelsFile_path))
                {
                    foreach (string model in models)
                    {
                        outputFile.WriteLine(model);
                    }
                }
                Impor_mtl();
                SCREENSHOT();
            }
            else{
                javaClass.Call("CopyFile", absoluteFilePathInSystem, obj_destination_path);
            }
#endif

            obj_destination_path = System.IO.Path.Combine(modelsFolder_path, destinationFileName);
            /// чтобы не заполнял одим и тем же
            if (File.Exists(obj_destination_path) == false) {
                System.IO.File.Copy(path, obj_destination_path, true);
                models.Add(obj_destination_path);
                ///Записывает в файл
                using (StreamWriter outputFile = new StreamWriter(savedModelsFile_path)) {
                    foreach (string model in models) {
                        outputFile.WriteLine(model);
                    }
                }
                Impor_mtl();
                /// Чтобы не плодить скриншоты
                SCREENSHOT();
            } else {
                System.IO.File.Copy(path, obj_destination_path, true);
                Impor_mtl();
            }
        }

        //Импорт текстуры
        if (importing_mesh == false && closed == false) {

#if !UNITY_EDITOR && UNITY_ANDROID
            png_destination_path = sd_card_path +"/"+ destinationFileName;
            RebuiltMTL (png_destination_path);
            if (javaClass.Call<bool>("FileExists", png_destination_path) == false) {

                javaClass.Call("CopyFile", absoluteFilePathInSystem, png_destination_path);
                textures.Add(png_destination_path);
               
                using (StreamWriter outputFile = new StreamWriter(savedTexturesFile_path))
                {
                   foreach(string texture in textures){
						outputFile.WriteLine (texture);
					}
                }

            }
#endif

            png_destination_path = System.IO.Path.Combine(texturesFolder_path, destinationFileName);
            RebuiltMTL(png_destination_path);

            if (File.Exists(png_destination_path) == false) {
                System.IO.File.Copy(path, png_destination_path, true);
                textures.Add(png_destination_path);

                using (StreamWriter outputFile = new StreamWriter(savedTexturesFile_path)) {
                    foreach (string texture in textures) {
                        outputFile.WriteLine(texture);
                    }
                }
            }
        }
        //	background3.SetActive (false);
        background2.SetActive(false);
    }

    //Наследуется из GotFile. перестраивает .mtl файл и вписывает путь к текстуре в папке приложения.
    public void RebuiltMTL(string mm) {

        string l;
        string[] cmps;
        List<string> text = new List<string>();
        bool reached = false;

        foreach (string ln in File.ReadAllLines(model_mtl_pathInFolder)) {
            l = ln.Trim().Replace("  ", " ");
            cmps = l.Split(' ');
            string data = l.Remove(0, l.IndexOf(' ') + 1);
            if (l == "newmtl " + mat_name) {
                reached = true;
            }
            if (cmps[0] == "map_Kd" && reached == true) {
                l = "map_Kd " + mm;
                reached = false;
            }
            text.Add(l);
        }
        File.WriteAllLines(model_mtl_pathInFolder, text.ToArray());
    }

    //наследуется из GotFile, ипорта меша. Копирует .mtl в папку приложения
    public void Impor_mtl()
    {
        string mtl_path = null;
        string mtl_destination_path = null;
#if !UNITY_EDITOR && UNITY_ANDROID
         mtl_path = absoluteFilePathInSystem.Remove(absoluteFilePathInSystem.Length - 4) + ".mtl";
         mtl_destination_path = obj_destination_path.Remove(obj_destination_path.Length - 4) + ".mtl";
        if (javaClass.Call<bool>("FileExists", mtl_path) == true) {
            javaClass.Call("CopyFile", mtl_path, mtl_destination_path);
        }
#endif
#if UNITY_EDITOR
        mtl_path = path.Remove(path.Length - 4) + ".mtl";
        mtl_destination_path = obj_destination_path.Remove(obj_destination_path.Length - 4) + ".mtl";
        if (File.Exists(mtl_path)) {
            System.IO.File.Copy(mtl_path, mtl_destination_path, true);
        }
#endif
    }

    //наследуется из GotFile. Создает скриншот и сохнаяет его в папку приложения.                      
    public void SCREENSHOT() {
        //	batching_saving1.SetActive (true);
        batching_saving2.SetActive(true);
        Renderer model_renderer;
        model_for_screenshot = new GameObject(destinationFileName);

        ///Сборка модели и перемещение камеры к ней
        Mesh holderMesh = new Mesh();
        Importer newMesh = new Importer();

        holderMesh = newMesh.ImportFile(obj_destination_path);

        model_for_screenshot.gameObject.AddComponent<MeshRenderer>();
        model_for_screenshot.gameObject.AddComponent<MeshFilter>();
        model_for_screenshot.GetComponent<MeshFilter>().mesh = holderMesh;
        model_for_screenshot.GetComponent<Renderer>().material = m1;
        model_renderer = model_for_screenshot.GetComponent<Renderer>();

        /// Отвечает за тдаление камеры в зависимости от размера модели
        if (model_renderer.bounds.size.y > 1) {
            offset = new Vector3(offset.x, offset.y, -Mathf.Abs(model_renderer.bounds.size.y));
        } else {
            offset = new Vector3(offset.x, offset.y, -Mathf.Abs(model_renderer.bounds.size.y) * 1.5f);
        }
        cam.transform.position = model_renderer.bounds.center + offset + new Vector3(model_renderer.bounds.size.x * 1.7f, model_renderer.bounds.size.y * 0.5f, 0);
        cam.transform.LookAt(model_renderer.bounds.center);

        //Создание и сохранка скриншота
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string cut_file_name = destinationFileName.Remove(destinationFileName.Length - 4) + ".png";
        screenShot_destination_path = System.IO.Path.Combine(screenShotsFolder_path, cut_file_name);

#if !UNITY_EDITOR && UNITY_ANDROID
        screenShot_destination_path = sd_card_path +"/"+ cut_file_name;
        javaClass.Call("WriteBytes", screenShot_destination_path, bytes);
#endif
#if UNITY_EDITOR
        System.IO.File.WriteAllBytes(screenShot_destination_path, bytes);
#endif
        screenShot.LoadImage(bytes);
        ///Создание кнопки
        int j = models.Count - 1;
        //GameObject newButton = new GameObject ();
        GameObject newButton = Instantiate(button, ui_content);
        newButton.GetComponent<Button>().onClick.AddListener(delegate { LoadModel(j); });
        newButton.transform.Find("Import").GetComponent<Button>().onClick.AddListener(delegate { TextureMaterial(j); });
        newButton.GetComponent<Image>().sprite = Sprite.Create(screenShot, new Rect(0, 0, resWidth, resHeight), Vector2.zero);

        Destroy(model_for_screenshot);
        screenshots.Add(screenShot_destination_path);
        //	batching_saving1.SetActive (false);
        batching_saving2.SetActive(false);

        using (StreamWriter outputFile = new StreamWriter(savedScreenShotFile_path)) {
            foreach (string screenshot in screenshots) {
                outputFile.WriteLine(screenshot); }
        }
    }

    public void LoadModel(int pathNumber) {

        mesh_path = models[pathNumber];
        START();
    }

    public void Instruction() {
        background3.SetActive(true);
        instruction_text.SetActive(true);

    }

    public void About() {
        background3.SetActive(true);
        about_text.SetActive(true);

    }

    public void MENU() {
        foreach (GameObject z in someshit) {
            Destroy(z);
        }
        foreach (GameObject l in someshit2) {
            Destroy(l);
        }
        background3.SetActive(false);
        material_buttons_scroll.SetActive(false);

        instruction_text.SetActive(false);
        about_text.SetActive(false);
        disk_scroll.SetActive(false);
    }

    public static void FonOn()
    {
        background2_2.SetActive(true);
    }

    public static void FonOff() {
        background2_2.SetActive(false);
    }

    public void OpenWeb() {
        Application.OpenURL("https://vk.com/pplos");
    }
 
	public void START(){
		SceneManager.LoadScene (1);
	}

}	