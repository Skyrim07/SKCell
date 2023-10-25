using System;

namespace SKCell
{
    public class SKVariableMonitor<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Called when the value of the monitored variable changes.
        /// </summary>
        public Action<T> onValueChanged;

        private T _currentValue;
        private Func<T> _valueProvider;

        /// <summary>
        /// Create a value monitor. 
        /// <para>Example: SKVariableMonitor m = new SKVariableMonitor(()=>myInt);</para>
        /// </summary>
        /// <param name="valueProvider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SKVariableMonitor(Func<T> valueProvider)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));

            _valueProvider = valueProvider;
            _currentValue = _valueProvider();

            SKCore.Tick000 += MonitorUpdate;
        }

        public T GetValue()
        {
            return _valueProvider();
        }
        public void Dispose()
        {
            SKCore.Tick000 -= MonitorUpdate;
        }

        private void MonitorUpdate()
        {
            T newValue = _valueProvider();

            if (!_currentValue.Equals(newValue))
            {
                _currentValue = newValue;
                onValueChanged?.Invoke(newValue);
            }
        }
    }
}