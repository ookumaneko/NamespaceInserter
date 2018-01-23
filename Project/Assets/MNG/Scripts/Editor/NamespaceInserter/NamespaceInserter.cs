using System.Text;
using System.IO;
using System.Collections.Generic;

namespace MaxNeet.Editor
{
	public class NamespaceInserter
	{
	    const string TEXT_CLASS = "class";
	    const string TEXT_NAMESPACE = "namespace";
	    const string TEXT_ENUM = "enum";
	    const string TEXT_BRACE_OPEN = "{";
	    const string TEXT_BRACE_CLOSE = "}";
	
	    const string TEXT_TAB = "\t";
        const string TEXT_SLASH = "/";
        const string TEXT_START_COMMENT_GROUP = "/*";
        const string TEXT_END_COMMENT_GROUP = "*/";

        const int INVALID_VALUE = -1;
	    const int NAMESPACE_DECLEAR_INDEX = 1;
	
	    List<string> m_insertOnTopTexts;
	
	    public NamespaceInserter()
	    {
	        m_insertOnTopTexts = new List<string>();
	        m_insertOnTopTexts.Add(TEXT_CLASS);
	        m_insertOnTopTexts.Add(TEXT_ENUM);
	    }
	
	    public void InsertNamespace(string namespaceName, List<string> fileList, bool isOverwrite, bool isSkipOnClassDecleration = true)
	    {
	        int count = fileList.Count;
	        for (int i = 0; i < count; ++i)
	        {
	            InsertToFile(namespaceName, fileList[i], isOverwrite, isSkipOnClassDecleration);
	        }
	    }
	
	    private void InsertToFile(string namespaceName, string fileName, bool isOverwrite, bool isSkipOnClassDecleration)
	    {
	        bool isSave = true;
	        List<string> fileText = CreateFileText(fileName);
	        int namespaceIndex = FindNamespaceIndex(fileText, isSkipOnClassDecleration);
	
	        if (namespaceIndex == INVALID_VALUE)
	        {
	            InsertNamespaceToFileText(namespaceName, ref fileText);
	        }
	        else
	        {
	            isSave = isOverwrite;
	            if (isOverwrite)
	            {
	                OverwriteNamespace(namespaceName, namespaceIndex, ref fileText);
	            }
	        }
	
	        if (isSave)
	        {
	            SaveToFile(fileText, fileName);
	        }
	    }
	
	    private List<string> CreateFileText(string fileName)
	    {
	        List<string> fileText = new List<string>();
	        using (var stream = new StreamReader(fileName))
	        {
	            string line = string.Empty;
	            while ((line = stream.ReadLine()) != null)
	            {
	                fileText.Add(line);
	            }
	        }
	
	        return fileText;
	    }
	
	    private int FindNamespaceIndex(List<string> fileText, bool isSkipOnClassDecleration)
	    {
            bool isInCommentGroup = false;
	        int count = fileText.Count;
	        for (int i = 0; i < count; ++i)
	        {
                if (isInCommentGroup)
                {
                    if (fileText[i].Contains(TEXT_END_COMMENT_GROUP))
                    {
                        isInCommentGroup = false;
                    }

                    continue;
                }

                if (fileText[i].Contains(TEXT_START_COMMENT_GROUP))
                {
                    isInCommentGroup = true;
                    continue;
                }

                if (fileText[i].Contains(TEXT_NAMESPACE))
	            {
	                return i;
	            }
	
	            // namespaceはclass宣言の前のはず
	            if (isSkipOnClassDecleration && fileText[i].Contains(TEXT_CLASS))
	            {
	                return INVALID_VALUE;
	            }
	        }
	
	        return INVALID_VALUE;
	    }
	
	    private int FindInsertIndex(List<string> fileText)
	    {
	        int listCount = m_insertOnTopTexts.Count;
	        int count = fileText.Count;
	        for (int ii = 0; ii < count; ++ii)
	        {
	            for (int kk = 0; kk < listCount; ++kk)
	            {
	                if (fileText[ii].Contains(m_insertOnTopTexts[kk]))
	                {
	                    return ii;
	                }
	            }
	        }
	
	        return INVALID_VALUE;
	    }
	
	    private int FindTextIndex(List<string> fileText, string textToFind)
	    {
	        int count = fileText.Count;
	        for (int i = 0; i < count; ++i)
	        {
	            if (fileText[i].Contains(textToFind))
	            {
	                return i;
	            }
	        }
	
	        return INVALID_VALUE;
	    }
	
	    private void OverwriteNamespace(string namespaceName, int namespaceIndex, ref List<string> fileText)
	    {
	        StringBuilder builder = new StringBuilder();
	        builder.Append(TEXT_NAMESPACE);
	        builder.Append(" ");
	        builder.Append(namespaceName);
	
	        // 既にnamespace有る場合はインデントいらない？
	        //int lastIndex = FindTextIndex(fileText, TEXT_BRACE_CLOSE);
	        //int count = fileText.Count;
	        //for (int i = namespaceIndex; i < lastIndex; ++i)
	        //{
	        //    fileText[i] = fileText[i].Insert(0, TEXT_TAB);
	        //}
	
	        fileText[namespaceIndex] = builder.ToString();
	    }
	
	    private void InsertNamespaceToFileText(string namespaceName, ref List<string> fileText)
	    {
	        int insertIndex = FindInsertIndex(fileText);
	
	        int count = fileText.Count;
	        for (int i = insertIndex; i < count; ++i)
	        {
	            fileText[i] = fileText[i].Insert(0, TEXT_TAB);
	        }
	
	        StringBuilder builder = new StringBuilder();
	        builder.Append(TEXT_NAMESPACE);
	        builder.Append(" ");
	        builder.Append(namespaceName);
	
	        fileText.Insert(insertIndex, builder.ToString());
	        fileText.Insert(insertIndex + 1, TEXT_BRACE_OPEN);
	        fileText.Add(TEXT_BRACE_CLOSE);
	    }
	
	    private void SaveToFile(List<string> list, string path)
	    {
	        using (var stream = new StreamWriter(path, false))
	        {
	            int count = list.Count;
	            for (int ii = 0; ii < count; ++ii)
	            {
	                stream.WriteLine(list[ii]);
	            }
	        }
	
	        list.Clear();
	    }
	}
}
