using INB.Infra.Data.LinqToSQL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace INB.Infra.Data.LinqToSQL.Log
{
	public class LogBase<TLog, TContext> : ILog<TLog, TContext>, IDisposable
		where TLog : class, new()
		where TContext : System.Data.Linq.DataContext
	{
		private INB.Infra.Data.Interfaces.IIdentification _Identification;
		private TContext _DataContext;
		private GravaLog<TLog, TContext> _GravaLog;
		private StreamWriter _sw;
		private MemoryStream _ms;
		private bool _SaveSQL;

		public LogBase(TContext DataContext, INB.Infra.Data.Interfaces.IIdentification Identification)
		{
			_DataContext = DataContext;
			_Identification = Identification;
			_GravaLog = new GravaLog<TLog, TContext>(_DataContext);
		}

		public LogBase(TContext DataContext, INB.Infra.Data.Interfaces.IIdentification Identification, bool SaveSQL)
			: this(DataContext, Identification)
		{
			_SaveSQL = SaveSQL;

			if (SaveSQL)
			{
				_ms = new MemoryStream();
				_sw = new StreamWriter(_ms);
				_DataContext.Log = _sw;
			}
		}

		/// <summary>
		/// Executa a procedure spu_LogTransfereHistorico.
		/// </summary>
		/// <param name="Dias"></param>
		public void GerarHistorico(int Dias)
		{
			_DataContext.ExecuteCommand(string.Format("exec spu_LogTransfereHistorico {0};", Dias));
		}

		public void RegistrarSerializa(eTipoLog TipoLog, object pObjeto)
		{
			if (_SaveSQL)
			{
				
				_DataContext.Log.Flush(); //descarrega os dados no memoryStream
				_ms.Position = 0; //posiciona o cursor no início para iniciar a leitura
				StreamReader sr = new StreamReader(_ms);	//Não use dispose no StreamReader ou ele irá matar o MemoryStream
				string serializedSQL = sr.ReadToEnd();
				string serializedObject = _GravaLog.SerializeObject(pObjeto);
				_ms.Position = 0; //reinicia o tamanho novamente
				_ms.SetLength(0); //limpa o memoryStream

				string Entidade;
				if (pObjeto is Type)
				{
					Entidade = pObjeto.ToString();
				}
				else if (pObjeto is string)
				{
					Entidade = (string)pObjeto;
				}
				else
				{
					Entidade = pObjeto.GetType().ToString();
				}

				_GravaLog.Incluir(_Identification.CodigoSistema, _Identification.Usuario, TipoLog, _Identification.NomeEstacao, Entidade, _Identification.IP, serializedObject, serializedSQL, _Identification.UsuarioAutenticado);
			}
			else
			{
				_GravaLog.IncluirSerializa(_Identification.CodigoSistema, _Identification.Usuario, TipoLog, _Identification.NomeEstacao, _Identification.IP, pObjeto, _Identification.UsuarioAutenticado);
			}
		}

		public void Registrar(eTipoLog TipoLog, object pObjeto, string pTextoLog, string pTextoSQL = "")
		{
			if (_SaveSQL & string.IsNullOrWhiteSpace(pTextoSQL))
			{
				_DataContext.Log.Flush();
				_ms.Position = 0;
				StreamReader sr = new StreamReader(_ms);	//Não use dispose no StreamReader ou ele irá matar o MemoryStream
				pTextoSQL = sr.ReadToEnd();
				_ms.Position = 0; //reinicia o tamanho novamente
				_ms.SetLength(0); //limpa o memoryStream
			}

			string Entidade = "";
			if (pObjeto is Type)
			{
				Entidade = pObjeto.ToString();
			}
			else if (pObjeto is string)
			{
				Entidade = (string)pObjeto;
			}
			else
			{
				if (pObjeto != null)
				{
					Entidade = pObjeto.GetType().ToString();
				}
			}

			_GravaLog.Incluir(_Identification.CodigoSistema, _Identification.Usuario, TipoLog, _Identification.NomeEstacao, Entidade, _Identification.IP, pTextoLog, pTextoSQL, _Identification.UsuarioAutenticado);
		}

		public void Registrar(eTipoLog TipoLog, object pObjeto)
		{
			Registrar(TipoLog, pObjeto);
		}

		public void RegistrarIncluir(object pObjeto, string pTextoLog)
		{
			Registrar(eTipoLog.Insert, pObjeto, pTextoLog);
		}

		public void RegistrarIncluirSerializa(object pObjeto)
		{
			RegistrarSerializa(eTipoLog.Insert, pObjeto);
		}

		public void RegistrarAlterar(object pObjeto, string pTextoLog)
		{
			Registrar(eTipoLog.Update, pObjeto, pTextoLog);
		}

		public void RegistrarAlterarSerializa(object pObjeto)
		{
			RegistrarSerializa(eTipoLog.Update, pObjeto);
		}

		public void RegistrarExcluir(object pObjeto, string pTextoLog)
		{
			Registrar(eTipoLog.Delete, pObjeto, pTextoLog);
		}

		public void RegistrarExcluirSerializa(object pObjeto)
		{
			RegistrarSerializa(eTipoLog.Delete, pObjeto);
		}



		public void Dispose()
		{
			try
			{
				if (_ms != null)
					_ms.Dispose();

				if (_sw != null)
					_sw.Dispose();
								
			}
			catch (Exception ex) { }
		}
	}
}
