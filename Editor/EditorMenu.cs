using Scripts.Tools.CustomEdit;
using Scripts.Services.EventBus;
using UnityEditor;

namespace Scripts.CustomEdit.EditorMenu
{
    public class EditorMenu : EditorWindow
    {
        [MenuItem(DirectoryNames.SERVICES_PATH + nameof(EventBus) + "/" + nameof(EventBus.Reset))]
        static private void ResetEventBus() {
            EventBus.Reset();
        }
    }
}

