using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using TMPro;
using UnityEngine.UI;

//Para la realizacion de la tabla utlice la libreria "LitJson" ya que me permite acceder a los datos del archivo JSON de manera mas customizable
//Creando un string y modificandola desde el editor, puedo acceder directamente a los valores que estoy buscando sin la necesidad de tener predifinidas las variables a utilizar

//El primer paso para desarrollar la prueba fue desglozar el problema planteado, en multiples tareas pequeñas
//Primero fue definir como se crearia la interfaz, en este caso decidi separar los datos en cada columna
//Acceder a los "ID" y mostrar todo eso en una sola columna y asi con el resto de datos

//Una vez definido esto, lo siguiente seria definir el como se accederia a los datos del archivo JSON
//Primero fue crear una clase que contendria los datos de cada columna, con los datos Header y una lista con su contenido correspondiente
//Con esto podría crear una lista de tipo "DataJson" (clase que contiene la informacion de cada columna) y crear columnas en base a la cantidad de objeto que contendria esa lista

//Para la recarga de datos tuve bastantes dilemas de como construirla pero habiendo tantas posibilidades de cambios en la data, decidi que la mas fácil y con menor probabilidades de errores
//era simplemente crear nuevamente la tabla desde 0, permitiendo facilmente la modificacion del archivo JSON ya sea agregando columnas o contenido a cada objeto de "Data"

public class ReadData : MonoBehaviour
{
    string fileName = "JsonChallenge.json";

    string path = " ";

    string jsonString = " ";

    JsonData itemData = new JsonData();

    [Header("Keys para acceder a los datos del archivo JSON")]
    [SerializeField] string titleKey = " ";
    [SerializeField] string headerKey = " ";
    [SerializeField] string dataKey = " ";

    [Header("Lista del contenido")]
    [SerializeField] List<DataJson> data = new List<DataJson>();

    [Header("UI")]
    [SerializeField] TextMeshProUGUI titleText = null;
    [SerializeField] GameObject Columns = null;

    private void Awake()
    {
        try {
        //ubicacion del archivo JSON
        path = Application.streamingAssetsPath + "/" + fileName;

        //Se guarda todo en una variable string 
        jsonString = File.ReadAllText(path);

        //Se convierte en un objeto de tipo JsonData para acceder a cada componente del archivo JSON
        itemData = JsonMapper.ToObject(jsonString);

        //Se asignan los valores del archivo Json a un objeto de "DataJson"
        ReadJson(itemData);

        //Creamos la UI
        CreateColumns();

        //Asignamos el titulo de la tabla
        string title = itemData[titleKey].ToString();
        titleText.text = title;
        }

        catch {
            titleText.text = "There is no JSON File at " + Application.streamingAssetsPath.ToString();
        }
    }

    //En este metodo se llama multiples veces al metodo de mas abajo para leer todo el contenido del archivo JSON
    void ReadJson(JsonData item) {

        for (int i = 0; i < item[headerKey].Count; i++)
        {
            Read(i, item);
        }
    }

    //En este metodo se realiza la lectura del archivo JSON
    //primero creamos un objeto te tipo "DataJson" (el cual contiene un Header y una lista de strings que corresponde al contenido)
    //ContentList es donde almacenaremos el contenido de cada columna para despues asignarlo a nuestro objeto DataJson y añadirlo a nuestra lista final

    void Read(int index, JsonData item) {
        DataJson d = new DataJson();
        string content;
        List<string> contentList = new List<string>();

        d.header = item[headerKey][index].ToString();

        for (int i = 0; i < item[dataKey].Count; i++)
        {
            content = item[dataKey][i][index]?.ToString();
            contentList.Add(content);
        }
        d.content = contentList;
        data.Add(d);
    }

    //Esto esta dividido en 2 metodos
    //CreateUI es para crear el contenido de cada columna, por ejemplo, primero creamos todo el contenido de "ID", luego de "Name" y asi
    //CreateColumns es para crear el objeto que contendra cada una de las columnas

    void CreateColumns() {
        for (int i = 0; i < data.Count; i++)
        {
            GameObject column = new GameObject();
            column.transform.SetParent(Columns.transform);
            column.AddComponent<VerticalLayoutGroup>();
            column.GetComponent<VerticalLayoutGroup>().spacing = 30;
            CreateUI(i, column.transform);
        }
    }

    //Creamos el Header y le asignamos tanto su texto como su estilo de fuente
    //Luego creamos el contenido de esa columna, recorriendo una lista de strings previamente creada y con el contenido del JSON

    void CreateUI(int index, Transform parent) {

        string header = data[index].header;

        List<string> content = data[index].content;

        GameObject headerObj = new GameObject();
        headerObj.transform.SetParent(parent);
        headerObj.AddComponent<TextMeshProUGUI>();
        headerObj.name = "Header";
        TextMeshProUGUI tmpHeader = headerObj.GetComponent<TextMeshProUGUI>();
        tmpHeader.text = header;
        tmpHeader.fontStyle = FontStyles.Bold;
        tmpHeader.enableAutoSizing = true;

        foreach (string c in content) {
            GameObject contentObj = new GameObject();
            contentObj.transform.SetParent(parent);
            contentObj.AddComponent<TextMeshProUGUI>();
            contentObj.name = "Content";
            TextMeshProUGUI tmpContent = contentObj.GetComponent<TextMeshProUGUI>();
            tmpContent.text = c;
            tmpContent.enableAutoSizing = true;
        }
    }


    //Para recargar el archivo Json repetimos el proceso del metodo Awake
    //Primero asignamos los datos a un string "newJson" con un objeto JsonData "newItem" para comprobar si efectivamente han habido cambios
    //Si son iguales no ocurre nada, evitando hacer procesos innecesarios
    //Pero si han habido cambios, primero destruimos las columnas con la informacion y creamos de nuevo la tabla con los nuevos datos que se han entregado
    public void ReloadData() {
        try
        {
            string newJson = File.ReadAllText(path);
            JsonData newItem = JsonMapper.ToObject(newJson);

            if (newJson == jsonString) return;

            DestroyChilds();

            ReadJson(newItem);

            //Creamos la UI
            CreateColumns();
        }
        catch {
            titleText.text = "There is no JSON File at " + Application.streamingAssetsPath.ToString();
        }
    }

    //Para destruir los elementos de la tabla, primero limpiamos la lista "data" para evitar repetir los datos
    //Y luego recorremos el GameObject que contiene los elementos de la tabla para destruirlos
    void DestroyChilds() {
        data.Clear();
        for (int i = 0; i < Columns.transform.childCount; i++)
        {
            Destroy(Columns.transform.GetChild(i).gameObject);
        }
    }
}
