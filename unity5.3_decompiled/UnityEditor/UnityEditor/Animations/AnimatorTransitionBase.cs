namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    public class AnimatorTransitionBase : Object
    {
        private PushUndoIfNeeded undoHandler = new PushUndoIfNeeded(true);

        public void AddCondition(AnimatorConditionMode mode, float threshold, string parameter)
        {
            this.undoHandler.DoUndo(this, "Condition added");
            AnimatorCondition[] conditions = this.conditions;
            AnimatorCondition item = new AnimatorCondition {
                mode = mode,
                parameter = parameter,
                threshold = threshold
            };
            ArrayUtility.Add<AnimatorCondition>(ref conditions, item);
            this.conditions = conditions;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string BuildTransitionName(string source, string destination);
        public string GetDisplayName(Object source)
        {
            return (!(source is AnimatorState) ? this.GetDisplayNameStateMachineSource(source as AnimatorStateMachine) : this.GetDisplayNameStateSource(source as AnimatorState));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern string GetDisplayNameStateMachineSource(AnimatorStateMachine source);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern string GetDisplayNameStateSource(AnimatorState source);
        public void RemoveCondition(AnimatorCondition condition)
        {
            this.undoHandler.DoUndo(this, "Condition removed");
            AnimatorCondition[] conditions = this.conditions;
            ArrayUtility.Remove<AnimatorCondition>(ref conditions, condition);
            this.conditions = conditions;
        }

        public AnimatorCondition[] conditions { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public AnimatorState destinationState { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public AnimatorStateMachine destinationStateMachine { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isExit { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool mute { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal bool pushUndo
        {
            set
            {
                this.undoHandler.pushUndo = value;
            }
        }

        public bool solo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

