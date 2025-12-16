using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Lists.Models.Data;

namespace Lists.Models.Mapping
{
    public class ContactInformationMapper : 
        IMapper<SqlDataReader, ContactInformation>
    {
        public ContactInformation Convert(SqlDataReader source)
        {
            var contactInformation = new ContactInformation()
            {
               Email = source.GetString(source.GetOrdinal("email")),
               PersonId = source.GetInt64(source.GetOrdinal("person_id"))
            };

            if (!source.IsDBNull(source.GetOrdinal("provider_name")))
            {
                contactInformation.CellPhoneProvider = new CellPhoneProvider()
                {
                    Name = source.GetString(source.GetOrdinal("provider_name"))
                };
            }

            if (!source.IsDBNull(source.GetOrdinal("cell_phone_provider")))
            {
                if (contactInformation.CellPhoneProvider == null)
                {
                    contactInformation.CellPhoneProvider = new CellPhoneProvider();
                }

                contactInformation.CellPhoneProvider.Id = source.GetInt64(source.GetOrdinal("cell_phone_provider"));
            }

            if (!source.IsDBNull(source.GetOrdinal("provider_email_format")))
            {
                if (contactInformation.CellPhoneProvider == null)
                {
                    contactInformation.CellPhoneProvider = new CellPhoneProvider();
                }

                contactInformation.CellPhoneProvider.AddressFormat = source.GetString(source.GetOrdinal("provider_email_format"));
            }

            if (!source.IsDBNull(source.GetOrdinal("cell_phone_number")))
            {
                if (contactInformation.CellPhoneProvider == null)
                {
                    contactInformation.CellPhoneProvider = new CellPhoneProvider();
                }

                contactInformation.CellPhoneNumber = source.GetString(source.GetOrdinal("cell_phone_number"));
            }

            return contactInformation;
        }
    }
}
