using Community.VisualStudio.Toolkit;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project = EnvDTE.Project;

namespace CXCommenter
{
    public class XmlCommenter
    {

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
                    IterateThroughAllCodeElements(item.SubProject);
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
        /// Function Name - IterateThroughAllCodeElements
        /// </summary>
        ///<param name="EnvDTE.Project SubProject">TODO: Describe EnvDTE.Project SubProject here</param>
        ///<returns>void</returns>
        public static void IterateThroughAllCodeElements(Project SubProject)
        {

            foreach (CodeElement codeElement in SubProject.CodeModel.CodeElements)
            {
                if (codeElement.Kind == vsCMElement.vsCMElementNamespace)
                {
                    CodeNamespace codeNamespace = (CodeNamespace)codeElement;

                    foreach (CodeElement typeElement in codeNamespace.Members)
                    {
                        if (typeElement.Kind == vsCMElement.vsCMElementClass ||
                            typeElement.Kind == vsCMElement.vsCMElementStruct ||
                            typeElement.Kind == vsCMElement.vsCMElementInterface)
                        {
                            CodeType codeType = (CodeType)typeElement;

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
                                                  "///  Funcation Name - " + functionName + Environment.NewLine +
                                                  "/// </summary>" + Environment.NewLine +
                                                  "///<param>" + parameters[0] + "</param>" + Environment.NewLine +
                                                  "///<return>" + returnType + "</return>" + Environment.NewLine;

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
                    }
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
                if (item.SubProject != null)
                {
                    IterateThroughAllCodeElements(item.SubProject);
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
        /// Function Name - CommentCodeElements
        /// </summary>
        ///<param name="EnvDTE.ProjectItem item">TODO: Describe EnvDTE.ProjectItem item here</param>
        ///<returns>void</returns>
        public static void CommentCodeElements(ProjectItem item)
        {
            if (item.FileCodeModel != null)
            {
                foreach (CodeElement codeElement in item.FileCodeModel.CodeElements)
                {
                    if (codeElement.Kind == vsCMElement.vsCMElementNamespace)
                    {
                        CodeNamespace codeNamespace = (CodeNamespace)codeElement;
                        CommentCodeElementsInNamespace(codeNamespace);
                    }
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
                            comment += "///<param name=\"" + param + "\">" + "TODO: Describe " + param + " here" + "</param>" + Environment.NewLine;
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



    }
}
