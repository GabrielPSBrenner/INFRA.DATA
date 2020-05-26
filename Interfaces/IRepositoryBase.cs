using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Data.Entity;

namespace INB.Infra.Data.Interfaces
{
    public interface IRepositoryBase<TEntity, TContext, TLog> where TEntity : class where TContext: DbContext where TLog : class
    {        
        TEntity Incluir(TEntity obj);
        void Alterar(TEntity obj);
        void Excluir(TEntity obj);
        void Excluir(params object[] keyvalues);
        TEntity SelecionarChave(params object[] keyvalues);
		List<TEntity> SelecionarTodos();
        List<TEntity> SelecionarTodosNoTracking();

        TContext DataContext { get; }
		IIdentification UserIdentification { get; }
    }
}
