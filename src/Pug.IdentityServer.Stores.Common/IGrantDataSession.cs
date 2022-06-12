using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Pug.Application.Data;

namespace Pug.IdentityServer.Services.Data
{
	public interface IGrantDataSession : IApplicationDataSession
	{
		Task InsertAsync( PersistedGrant grant );

		Task<PersistedGrant> GetPersistedGrant( string identifier );

		Task<IEnumerable<PersistedGrant>> GetPersistedGrants( string type, string subject, string client, string session );

		Task DeletePersistedGrant( string identifier );
		
		Task RemovePersistedGrants( string type, string subject, string client, string session );
	}
}