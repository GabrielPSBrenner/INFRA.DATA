using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INB.Infra.Data.LinqToSQL.Interfaces
{
	internal interface IGravaLog<TLog, TContext>
		where TLog : class
		where TContext : System.Data.Linq.DataContext
	{
		string SerializeObject(object obj);
		void Incluir(TLog oLog);
		void Incluir(int CodigoSistema, int Usuario, eTipoLog TipoLog, string NomeEstacao, string Entidade, string IP, string TextoLog, string TextoSQL, int? UsuarioAutenticado = null);
		void IncluirSerializa(int CodigoSistema, int Usuario, eTipoLog TipoLog, string NomeEstacao, string IP, object Objeto, int? UsuarioAutenticado = null);
		//void IncluirSerializaEntity(int CodigoSistema, int Usuario, eTipoLog TipoLog, string NomeEstacao, string IP, object Objeto, int? UsuarioAutenticado = null);
	}
}
