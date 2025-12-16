using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Lists.Models.Data;

namespace Lists.Models.Mapping
{
    public class PersonMapper : 
        IMapper<SqlDataReader, Person>
    {
        public Person Convert(SqlDataReader source)
        {
            var person = new Person()
            {
                FirstName = source.GetString(source.GetOrdinal("f_name")),
                Id = source.GetInt64(source.GetOrdinal("person_id")),
                LastName = source.GetString(source.GetOrdinal("l_name")),
                Username = source.GetString(source.GetOrdinal("username")),
                IsGuest = source.GetBoolean(source.GetOrdinal("is_guest")),
                IsAdmin = source.GetBoolean(source.GetOrdinal("is_admin"))
            };

            person.ContactInfo = new ContactInformation(person.Id);

            if (!source.IsDBNull(source.GetOrdinal("m_name")))
            {
                person.MiddleName = source.GetString(source.GetOrdinal("m_name"));
            }

            return person;
        }
    }
}
