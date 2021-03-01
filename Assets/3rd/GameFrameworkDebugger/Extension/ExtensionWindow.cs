namespace UnityGameFramework.Runtime
{
    public partial class DebuggerComponent
    {
        private ExampleWindow exampleWindow;
        public void RegisterExtensionWindows()
        {
            exampleWindow = new ExampleWindow();
            RegisterDebuggerWindow("UserData", exampleWindow);
        }

        public void InitUserData()
        {
            //exampleWindow.InitUserData();
        }
    }
}
