using INB.Infra.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INB.Infra.Data.LinqToSQL.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TContext"></typeparam>
	/// <typeparam name="TLog"></typeparam>
	public interface IRepositoryBase<TEntity, TContext, TLog>
		where TEntity : class
		where TContext : System.Data.Linq.DataContext
		where TLog : class
	{
		/// <summary>
		/// Insere o registro no banco.
		/// </summary>
		/// <param name="obj">Objeto a ser persistido.</param>
		/// <returns></returns>
		TEntity Incluir(TEntity obj);

		/// <summary>
		/// Persiste o objeto alterado no banco.
		/// </summary>
		/// <param name="obj">Objeto a ser persistido.</param>
		void Alterar(TEntity obj);

		/// <summary>
		/// Exclui o registro do banco.
		/// </summary>
		/// <param name="obj">Objeto com identificador referente ao registro a ser excluído.</param>
		void Excluir(TEntity obj);

		/// <summary>
		///  Exclui o registro do banco utilizando chave primária.
		/// </summary>
		/// <param name="keyvalues">Identificador.</param>
		//void Excluir(params object[] keyvalues);
		////TEntity SelecionarChave(params object[] keyvalues);
		////List<TEntity> SelecionarTodos();
		////List<TEntity> SelecionarTodosNoTracking();

		/// <summary>
		/// 
		/// </summary>
		TContext DataContext { get; }
		IIdentification UserIdentification { get; }
	}
}
