﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by CXCommenter: 0.9.5.0.
// </auto-generated>
//------------------------------------------------------------------------------
// <copyright file="MyCommand.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
/// <summary>
///       Namespace Name - CXCommenter.
/// </summary>
namespace CXCommenter
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {

        /// <summary>
        ///     Function Name - ExecuteAsync.
        /// </summary>
        /// <param name="Microsoft.VisualStudio.Shell.OleMenuCmdEventArgs e">: Describe Microsoft.VisualStudio.Shell.OleMenuCmdEventArgs e here.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            XmlCommenter.CommentSingleFile();
        }
    }
}
