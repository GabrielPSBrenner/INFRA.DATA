using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INB.Infra.Data.Util
{
    public class CopyCollection<TEntityOrigem, TEntityDestino>
        where TEntityOrigem : class
        where TEntityDestino : class //TEntity será o objeto de retorno das listas
    {

        public static IEnumerable<TEntityDestino> ConverteColecao(IEnumerable<TEntityOrigem> oListaOrigem)
        {
            List<TEntityDestino> oRetorno = new List<TEntityDestino>();


            foreach (object obj in oListaOrigem)
            {
                TEntityDestino objRetorno;
                objRetorno = (TEntityDestino)Activator.CreateInstance(typeof(TEntityDestino), new object[] { });
                CopyObject.CopyObjectData(obj, objRetorno);
                oRetorno.Add(objRetorno);
            }
            return oRetorno;

        }
    }
}
