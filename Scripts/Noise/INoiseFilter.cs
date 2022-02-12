/*
Copyright (c) 2018 Sebastian Lague
*/

using UnityEngine;

namespace Noise
{
    public interface INoiseFilter
    {
        float Evaluate(Vector3 point);
    }
}