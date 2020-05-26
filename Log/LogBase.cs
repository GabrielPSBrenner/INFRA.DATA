using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Runtime.Serialization;
using INB.Infra.Data;
using INB.Infra.Data.Interfaces;
using System.IO;

namespace INB.Infra.Data.Log
{
    public class LogBase<TLog, TContext>:  ILog<TLog, TContext>
        where TLog : class
        where TContext : DbContext
    {
        private IIdentification _Identification;
        private TContext _DataContext;
        private GravaLog<TLog,TContext> _GravaLog;
		private bool _SaveSQL;
		private StringBuilder _sbLog;

		public LogBase(TContext DataContext, IIdentification Identification)
        {
            _DataContext = DataContext;
            _Identification = Identification;
			_GravaLog = new GravaLog<TLog, TContext>(_DataContext);

        }

		public LogBase(TContext DataContext, IIdentification Identification, bool SaveSQL)
			: this(DataContext, Identification)
		{
			_SaveSQL = SaveSQL;

			if (SaveSQL)
			{
				_sbLog = new StringBuilder();
				_DataContext.Database.Log = message =>
				{
					_sbLog.AppendLine(message);
				};
			}
		}

		/// <summary>
		/// Executa a procedure spu_LogTransfereHistorico.
		/// </summary>
		/// <param name="Dias"></param>
		public void GerarHistorico(int Dias)
		{
			_DataContext.Database.ExecuteSqlCommand(string.Format("exec spu_LogTransfereHistorico {0};", Dias));
		}

        public void RegistrarSerializa(eTipoLog TipoLog, object pObjeto)
        {
			if (_SaveSQL & _sbLog != null)
			{
				string serializedSQL = _sbLog.ToString();
				string serializedObject = _GravaLog.SerializeObject(pObjeto);
				_sbLog.Clear();

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
            //_GravaLog.IncluirSerializa(_Identification.CodigoSistema, _Identification.Usuario, TipoLog, _Identification.NomeEstacao, _Identification.IP, pObjeto, _Identification.UsuarioAutenticado);
        }

        public void Registrar(eTipoLog TipoLog, object pObjeto, string pTextoLog, string pTextoSQL = "")
        {
			if (_SaveSQL & string.IsNullOrWhiteSpace(pTextoSQL))
			{
				
				pTextoSQL = _sbLog.ToString();
				_sbLog.Clear();
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
            //_GravaLog.Incluir(_Identification.CodigoSistema, _Identification.Usuario, TipoLog, _Identification.NomeEstacao, pObjeto.GetType().ToString(), _Identification.IP, pTextoLog, pTextoSQL, _Identification.UsuarioAutenticado);
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

    }
}
