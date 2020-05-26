using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using INB.Infra.Data.Interfaces;
using INB.Infra.Data.Log;
using System.Linq.Expressions;

namespace INB.Infra.Data
{
	public abstract class RepositoryBase<TEntity, TContext, TLog> : IRepositoryBase<TEntity, TContext, TLog>
		where TEntity : class
		where TContext : DbContext
		where TLog : class
	{
		private TContext _MyDataContext;
		private IIdentification _UserIdentification;
		private bool _IgnoreLog;
		private bool _SubmitSaveChanges;
		private DbSet<TEntity> _DbSet;

		private LogBase<TLog, TContext> _Log;

		#region Constructors

		public RepositoryBase(TContext MyDataContext, IIdentification UserIdentification = null, bool SubmitSaveChanges = true, bool IgnoreLog = false)
		{
			_MyDataContext = MyDataContext;
			_DbSet = _MyDataContext.Set<TEntity>();
			_IgnoreLog = IgnoreLog;
			_UserIdentification = UserIdentification;
			_SubmitSaveChanges = SubmitSaveChanges;

			if (IgnoreLog == false)
			{
				_Log = new LogBase<TLog, TContext>(_MyDataContext, UserIdentification);
			}
		}

		public RepositoryBase(TContext MyDataContext, bool SubmitSaveChanges = true)
		{
			_MyDataContext = MyDataContext;
			_DbSet = _MyDataContext.Set<TEntity>();
			_IgnoreLog = true;
			_UserIdentification = null;
			_SubmitSaveChanges = SubmitSaveChanges;
		}

		#endregion

		#region Properties

		public TContext DataContext
		{
			get
			{
				return _MyDataContext;
			}
		}

		public IIdentification UserIdentification
		{
			get
			{
				return _UserIdentification;
			}
		}

		public LogBase<TLog, TContext> Log
		{
			get
			{
				return _Log;
			}
		}

		public bool IgnoreLog
		{
			get
			{
				return _IgnoreLog;
			}
			set
			{
				_IgnoreLog = value;
			}
		}

		public bool SubmitSaveChanges
		{
			get
			{
				return _SubmitSaveChanges;
			}
			set
			{
				_SubmitSaveChanges = value;
			}
		}

		#endregion

		/// <summary>
		/// Insere o registro no banco.
		/// </summary>
		/// <param name="obj">Objeto a ser persistido.</param>
		/// <param name="SaveChanges">Define se a operação será efetivada imediatamente.</param>
		/// <returns></returns>
		public TEntity Incluir(TEntity obj, bool SaveChanges)
		{
			/*
			 * Primeiro registra o log pois o método que serializa irá remover o objeto do contexto para garantir que uma 
			 * exceção por referência  circular não seja gerada.
			 */

			if (!_IgnoreLog)
			{
				_Log.RegistrarSerializa(eTipoLog.Insert, obj);
			}

			_DbSet.Add(obj);

			if (SaveChanges)
			{
				//try
				//{
				DataContext.SaveChanges();
				//}
				//catch (System.Data.Entity.Validation.DbEntityValidationException Ex)
				//{
				//	string Msg = "";
				//	Msg = Helpers.Erro.MsgErro(Ex);
				//	throw new Exception(Msg, Ex);
				//}

				//catch (Exception ex)
				//{
				//	throw;
				//}
			}

			return obj;
		}


		/// <summary>
		/// Insere o registro no banco.
		/// </summary>
		/// <param name="obj">Objeto a ser persistido.</param>
		/// <returns></returns>
		public TEntity Incluir(TEntity obj)
		{
			Incluir(obj, _SubmitSaveChanges);
			return obj;
		}

		/// <summary>
		/// Persiste o objeto alterado no banco.
		/// </summary>
		/// <param name="obj">Objeto a ser persistido.</param>
		/// <param name="SaveChanges">Define se a operação será efetivada imediatamente.</param>
		public void Alterar(TEntity obj, bool SaveChanges)
		{
			/*
			 * Primeiro registra o log pois o método que serializa irá remover o objeto do contexto para garantir que uma 
			 * exceção por referência  circular não seja gerada.
			 */

			if (!_IgnoreLog)
			{
				_Log.RegistrarSerializa(eTipoLog.Update, obj);
			}

			_DbSet.Attach(obj);
			DataContext.Entry<TEntity>(obj).State = EntityState.Modified;

			if (SaveChanges)
			{
				//try
				//{
				DataContext.SaveChanges();
				//}
				//catch (System.Data.Entity.Validation.DbEntityValidationException Ex)
				//{
				//	string Msg = "";
				//	Msg = Helpers.Erro.MsgErro(Ex);
				//	throw new Exception(Msg, Ex);
				//}
				//catch (Exception ex)
				//{
				//	throw;
				//}
			}

		}

		/// <summary>
		/// Persiste o objeto alterado no banco.
		/// </summary>
		/// <param name="obj">Objeto a ser persistido.</param>
		public void Alterar(TEntity obj)
		{
			Alterar(obj, _SubmitSaveChanges);
		}

		/// <summary>
		/// Persiste as alterações apenas das propriedades definidas.
		/// </summary>
		/// <param name="SaveChanges">Define se a operação será efetivada imediatamente.</param>
		/// <param name="obj">Objeto a ser persistido.</param>
		/// <param name="propertiesToUpdate">Propriedades a serem modificadas.</param>
		public void Alterar(TEntity obj, bool SaveChanges, params Expression<Func<TEntity, object>>[] propertiesToUpdate)
		{
			/*
			 * Primeiro registra o log pois o método que serializa irá remover o objeto do contexto para garantir que uma 
			 * exceção por referência  circular não seja gerada.
			 */
			if (!_IgnoreLog)
			{
				_Log.RegistrarSerializa(eTipoLog.Update, obj);
			}

			_DbSet.Attach(obj);

			foreach (var p in propertiesToUpdate)
			{
				DataContext.Entry(obj).Property(p).IsModified = true;
			}

			if (SaveChanges)
			{
				//try
				//{
				DataContext.SaveChanges();
				//}
				//catch (System.Data.Entity.Validation.DbEntityValidationException Ex)
				//{
				//	string Msg = "";
				//	Msg = Helpers.Erro.MsgErro(Ex);
				//	throw new Exception(Msg, Ex);
				//}
				//catch (Exception ex)
				//{
				//	throw;
				//}
			}
		}

		/// <summary>
		/// Persiste as alterações apenas das propriedades definidas.
		/// </summary>
		/// <param name="obj">Objeto a ser persistido.</param>
		/// <param name="propertiesToUpdate">Propriedades a serem modificadas.</param>
		public void Alterar(TEntity obj, params Expression<Func<TEntity, object>>[] propertiesToUpdate)
		{
			Alterar(obj, _SubmitSaveChanges, propertiesToUpdate);
		}

		/// <summary>
		/// Exclui o registro do banco.
		/// </summary>
		/// <param name="obj">Objeto com identificador referente ao registro a ser excluído.</param>
		/// <param name="SaveChanges">Define se a operação será efetivada imediatamente.</param>
		public void Excluir(TEntity obj, bool SaveChanges)
		{
			if (!_IgnoreLog)
			{
				_Log.RegistrarSerializa(eTipoLog.Delete, obj);
				_DbSet.Attach(obj);
			}

			try
			{
				_DbSet.Remove(obj);
			}
			catch (Exception ex)
			{ }

			DataContext.Entry(obj).State = EntityState.Deleted;

			if (SaveChanges)
			{
				//try
				//{
				DataContext.SaveChanges();
				//}
				//catch (System.Data.Entity.Validation.DbEntityValidationException Ex)
				//{
				//	string Msg = "";
				//	Msg = Helpers.Erro.MsgErro(Ex);
				//	throw new Exception(Msg, Ex);
				//}
				//catch (Exception ex)
				//{
				//	throw;
				//}
			}
		}

		/// <summary>
		/// Exclui o registro do banco.
		/// </summary>
		/// <param name="obj">Objeto com identificador referente ao registro a ser excluído.</param>
		public void Excluir(TEntity obj)
		{
			Excluir(obj, _SubmitSaveChanges);
		}

		/// <summary>
		///  Exclui o registro do banco utilizando chave primária.
		/// </summary>
		/// <param name="SaveChanges">Define se a operação será efetivada imediatamente.</param>
		/// <param name="keyValues">Identificador.</param>
		public void Excluir(bool SaveChanges, params object[] keyValues)
		{
			var obj = _DbSet.Find(keyValues);
			if (obj != null)
			{
				if (!_IgnoreLog)
				{
					_Log.RegistrarSerializa(eTipoLog.Delete, obj);
					_DbSet.Attach(obj); //a serialização do objeto retira ele do contexto
				}

				_DbSet.Remove(obj);
				DataContext.Entry(obj).State = EntityState.Deleted;

				if (SaveChanges)
				{
					//try
					//{
					DataContext.SaveChanges();
					//}
					//catch (System.Data.Entity.Validation.DbEntityValidationException Ex)
					//{
					//	string Msg = "";
					//	Msg = Helpers.Erro.MsgErro(Ex);
					//	throw new Exception(Msg, Ex);
					//}
					//catch (Exception ex)
					//{
					//	throw;
					//}
				}
			}
		}

		/// <summary>
		///  Exclui o registro do banco utilizando chave primária.
		/// </summary>
		/// <param name="keyValues">Identificador.</param>
		public void Excluir(params object[] keyValues)
		{
			Excluir(_SubmitSaveChanges, keyValues);
		}

		public TEntity SelecionarChave(params object[] keyvalues)
		{
			var Result = _DbSet.Find(keyvalues);
			if (!_IgnoreLog)
			{
				_Log.RegistrarSerializa(eTipoLog.Select, Result);
				DataContext.SaveChanges();
			}
			return Result;
		}

		/// <summary>
		/// Faz o mesmo que o SelecionarChave(params object[] keyvalues), 
		/// mas ignora o log independente da configuração na hora de instanciar o repositório.
		/// </summary>
		/// <param name="keyvalues"></param>
		/// <returns></returns>
		public TEntity SelecionarChaveSemLog(params object[] keyvalues)
		{
			return _DbSet.Find(keyvalues);
		}

		public List<TEntity> SelecionarTodos()
		{
			var Result = _DbSet.ToList();
			if (!_IgnoreLog)
			{
				var objLog = Result.FirstOrDefault();
				if (objLog == null)
				{
					var objLog1 = _DbSet.Create();
					_Log.Registrar(eTipoLog.Select, objLog1, "SelecionarTodos()");
				}
				else
				{
					_Log.Registrar(eTipoLog.Select, objLog, "SelecionarTodos()");
				}
				DataContext.SaveChanges();
			}
			return Result;
		}

		public List<TEntity> SelecionarTodosNoTracking()
		{
			//O MÉTODO NOTRACKING FAZ O CARREGAMENTO E NÃO MANTÉM O OBJETO NO CONTEXTO.
			var Result = _DbSet.AsNoTracking().ToList();

			if (!_IgnoreLog)
			{
				var objLog = Result.FirstOrDefault();
				if (objLog == null)
				{
					var objLog1 = _DbSet.Create();
					_Log.Registrar(eTipoLog.Select, objLog1, "SelecionarTodosNoTracking()");
				}
				else
				{
					_Log.Registrar(eTipoLog.Select, objLog, "SelecionarTodosNoTracking()");
				}
				DataContext.SaveChanges();
			}

			return Result;
		}

	}

}
