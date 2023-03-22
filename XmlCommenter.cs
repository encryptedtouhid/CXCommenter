﻿using EnvDTE;
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
        public static void CommentSolution()
        {
            // Get the DTE object.
            DTE2 dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2;

            // Get the solution object.
            Solution2 solution = dte.Solution as Solution2;

            // Loop through all projects in the solution.
            foreach (Project project in solution.Projects)
            {
                // Add XML comments to the project.
                CommentProject(project);
            }
        }

        public static void CommentProject(Project project)
        {
            foreach (CodeElement codeElement in project.CodeModel.CodeElements)
            {
                // Check if the code element is a namespace
                if (codeElement.Kind == vsCMElement.vsCMElementNamespace)
                {
                    CodeNamespace codeNamespace = (CodeNamespace)codeElement;

                    // Loop through all types in the namespace
                    foreach (CodeElement typeElement in codeNamespace.Members)
                    {
                        // Check if the code element is a class, struct, or interface
                        if (typeElement.Kind == vsCMElement.vsCMElementClass ||
                            typeElement.Kind == vsCMElement.vsCMElementStruct ||
                            typeElement.Kind == vsCMElement.vsCMElementInterface)
                        {
                            CodeType codeType = (CodeType)typeElement;

                            // Loop through all members in the type
                            foreach (CodeElement memberElement in codeType.Members)
                            {
                                CodeElement2 codeElement2 = (CodeElement2)memberElement;
                                // Check if the member is a method or property
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
                                                  "/// TODO: Add XML comment for " + functionName + Environment.NewLine +
                                                  "/// </summary>" + Environment.NewLine +
                                                  "///<param>" + parameters[0]  + "</param>" + Environment.NewLine +
                                                  "///<return>" + returnType + "</return>" + Environment.NewLine ;
                                               
                                    codeElement2.StartPoint.CreateEditPoint().Insert(comment);

                                    // Format the XML comment
                                    codeElement2.StartPoint.CreateEditPoint().SmartFormat(codeElement2.EndPoint);

                                    // Save the changes to the code file
                                    codeElement2.ProjectItem.Save();


                                }
                                else if(memberElement.Kind == vsCMElement.vsCMElementProperty)
                                {
                                    string comment = "/// <summary>" + Environment.NewLine +
                                                   "/// TODO: Add XML comment for " + codeElement2.Name + Environment.NewLine +
                                                   "/// </summary>" + Environment.NewLine;
                                    codeElement2.StartPoint.CreateEditPoint().Insert(comment);

                                    // Format the XML comment
                                    codeElement2.StartPoint.CreateEditPoint().SmartFormat(codeElement2.EndPoint);

                                    // Save the changes to the code file
                                    codeElement2.ProjectItem.Save();
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
