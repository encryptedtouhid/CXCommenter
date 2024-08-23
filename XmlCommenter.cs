using CXCommenter.Models;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Project = EnvDTE.Project;

namespace CXCommenter
{
    public class XmlCommenter
    {
        public static string appName = Assembly.GetCallingAssembly().GetName().Name;
        public static string appVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();


        public static void CommentSolution()
        {
            DTE2 dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2;
            Solution2 solution = dte.Solution as Solution2;
            foreach (Project project in solution.Projects)
            {
                CommentProject(project);
            }
        }

        public static void CommentSingleFile()
        {
            ProjectItem projectItem = null;
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE2 dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            Document activeDocument = dte.ActiveDocument;

            // Ensure the active document is a C# file
            if (activeDocument.Language == "CSharp")
            {
                projectItem = activeDocument.ProjectItem;
                RemoveXmlCommentsFromActiveDocument(activeDocument);
                CommentCodeElements(projectItem);
            }
        }

        public static void CommentProject(Project project)
        {
            foreach (ProjectItem item in project.ProjectItems)
            {
                if (item.SubProject != null)
                {
                    //IterateThroughAllCodeElements(item.SubProject);
                }
                else if (item.ProjectItems != null && item.ProjectItems.Count > 0)
                {
                    IterateThroughAllCodeElementsInFolder(item);
                }
                else if (item.Name.EndsWith(".cs"))
                {
                    CommentCodeElements(item);
                }
            }
        }

        public static void IterateThroughAllCodeElementsInFolder(ProjectItem folder)
        {
            foreach (ProjectItem item in folder.ProjectItems)
            {
                if (item.ProjectItems != null && item.ProjectItems.Count > 0)
                {
                    IterateThroughAllCodeElementsInFolder(item);
                }
                else if (item.Name.EndsWith(".cs"))
                {
                    CommentCodeElements(item);
                }
            }
        }

        public static void CommentCodeElements(ProjectItem item)
        {
            if (item.FileCodeModel != null)
            {
                int loopCount = 0;
                foreach (CodeElement codeElement in item.FileCodeModel.CodeElements)
                {
                    if (loopCount == 0)
                    {
                        CommentFileHeader(codeElement);
                    }

                    if (codeElement.Kind == vsCMElement.vsCMElementNamespace)
                    {
                        CodeNamespace codeNamespace = (CodeNamespace)codeElement;
                        CommentCodeElementsInNamespace(codeNamespace);
                    }
                    loopCount++;
                }
            }

        }

        public static void CommentCodeElementsInNamespace(CodeNamespace codeNamespace)
        {
            CommentNamespace(codeNamespace);
            foreach (CodeElement typeElement in codeNamespace.Members)
            {
                if (typeElement.Kind == vsCMElement.vsCMElementClass ||
                    typeElement.Kind == vsCMElement.vsCMElementStruct ||
                    typeElement.Kind == vsCMElement.vsCMElementInterface)
                {
                    CodeType codeType = (CodeType)typeElement;
                    CommentCodeElementsInType(codeType);
                }
            }
        }

