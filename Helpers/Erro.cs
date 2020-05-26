using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INB.Infra.Data.Helpers
{
    public class Erro
    {
        public static string MsgErro(System.Data.Entity.Validation.DbEntityValidationException e)
        {
            string Retorno = "";

            foreach (var eve in e.EntityValidationErrors)
            {
                Retorno += "A entidade do tipo " + eve.Entry.Entity.GetType().Name + " no estado "+ eve.Entry.State  +" apresentou os seguintes erros:" ;
                foreach (var ve in eve.ValidationErrors)
                {
                    Retorno += "- Propriedade: " + ve.PropertyName + "|Erro: " + ve.ErrorMessage;
                }
            }
            return Retorno;
        }
    }
}
