using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Linq;

public class ScriptLoggerWindow : EditorWindow
{
    private string folderPath = "Assets/Scripts"; // Default folder path
    private Vector2 scrollPosScripts;
    private Vector2 scrollPosLogs;

    private Dictionary<string, bool> scriptToggles = new Dictionary<string, bool>();
    private List<LogEntry> logEntries = new List<LogEntry>();
    
    private string scriptSearchFilter = "";
    private bool showNormalLogs = true;
    private bool showWarnings = true;
    private bool showErrors = true;
    private string searchFilter = "";
    private bool autoScroll = true;
    private bool showStackTraces = false;
    
    // For storing preferences
    private const string PREF_FOLDER_PATH = "ScriptLogger_FolderPath";
    private const string PREF_SCRIPT_FILTER = "ScriptLogger_ScriptFilter";
    private const string PREF_LOG_FILTER = "ScriptLogger_LogFilter";
    private const string PREF_SHOW_NORMAL = "ScriptLogger_ShowNormal";
    private const string PREF_SHOW_WARNINGS = "ScriptLogger_ShowWarnings";
    private const string PREF_SHOW_ERRORS = "ScriptLogger_ShowErrors";
    private const string PREF_AUTO_SCROLL = "ScriptLogger_AutoScroll";
    private const string PREF_SHOW_STACK = "ScriptLogger_ShowStack";
    private const string PREF_SCRIPT_TOGGLES_PREFIX = "ScriptLogger_ScriptToggle_";
    
    private GUIStyle errorStyle;
    private GUIStyle warningStyle;
    private GUIStyle infoStyle;
    private GUIStyle headerStyle;
    
    private Dictionary<int, bool> expandedLogs = new Dictionary<int, bool>();

    [MenuItem("Tools/Script Logger")]
    public static void ShowWindow()
    {
        GetWindow<ScriptLoggerWindow>("Script Logger");
    }

    private void OnEnable()
    {
        // Register for editor play mode state change
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        
        // Load preferences
        LoadPreferences();
        
        // Set up log callback
        Application.logMessageReceived += HandleLog;
        
        // Load or refresh script list
        RefreshScriptList();
    }

