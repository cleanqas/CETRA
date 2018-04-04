using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class IdentityBank
    {

        /// <summary>
        /// Default constructor for Branch 
        /// </summary>
        public IdentityBank()
        {
            Id = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// Constructor that takes names as argument 
        /// </summary>
        /// <param name="name"></param>
        public IdentityBank(string name, string acronym)
            : this()
        {
            Name = name;
            Acronym = acronym;
        }

        public IdentityBank(string name, string acronym, string id)
        {
            Name = name;
            Id = id;
            Acronym = acronym;
        }

        /// <summary>
        /// Bank ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Bank name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Bank Acronym
        /// </summary>
        public string Acronym { get; set; }

    }
}
