namespace UnityEngineInternal
{
    using System;

    [Serializable, AttributeUsage(AttributeTargets.Method)]
    public class TypeInferenceRuleAttribute : Attribute
    {
        private readonly string _rule;

        public TypeInferenceRuleAttribute(string rule)
        {
            this._rule = rule;
        }

        public TypeInferenceRuleAttribute(TypeInferenceRules rule) : this(rule.ToString())
        {
        }

        public override string ToString()
        {
            return this._rule;
        }
    }
}

