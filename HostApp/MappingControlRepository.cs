using SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HostApp
{
    public static class MappingControlRepository
    {
        private static Dictionary<string, MappingControlFactory> mappingControls;

        /// <summary>
        /// Check all types for the MappingControl attribute and store them for later use. Optionally merging it with additional Mapping Controls.
        /// </summary>
        /// <param name="additionalMappingControlFactories"></param>
        public static void Buffer(IReadOnlyList<MappingControlFactory> additionalMappingControlFactories = null)
        {
            mappingControls = new Dictionary<string, MappingControlFactory>();

            // Search internally for controls.
            foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()))
            {
                var attribute = type.GetCustomAttribute<MappingControlAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                mappingControls.Add(type.FullName, new TypeMappingControlFactory(attribute.Title, attribute.Order, type));
            }

            if (additionalMappingControlFactories == null)
            {
                return;
            }

            foreach (var mappingControlFactory in additionalMappingControlFactories)
            {
                var fullTypeName = mappingControlFactory.GetTypeName();
                if (mappingControls.ContainsKey(fullTypeName))
                {
                    Console.WriteLine("Mapping Control {0} already exists in the action buffer. Overwriting.", fullTypeName);
                }

                mappingControls.Add(fullTypeName, mappingControlFactory);
            }
        }

        /// <summary>
        /// Gets all mapping controls and their targetted typename
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, MappingControlFactory> GetAllMappingControls()
        {
            return mappingControls;
        }

        /// <summary>
        /// Gets a specific mapping control factory by its type name
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        public static MappingControlFactory GetMappingControlFactory(string typeFullName)
        {
            if (mappingControls.TryGetValue(typeFullName, out var mappingControlFactory))
            {
                return mappingControlFactory;
            }

            return null;
        }
    }
}
