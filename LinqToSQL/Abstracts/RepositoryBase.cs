using INB.Infra.Data.Interfaces;
using INB.Infra.Data.LinqToSQL.Log;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace INB.Infra.Data.LinqToSQL.Abstracts
{
	public abstract class RepositoryBase<TEntity, TContext, TLog> : INB.Infra.Data.LinqToSQL.Interfaces.IRepositoryBase<TEntity, TContext, TLog>
		where TEntity : class, new()
		where TContext : System.Data.Linq.DataContext
		where TLog : class, new()
	{
		private TContext _MyDataContext;
		private IIdentification _UserIdentification;
		private bool _IgnoreLog;
		private bool _SubmitSaveChanges;
		private Table<TEntity> _DbSet;

		private LogBase<TLog, TContext> _Log;

		#region Constructors

		public RepositoryBase(TContext MyDataContext, IIdentification UserIdentification = null, bool SubmitSaveChanges = true, bool IgnoreLog = false)
		{
			_MyDataContext = MyDataContext;
			_DbSet = _MyDataContext.GetTable<TEntity>();
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
			_DbSet = _MyDataContext.GetTable<TEntity>();
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

			_DbSet.InsertOnSubmit(obj);

			if (SaveChanges)
			{
				DataContext.SubmitChanges();
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

			_DbSet.Attach(obj, true);
			//DataContext.Entry<TEntity>(obj).State = EntityState.Modified;

			if (SaveChanges)
			{
				DataContext.SubmitChanges();
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
		/// Exclui o registro do banco.
		/// </summary>
		/// <param name="obj">Objeto com identificador referente ao registro a ser excluído.</param>
		/// <param name="SaveChanges">Define se a operação será efetivada imediatamente.</param>
		public void Excluir(TEntity obj, bool SaveChanges)
		{
			if (!_IgnoreLog)
			{
				_Log.RegistrarSerializa(eTipoLog.Delete, obj);
				try {
					_DbSet.Attach(obj, true);
				}
				catch (Exception ex) {
					
					try {
						DetachEntity(obj);
						_DbSet.Attach(obj, true);	
					}
					catch (Exception ex2) { }					
				}
				
			}

			_DbSet.DeleteOnSubmit(obj);
			
			if (SaveChanges)
			{
				DataContext.SubmitChanges();
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

		private void DetachEntity(object obj)
		{
			obj.GetType().GetMethod("Initialize", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(obj, null);
		}
	}
}
