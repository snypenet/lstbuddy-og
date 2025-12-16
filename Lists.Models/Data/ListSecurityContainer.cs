using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.Data
{
    public class ListSecurityContainer
    {
        private List<ListSecurity> security = new List<ListSecurity>();

        public ListSecurityContainer(long listId)
        {
            Load(listId);
        }

        public ListSecurityContainer() { }

        public void Load(long listId)
        {
            ListId = listId;
            var list = new List(listId);
            security.AddRange(list.Editors.Editors.Select(v => new ListSecurity { Person = v, ListId = listId, Access = ListSecurityType.Edit }));
            security.AddRange(list.Viewers.Viewers.Select(v => new ListSecurity { Person = v, ListId = listId, Access = ListSecurityType.View }));
        }

        public void AddRange(IEnumerable<ListSecurity> s)
        {
            security.AddRange(s);
        }

        public long ListId { get; set; }

        public bool Contains(Person person)
        {
            return security.Select(s => s.Person.Id).Contains(person.Id);
        }

        public List<ListSecurity> Security
        {
            get
            {
                return security;
            }
        }

        public ListSecurityContainer Filter(Func<ListSecurity, bool> predicate)
        {
            var listSecurity = new ListSecurityContainer();
            listSecurity.AddRange(security.Where(predicate));
            listSecurity.ListId = this.ListId;
            return listSecurity;
        }
    }
}
