namespace UnityEngine.Internal
{
    using System;

    [Serializable, AttributeUsage(AttributeTargets.GenericParameter | AttributeTargets.Parameter)]
    public class DefaultValueAttribute : Attribute
    {
        private object DefaultValue;

        public DefaultValueAttribute(string value)
        {
            this.DefaultValue = value;
        }

        public override bool Equals(object obj)
        {
            DefaultValueAttribute attribute = obj as DefaultValueAttribute;
            if (attribute == null)
            {
                return false;
            }
            if (this.DefaultValue == null)
            {
                return (attribute.Value == null);
            }
            return this.DefaultValue.Equals(attribute.Value);
        }

        public override int GetHashCode()
        {
            if (this.DefaultValue == null)
            {
                return base.GetHashCode();
            }
            return this.DefaultValue.GetHashCode();
        }

        public object Value
        {
            get
            {
                return this.DefaultValue;
            }
        }
    }
}

