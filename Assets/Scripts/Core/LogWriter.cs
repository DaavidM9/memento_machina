using System.IO;
using UnityEngine;

public class LogWriter : MonoBehaviour
{
    private string logPath;

    void Awake()
    {
        // Crear archivo de log en la carpeta del ejecutable
        logPath = Path.Combine(Application.persistentDataPath, "game_log.txt");
        Application.logMessageReceived += Log;
    }

    void Log(string logString, string stackTrace, LogType type)
    {
        try 
        {
            File.AppendAllText(logPath, $"{System.DateTime.Now}: {logString}\n{stackTrace}\n");
        }
        catch (System.Exception e)
        {
            // En caso de error al escribir el log
            System.Console.WriteLine($"Error writing log: {e.Message}");
        }
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= Log;
    }
}