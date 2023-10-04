using System.Windows.Forms;
using System;
using System.Security.Cryptography.X509Certificates;

namespace HostApp
{
    /// <summary>
    /// Creates instances of the Control, simply using Activator.CreateInstance
    /// </summary>
    public abstract class MappingControlFactory
    {
        public string Title { get; private set; }
        public int Order { get; private set; }

        public MappingControlFactory(string title, int order)
        {
            Title = title;
            Order = order;
        }
        
        public virtual T CreateInstance<T>() where T : Control
        {
            return (T)Activator.CreateInstance(ToType());
        }

        public abstract Type ToType();
        public abstract string GetTypeName();
    }

    /// <summary>
    /// Creates instances of the Control, simply using Activator.CreateInstance
    /// </summary>
    public class TypeMappingControlFactory : MappingControlFactory
    {
        private Type controlType;

        public TypeMappingControlFactory(string title, int order, Type controlType)
            :base(title, order)
        {
            this.controlType = controlType;
        }

        public override Type ToType()
        {
            return controlType;
        }

        public override string GetTypeName()
        {
            return controlType.FullName;
        }
    }

    /// <summary>
    /// Creates the Control using the specified AppDomain.
    /// </summary>
    internal class PluginMappingControlFactory : MappingControlFactory
    {
        private PluginHostProxy pluginHost;
        private string controlTypeName;

        internal PluginMappingControlFactory(string title, int order, PluginHostProxy pluginHost, string controlTypeName)
            : base(title, order)
        {
            this.pluginHost = pluginHost;
            this.controlTypeName = controlTypeName;
        }

        public override T CreateInstance<T>()
        {
            return (T)pluginHost.CreateControl(controlTypeName);
        }

        /// <summary>
        /// Since we can't get the Type in the other appdomain, we return the host/contract class it derives from instead.
        /// </summary>
        /// <returns></returns>
        public override Type ToType()
        {
            throw new InvalidOperationException("Cannot get Type in other appdomain.");
        }

        public override string GetTypeName()
        {
            return controlTypeName;
        }
    }
}