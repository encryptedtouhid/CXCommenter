﻿using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Project = EnvDTE.Project;

namespace CXCommenter
{
    public class XmlCommenter
    {
        public static string appName = Assembly.GetCallingAssembly().GetName().Name;
        public static string appVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();

        /// <summary>
        /// Function Name - CommentSolution
        /// </summary>
        ///<returns>void</returns>
        public static void CommentSolution()
        {
            DTE2 dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2;
            Solution2 solution = dte.Solution as Solution2;
            foreach (Project project in solution.Projects)
            {
                CommentProject(project);
            }
        }


        /// <summary>
        /// Function Name - CommentProject
        /// </summary>
        ///<param name="EnvDTE.Project project">TODO: Describe EnvDTE.Project project here</param>
        ///<returns>void</returns>
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

        /// <summary>
        /// Function Name - IterateThroughAllCodeElementsInFolder
        /// </summary>
        ///<param name="EnvDTE.ProjectItem folder">TODO: Describe EnvDTE.ProjectItem folder here</param>
        ///<returns>void</returns>
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


        /// <summary>
        /// Function Name - CommentCodeElements
        /// </summary>
        ///<param name="EnvDTE.ProjectItem item">TODO: Describe EnvDTE.ProjectItem item here</param>
        ///<returns>void</returns>
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


        /// <summary>
        /// Function Name - CommentCodeElementsInNamespace
        /// </summary>
        ///<param name="EnvDTE.CodeNamespace codeNamespace">TODO: Describe EnvDTE.CodeNamespace codeNamespace here</param>
        ///<returns>void</returns>
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


        /// <summary>
        /// Function Name - CommentCodeElementsInType
        /// </summary>
        ///<param name="EnvDTE.CodeType codeType">TODO: Describe EnvDTE.CodeType codeType here</param>
        ///<returns>void</returns>
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
                    List<string> parameters = new List<string>();
                    foreach (CodeParameter parameter in codeFunction.Parameters)
                    {
                        parameters.Add(parameter.Type.AsString + " " + parameter.Name);
                    }

                    string comment = "/// <summary>" + Environment.NewLine +
                                      "/// Function Name - " + functionName + Environment.NewLine +
                                      "/// </summary>" + Environment.NewLine;
                    if (parameters.Count > 0)
                    {
                        foreach (string param in parameters)
                        {
                            comment += "///<param name=\"" + param + "\">" + ": Describe " + param + " here" + "</param>" + Environment.NewLine;
                        }
                    }
                    comment += "///<returns>" + returnType + "</returns>" + Environment.NewLine;

                    codeElement2.StartPoint.CreateEditPoint().Insert(comment);
                    codeElement2.StartPoint.CreateEditPoint().SmartFormat(codeElement2.EndPoint);
                    codeElement2.ProjectItem.Save();
                }
                else if (memberElement.Kind == vsCMElement.vsCMElementProperty)
                {
                    string comment = "/// <summary>" + Environment.NewLine +
                                   "/// Property Name - " + codeElement2.Name + Environment.NewLine +
                                   "/// </summary>" + Environment.NewLine;
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
                             "/// Namespace Name - " + codeNamespace.Name + Environment.NewLine +
                             "/// </summary>" + Environment.NewLine;

            startPoint.Insert(comment);
            startPoint.SmartFormat(endPoint);
        }
        public static void CommentFileHeader(CodeElement codeElement)
        {
            if (codeElement.StartPoint != null)
            {
                string fileName = codeElement.ProjectItem.FileNames[0];
                string extension = Path.GetExtension(codeElement.ProjectItem.FileNames[0]);
                EditPoint editPoint = (EditPoint)codeElement.StartPoint.CreateEditPoint();
                string header = "//------------------------------------------------------------------------------" + Environment.NewLine +
                                "// <auto-generated>" + Environment.NewLine +
                                "//     This code was generated by " + appName + ": " + appVersion + Environment.NewLine +
                                "// </auto-generated>" + Environment.NewLine +
                                "//------------------------------------------------------------------------------" + Environment.NewLine;
                string header2 = "// <copyright file=\"" + fileName + "." + extension + "\" company=\"PlaceholderCompany\">" + Environment.NewLine +
                                 "// Copyright (c) PlaceholderCompany. All rights reserved." + Environment.NewLine +
                                 "// </copyright>" + Environment.NewLine;

                string fileContent = editPoint.GetText(codeElement.EndPoint);
                if (!fileContent.StartsWith(header))
                {
                    editPoint.Insert(header);
                    codeElement.ProjectItem.Save();
                }
            }
        }

    }
}
