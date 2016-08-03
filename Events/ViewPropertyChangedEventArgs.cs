using System;

namespace FeatherMvvm.Events
{
    public class ViewPropertyChangedEventArgs : EventArgs
    {
        public ViewPropertyChangedEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }
    }
}