        public static void CommentCodeElementsInType(CodeType codeType)
        {
            foreach (CodeElement memberElement in codeType.Members)
            {
                CodeElement2 codeElement2 = (CodeElement2)memberElement;

                if (memberElement.Kind == vsCMElement.vsCMElementFunction)
                {
                    CodeFunction codeFunction = (CodeFunction)memberElement;
                    string functionName = codeFunction.Name;
                    string returnType = codeFunction.Type.AsString;
                    List<Parameters> parameters = new List<Parameters>();
                    foreach (CodeParameter parameter in codeFunction.Parameters)
                    {
                        Parameters pr = new Parameters();
                        pr.VariableName = parameter.Name;
                        pr.TypeName = parameter.Type.AsString;
                        parameters.Add(pr);
                    }

                    string comment = Environment.NewLine +
                                      "/// <summary>" + Environment.NewLine +
                                      "///  Function Name :  " + functionName + "." + Environment.NewLine +
                                      "/// </summary>" + Environment.NewLine;
                    if (parameters.Count > 0)
                    {
                        foreach (Parameters param in parameters)
                        {
                            comment += "/// <param name=\"" + param.VariableName + "\">" + "This " + param.VariableName+ "'s Datatype is : " + param.TypeName +"." + "</param>" + Environment.NewLine;
                        }
                    }
                    comment += "/// <returns>" + returnType + "." + "</returns>" + Environment.NewLine;

                    codeElement2.StartPoint.CreateEditPoint().Insert(comment);
                    codeElement2.StartPoint.CreateEditPoint().SmartFormat(codeElement2.EndPoint);
                    codeElement2.ProjectItem.Save();
                }
                else if (memberElement.Kind == vsCMElement.vsCMElementProperty)
                {
                    string comment = Environment.NewLine + "/// <summary>" + Environment.NewLine +
                                     "///      Property Name - " + codeElement2.Name + "." + Environment.NewLine +
                                     "///  </summary>" + Environment.NewLine;
                    codeElement2.StartPoint.CreateEditPoint().Insert(comment);
                    codeElement2.StartPoint.CreateEditPoint().SmartFormat(codeElement2.EndPoint);
                    codeElement2.ProjectItem.Save();
                }
            }
        }

        public static void CommentNamespace(CodeNamespace codeNamespace)
        {
            EditPoint startPoint = codeNamespace.StartPoint.CreateEditPoint();
            EditPoint endPoint = codeNamespace.EndPoint.CreateEditPoint();

            string comment = "/// <summary>" + Environment.NewLine +
                             "///       Namespace Name - " + codeNamespace.Name + "." + Environment.NewLine +
                             "/// </summary>" + Environment.NewLine;

            startPoint.Insert(comment);
            startPoint.SmartFormat(endPoint);
        }

        public static void CommentFileHeader(CodeElement codeElement)
        {
            if (codeElement.StartPoint != null)
            {
                string fileName = Path.GetFileName(codeElement.ProjectItem.FileNames[0]);
                EditPoint editPoint = (EditPoint)codeElement.StartPoint.CreateEditPoint();
                string headerPart1 = "//------------------------------------------------------------------------------" + Environment.NewLine +
                                "// <auto-generated>" + Environment.NewLine +
                                "//     This xml documentation was generated by " + appName + ": " + appVersion + "." + Environment.NewLine +
                                "// </auto-generated>" + Environment.NewLine +
                                "//------------------------------------------------------------------------------" + Environment.NewLine;


                string headerPart2 = "// <copyright file=\"" + fileName + "\" company=\"PlaceholderCompany\">" + Environment.NewLine +
                                 "//     Copyright (c) PlaceholderCompany. All rights reserved." + Environment.NewLine +
                                 "// </copyright>" + Environment.NewLine;

                string fileContent = editPoint.GetText(codeElement.EndPoint);
                if (!fileContent.StartsWith(headerPart1 + headerPart2))
                {
                    editPoint.Insert(headerPart1 + headerPart2);
                    codeElement.ProjectItem.Save();
                }
            }
        }

        public static void RemoveXmlCommentsFromActiveDocument(Document activeDocument)
        {
            TextDocument textDocument = (TextDocument)activeDocument.Object("TextDocument");
            EditPoint editPoint = textDocument.StartPoint.CreateEditPoint();
            string text = editPoint.GetText(textDocument.EndPoint);

            // Define the regex pattern to match XML comments
            string xmlCommentPattern = @"///.*\r?\n";

            string xmlCommentPatternforHeader = @"//.*\r?\n";

            string modifiedContentsfterheader = Regex.Replace(text, xmlCommentPatternforHeader, string.Empty);
            // Remove XML comments
            string modifiedContents = Regex.Replace(modifiedContentsfterheader, xmlCommentPattern, string.Empty);

            // Replace the text in the active document
            editPoint.ReplaceText(textDocument.EndPoint, modifiedContents, (int)vsEPReplaceTextOptions.vsEPReplaceTextKeepMarkers);
        }

    }
}
