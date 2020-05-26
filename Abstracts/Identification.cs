using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace INB.Infra.Data
{
	[Serializable]
	[DataContract]
	public abstract class Identification : INB.Infra.Data.Interfaces.IIdentification
    {
        private int _Usuario;
        private int? _UsuarioAutenticado;
        private int _CodigoSistema;
        private string _IP;
        private string _NomeEstacao;

		[DataMember]
        public int Usuario
        {
            get
            {
                return _Usuario;
            }
            set
            {
                _Usuario = value;
            }
        }

		[DataMember]
		public int? UsuarioAutenticado
        {
            get
            {
                return _UsuarioAutenticado;
            }
            set
            {
                _UsuarioAutenticado = value;
            }
        }

		[DataMember]
		public int CodigoSistema
        {
            get
            {
                return _CodigoSistema;
            }
            set
            {
                _CodigoSistema = value;
            }
        }

		[DataMember]
        public string IP
        {
            get
            {
				return _IP;
            }
            set
            {
                _IP = value;
            }
        }

		[DataMember]
        public string NomeEstacao
        {
            get
            {
                return _NomeEstacao;
            }
            set
            {
                _NomeEstacao = value;
            }
        }
    }
}
