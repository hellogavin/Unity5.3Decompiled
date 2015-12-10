namespace UnityEditor.Sprites
{
    using System;

    internal class TightPackerPolicy : DefaultPackerPolicy
    {
        protected override bool AllowRotationFlipping
        {
            get
            {
                return false;
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

