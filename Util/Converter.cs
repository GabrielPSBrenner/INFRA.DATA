using INB.Infra.Data.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace INB.Infra.Data.Util
{
	public class Converter
	{

		/// <summary>
		/// Serializa o objeto para JSON. Se não for possível tentará serializar para XML.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string ObjectToString(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			try
			{
				return ObjectToJSON(obj);
			}
			catch (Exception ex)
			{
				var foo = ex;
				return ObjectToXML(obj);
			}
		}

		/// <summary>
		/// Serializa o objeto em Json String. Possui tratamento contra referência circular.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string ObjectToJSON(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");
			
			//var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
			
			//MemoryStream ms = new MemoryStream();
			//serializer.WriteObject(ms, obj);
			//string json = Encoding.UTF8.GetString(ms.ToArray());
			//return json.ToString();

			var objToSerialize = Activator.CreateInstance(obj.GetType());
			Util.CopyObject.ReflectObject(objToSerialize, obj); //Evita erro de referência circular

			return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(objToSerialize);
		}

		public static string ObjectToXML(object obj)
		{

			var serializer = new DataContractSerializer(obj.GetType(), null, Int32.MaxValue, false, false, null, new AllowAllContractResolver());
			var sb = new StringBuilder();
			using (var xw = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = true, NamespaceHandling = NamespaceHandling.OmitDuplicates, Indent = true }))
			{
				serializer.WriteObject(xw, obj);
				xw.Flush();
				return sb.ToString();
			}

		}


	}
}
