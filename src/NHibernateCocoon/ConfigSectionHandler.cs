using System;
using System.Configuration;
using System.Xml;

namespace NHibernateCocoon
{
	/// <summary>
    /// This is the configuration section handler for NHibernateCocoon.
	/// </summary>
	public class ConfigSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
        /// Creates the configuration data for NHibernateCocoon.
		/// </summary>
		/// <param name="parent">The config parent.</param>
		/// <param name="configContext">The config context.</param>
		/// <param name="section">The config section.</param>
        /// <returns>The configuration data for Cocoon.</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			var fluentMappingAssemblyName = GetFluentMappingAssemblyName(section);
			var transactionScope = GetTransactionScope(section);

			return new Config
			{
				FluentMappingAssemblyName = fluentMappingAssemblyName,
				TransactionScope = transactionScope
			};
		}

		private string GetFluentMappingAssemblyName(XmlNode section)
		{
			var node = section.SelectSingleNode("fluentMapping");
			if (node == null)
			{
                throw new ConfigurationErrorsException("The nhibernateCocoon section does not have a fluentMapping node.");
			}

			if (node.Attributes == null)
			{
				throw new ConfigurationErrorsException("The fluentMapping node does not have any attributes.");
			}

			var assembly = node.Attributes["assembly"];
			if (assembly == null)
			{
				throw new ConfigurationErrorsException("The fluentMapping node does not have an assembly attribute.");
			}

			if (String.IsNullOrEmpty(assembly.Value))
			{
				throw new ConfigurationErrorsException("The fluentMapping assembly attribute does not have a value.");
			}

			return assembly.Value;
		}

		private string GetTransactionScope(XmlNode section)
		{
			// The default transaction scope is request
			var transactionScope = "request";

			var node = section.SelectSingleNode("transactionScope");
			if (node != null)
			{
				if (node.Attributes == null)
				{
					throw new ConfigurationErrorsException("The transactionScope node does not have any attributes.");
				}

				var value = node.Attributes["value"];
				if (value == null)
				{
					throw new ConfigurationErrorsException("The transactionScope node does not have a value attribute.");
				}

				if (!String.IsNullOrEmpty(value.Value) && value.Value.ToLower() == "method")
				{
					transactionScope = "method";
				}
			}

			return transactionScope;
		}
	}
}
