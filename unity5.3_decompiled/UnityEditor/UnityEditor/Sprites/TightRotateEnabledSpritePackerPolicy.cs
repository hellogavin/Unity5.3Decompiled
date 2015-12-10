namespace UnityEditor.Sprites
{
    using System;

    internal class TightRotateEnabledSpritePackerPolicy : DefaultPackerPolicy
    {
        protected override bool AllowRotationFlipping
        {
            get
            {
                return true;
            }
        }

        protected override bool AllowTightWhenTagged
        {
            get
            {
                return false;
            }
        }

        protected override string TagPrefix
        {
            get
            {
                return "[RECT]";
            }
        }
    }
}