    private void OnDisable()
    {
        // Unregister from editor events
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        Application.logMessageReceived -= HandleLog;
        
        // Save preferences when window is closed
        SavePreferences();
    }
    
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // Save preferences when entering play mode
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            SavePreferences();
        }
        // Load preferences when exiting play mode
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            LoadPreferences();
            RefreshScriptList();
        }
    }
    
    private void SavePreferences()
    {
        EditorPrefs.SetString(PREF_FOLDER_PATH, folderPath);
        EditorPrefs.SetString(PREF_SCRIPT_FILTER, scriptSearchFilter);
        EditorPrefs.SetString(PREF_LOG_FILTER, searchFilter);
        EditorPrefs.SetBool(PREF_SHOW_NORMAL, showNormalLogs);
        EditorPrefs.SetBool(PREF_SHOW_WARNINGS, showWarnings);
        EditorPrefs.SetBool(PREF_SHOW_ERRORS, showErrors);
        EditorPrefs.SetBool(PREF_AUTO_SCROLL, autoScroll);
        EditorPrefs.SetBool(PREF_SHOW_STACK, showStackTraces);
        
        // Save script toggle states
        foreach (var pair in scriptToggles)
        {
            string key = PREF_SCRIPT_TOGGLES_PREFIX + pair.Key;
            EditorPrefs.SetBool(key, pair.Value);
        }
    }
    
    private void LoadPreferences()
    {
        folderPath = EditorPrefs.GetString(PREF_FOLDER_PATH, "Assets/Scripts");
        scriptSearchFilter = EditorPrefs.GetString(PREF_SCRIPT_FILTER, "");
        searchFilter = EditorPrefs.GetString(PREF_LOG_FILTER, "");
        showNormalLogs = EditorPrefs.GetBool(PREF_SHOW_NORMAL, true);
        showWarnings = EditorPrefs.GetBool(PREF_SHOW_WARNINGS, true);
        showErrors = EditorPrefs.GetBool(PREF_SHOW_ERRORS, true);
        autoScroll = EditorPrefs.GetBool(PREF_AUTO_SCROLL, true);
        showStackTraces = EditorPrefs.GetBool(PREF_SHOW_STACK, false);
    }
    
    private void RefreshScriptList()
    {
        // Remember toggle states before clearing
        Dictionary<string, bool> previousToggles = new Dictionary<string, bool>(scriptToggles);
        
        scriptToggles.Clear();

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError($"Invalid folder path: {folderPath}");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:MonoScript", new[] { folderPath });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
            if (monoScript != null && monoScript.GetClass() != null)
            {
                string className = monoScript.GetClass().Name;
                if (!scriptToggles.ContainsKey(className))
                {
                    // Restore previous toggle state if available, otherwise check preferences
                    if (previousToggles.ContainsKey(className))
                    {
                        scriptToggles.Add(className, previousToggles[className]);
                    }
                    else
                    {
                        string key = PREF_SCRIPT_TOGGLES_PREFIX + className;
                        bool toggleState = EditorPrefs.GetBool(key, false);
                        scriptToggles.Add(className, toggleState);
                    }
                }
            }
        }
    }

    private void InitStyles()
    {
        if (errorStyle == null)
        {
            errorStyle = new GUIStyle(EditorStyles.label);
            errorStyle.normal.textColor = new Color(0.9f, 0.3f, 0.3f);
            errorStyle.fontStyle = FontStyle.Bold;
        }
        
        if (warningStyle == null)
        {
            warningStyle = new GUIStyle(EditorStyles.label);
            warningStyle.normal.textColor = new Color(0.9f, 0.8f, 0.2f);
        }
        
        if (infoStyle == null)
        {
            infoStyle = new GUIStyle(EditorStyles.label);
            infoStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
        }
        
        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.fontSize = 12;
        }
    }

    private void OnGUI()
    {
        InitStyles();
        
        EditorGUILayout.LabelField("Script Logger", headerStyle);
        EditorGUILayout.Space();

        // Folder selection
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Folder Path:", GUILayout.Width(80));
        folderPath = EditorGUILayout.TextField(folderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string newPath = EditorUtility.OpenFolderPanel("Select Scripts Folder", "Assets", "");
            if (!string.IsNullOrEmpty(newPath))
            {
                // Convert to relative path
                if (newPath.StartsWith(Application.dataPath))
                {
                    folderPath = "Assets" + newPath.Substring(Application.dataPath.Length);
                    RefreshScriptList();
                }
            }
        }
        if (GUILayout.Button("Refresh", GUILayout.Width(60)))
        {
            RefreshScriptList();
        }
        EditorGUILayout.EndHorizontal(); // Matching end for folder selection

        EditorGUILayout.Space();

        // Script search field
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Search Scripts:", GUILayout.Width(90));
        string newScriptSearch = EditorGUILayout.TextField(scriptSearchFilter);
        if (newScriptSearch != scriptSearchFilter)
        {
            scriptSearchFilter = newScriptSearch;
        }
        EditorGUILayout.EndHorizontal();

        // Quick toggle buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Select All", GUILayout.Width(80)))
        {
            foreach (var key in new List<string>(scriptToggles.Keys))
            {
                if (ShouldShowScript(key))
                {
                    scriptToggles[key] = true;
                }
            }
        }
        if (GUILayout.Button("Select None", GUILayout.Width(80)))
        {
            foreach (var key in new List<string>(scriptToggles.Keys))
            {
                if (ShouldShowScript(key))
                {
                    scriptToggles[key] = false;
                }
            }
        }
        if (GUILayout.Button("Invert", GUILayout.Width(80)))
        {
            foreach (var key in new List<string>(scriptToggles.Keys))
            {
                if (ShouldShowScript(key))
                {
                    scriptToggles[key] = !scriptToggles[key];
                }
            }
        }
        EditorGUILayout.EndHorizontal(); // Matching end for quick toggle buttons

        // Scripts section with filtering
        EditorGUILayout.LabelField($"Scripts ({CountVisibleScripts()}):", headerStyle);
        scrollPosScripts = EditorGUILayout.BeginScrollView(scrollPosScripts, GUILayout.Height(150));
        List<string> scriptKeys = new List<string>(scriptToggles.Keys);
        foreach (var script in scriptKeys)
        {
            if (ShouldShowScript(script))
            {
                scriptToggles[script] = EditorGUILayout.ToggleLeft(script, scriptToggles[script]);
            }
        }
        EditorGUILayout.EndScrollView(); // Matching end for scripts section

        EditorGUILayout.Space();

        // Log filter options
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Filter Logs:", GUILayout.Width(70));
        searchFilter = EditorGUILayout.TextField(searchFilter);
        EditorGUILayout.EndHorizontal(); // Matching end for filter text field

        EditorGUILayout.BeginHorizontal();
        showNormalLogs = EditorGUILayout.ToggleLeft("Info", showNormalLogs, GUILayout.Width(60));
        showWarnings = EditorGUILayout.ToggleLeft("Warnings", showWarnings, GUILayout.Width(85));
        showErrors = EditorGUILayout.ToggleLeft("Errors", showErrors, GUILayout.Width(70));
        autoScroll = EditorGUILayout.ToggleLeft("Auto-scroll", autoScroll, GUILayout.Width(85));
        showStackTraces = EditorGUILayout.ToggleLeft("Show Stack Traces", showStackTraces, GUILayout.Width(130));
        EditorGUILayout.EndHorizontal(); // Matching end for filter options

        EditorGUILayout.Space();

        // Logs section
        EditorGUILayout.LabelField($"Logs ({GetVisibleLogCount()}):", headerStyle);
        scrollPosLogs = EditorGUILayout.BeginScrollView(scrollPosLogs, GUILayout.ExpandHeight(true));
        
        // Display filtered logs
        for (int i = 0; i < logEntries.Count; i++)
        {
            LogEntry entry = logEntries[i];
            
            // Apply filters
            if (ShouldShowLog(entry))
            {
                // Select style based on log type
                GUIStyle style = GetStyleForLogType(entry.type);
                
                // Show the log message
                EditorGUILayout.BeginHorizontal();
                
                // Expandable foldout if stack trace is available
                if (!string.IsNullOrEmpty(entry.stackTrace) && showStackTraces)
                {
                    if (!expandedLogs.ContainsKey(i))
                    {
                        expandedLogs[i] = false;
                    }
                    
                    expandedLogs[i] = EditorGUILayout.Foldout(expandedLogs[i], "", true);
                    EditorGUILayout.LabelField($"[{entry.time.ToString("HH:mm:ss")}] {entry.message}", style);
                }
                else
                {
                    EditorGUILayout.LabelField($"[{entry.time.ToString("HH:mm:ss")}] {entry.message}", style);
                }
                EditorGUILayout.EndHorizontal(); // Matching end for log entry
                
                // Show stack trace if expanded
                if (showStackTraces && !string.IsNullOrEmpty(entry.stackTrace) && expandedLogs.ContainsKey(i) && expandedLogs[i])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(entry.stackTrace, EditorStyles.wordWrappedLabel);
                    EditorGUI.indentLevel--;
                }
            }
        }
        
        EditorGUILayout.EndScrollView(); // Matching end for logs section

        // Buttons for log management
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Logs"))
        {
            logEntries.Clear();
            expandedLogs.Clear();
        }
        if (GUILayout.Button("Copy to Clipboard"))
        {
            CopyLogsToClipboard();
        }
        if (GUILayout.Button("Save to File"))
        {
            SaveLogsToFile();
        }
        EditorGUILayout.EndHorizontal(); // Matching end for buttons

        // Save preferences when settings change
        if (Event.current.type == EventType.MouseUp || 
            Event.current.type == EventType.KeyUp)
        {
            SavePreferences();
        }
    }
    
    private int GetVisibleLogCount()
    {
        int count = 0;
        foreach (var log in logEntries)
        {
            if (ShouldShowLog(log))
            {
                count++;
            }
        }
        return count;
    }
    
    private bool ShouldShowLog(LogEntry entry)
    {
        // Check log type filters
        if (entry.type == LogType.Log && !showNormalLogs) return false;
        if (entry.type == LogType.Warning && !showWarnings) return false;
        if (entry.type == LogType.Error && !showErrors) return false;
        if (entry.type == LogType.Exception && !showErrors) return false;
        if (entry.type == LogType.Assert && !showErrors) return false;
        
        // Check text filter
        if (!string.IsNullOrEmpty(searchFilter) && 
            !entry.message.ToLower().Contains(searchFilter.ToLower()))
        {
            return false;
        }
        
        return true;
    }
    
    private GUIStyle GetStyleForLogType(LogType type)
    {
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
            case LogType.Assert:
                return errorStyle;
            case LogType.Warning:
                return warningStyle;
            default:
                return infoStyle;
        }
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // If no scripts are toggled, capture all logs
        bool anyScriptToggled = scriptToggles.Values.Any(v => v);
        if (!anyScriptToggled)
        {
            logEntries.Add(new LogEntry
            {
                message = logString,
                stackTrace = stackTrace,
                type = type,
                time = DateTime.Now
            });
            Repaint();
            return;
        }

        // Check if log matches any toggled script
        foreach (var scriptPair in scriptToggles)
        {
            string scriptName = scriptPair.Key;
            bool isToggled = scriptPair.Value;
            
            if (isToggled && (logString.Contains(scriptName) || 
                             (stackTrace != null && stackTrace.Contains(scriptName))))
            {
                logEntries.Add(new LogEntry
                {
                    message = logString,
                    stackTrace = stackTrace,
                    type = type,
                    time = DateTime.Now
                });
                Repaint();
                break;
            }
        }
    }

    private void CopyLogsToClipboard()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var log in logEntries)
        {
            sb.AppendLine($"[{log.time.ToString("HH:mm:ss")}] [{log.type}] {log.message}");
            if (!string.IsNullOrEmpty(log.stackTrace))
            {
                sb.AppendLine(log.stackTrace);
            }
        }
        EditorGUIUtility.systemCopyBuffer = sb.ToString();
    }

    private void SaveLogsToFile()
    {
        string path = EditorUtility.SaveFilePanel("Save Logs to File", "", "logs.txt", "txt");
        if (!string.IsNullOrEmpty(path))
        {
            StringBuilder sb = new StringBuilder();
            foreach (var log in logEntries)
            {
                sb.AppendLine($"[{log.time.ToString("HH:mm:ss")}] [{log.type}] {log.message}");
                if (!string.IsNullOrEmpty(log.stackTrace))
                {
                    sb.AppendLine(log.stackTrace);
                }
            }
            File.WriteAllText(path, sb.ToString());
        }
    }

    // Helper method to determine if a script should be shown based on search filter
    private bool ShouldShowScript(string scriptName)
    {
        if (string.IsNullOrEmpty(scriptSearchFilter))
            return true;
        
        return scriptName.ToLower().Contains(scriptSearchFilter.ToLower());
    }
    
    // Helper method to count visible scripts
    private int CountVisibleScripts()
    {
        if (string.IsNullOrEmpty(scriptSearchFilter))
            return scriptToggles.Count;
        
        int count = 0;
        foreach (var script in scriptToggles.Keys)
        {
            if (ShouldShowScript(script))
                count++;
        }
        return count;
    }

    private class LogEntry
    {
        public string message;
        public string stackTrace;
        public LogType type;
        public DateTime time;
    }
}
