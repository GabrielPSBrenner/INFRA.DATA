using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INB.Infra.Data.LinqToSQL.Interfaces
{
	public interface ILog<TLog, TContext>
		where TLog : class
		where TContext : System.Data.Linq.DataContext
	{
		void GerarHistorico(int Dias);
		void RegistrarSerializa(eTipoLog TipoLog, object pObjeto);
		void Registrar(eTipoLog TipoLog, object pObjeto, string pTextoLog, string pTextoSQL = "");
		void Registrar(eTipoLog TipoLog, object pObjeto);
		void RegistrarIncluir(object pObjeto, string pTextoLog);
		void RegistrarIncluirSerializa(object pObjeto);
		void RegistrarAlterar(object pObjeto, string pTextoLog);
		void RegistrarAlterarSerializa(object pObjeto);
		void RegistrarExcluir(object pObjeto, string pTextoLog);
		void RegistrarExcluirSerializa(object pObjeto);
		
	}
}
