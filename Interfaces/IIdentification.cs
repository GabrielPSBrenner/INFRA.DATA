using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace INB.Infra.Data.Interfaces
{
	public interface IIdentification
    {
		
        int Usuario { get; set; }
		
        int? UsuarioAutenticado { get; set; }
		
        int CodigoSistema { get; set; }
		
        string IP { get; set; }
		
        string NomeEstacao { get; set; }
    }
}
