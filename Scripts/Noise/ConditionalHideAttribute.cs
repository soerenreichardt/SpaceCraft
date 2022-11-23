/*
Copyright (c) 2018 Sebastian Lague
*/

using System;
using UnityEngine;

namespace Noise
{
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        public readonly string conditionalSourceField;
        public readonly int enumIndex;

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