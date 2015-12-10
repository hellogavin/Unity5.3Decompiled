namespace UnityEditor.Modules
{
    using System;

    internal class DefaultBuildWindowExtension : IBuildWindowExtension
    {
        public virtual bool EnabledBuildAndRunButton()
        {
            return true;
        }

        public virtual bool EnabledBuildButton()
        {
            return true;
        }

        public virtual bool ShouldDrawDevelopmentPlayerCheckbox()
        {
            return true;
        }

        public virtual bool ShouldDrawExplicitNullCheckbox()
        {
            return false;
        }

        public virtual bool ShouldDrawForceOptimizeScriptsCheckbox()
        {
            return false;
        }

        public virtual bool ShouldDrawProfilerCheckbox()
        {
            return true;
        }

        public virtual bool ShouldDrawScriptDebuggingCheckbox()
        {
            return true;
        }

        public virtual void ShowInternalPlatformBuildOptions()
        {
        }

        public virtual void ShowPlatformBuildOptions()
        {
        }
    }
}

