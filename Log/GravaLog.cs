using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Data.Entity;
using System.Reflection;
using INB.Infra.Data.Interfaces;
using INB.Infra.Data.Helpers;
using INB.Infra.Data.Util;
using System.Data.Entity.Core.Objects;

namespace INB.Infra.Data.Log
{
	internal class GravaLog<TLog, TContext> : IGravaLog<TLog, TContext>
		where TLog : class
		where TContext : DbContext
	{

		private TContext _DataContext;
		private DbSet<TLog> _DbSet;

		public GravaLog(TContext DataContext)
		{
			_DataContext = DataContext;
			_DbSet = _DataContext.Set<TLog>();
		}

		public string SerializeObject(object obj)
		{
			if (obj == null) throw new ArgumentNullException("obj");

			string Result = "";
			bool BelongToTheContext = false;
			EntityState backupState = EntityState.Unchanged;
			try
			{
				//Se o objeto não percenter ao contexto uma exception será disparada
				//para evitar referência circular
				//http://stackoverflow.com/questions/4754375/how-did-i-solve-the-json-serializing-circular-reference-error

				backupState = _DataContext.Entry(obj).State;
				_DataContext.Entry(obj).State = EntityState.Detached;
				BelongToTheContext = true;
			}
			catch (Exception ex)
			{
				var foo = ex;
			}

			Result = Converter.ObjectToString(obj);

			if (BelongToTheContext)
			{
				_DataContext.Entry(obj).State = backupState;
			}

			return Result;
		}

		public void Incluir(TLog oLog)
		{
			_DbSet.Add(oLog);
			//_DataContext.Entry<TLog>(oLog).State = EntityState.Added;
		}

		public void Incluir(int CodigoSistema, int Usuario, eTipoLog TipoLog, string NomeEstacao, string Entidade, string IP, string TextoLog, string TextoSQL, int? UsuarioAutenticado = null)
		{
			TLog obj = _DbSet.Create();
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
					Entidade = (string) Objeto;
				}
				else {
					Entidade = Objeto.GetType().ToString(); 
				}

				TLog obj = _DbSet.Create();
				Type Myobj = obj.GetType();
				var PropBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
				PropertyInfo LOUsuario, LOCodigoTipoLog, LONomeEstacao, LODataHora, LOEntidade, LOIPRede, LOLog, LOSql, LOUsuarioAutenticado, LOCodigoSistema;

				LOCodigoSistema = Myobj.GetProperty("LOCodigoSistema", PropBindingFlags);
				VerifyProperty(LOCodigoSistema, "LOCodigoSistema");
				LOCodigoSistema.SetValue(obj, CodigoSistema, null);

				LOUsuario = Myobj.GetProperty("LOUsuario", PropBindingFlags);
				VerifyProperty(LOUsuario, "LOUsuario");
				LOUsuario.SetValue(obj, Usuario, null);

                LOCodigoTipoLog = Myobj.GetProperty("LOCodigoTipoLOG", PropBindingFlags);
                VerifyProperty(LOCodigoTipoLog, "LOCodigoTipoLOG");
				LOCodigoTipoLog.SetValue(obj, TipoLog, null);

				LONomeEstacao = Myobj.GetProperty("LONomeEstacao", PropBindingFlags);
				VerifyProperty(LONomeEstacao, "LONomeEstacao");
				LONomeEstacao.SetValue(obj, NomeEstacao, null);

				LODataHora = Myobj.GetProperty("LODataHora", PropBindingFlags);
				VerifyProperty(LODataHora, "LODataHora");
				LODataHora.SetValue(obj, DateTime.Now, null);

                LOEntidade = Myobj.GetProperty("LoEntidade", PropBindingFlags);
                VerifyProperty(LOEntidade, "LoEntidade");
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

		//	TLog obj = _DbSet.Create();
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
				throw new Exception(string.Format("Propriedade {0} ausente. Verifique a tabela no banco ou entity model (edmx). Eles podem estar desatualizados.", name));
		}
	}
}
