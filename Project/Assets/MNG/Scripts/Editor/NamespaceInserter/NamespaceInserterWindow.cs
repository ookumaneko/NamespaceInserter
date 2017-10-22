using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace MNG
{
	public class NamespaceInserterWindow
		: EditorWindow
	{
	    const string ON_COMPLETE_MESSAGE = "DONE!";
	    const string WINDOW_NAME = "NamespaceInserter";
	    const string TEXTFIELD_NAME = "ForcusField";
	    const string SCRIPT_FILE_EXTENSION = ".cs";
        const string TEXT_START_BUTTON = "Start";
        const string TEXT_OK = "OK";
        const string TEXT_OVERWRITE = "Overwrite existing namespace";
        const string TEXT_INSERT_NAMESPACE = "Type namespace name";
        const string TEXT_LOG_FILES = "Debug Log selected Files";

        string m_namespaceName = "";
	    bool m_isOverwrite = true;
        bool m_isLogSelectedFiles = false;
	    bool m_isInitialized = false;
	
	    [MenuItem(EditorDefine.MENU_TOOLS + WINDOW_NAME)]
	    static void ShowWindow()
	    {
	        EditorWindow.GetWindow<NamespaceInserterWindow>();
	    }
	
	    private void OnGUI()
	    {
	        GUILayout.Label(TEXT_INSERT_NAMESPACE);
	        GUILayout.Space(10f);
	        GUI.SetNextControlName(TEXTFIELD_NAME);
	        m_isOverwrite = GUILayout.Toggle(m_isOverwrite, TEXT_OVERWRITE);
            m_isLogSelectedFiles = GUILayout.Toggle(m_isLogSelectedFiles, TEXT_LOG_FILES);
            m_namespaceName = GUILayout.TextField(m_namespaceName);
	
	        GUILayout.Space(10.0f);

            // Disable button when namespace is empty or file isnt selected
            EditorGUI.BeginDisabledGroup(IsDisableButton());
	        GUILayout.BeginHorizontal();
	        if (GUILayout.Button(TEXT_START_BUTTON, GUILayout.Height(30f)))
	        {
                StartInsertNamespace();
            }
	        GUILayout.EndHorizontal();

            // set initial forcus to text field
            if (m_isInitialized == false)
	        {
	            EditorGUI.FocusTextInControl(TEXTFIELD_NAME);
                m_isInitialized = true;
            }	        
	    }

        private void StartInsertNamespace()
        {
            Debug.Log("Start! namespace = " + m_namespaceName);
            List<string> fileList = CreateFileList();
            if (m_isLogSelectedFiles)
            {
                OutputSelectedFileLog(fileList);
            }

            NamespaceInserter insterter = new NamespaceInserter();
            insterter.InsertNamespace(m_namespaceName, fileList, m_isOverwrite);
            Debug.Log("Finished inserting!");
            EditorUtility.DisplayDialog(WINDOW_NAME, ON_COMPLETE_MESSAGE, TEXT_OK);
        }

        private bool IsDisableButton()
        {
            return ( string.IsNullOrEmpty(m_namespaceName) || !IsScriptFileSelected() );
        }

        private bool IsScriptFileSelected()
        {
            string assetPath = Application.dataPath;
            foreach (var files in Selection.assetGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(files);
                if (Path.GetExtension(path) == SCRIPT_FILE_EXTENSION)
                {
                    return true;
                }
            }

            return false;
        }

        private List<string> CreateFileList()
	    {
	        string assetPath = Application.dataPath;
	        List<string> fileList = new List<string>();
	        foreach (var files in Selection.assetGUIDs)
	        {
	            var path = AssetDatabase.GUIDToAssetPath(files);
	            if (Path.GetExtension(path) == SCRIPT_FILE_EXTENSION)
	            {
	                fileList.Add(assetPath + "/" + GetChiledPath(path));
	            }
	        }
	
	        return fileList;
	    }
	
	    private static string GetChiledPath(string path)
	    {
	        return path.Remove(0, path.IndexOf("/") + 1);
	    }

        private void OutputSelectedFileLog(List<string> fileList)
        {
            int count = fileList.Count;
            for (int i = 0; i < count; ++i)
            {
                Debug.Log("File = " + fileList[i]);
            }
        }
    }
}
