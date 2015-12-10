namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using UnityEditor.VersionControl;
    using UnityEditorInternal;
    using UnityEditorInternal.VersionControl;
    using UnityEngine;

    internal class AssetModificationProcessorInternal
    {
        private static IEnumerable<Type> assetModificationProcessors;
        internal static MethodInfo[] isOpenForEditMethods;

        private static bool CheckArguments(object[] args, MethodInfo method)
        {
            Type[] types = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                types[i] = args[i].GetType();
            }
            return CheckArgumentTypes(types, method);
        }

        private static bool CheckArgumentsAndReturnType(object[] args, MethodInfo method, Type returnType)
        {
            Type[] types = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                types[i] = args[i].GetType();
            }
            return CheckArgumentTypesAndReturnType(types, method, returnType);
        }

        private static bool CheckArgumentTypes(Type[] types, MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (types.Length != parameters.Length)
            {
                string[] textArray1 = new string[] { "Parameter count did not match. Expected: ", types.Length.ToString(), " Got: ", parameters.Length.ToString(), " in ", method.DeclaringType.ToString(), ".", method.Name };
                Debug.LogWarning(string.Concat(textArray1));
                return false;
            }
            int index = 0;
            foreach (Type type in types)
            {
                ParameterInfo info = parameters[index];
                if (type != info.ParameterType)
                {
                    Debug.LogWarning(string.Concat(new object[] { "Parameter type mismatch at parameter ", index, ". Expected: ", type.ToString(), " Got: ", info.ParameterType.ToString(), " in ", method.DeclaringType.ToString(), ".", method.Name }));
                    return false;
                }
                index++;
            }
            return true;
        }

        private static bool CheckArgumentTypesAndReturnType(Type[] types, MethodInfo method, Type returnType)
        {
            if (returnType != method.ReturnType)
            {
                Debug.LogWarning("Return type mismatch. Expected: " + returnType.ToString() + " Got: " + method.ReturnType.ToString() + " in " + method.DeclaringType.ToString() + "." + method.Name);
                return false;
            }
            return CheckArgumentTypes(types, method);
        }

        private static void FileModeChanged(string[] assets, FileMode mode)
        {
            if (Provider.enabled && Provider.PromptAndCheckoutIfNeeded(assets, string.Empty))
            {
                Provider.SetFileMode(assets, mode);
            }
        }

        internal static MethodInfo[] GetIsOpenForEditMethods()
        {
            if (isOpenForEditMethods == null)
            {
                List<MethodInfo> list = new List<MethodInfo>();
                IEnumerator<Type> enumerator = AssetModificationProcessors.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        MethodInfo method = enumerator.Current.GetMethod("IsOpenForEdit", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                        if (method != null)
                        {
                            RequireTeamLicense();
                            string str = string.Empty;
                            bool flag = false;
                            Type[] types = new Type[] { str.GetType(), str.GetType().MakeByRefType() };
                            if (CheckArgumentTypesAndReturnType(types, method, flag.GetType()))
                            {
                                list.Add(method);
                            }
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
                isOpenForEditMethods = list.ToArray();
            }
            return isOpenForEditMethods;
        }

        internal static bool IsOpenForEdit(string assetPath, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrEmpty(assetPath))
            {
                return true;
            }
            bool flag = AssetModificationHook.IsOpenForEdit(assetPath, out message);
            foreach (MethodInfo info in GetIsOpenForEditMethods())
            {
                object[] parameters = new object[] { assetPath, message };
                if (!((bool) info.Invoke(null, parameters)))
                {
                    message = parameters[1] as string;
                    return false;
                }
            }
            return flag;
        }

        internal static void OnStatusUpdated()
        {
            WindowPending.OnStatusUpdated();
            IEnumerator<Type> enumerator = AssetModificationProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    MethodInfo method = enumerator.Current.GetMethod("OnStatusUpdated", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                    if (method != null)
                    {
                        RequireTeamLicense();
                        object[] args = new object[0];
                        if (CheckArgumentsAndReturnType(args, method, typeof(void)))
                        {
                            method.Invoke(null, args);
                        }
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        private static void OnWillCreateAsset(string path)
        {
            IEnumerator<Type> enumerator = AssetModificationProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    MethodInfo method = enumerator.Current.GetMethod("OnWillCreateAsset", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                    if (method != null)
                    {
                        object[] args = new object[] { path };
                        if (CheckArguments(args, method))
                        {
                            method.Invoke(null, args);
                        }
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            AssetDeleteResult didNotDelete = AssetDeleteResult.DidNotDelete;
            if (!InternalEditorUtility.HasTeamLicense())
            {
                return didNotDelete;
            }
            IEnumerator<Type> enumerator = AssetModificationProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    MethodInfo method = enumerator.Current.GetMethod("OnWillDeleteAsset", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                    if (method != null)
                    {
                        RequireTeamLicense();
                        object[] args = new object[] { assetPath, options };
                        if (CheckArgumentsAndReturnType(args, method, didNotDelete.GetType()))
                        {
                            didNotDelete |= (int) method.Invoke(null, args);
                        }
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            if (didNotDelete != AssetDeleteResult.DidNotDelete)
            {
                return didNotDelete;
            }
            return AssetModificationHook.OnWillDeleteAsset(assetPath, options);
        }

        private static AssetMoveResult OnWillMoveAsset(string fromPath, string toPath, string[] newPaths, string[] NewMetaPaths)
        {
            AssetMoveResult didNotMove = AssetMoveResult.DidNotMove;
            if (InternalEditorUtility.HasTeamLicense())
            {
                didNotMove = AssetModificationHook.OnWillMoveAsset(fromPath, toPath);
                IEnumerator<Type> enumerator = AssetModificationProcessors.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        MethodInfo method = enumerator.Current.GetMethod("OnWillMoveAsset", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                        if (method != null)
                        {
                            RequireTeamLicense();
                            object[] args = new object[] { fromPath, toPath };
                            if (CheckArgumentsAndReturnType(args, method, didNotMove.GetType()))
                            {
                                didNotMove |= (int) method.Invoke(null, args);
                            }
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
            return didNotMove;
        }

        private static void OnWillSaveAssets(string[] assets, out string[] assetsThatShouldBeSaved, out string[] assetsThatShouldBeReverted, int explicitlySaveScene)
        {
            assetsThatShouldBeReverted = new string[0];
            assetsThatShouldBeSaved = assets;
            bool flag = ((assets.Length > 0) && EditorPrefs.GetBool("VerifySavingAssets", false)) && InternalEditorUtility.isHumanControllingUs;
            if (((explicitlySaveScene != 0) && (assets.Length == 1)) && assets[0].EndsWith(".unity"))
            {
                flag = false;
            }
            if (flag)
            {
                AssetSaveDialog.ShowWindow(assets, out assetsThatShouldBeSaved);
            }
            else
            {
                assetsThatShouldBeSaved = assets;
            }
            IEnumerator<Type> enumerator = AssetModificationProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    MethodInfo method = enumerator.Current.GetMethod("OnWillSaveAssets", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                    if (method != null)
                    {
                        object[] args = new object[] { assetsThatShouldBeSaved };
                        if (CheckArguments(args, method))
                        {
                            string[] strArray = (string[]) method.Invoke(null, args);
                            if (strArray != null)
                            {
                                assetsThatShouldBeSaved = strArray;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            if (assetsThatShouldBeSaved != null)
            {
                List<string> list = new List<string>();
                foreach (string str in assetsThatShouldBeSaved)
                {
                    if (!AssetDatabase.IsOpenForEdit(str))
                    {
                        list.Add(str);
                    }
                }
                assets = list.ToArray();
                if ((assets.Length != 0) && !Provider.PromptAndCheckoutIfNeeded(assets, string.Empty))
                {
                    Debug.LogError("Could not check out the following files in version control before saving: " + string.Join(", ", assets));
                    assetsThatShouldBeSaved = new string[0];
                }
            }
        }

        private static void RequireTeamLicense()
        {
            if (!InternalEditorUtility.HasTeamLicense())
            {
                throw new MethodAccessException("Requires team license");
            }
        }

        private static IEnumerable<Type> AssetModificationProcessors
        {
            get
            {
                if (assetModificationProcessors == null)
                {
                    List<Type> list = new List<Type>();
                    list.AddRange(EditorAssemblies.SubclassesOf(typeof(AssetModificationProcessor)));
                    list.AddRange(EditorAssemblies.SubclassesOf(typeof(AssetModificationProcessor)));
                    assetModificationProcessors = list.ToArray();
                }
                return assetModificationProcessors;
            }
        }

        private enum FileMode
        {
            Binary,
            Text
        }
    }
}

