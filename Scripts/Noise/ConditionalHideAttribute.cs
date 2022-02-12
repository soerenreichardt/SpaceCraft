/*
Copyright (c) 2018 Sebastian Lague
*/

using System;
using UnityEngine;

namespace Noise
{
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct,
        Inherited = true)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        public string conditionalSourceField;
        public int enumIndex;

        public ConditionalHideAttribute(string boolVariableName)
        {
            this.conditionalSourceField = boolVariableName;
        }

        public ConditionalHideAttribute(string enumVariableName, int enumIndex)
        {
            this.conditionalSourceField = enumVariableName;
            this.enumIndex = enumIndex;
        }
    }
}