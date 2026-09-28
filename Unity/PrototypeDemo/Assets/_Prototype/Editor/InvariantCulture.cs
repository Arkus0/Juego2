using System.Globalization;
using System.Threading;
using UnityEditor;

namespace Proto.EditorTools
{
    /// Spanish Windows locale writes "8,62"; all prototype tooling uses invariant formatting.
    [InitializeOnLoad]
    static class InvariantCulture
    {
        static InvariantCulture()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }
    }
}
