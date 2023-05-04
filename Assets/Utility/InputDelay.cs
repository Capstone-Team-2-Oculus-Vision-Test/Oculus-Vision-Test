using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility
{
    
    public class InputDelay
    {
        private float _minDelay = float.MaxValue;
        private float _maxDelay = float.MinValue;
        private const int MaxSamples = 10;
        private readonly List<float> _samples = new();
        
        public float UpdateMinMax(float delay)
        {
            _samples.Add(delay);
            if (_samples.Count > MaxSamples)
            {
                _samples.RemoveAt(0);
            }
            float averageDelay = _samples.Average();
            _minDelay = Mathf.Min(_minDelay, delay - averageDelay);
            _maxDelay = Mathf.Max(_maxDelay, delay - averageDelay);
            Debug.Log("Min delay: " + _minDelay + " Max delay: " + _maxDelay);
            return Mathf.Clamp(averageDelay, _minDelay, _maxDelay);
        }
    }
}