namespace CXCommenter
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        /// <summary>
        /// TODO: Add XML comment for ExecuteAsync
        /// </summary>
        ///<param name="Microsoft.VisualStudio.Shell.OleMenuCmdEventArgs e">TODO: Describe Microsoft.VisualStudio.Shell.OleMenuCmdEventArgs e here</param>
        ///<returns>System.Threading.Tasks.Task</returns>
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            XmlCommenter.CommentSolution();
        }
    }
}
