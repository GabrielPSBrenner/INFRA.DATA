using INB.Infra.Data.Helpers;
using INB.Infra.Data.LinqToSQL.Interfaces;
using INB.Infra.Data.Util;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace INB.Infra.Data.LinqToSQL.Log
{
	internal class GravaLog<TLog, TContext> : IGravaLog<TLog, TContext>
		where TLog : class, new()
		where TContext : System.Data.Linq.DataContext
	{

		private TContext _DataContext;
		private Table<TLog> _DbSet;

		public GravaLog(TContext DataContext)
		{
			_DataContext = DataContext;
			_DbSet = _DataContext.GetTable<TLog>();
		}

		public string SerializeObject(object obj)
		{
			if (obj == null) throw new ArgumentNullException("obj");

			string Result = "";
			
			Result = Converter.ObjectToString(obj);

			return Result;
		}

		public void Incluir(TLog oLog)
		{
			_DbSet.InsertOnSubmit(oLog);
			//_DataContext.Entry<TLog>(oLog).State = EntityState.Added;
			
		}

		internal T Create<T>() where T : class, new()
		{
			T val = new T();
			return val;
		}

		public void Incluir(int CodigoSistema, int Usuario, eTipoLog TipoLog, string NomeEstacao, string Entidade, string IP, string TextoLog, string TextoSQL, int? UsuarioAutenticado = null)
		{
			var obj = new TLog();
			Type Myobj = obj.GetType();
			var PropBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
			PropertyInfo LOUsuario, LOCodigoTipoLog, LONomeEstacao, LODataHora, LOEntidade, LOIPRede, LOLog, LOSql, LOUsuarioAutenticado, LOCodigoSistema;

			LOCodigoSistema = Myobj.GetProperty("LOCodigoSistema", PropBindingFlags);
			VerifyProperty(LOCodigoSistema, "LOCodigoSistema");
			LOCodigoSistema.SetValue(obj, CodigoSistema, null);

			LOUsuario = Myobj.GetProperty("LOUsuario", PropBindingFlags);
			VerifyProperty(LOUsuario, "LOUsuario");
			LOUsuario.SetValue(obj, Usuario, null);

			LOCodigoTipoLog = Myobj.GetProperty("LOCodigoTipoLog", PropBindingFlags);
			VerifyProperty(LOCodigoTipoLog, "LOCodigoTipoLog");
			LOCodigoTipoLog.SetValue(obj, TipoLog, null);

			LONomeEstacao = Myobj.GetProperty("LONomeEstacao", PropBindingFlags);
			VerifyProperty(LONomeEstacao, "LONomeEstacao");
			LONomeEstacao.SetValue(obj, NomeEstacao, null);

			LODataHora = Myobj.GetProperty("LODataHora", PropBindingFlags);
			VerifyProperty(LODataHora, "LODataHora");
			LODataHora.SetValue(obj, DateTime.Now, null);

			LOEntidade = Myobj.GetProperty("LOEntidade", PropBindingFlags);
			VerifyProperty(LOEntidade, "LOEntidade");
			LOEntidade.SetValue(obj, Entidade, null);

			LOIPRede = Myobj.GetProperty("LOIPRede", PropBindingFlags);
			VerifyProperty(LOIPRede, "LOIPRede");
			LOIPRede.SetValue(obj, IP, null);

			LOLog = Myobj.GetProperty("LOLog", PropBindingFlags);
			VerifyProperty(LOLog, "LOLog");
			LOLog.SetValue(obj, TextoLog, null);

			LOSql = Myobj.GetProperty("LOSql", PropBindingFlags);
			VerifyProperty(LOSql, "LOSql");
			LOSql.SetValue(obj, TextoSQL, null);

			LOUsuarioAutenticado = Myobj.GetProperty("LOUsuarioAutenticado", PropBindingFlags);
			VerifyProperty(LOUsuarioAutenticado, "LOUsuarioAutenticado");
			LOUsuarioAutenticado.SetValue(obj, UsuarioAutenticado, null);

			Incluir((TLog)obj);
		}

		public void IncluirSerializa(int CodigoSistema, int Usuario, eTipoLog TipoLog, string NomeEstacao, string IP, object Objeto, int? UsuarioAutenticado = null)
		{
			if (Objeto != null)
			{
				string Entidade;
				if (Objeto is Type)
				{
					Entidade = Objeto.ToString();
				}
				else if (Objeto is string)
				{
					Entidade = (string)Objeto;
				}
				else
				{
					Entidade = Objeto.GetType().ToString();
				}

				var obj = new TLog();
				Type Myobj = obj.GetType();

				var PropBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
				PropertyInfo LOUsuario, LOCodigoTipoLog, LONomeEstacao, LODataHora, LOEntidade, LOIPRede, LOLog, LOSql, LOUsuarioAutenticado, LOCodigoSistema;


				LOCodigoSistema = Myobj.GetProperty("LOCodigoSistema", PropBindingFlags);
				VerifyProperty(LOCodigoSistema, "LOCodigoSistema");
				LOCodigoSistema.SetValue(obj, CodigoSistema, null);

				LOUsuario = Myobj.GetProperty("LOUsuario", PropBindingFlags);
				VerifyProperty(LOUsuario, "LOUsuario");
				LOUsuario.SetValue(obj, Usuario, null);

				LOCodigoTipoLog = Myobj.GetProperty("LOCodigoTipoLog", PropBindingFlags);
				VerifyProperty(LOCodigoTipoLog, "LOCodigoTipoLog");
				LOCodigoTipoLog.SetValue(obj, TipoLog, null);

				LONomeEstacao = Myobj.GetProperty("LONomeEstacao", PropBindingFlags);
				VerifyProperty(LONomeEstacao, "LONomeEstacao");
				LONomeEstacao.SetValue(obj, NomeEstacao, null);

				LODataHora = Myobj.GetProperty("LODataHora", PropBindingFlags);
				VerifyProperty(LODataHora, "LODataHora");
				LODataHora.SetValue(obj, DateTime.Now, null);

				LOEntidade = Myobj.GetProperty("LOEntidade", PropBindingFlags);
				VerifyProperty(LOEntidade, "LOEntidade");
				LOEntidade.SetValue(obj, Entidade, null);

				LOIPRede = Myobj.GetProperty("LOIPRede", PropBindingFlags);
				VerifyProperty(LOIPRede, "LOIPRede");
				LOIPRede.SetValue(obj, IP, null);

				LOLog = Myobj.GetProperty("LOLog", PropBindingFlags);
				VerifyProperty(LOLog, "LOLog");
				LOLog.SetValue(obj, SerializeObject(Objeto), null);

				LOSql = Myobj.GetProperty("LOSql", PropBindingFlags);
				VerifyProperty(LOSql, "LOSql");
				LOSql.SetValue(obj, "", null);

				LOUsuarioAutenticado = Myobj.GetProperty("LOUsuarioAutenticado", PropBindingFlags);
				VerifyProperty(LOUsuarioAutenticado, "LOUsuarioAutenticado");
				LOUsuarioAutenticado.SetValue(obj, UsuarioAutenticado, null);

				Incluir((TLog)obj);
			}
		}

		//public void IncluirSerializaEntity(int CodigoSistema, int Usuario, eTipoLog TipoLog, string NomeEstacao, string IP, object Objeto, int? UsuarioAutenticado = null)
		//{
		//	string Entidade = Objeto.GetType().ToString();

		//	var obj = new TLog();
		//	Type Myobj = obj.GetType();

		//	PropertyInfo LOUsuario, LOCodigoTipoLog, LONomeEstacao, LODataHora, LOEntidade, LOIPRede, LOLog, LOSql, LOUsuarioAutenticado, LOCodigoSistema;

		//	LOCodigoSistema = Myobj.GetProperty("LOCodigoSistema", BindingFlags.Public | BindingFlags.Instance);
		//	VerifyProperty(LOCodigoSistema, "LOCodigoSistema");
		//	LOCodigoSistema.SetValue(obj, CodigoSistema, null);

		//	LOUsuario = Myobj.GetProperty("LOUsuario", BindingFlags.Public | BindingFlags.Instance);
		//	VerifyProperty(LOUsuario, "LOUsuario");
		//	LOUsuario.SetValue(obj, Usuario, null);

		//	LOCodigoTipoLog = Myobj.GetProperty("LOCodigoTipoLog", BindingFlags.Public | BindingFlags.Instance);
		//	VerifyProperty(LOCodigoTipoLog, "LOCodigoTipoLog");
		//	LOCodigoTipoLog.SetValue(obj, TipoLog, null);

		//	LONomeEstacao = Myobj.GetProperty("LONomeEstacao", BindingFlags.Public | BindingFlags.Instance);
		//	VerifyProperty(LONomeEstacao, "LONomeEstacao");
		//	LONomeEstacao.SetValue(obj, NomeEstacao, null);

		//	LODataHora = Myobj.GetProperty("LODataHora", BindingFlags.Public | BindingFlags.Instance);
		//	VerifyProperty(LODataHora, "LODataHora");
		//	LODataHora.SetValue(obj, DateTime.Now, null);

		//	LOEntidade = Myobj.GetProperty("LOEntidade", BindingFlags.Public | BindingFlags.Instance);
		//	VerifyProperty(LOEntidade, "LOEntidade");
		//	LOEntidade.SetValue(obj, Entidade, null);

		//	LOIPRede = Myobj.GetProperty("LOIPRede", BindingFlags.Public | BindingFlags.Instance);
		//	VerifyProperty(LOIPRede, "LOIPRede");
		//	LOIPRede.SetValue(obj, IP, null);

		//	LOLog = Myobj.GetProperty("LOLog", BindingFlags.Public | BindingFlags.Instance);
		//	VerifyProperty(LOLog, "LOLog");
		//	LOLog.SetValue(obj, ObjectToXmlString(Objeto), null);

		//	LOSql = Myobj.GetProperty("LOSql", BindingFlags.Public | BindingFlags.Instance);
		//	VerifyProperty(LOSql, "LOSql");
		//	LOSql.SetValue(obj, "", null);

		//	LOUsuarioAutenticado = Myobj.GetProperty("LOUsuarioAutenticado", BindingFlags.Public | BindingFlags.Instance);
		//	VerifyProperty(LOUsuarioAutenticado, "LOUsuarioAutenticado");
		//	LOUsuarioAutenticado.SetValue(obj, UsuarioAutenticado, null);

		//	Incluir((TLog)obj);
		//}

		private void VerifyProperty(object propInfo, string name)
		{
			if (propInfo == null)
				throw new Exception(string.Format("Propriedade {0} ausente. Verifique a tabela no banco ou entity model (dbml). Eles podem estar desatualizados.", name));
		}
	}
}
