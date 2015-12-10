namespace UnityEditor.Modules
{
    using System;
    using UnityEngine;

    internal abstract class DefaultPlatformSupportModule : IPlatformSupportModule
    {
        protected ICompilationExtension compilationExtension;

        protected DefaultPlatformSupportModule()
        {
        }

        public abstract IBuildPostprocessor CreateBuildPostprocessor();
        public virtual IBuildWindowExtension CreateBuildWindowExtension()
        {
            return null;
        }

        public virtual ICompilationExtension CreateCompilationExtension()
        {
            return ((this.compilationExtension == null) ? (this.compilationExtension = new DefaultCompilationExtension()) : this.compilationExtension);
        }

        public virtual IDevice CreateDevice(string id)
        {
            throw new NotSupportedException();
        }

        public virtual IPluginImporterExtension CreatePluginImporterExtension()
        {
            return null;
        }

        public virtual IPreferenceWindowExtension CreatePreferenceWindowExtension()
        {
            return null;
        }

        public virtual IScriptingImplementations CreateScriptingImplementations()
        {
            return null;
        }

        public virtual ISettingEditorExtension CreateSettingsEditorExtension()
        {
            return null;
        }

        public virtual IUserAssembliesValidator CreateUserAssembliesValidatorExtension()
        {
            return null;
        }

        public virtual GUIContent[] GetDisplayNames()
        {
            return null;
        }

        public virtual void OnActivate()
        {
        }

        public virtual void OnDeactivate()
        {
        }

        public virtual void OnLoad()
        {
        }

        public virtual void OnUnload()
        {
        }

        public virtual string[] AssemblyReferencesForUserScripts
        {
            get
            {
                return new string[0];
            }
        }

        public virtual string ExtensionVersion
        {
            get
            {
                return null;
            }
        }

        public abstract string JamTarget { get; }

        public abstract string TargetName { get; }
    }
}

